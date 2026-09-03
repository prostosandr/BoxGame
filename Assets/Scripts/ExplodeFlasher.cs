using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExplodeFlasher : MonoBehaviour
{
    [SerializeField] private AudioPlayer _player;
    [SerializeField] private SoundData _sound;
    [SerializeField] private Image _whitePanel;
    [SerializeField] private Image _blackPanel;

    [SerializeField] private float _whiteFadeSpeed = 5f;
    [SerializeField] private float _blackFadeSpeed = 2f;

    private Coroutine _flashCoroutine;

    public void Flash()
    {
        _player.PlayOnce(_sound, false);

        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }

        _flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        SetPanelAlpha(_whitePanel, 1f);
        SetPanelAlpha(_blackPanel, 1f);

        _whitePanel.gameObject.SetActive(true);
        _blackPanel.gameObject.SetActive(true);

        while (_whitePanel.color.a > 0f)
        {
            float newAlpha = Mathf.MoveTowards(_whitePanel.color.a, 0f, _whiteFadeSpeed * Time.deltaTime);
            SetPanelAlpha(_whitePanel, newAlpha);
            
            yield return null;
        }
        
        _whitePanel.ga
        meObject.SetActive(false);

        while (_blackPanel.color.a > 0f)
        {
            float newAlpha = Mathf.MoveTowards(_blackPanel.color.a, 0f, _blackFadeSpeed * Time.deltaTime);
            SetPanelAlpha(_blackPanel, newAlpha);
            
            yield return null;
        }
        
        _blackPanel.gameObject.SetActive(false);

        _flashCoroutine = null;
    }

    private void SetPanelAlpha(Image panel, float alpha)
    {
        Color color = panel.color;
        color.a = alpha;
        panel.color = color;
    }
}
