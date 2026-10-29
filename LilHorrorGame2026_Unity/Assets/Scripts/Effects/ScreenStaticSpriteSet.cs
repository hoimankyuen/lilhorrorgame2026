using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenStaticSpriteSet", menuName = "Data/Screen Static Sprite Set")]
public class ScreenStaticSpriteSet : ScriptableObject
{
    [SerializeField] private List<Sprite> m_sprites;

    public Sprite Get(int index)
    {
        if (index < 0)
            index = 0;
        else if (index > m_sprites.Count - 1)
            index = m_sprites.Count - 1;
        
        return m_sprites[index];
    }
}
