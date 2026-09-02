using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;


[RequireComponent(typeof(AudioPlayer))]
public class HandMover : MonoBehaviour
{
    [SerializeField] private SoundData _smashSound;

    [SerializeField] private Transform _goodHand;
    [SerializeField] private Transform _brokeHand;
    [SerializeField] private ParticleSystem _bloodEffect;
    [SerializeField] private ParticleSystem _destroyEffect;
    [SerializeField] private Camera _camera;
    [SerializeField] private DamageFlash _damageFlash;

    [SerializeField] private Interactor _interactor;
    [SerializeField] private Transform _handPoint;
    [SerializeField] private float _moveDuration;
    [SerializeField] private float _upOffset;
    [SerializeField] private float _selectedUpOffset;

    [SerializeField] private float _smashUpOffset = 1.5f;
    [SerializeField] private float _raiseDuration = 0.4f;
    [SerializeField] private float _strikeDuration = 0.1f;
    [SerializeField] private float _stayDuration = 0.5f;
    [SerializeField] private float _smashDownOffset = 0.4f;
    [SerializeField] private Vector3 _swingRotation = new Vector3(-30f, 0f, 0f);
    [SerializeField] private Vector3 _strikeRotation = new Vector3(15f, 0f, 0f);

    [SerializeField] private ExplodeFlasher _explodeFlasher;

    private Tween _moveTween;
    private Sequence _smashSequence;
    private AudioPlayer _audioPlayer;
    private bool _isSmashing;
    private bool _canMove;
    private bool _isSelected;
    private bool _isBrokenHand;
    private bool _isDestroyedHand;

    public bool IsSmashing => _isSmashing;
    public bool IsDestroyedHand => _isDestroyedHand;

    public event Action Destroyed;

    private void Awake()
    {
        _audioPlayer = GetComponent<AudioPlayer>();

        transform.position = _handPoint.position;

        _isSmashing = false;
        _canMove = false;
        _isSelected = false;
        _isBrokenHand = false;
        _isDestroyedHand = false;

        _goodHand.gameObject.SetActive(true);
        _brokeHand.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _interactor.Selected += RaiseHand;
        _interactor.Deselected += ReturnHand;
        _interactor.InteractStarted += StartSmashAnimation;
    }

    private void OnDisable()
    {
        _interactor.Selected -= RaiseHand;
        _interactor.Deselected -= ReturnHand;
        _interactor.InteractStarted -= StartSmashAnimation;

        _moveTween?.Kill();
        _smashSequence?.Kill();
    }

    public void SelectHand(IInteract target)
    {
        if (_isDestroyedHand || _isSelected)
            return;

        _isSelected = true;
        _moveTween?.Kill();

        if (target != null)
        {
            Vector3 targetPosition = target.Transform.position + Vector3.up * _upOffset;
            _moveTween = transform.DOMove(targetPosition, _moveDuration).SetEase(Ease.OutQuad);
        }
        else
        {
            Vector3 targetPosition = transform.position + Vector3.up * _selectedUpOffset;
            _moveTween = transform.DOMove(targetPosition, _moveDuration).SetEase(Ease.OutQuad);
        }

        _canMove = true;
    }

    public void DeselectHand()
    {
        _isSelected = false;

        ReturnHand();

        _canMove = false;
    }

    private void RaiseHand(IInteract target)
    {
        if (_isDestroyedHand)
            return;

        if (_canMove == false)
            return;

        if (_isSmashing)
            return;

        _moveTween?.Kill();

        Vector3 targetPosition = target.Transform.position + Vector3.up * _upOffset;
        _moveTween = transform.DOMove(targetPosition, _moveDuration).SetEase(Ease.OutQuad);
    }

    private void ReturnHand()
    {
        if (_isDestroyedHand)
            return;

        if (_canMove == false)
            return;

        if (_isSmashing)
            return;

        _moveTween?.Kill();

        if (_isSelected)
            _moveTween = transform.DOMove(_handPoint.position + Vector3.up * _selectedUpOffset, _moveDuration).SetEase(Ease.OutQuad);
        else
            _moveTween = transform.DOMove(_handPoint.position, _moveDuration).SetEase(Ease.OutQuad);
    }

    private void StartSmashAnimation(IInteract target)
    {
        if (_isDestroyedHand)
            return;

        if (_canMove == false)
            return;

        if (_isSmashing)
            return;

        _interactor.LockInteraction();
        _isSmashing = true;

        _moveTween?.Kill();
        _smashSequence?.Kill();

        Vector3 raisePosition = target.Transform.position + Vector3.up * _smashUpOffset;
        Vector3 strikePosition = target.Transform.position + Vector3.up * (_upOffset * _smashDownOffset);

        _smashSequence = DOTween.Sequence();

        if(target is Box box)
        {
            Trap trap = box.GetTrap();

            if (trap != null)
            {
                    SmashTrap(raisePosition, strikePosition, trap.TrapType);
            }
            else
            {
                Smash(raisePosition, strikePosition);
            }
        }
        else 
        {
            Smash(raisePosition, strikePosition);
        }
      
        _smashSequence.OnComplete(() =>
        {
            _isSmashing = false;

            if (_interactor.CurrentObject != null)
            {
                RaiseHand(_interactor.CurrentObject);
            }
        });
    }

    private void Smash(Vector3 raisePosition, Vector3 strikePosition)
    {
        _smashSequence
                .Append(transform.DOMove(raisePosition, _raiseDuration).SetEase(Ease.OutCubic))
                .Join(transform.DOLocalRotate(_swingRotation, _raiseDuration).SetEase(Ease.OutCubic))
                .Append(transform.DOMove(strikePosition, _strikeDuration).SetEase(Ease.InQuad))
                .Join(transform.DOLocalRotate(_strikeRotation, _strikeDuration).SetEase(Ease.OutCubic))
                .JoinCallback(() =>
                {
                    _audioPlayer.PlayOnce(_smashSound,false);
                })
                .AppendCallback(() =>
                {
                    _interactor.Interact();
                })
                .AppendInterval(_stayDuration)
                .Join(_camera.transform.DOShakePosition(0.2f, new Vector3(0.2f, 0.2f, 0), 5, 45, false, true))
                .Append(transform.DOMove(_handPoint.position + Vector3.up * _selectedUpOffset, _moveDuration).SetEase(Ease.OutQuad))
                .Join(transform.DOLocalRotate(Vector3.zero, _moveDuration).SetEase(Ease.OutCubic));
    }

    private  void SmashTrap(Vector3 raisePosition, Vector3 strikePosition, TrapType trapType)
    {
        _smashSequence
                .Append(transform.DOMove(raisePosition, _raiseDuration).SetEase(Ease.OutCubic))
                .Join(transform.DOLocalRotate(_swingRotation, _raiseDuration * 1.3f).SetEase(Ease.OutCubic))
                .Append(transform.DOMove(strikePosition, _strikeDuration).SetEase(Ease.InQuad))
                .Join(transform.DOLocalRotate(_strikeRotation, _strikeDuration).SetEase(Ease.OutCubic))
                .JoinCallback(()=>
                {
                    _audioPlayer.PlayOnce(_smashSound,false);
                })
                .AppendCallback(async () =>
                {
                    switch(trapType)
                    {
                        case (TrapType.Damager):
                            InteractWithDamagerTrap();
                            break;

                        case (TrapType.NonLethal):
                            InteractWithNonLethalTrap();
                            break;

                        case (TrapType.Lethal):
                            InteractWithLethalTrap();
                            await Task.Delay(500);
                            break;
                    }

                })
                .Append(transform.DOShakePosition(0.4f, 0.15f, 40, 90, false, false))
                .Join(_camera.transform.DOShakePosition(0.5f, new Vector3(0.5f, 0.5f, 0), 10, 90, false, true))
                .AppendInterval(0.3f)
                .Append(transform.DOMove(transform.position + Vector3.up * _selectedUpOffset, _moveDuration * 0.6f).SetEase(Ease.OutQuad))
                .Join(transform.DOLocalRotate(Vector3.zero, _moveDuration * 0.6f).SetEase(Ease.OutCubic))
                .Append(transform.DOShakePosition(0.2f, 0.08f, 25))
                .Append(transform.DOMove(_handPoint.position + Vector3.up * _selectedUpOffset, _moveDuration * 0.6f).SetEase(Ease.OutQuad));
    }

    private void InteractWithDamagerTrap()
    {
        _interactor.Interact();

        if (_isBrokenHand == false)
        {
            _brokeHand.gameObject.SetActive(true);
            _goodHand.gameObject.SetActive(false);
            _isBrokenHand = true;
            _bloodEffect.Play();
            _damageFlash.TakeDamageEffect();
        }
        else
        {
            _brokeHand.gameObject.SetActive(false);
            _isDestroyedHand = true;
            Destroyed?.Invoke();
            _destroyEffect.gameObject.transform.SetParent(null);
            _destroyEffect.Play();
            _bloodEffect.gameObject.SetActive(false);
            _damageFlash.TakeDamageStrongEffect();
        }
    }

    private void InteractWithNonLethalTrap()
    {
        _interactor.Interact();
    }

    private async void InteractWithLethalTrap()
    { 
        _interactor.Interact();

        await Task.Delay(500);

        _explodeFlasher.Flash();

        _goodHand.gameObject.SetActive(false);
        _brokeHand.gameObject.SetActive(false);
        _isDestroyedHand = true;
        Destroyed?.Invoke();
        _destroyEffect.gameObject.transform.SetParent(null);
        _destroyEffect.Play();
        _bloodEffect.gameObject.SetActive(false);
        _damageFlash.TakeDamageStrongEffect();
    }
}