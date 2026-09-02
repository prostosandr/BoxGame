using System;
using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
[RequireComponent(typeof(TableButtonAnimator))]
[RequireComponent(typeof(Collider))]
public class TableButton : MonoBehaviour, IInteract
{
    [SerializeField] private SoundData _buttonClick;

    private AudioPlayer _audioPlayer;
    private TableButtonAnimator _animator;
    private Collider _collider;

    public Transform Transform => transform;

    public event Action<TableButton> Clicked;

    private void Awake()
    {
        _audioPlayer = GetComponent<AudioPlayer>();
        _animator = GetComponent<TableButtonAnimator>();
        _collider = GetComponent<Collider>();
    }

    public void Interact()
    {
        _audioPlayer.PlayOnce(_buttonClick,false);
        _animator.PlayClick();
        Clicked?.Invoke(this);
    }

    public void Open()
    {
        _animator.PlayOpen();
        _collider.enabled = true;
    }

    public void Close()
    {
        _animator.PlayClose();
        _collider.enabled = false;
    }
}
