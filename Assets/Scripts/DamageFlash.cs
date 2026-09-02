using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class DamageFlash : MonoBehaviour
{
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private float _flashDuration = 0.4f;
    [SerializeField] private float _maxIntensity = 0.45f;
    [SerializeField] private float _destroyFlashDuration = 0.4f;
    [SerializeField] private float _destroyMaxIntensity = 0.85f;

    private Vignette vignette;

    private void Awake()
    {
        if (_globalVolume != null && _globalVolume.profile.TryGet<Vignette>(out Vignette outVignette))
        {
            vignette = outVignette;
            vignette.intensity.overrideState = true;
            vignette.color.overrideState = true;
            vignette.intensity.value = 0f;
        }
    }

    public void TakeDamageEffect()
    {
        vignette.intensity.value = _maxIntensity;

        DOTween.To(() => vignette.intensity.value,
                   x => vignette.intensity.value = x,
                   0f,
                   _flashDuration)
               .SetEase(Ease.OutQuad);
    }

    public void TakeDamageStrongEffect()
    {
        vignette.intensity.value = _destroyMaxIntensity;

        DOTween.To(() => vignette.intensity.value,
                   x => vignette.intensity.value = x,
                   0f,
                   _destroyFlashDuration)
               .SetEase(Ease.OutQuad);
    }
}