using System;
using UnityEngine;

public class SpriteLibrary : MonoBehaviour
{
    public static SpriteLibrary instance;

    [SerializeField]
    public Sprite circle;

    [SerializeField]
    public Sprite capsule;

    public enum SpriteType
    {
        Circle,
        Capsule
    }

    public Sprite GetSprite(SpriteType sprite)
    {
        if (sprite == SpriteType.Circle) { return circle; }
        if (sprite == SpriteType.Capsule) { return capsule; }
        return null;
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
