using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TargetShow : MonoBehaviour
{
    [SerializeField] private Image targetIcon;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Text countText;
    [SerializeField] private GameObject targetSucess;
    private BlockManager blockManager;
    private Animator animator;

    public void SetUp(TargetData data)
    {
        animator = targetIcon.GetComponent<Animator>();
        gameObject.SetActive(true);
        targetSucess.SetActive(false);
        blockManager = BlockManager.Instance;

        targetIcon.enabled = true;
        Sprite iconSprite;
        abilityIcon.enabled = false;

        if (data.specialObject == Special_Object_Type.none)
        {
            

            int blocktype = (int)data.normalBlockType;
            int abilitytype = (int)data.blockType;
            if (blocktype >= 0 && blocktype < 6)
            {
                iconSprite = blockManager.IconSprite(blocktype);

            }
            else
            {
                int num = blocktype - 7;
                iconSprite = blockManager.ObstacleSprite(num + 1);
            }

           

            Sprite abilitySprite = null;
            if (data.blockType != BlockType.Normal_Block && data.blockType != BlockType.None)
            {
                abilityIcon.enabled = true;
                if (abilitytype > 0 && abilitytype <= 4)
                {
                    abilitySprite = blockManager.AbilitySprite(abilitytype - 1);
                }
                else
                {
                    if (abilitytype == 5)
                    {
                        abilitySprite = blockManager.ColorAbilitySprite();
                    }
                }
                abilityIcon.sprite = abilitySprite;

            }
            else
            {
                if(blocktype == 5 && data.blockType == BlockType.Normal_Block)
                {
                    iconSprite = blockManager.IconSprite(6);
                }
            }
            
        }
        else
        {
            iconSprite = blockManager.GemTypeSprite((int)data.specialObject - 1);
        }


        targetIcon.sprite = iconSprite;
        countText.text = data.count.ToString();

    }

    public void UpdateTargetCount(int count)
    {
        countText.text = count.ToString();
        if(count == 0)
        {
            targetSucess.SetActive(true);
        }

    }

    IEnumerator CountUpdateAnimCO()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("IsAnim", false);
    }

    public void CountUpdateAnim(bool status)
    {
        animator.SetBool("IsAnim", status);
        StartCoroutine(CountUpdateAnimCO());
    }
}
