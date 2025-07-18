using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockLogo : MonoBehaviour
{
    public SpriteRenderer thisSprite;
    public BlockType thisBlockType;
    public GameObject effectObj;
    public GameObject abilityShowObject;
    public GameObject abilityEffectObj;
    public int rowValue;
    public int colValue;
    public int colorCode;

    private Color thisColor = new Color(1, 1, 1, 1f);
    private Color thisSelectedColor = new Color(1, 1, 1, 0.5f);
    public void SelectedColor()
    {
        thisSprite.color = thisSelectedColor;
    }

    public void NormalColor()
    {
        thisSprite.color = thisColor;
    }

    public void DestroyObject()
    {
        AbilityShow(false);
        ActiveAbilities(false);
        effectObj.SetActive(true);
        thisSprite.gameObject.SetActive(false);
        if (thisBlockType != BlockType.Normal_Block && thisBlockType != BlockType.None)
        {
            ActiveAbilities(true);
        }
        Invoke(nameof(DisableObj), 1f);
    }

    private void AbilityShow(bool status)
    {
        if (abilityShowObject != null)
        {
            abilityShowObject.SetActive(status);
        }
    }
    private void ActiveAbilities(bool status)
    {
        if (abilityEffectObj != null)
            abilityEffectObj.SetActive(status);
    }

    private void DisableObj()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}
