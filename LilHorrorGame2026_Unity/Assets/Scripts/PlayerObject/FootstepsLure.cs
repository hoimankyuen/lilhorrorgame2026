using UnityEngine;

public class FootstepsLure : Lure
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            ChangeActiveState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            ChangeActiveState(false);
        }
    }
}