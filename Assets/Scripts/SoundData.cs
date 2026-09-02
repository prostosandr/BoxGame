using UnityEngine;

[CreateAssetMenu(fileName = "NewSound", menuName = "Configs/Sound Data")]
public class SoundData : ScriptableObject
{
    [SerializeField] private AudioClip _clip;
    [Range(0f, 1f)][SerializeField] private float _volume;
    [Range(0.1f, 3f)][SerializeField] private float _minPitch;
    [Range(0.1f, 3f)][SerializeField] private float _maxPitch;

    public AudioClip Clip => _clip;
    public float Volume => _volume;
    public float GetRandomPitch() => Random.Range(_minPitch, _maxPitch);
}