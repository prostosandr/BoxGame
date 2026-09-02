using UnityEngine;

public class BoxEffectPlayer : MonoBehaviour
{
    [SerializeField] private ParticleSystem _boxShades;
    [SerializeField] private ParticleSystem _dirtEffect;

    public ParticleSystem BoxShades => _boxShades;
    public ParticleSystem DirtEffect => _dirtEffect;

    public void PLayEffect(ParticleSystem effect)
    {
        effect.Play();
    }
}