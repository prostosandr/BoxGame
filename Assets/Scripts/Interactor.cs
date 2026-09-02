using System;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private PlayerInputProvaider _playerInput;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _interactLayer;
    [SerializeField] private float _interactDistance;

    private RaycastHit _hit;
    private IInteract _currentObject;
    private bool _canCleanCurrentObject;

    public IInteract CurrentObject => _currentObject;

    public event Action<IInteract> InteractStarted;
    public event Action<IInteract> Intearacted;
    public event Action<IInteract> Selected;
    public event Action Deselected;

    private void Awake()
    {
        _canCleanCurrentObject = true;
    }

    private void OnEnable()
    {
        _playerInput.InteractPressed += StartInteractObject;
    }

    private void Update()
    {
        CheckObject();
    }

    private void OnDisable()
    {
        _playerInput.InteractPressed -= StartInteractObject;
    }

    private void CheckObject()
    {
        if (!_canCleanCurrentObject)
            return;

        Ray ray = _camera.ScreenPointToRay(_playerInput.MousePosition);

        if (Physics.Raycast(ray, out _hit, _interactDistance, _interactLayer))
        {
            if (_hit.collider.TryGetComponent(out IInteract interactObject))
            {
                if (_currentObject != interactObject)
                {
                    _currentObject = interactObject;
                    Selected?.Invoke(_currentObject);
                }
            }
            else
            {
                if (_currentObject != null)
                {
                    Deselected?.Invoke();
                    _currentObject = null;
                }
            }
        }
        else
        {
            if (_currentObject != null)
            {
                Deselected?.Invoke();
                _currentObject = null;
            }
        }
    }

    private void StartInteractObject()
    {
        if (_currentObject == null || !_canCleanCurrentObject)
            return;

        InteractStarted?.Invoke(_currentObject);
    }

    public void LockInteraction()
    {
        _canCleanCurrentObject = false;
    }

    public void Interact()
    {
        if (_currentObject == null)
            return;

        _currentObject.Interact();
        Intearacted?.Invoke(_currentObject);

        _canCleanCurrentObject = true;
        _currentObject = null;
    }
}
