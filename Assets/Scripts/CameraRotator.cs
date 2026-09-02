using DG.Tweening;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    [SerializeField] private PlayerInputProvaider _input;

    [SerializeField] private float _maxTiltX = 5f;
    [SerializeField] private float _maxTiltY = 10f;

    [SerializeField] private float _smoothness = 6f;

    private Quaternion _initialRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        _initialRotation = transform.localRotation;
    }

    private void Update()
    {
        RotateTowardsCursor();
    }

    private void RotateTowardsCursor()
    {
        Vector2 mousePos = _input.MousePosition;

        float normalizedX = (mousePos.x / Screen.width) * 2f - 1f;
        float normalizedY = (mousePos.y / Screen.height) * 2f - 1f;

        normalizedX = Mathf.Clamp(normalizedX, -1f, 1f);
        normalizedY = Mathf.Clamp(normalizedY, -1f, 1f);

        float targetPitch = -normalizedY * _maxTiltX;
        float targetYaw = normalizedX * _maxTiltY;

        Quaternion targetRotation = _initialRotation * Quaternion.Euler(targetPitch, targetYaw, 0f);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * _smoothness);
    }
    public void Shake()
    {
        transform.DOShakePosition(0.5f, new Vector3(0.5f, 0.5f, 0), 10, 90, false, true);
    }
}