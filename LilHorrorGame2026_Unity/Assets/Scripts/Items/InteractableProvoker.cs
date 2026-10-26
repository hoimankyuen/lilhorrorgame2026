using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class InteractableProvoker : Provoker, IInteractableComponent
{
    public void OnSelected(bool selected)
    {
    }
    
    public void OnInteract()
    {
    }
}
