using UnityEngine;

public interface IInteractableComponent
{
    // ======== Selection =======
    
    public void OnSelected(bool selected);
    
    // ======== Interaction =======

    public void OnInteract();
}
