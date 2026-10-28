using UnityEngine;
using UnityEngine.InputSystem;

public class MonsterTest : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    [SerializeField] private Monster m_monster;

    private void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray mouseRay = m_camera.ScreenPointToRay(Mouse.current.position.value);
            if (plane.Raycast(mouseRay, out float enter))
            {
                Vector3 targetPosition = mouseRay.GetPoint(enter);
                m_monster.WalkTo(targetPosition);
            }
        }
    }

}
