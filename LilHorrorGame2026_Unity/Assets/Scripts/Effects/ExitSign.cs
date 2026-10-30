using UnityEngine;

public class ExitSign : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    [Header("References")]
    [SerializeField] private Material m_signMaterial;
    [SerializeField] private TimeManager m_timeManager;
    [SerializeField] private Color m_activeExitSignColor;
    [SerializeField] private Color m_inactiveExitSignColor;

    private void Start()
    {
        Reset();
    }

    private void OnEnable()
    {
        m_timeManager.OnSunset += UnlockExitDoor;
    }

    private void OnDisable()
    {
        m_timeManager.OnSunset -= UnlockExitDoor;
    }

    private void Reset()
    {
        m_timeManager.OnSunset -= UnlockExitDoor;
        m_timeManager.OnSunset += UnlockExitDoor;
        m_signMaterial.SetColor(EmissionColor, m_inactiveExitSignColor);
    }

    private void UnlockExitDoor()
    {
        m_signMaterial.SetColor(EmissionColor, m_activeExitSignColor);
    }
}