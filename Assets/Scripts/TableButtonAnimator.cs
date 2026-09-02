using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TableButtonAnimator : MonoBehaviour
{
    private static readonly int IdleState = Animator.StringToHash("Idle");
    private static readonly int OpenState = Animator.StringToHash("Open");
    private static readonly int IdleOpenState = Animator.StringToHash("IdleOpen");
    private static readonly int ClickState = Animator.StringToHash("Click");
    private static readonly int CloseState = Animator.StringToHash("Close");

    private Animator _animator;

    [SerializeField] private float _transitionDuration = 0.15f;

    private int _currentState;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void PlayIdle() => FadeToState(IdleState);
    public void PlayOpen() => FadeToState(OpenState);
    public void PlayIdleOpen() => FadeToState(IdleOpenState);
    public void PlayClick() => FadeToState(ClickState);
    public void PlayClose() => FadeToState(CloseState);

    private void FadeToState(int stateHash)
    {
        if (_currentState == stateHash)
        {
            _animator.Play(stateHash, 0, 0f);
        }
        else
        {
            _animator.CrossFade(stateHash, _transitionDuration);
            _currentState = stateHash;
        }
    }
}
