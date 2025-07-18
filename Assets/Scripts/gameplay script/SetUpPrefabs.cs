using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpPrefabs : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
   public void SetUpSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
