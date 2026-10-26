using UnityEngine;


public interface IInteractableComponent
{
    public void OnSelected(bool selected);

    public void OnInteract();
}
