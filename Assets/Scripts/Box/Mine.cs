using System.Threading.Tasks;
using UnityEngine;

public class Mine : Trap
{
    [SerializeField] private SoundData _exploseSound;
    [SerializeField] private ParticleSystem _explodeEffect;

    public override void Interact()
    {
        base.Interact();

        Explode();
    }

    private async void Explode()
    {
        await Task.Delay(500);

        _audioPlayer.PlayOnce(_exploseSound, false);

        await Task.Delay(500);

        gameObject.SetActive(false);
    }
}
