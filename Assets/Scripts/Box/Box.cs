using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(BoxAudio))]
[RequireComponent(typeof(BoxContentProvider))]
[RequireComponent(typeof(BoxEffectPlayer))]
public class Box : MonoBehaviour, IInteract
{
    private BoxContentProvider _contentProvider;
    private BoxEffectPlayer _effectPlayer;
    private BoxContent _currentBoxContent;
    private BoxAudio _audio;

    private MeshRenderer _meshRenderer;
    private Collider _collider;

    private bool _isOpened;

    public Transform Transform => transform;
    public BoxContent CurrentBoxContent => _currentBoxContent;

    public bool IsOpened => _isOpened;

    public event Action<Box> Deactivated;
    public event Action Smashed;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        _audio = GetComponent<BoxAudio>();
        _contentProvider = GetComponent<BoxContentProvider>();
        _effectPlayer = GetComponent<BoxEffectPlayer>();
    }

    public void Initialize()
    {
        _meshRenderer.enabled = true;
        _collider.enabled = true;
        _isOpened = false;

        if (_currentBoxContent != null)
            _currentBoxContent.Deactivate();

        _currentBoxContent = _contentProvider.GetRandomBoxContent();
        _currentBoxContent.Activate();
    }

    public void Interact()
    {
        SmashBox();

        _currentBoxContent.Interact();
    }

    public void Open()
    {
        _effectPlayer.PLayEffect(_effectPlayer.BoxShades);

        _meshRenderer.enabled = false;
        _collider.enabled = false;
    }

    public void Deactivate()
    {
        Deactivated?.Invoke(this);
    }

    public void Land()
    {
        _effectPlayer.PLayEffect(_effectPlayer.DirtEffect);
        _audio.PlayDropSound();
    }

    public Trap GetTrap()
    {
        if (_currentBoxContent is Trap trap)
            return trap;

        return null;
    }

    public Reward GetReward()
    {
        if (_currentBoxContent is Reward reward)
            return reward;

        return null;
    }

    private void SmashBox()
    {
        Open();

        _isOpened = true;

        Smashed?.Invoke();
    }
}