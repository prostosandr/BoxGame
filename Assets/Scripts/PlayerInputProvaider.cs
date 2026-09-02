using System;
using UnityEngine;

public class PlayerInputProvaider : MonoBehaviour
{
    private PlayerInput _input;

    public Vector2 MousePosition => _input.Player.Look.ReadValue<Vector2>();

    public event Action InteractPressed;
    public event Action SelectLeftHandPressed;
    public event Action SelectRightHandPressed;

    private void Awake()
    {
        _input = new PlayerInput();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.Interact.performed += ctx => InteractPressed?.Invoke();
        _input.Player.SelectLeftHand.performed += ctx => SelectLeftHandPressed?.Invoke();
        _input.Player.SelectRightHand.performed += ctx => SelectRightHandPressed?.Invoke();
    }

    private void OnDisable()
    {
        _input.Player.Interact.performed -= ctx => InteractPressed?.Invoke();
        _input.Player.SelectLeftHand.performed -= ctx => SelectLeftHandPressed?.Invoke();
        _input.Player.SelectRightHand.performed -= ctx => SelectRightHandPressed?.Invoke();

        _input.Disable();
    }
}