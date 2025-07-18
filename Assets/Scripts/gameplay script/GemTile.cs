using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bgspriteRenderer;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject desEffect;
    private Gem_Type gemType;
    private int rowValue;
    private int colValue;
    public int RowValue { get { return rowValue; } }
    public int ColumeValue { get { return colValue; } }
    public Gem_Type GemType { get { return gemType; } }

    private bool isGemCollected;
    public void SetUp(Sprite icon, Gem_Type gemtype, int rowvalue, int colomeval)
    {
        desEffect.SetActive(false);
        gemType = gemtype;
        spriteRenderer.sprite = icon;
        Re_Pos(rowvalue, colomeval);
    }

    public void Re_Pos(int rowvalue, int colomeval)
    {
        rowValue = rowvalue;
        colValue = colomeval;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckTag(collision.tag);
    }
    

    private void CheckTag(string tag)
    {
        if(string.IsNullOrEmpty(tag)) return;

        if(string.Equals(tag, "Damage"))
        {
            if(CheckForVisible())
                DestroyGem(true);
        }
        else if (string.Equals(tag, "Disturb"))
        {
            DestroyGem(false);
        }
    }

    public void DestroyGem(bool isDes = false)
    {
        if (!isGemCollected)
        {
            isGemCollected = true;
            desEffect.SetActive(true);
            bgspriteRenderer.enabled = false;
            spriteRenderer.enabled = false;
            Invoke(nameof(DestroyObject), 1.5f);
            if (isDes)
            {
                GameManager.BlockDes.Invoke(Normal_Block_Type.none, BlockType.None, gemType, transform.position);
            }
        }
    }
    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private bool CheckForVisible()
    {
        bool isVisible = false;
        if(transform.position.y < 15)
        {
            isVisible = true;
        }
        return isVisible;
    }
    
}
