using UnityEngine;
using DG.Tweening;


public class TargetEffectShow : MonoBehaviour
{

    private Vector3 pointA;
    private Vector3 pointB;
    private float duration = 1f;
    private float timeElapsed;
    private float lerpValue;

    private BlockManager blockManager;
    [SerializeField] private SpriteRenderer icon;
    [SerializeField] private SpriteRenderer abilityicon;
    [SerializeField] private SpriteRenderer gemIcon;
    [SerializeField] private ParticleSystem particleSystem;
    
    public void SetUp(Vector3 pos1, Vector3 pos2, Normal_Block_Type type, BlockType abilityType, Special_Object_Type gemType)
    {
        Invoke(nameof(DestroyObject), duration);
        float time = duration - 0.2f;
        Invoke(nameof(DisableParticle), time);
        pointA = pos1;
        pointB = pos2;
        pointB.z = 0f;
        blockManager = BlockManager.Instance;
        SetUpShow(type, abilityType, gemType);
        ScaleAnim();
    }

    private void ScaleAnim()
    {
        transform.DOScale(1.2f, 0.35f);
        transform.DOScale(0.8f, 0.6f).SetDelay(0.3f);
    }

    private void SetUpShow(Normal_Block_Type type, BlockType abilityType, Special_Object_Type gemType)
    {
        icon.gameObject.SetActive(false);
        icon.enabled = true;
        abilityicon.enabled = false;
        gemIcon.gameObject.SetActive(false);
        Sprite iconSprite;

        if (gemType == Special_Object_Type.none)
        {
            icon.gameObject.SetActive(true);
            int blocktype = (int)type;

            if (blocktype >= 0 && blocktype < 6)
            {
                iconSprite = blockManager.IconSprite(blocktype);
            }
            else
            {
                int num = blocktype - 7;
                iconSprite = blockManager.ObstacleTargetEffectShowSprite(num);
            }
            int abilitytype = (int)abilityType;
            Sprite abilitySprite;
            if (abilityType != BlockType.Normal_Block && abilityType != BlockType.None)
            {
                abilityicon.enabled = true;
                if (abilitytype > 0 && abilitytype <= 4)
                {
                    abilitySprite = blockManager.AbilitySprite(abilitytype - 1);
                }
                else
                {
                    abilitySprite = blockManager.ColorAbilitySprite();
                }
                abilityicon.sprite = abilitySprite;
            }
            else
            {
                if (blocktype == 5 && abilityType == BlockType.Normal_Block)
                {
                    iconSprite = blockManager.IconSprite(6);
                }
            }
            icon.sprite = iconSprite;
        }
        else
        {
            gemIcon.gameObject.SetActive(true);
            iconSprite = blockManager.GemTypeSprite((int)gemType - 1);
            gemIcon.sprite = iconSprite;
        }



        
        transform.DOMove(pointB, duration);
    }




    private void DestroyObject()
    {
        Destroy(gameObject);
    }

    private void DisableParticle()
    {
        particleSystem.Stop();
    }
}
