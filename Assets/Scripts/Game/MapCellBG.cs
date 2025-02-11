using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCellBG : MonoBehaviour
{
    [Space]
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Setup(float size, Color color)
    {
        transform.localScale = Vector3.one * size;
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
