using Unity.VisualScripting;
using UnityEngine;

public class HandSelector : MonoBehaviour
{
    [SerializeField] private PlayerInputProvaider _input;
    [SerializeField] private HandMover _leftHand;
    [SerializeField] private HandMover _rightHand;
    [SerializeField] private Interactor _interactor;

    private void OnEnable()
    {
        _input.SelectLeftHandPressed += SelectLeftHand;
        _input.SelectRightHandPressed += SelectRightHand;

        _leftHand.Destroyed += SelectRightHand;
        _rightHand.Destroyed += SelectLeftHand;
    }

    private void OnDisable()
    {
        _input.SelectLeftHandPressed -= SelectLeftHand;
        _input.SelectRightHandPressed -= SelectRightHand;

        _leftHand.Destroyed -= SelectRightHand;
        _rightHand.Destroyed -= SelectLeftHand;
    }

    private void SelectLeftHand()
    {
        if (_rightHand.IsSmashing && !_rightHand.IsDestroyedHand)
            return;

        if (_leftHand.IsDestroyedHand)
            return;

        _leftHand.SelectHand(_interactor.CurrentObject);
        _rightHand.DeselectHand();
    }

    private void SelectRightHand()
    {
        if (_leftHand.IsSmashing && !_leftHand.IsDestroyedHand)
            return;

        if (_rightHand.IsDestroyedHand)
            return;

        _rightHand.SelectHand(_interactor.CurrentObject);
        _leftHand.DeselectHand();
    }
}
