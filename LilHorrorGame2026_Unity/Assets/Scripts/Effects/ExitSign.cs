using UnityEngine;

public class ExitSign : MonoBehaviour
{
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    [Header("References")]
    [SerializeField] private MeshRenderer m_signRenderer;
    [SerializeField] private Material m_signMaterial;
    [SerializeField] private TimeManager m_timeManager;
    [SerializeField] private Color m_activeExitSignColor;
    [SerializeField] private Color m_inactiveExitSignColor;

    private Material m_signMaterialInstance;

    private void Awake()
    {
        m_signMaterialInstance = Instantiate(m_signMaterial);
        m_signRenderer.material = m_signMaterialInstance;
    }
    
    private void Start()
    {
        m_timeManager.OnSunset += UnlockExitDoor;
        m_signMaterial.SetColor(EmissionColor, m_inactiveExitSignColor);
    }

    private void OnDestroy()
    {
        m_timeManager.OnSunset -= UnlockExitDoor;
        
        m_signRenderer.material = null;
        Destroy(m_signMaterialInstance);
    }

    private void UnlockExitDoor()
    {
        m_signMaterial.SetColor(EmissionColor, m_activeExitSignColor);
    }
}