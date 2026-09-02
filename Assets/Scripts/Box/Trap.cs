using UnityEngine;

[RequireComponent(typeof(AudioPlayer))]
public class Trap : BoxContent
{
    [SerializeField] private SoundData _trapSound;
    [SerializeField] private TrapType _trapType;
    [SerializeField] private float _penalty;

    protected AudioPlayer _audioPlayer;

    public TrapType TrapType => _trapType;
    public float Penalty => _penalty;

    private void Awake()
    {
        _audioPlayer = GetComponent<AudioPlayer>(); 
    }

    public override void Interact()
    {
        base.Interact();

        _audioPlayer.PlayOnce(_trapSound,false);
    }
}