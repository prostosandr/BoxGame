using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class Reward : BoxContent
{
    [SerializeField] private SoundData _rewardSound;
    [SerializeField] private float _price;

    private AudioPlayer _audioPlayer;

    public float Price => _price;

    private void Awake()
    {
        _audioPlayer = GetComponent<AudioPlayer>();
    }

    public override void Interact()
    {
        base.Interact();

        _audioPlayer.PlayOnce(_rewardSound, false);
    }
}