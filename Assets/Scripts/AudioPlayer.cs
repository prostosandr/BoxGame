using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayOnce(SoundData sound, bool isLoop)
    {
        if (sound == null || sound.Clip == null) 
            return;

        _audioSource.pitch = sound.GetRandomPitch();
        _audioSource.PlayOneShot(sound.Clip, sound.Volume);
        _audioSource.loop = isLoop;
    }
}