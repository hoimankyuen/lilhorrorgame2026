using TMPro;
using UnityEngine;

[System.Serializable]
public struct CreditEntryData
{
    public string Label;
    [TextArea] public string Content;
}

public class CreditEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text m_labelText;
    [SerializeField] private TMP_Text m_contentText;

    public void Setup(CreditEntryData data)
    {
        m_labelText.text = data.Label;
        m_contentText.text = data.Content;
    }
}
