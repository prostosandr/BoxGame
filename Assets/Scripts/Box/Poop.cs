using UnityEngine;

public class Poop : Trap
{
    [SerializeField] private SoundData _fliesSound;
    [SerializeField] private ParticleSystem _flies;
    [SerializeField] private AudioPlayer _fliesAudio;

    public override void Activate()
    {
        base.Activate(); 
    }

    public override void Interact()
    {
        base.Interact();

        _flies.Play();
        _fliesAudio.PlayOnce(_fliesSound, true);
    }

    public override void Deactivate()
    {
        base.Deactivate();

        _flies.Stop();
    }
}