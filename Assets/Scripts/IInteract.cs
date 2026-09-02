using UnityEngine;

public interface IInteract
{
    public Transform Transform { get; }

    public void Interact();
}