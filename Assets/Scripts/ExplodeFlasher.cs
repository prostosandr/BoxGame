using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ExplodeFlasher : MonoBehaviour
{
    [SerializeField] private AudioPlayer _player;
    [SerializeField] private SoundData _sound;
    [SerializeField] private Image _whitePanel;
    [SerializeField] private Image _blackPanel;

    [Header("Настройки скорости (чем больше число, тем быстрее затухание)")]
    [SerializeField] private float _whiteFadeSpeed = 5f; // Быстрое затухание белого
    [SerializeField] private float _blackFadeSpeed = 2f; // Более медленное затухание черного

    private Coroutine _flashCoroutine;

    public void Flash()
    {
        _player.PlayOnce(_sound, false);

        // Если вспышка уже идет, останавливаем ее и запускаем заново
        if (_flashCoroutine != null)
        {
            StopCoroutine(_flashCoroutine);
        }

        _flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Включаем панели и делаем их полностью видимыми
        SetPanelAlpha(_whitePanel, 1f);
        SetPanelAlpha(_blackPanel, 1f);

        _whitePanel.gameObject.SetActive(true);
        _blackPanel.gameObject.SetActive(true);

        // 2. Постепенно тушим белый фон до нуля
        while (_whitePanel.color.a > 0f)
        {
            float newAlpha = Mathf.MoveTowards(_whitePanel.color.a, 0f, _whiteFadeSpeed * Time.deltaTime);
            SetPanelAlpha(_whitePanel, newAlpha);
            yield return null;
        }
        _whitePanel.gameObject.SetActive(false);

        // 3. Постепенно тушим черный фон до нуля
        while (_blackPanel.color.a > 0f)
        {
            float newAlpha = Mathf.MoveTowards(_blackPanel.color.a, 0f, _blackFadeSpeed * Time.deltaTime);
            SetPanelAlpha(_blackPanel, newAlpha);
            yield return null;
        }
        _blackPanel.gameObject.SetActive(false);

        _flashCoroutine = null;
    }

    // Вспомогательный метод для изменения прозрачности (Alpha) у картинок
    private void SetPanelAlpha(Image panel, float alpha)
    {
        Color color = panel.color;
        color.a = alpha;
        panel.color = color;
    }
}
