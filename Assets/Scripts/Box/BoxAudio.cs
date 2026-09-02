using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class BoxAudio : MonoBehaviour
{
    [SerializeField] private SoundData _boxDropClip;

    private AudioPlayer _audioPlayer;

    private void Awake()
    {
        _audioPlayer = GetComponent<AudioPlayer>();
    }

    public void PlayDropSound()
    {
        _audioPlayer.PlayOnce(_boxDropClip, false);
    }
}