using UnityEngine;

public class FootstepsLure : Lure
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("Monster entered");
            ChangeActiveState(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Debug.Log("Monster exited");
            ChangeActiveState(false);
        }
    }
}