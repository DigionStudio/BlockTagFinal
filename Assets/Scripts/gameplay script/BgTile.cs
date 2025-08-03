using DG.Tweening;
using System;
using UnityEngine;

public class BgTile : MonoBehaviour
{
    [SerializeField] private GameObject bgTileObj;
    [SerializeField] private SpriteRenderer bgsprite;
    [SerializeField] private SpriteMask mask;
    [SerializeField] private GameObject limitObj;
    [SerializeField] private SpriteRenderer obstacleObj;
    [SerializeField] private GameObject animObj;
    [SerializeField] private SpriteRenderer[] obsDesAnimRenderer;

    private bool isSpecialObj;
    [SerializeField] private GameObject specialObsObject;
    [SerializeField] private Transform specialCollider;
    private bool isShield;
    private bool shieldStatus;
    [SerializeField] private GameObject[] specialTileEffects;

    private BoxCollider2D collider;
    //private int[] HitPonts = new int[5] { 8, 14, 16, 18, 10};
    private int[] HitPonts = new int[9] { 3, 6, 7, 8, 5, 4, 5, 3, 9 };
    //private int[] HitPonts = new int[5] { 2, 4, 5, 6, 3 };

    private Sprite iconSprite;
    private int obsSprite = -1;
    private bool isObstacle;
    private int totalHitpoint = 2;
    private int currentHitpoint = 2;
    private Normal_Block_Type blockType = Normal_Block_Type.none;

    private bool isDestroyed;
    private bool isEventInvoked;
    public bool HasDestroyed { get { return isDestroyed; } }
    public Normal_Block_Type BlockTypeCode { get { return blockType; } }

    [SerializeField] private SpriteRenderer crackSprite;
    [SerializeField] private Sprite[] crackIcon;
    [SerializeField] private Animator animator;
    [SerializeField] private Sprite invisibleSprite;
    public static Action<Normal_Block_Type> OnDamageTaken = delegate { };
    private bool isHealActive;
    private int specialIndex;
    private int colliderRadius;
    private readonly float shieldTime = 7;
    [SerializeField] private LayerMask layerMask;
    private void OnEnable()
    {
        OnDamageTaken += DamageTaken;
    }
    private void OnDisable()
    {
        OnDamageTaken -= DamageTaken;
    }
    public Normal_Block_Type BgTileType()
    {
        return blockType;
    }
    public void SetUpTile(bool isLimit, BgTileData tiledata)
    {
        collider = GetComponent<BoxCollider2D>();
        specialObsObject.SetActive(false);
        specialCollider.localScale = Vector3.zero;
        limitObj.SetActive(isLimit);
        SetUpNewBgTileData(tiledata);
    }
    public void SetUpNewBgTileData(BgTileData tiledata)
    {
        isEventInvoked = false;
        isDestroyed = false;
        crackSprite.gameObject.SetActive(false);
        BlockerStatus(false);
        if (tiledata != null)
        {
            if (tiledata.tileType == Normal_Block_Type.Invisible)
            {
                SetUpInvisibleTile();
            }
            else
            {
                BlockerStatus(true);
                isObstacle = true;
                int index = (int)tiledata.tileType - 7;
                obsSprite = index;
                totalHitpoint = HitPonts[index];
                currentHitpoint = totalHitpoint;
                iconSprite = BlockManager.Instance.ObstacleSprite(obsSprite + 1);
                obstacleObj.sprite = iconSprite;
                
            }
            blockType = tiledata.tileType;
            if((int)blockType >= (int)Normal_Block_Type.Treatment && (int)blockType <= (int)Normal_Block_Type.Shield) 
            {
                isSpecialObj = true;
            }
            else
            {
                isHealActive = true;
            }
            if(blockType == Normal_Block_Type.Shield) 
            {
                isShield = true;
            }
            SetUpObsDesRenderer();
            ResetTile();
            SetUpSpecialObs();
        }
    }
    private void ResetTile()
    {
        animObj.SetActive(false);
        animator.SetBool("IsObsAnim", false);
        animator.enabled = true;
        obstacleObj.enabled = true;
    }
    public void ResetBgTile()
    {
        isDestroyed = false;
        currentHitpoint = totalHitpoint;
        BlockerStatus(true);
        ResetTile();


    }
    private void SetUpObsDesRenderer()
    {
        if (blockType != Normal_Block_Type.Invisible)
        {
            Sprite[] obdDesSprite = BlockManager.Instance.ObsDesAnimSprites(obsSprite);
            if (obdDesSprite.Length > 0 && obdDesSprite != null)
            {
                for (int i = 0; i < obdDesSprite.Length; i++)
                {
                    obsDesAnimRenderer[i].sprite = obdDesSprite[i];
                }
            }
            else
            {
                obsDesAnimRenderer[0].transform.parent.gameObject.SetActive(false);
            }
        }
    }
    private void SetUpInvisibleTile()
    {
        obstacleObj.sprite = invisibleSprite;
        BlockerStatus(true);
    }


    public void TileStatus(bool isActive)
    {
        bgTileObj.SetActive(isActive);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.CompareTag("Hammer"))
        {
            if (!isDestroyed)
            {
                int num = totalHitpoint / 3;
                CheckHitpoint(num);
            }
        }
        else if (collision.gameObject.CompareTag("Damage"))
        {
            CheckHitpoint(1);
        }
        if ((int)blockType > (int)Normal_Block_Type.Invisible)
        {
            if (collision.gameObject.CompareTag("Treatment") && blockType != Normal_Block_Type.Treatment)
            {
                Heal(1);
            }
            else if (collision.gameObject.CompareTag("Recover") && blockType != Normal_Block_Type.Recover)
            {
                Heal(totalHitpoint);
            }
            else if (collision.gameObject.CompareTag("Shield") && blockType != Normal_Block_Type.Shield)
            {
                isObstacle = false;
            }
        }
        //if (!collision.gameObject.CompareTag("Disturb") && (int)blockType > 12 && (int)blockType <= 15)
        
    }

    private void Heal(int value)
    {
        if(currentHitpoint < totalHitpoint - value)
        {
            currentHitpoint += value;
        }
        else
        {
            currentHitpoint = totalHitpoint;
        }
        SetUpCrack();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (blockType != Normal_Block_Type.Shield && (int)blockType > 6)
        {
            if (collision.gameObject.CompareTag("Shield"))
            {
                isObstacle = true;
            }
        }
    }

    public void DoDamage()
    {
        CheckHitpoint(1);
    }

    private void CheckHitpoint(int num)
    {
        if (GameManager.gameStatus < 0)
        {
            if (isObstacle)
            {
                if (currentHitpoint > num)
                {
                    currentHitpoint -= num;
                    SetUpCrack();
                    OnDamageTaken.Invoke(blockType);
                }
                else
                {
                    currentHitpoint = 0;
                }

                if (!isDestroyed)
                {
                    if (currentHitpoint <= 0)
                    {
                        DestroyBgTile();
                    }
                }
            }
        }
    }

    private void DestroyBgTile()
    {
        print("adfsdf");
        specialObsObject.SetActive(false);
        currentHitpoint = 0;
        isDestroyed = true;
        obstacleObj.enabled = false;
        crackSprite.gameObject.SetActive(false);
        animObj.SetActive(isDestroyed);
        animator.SetBool("IsObsAnim", isDestroyed);
        if (isSpecialObj)
        {
            CancelInvoke(nameof(ShieldActive));
        }
        
        DestroyEvent(1);
        Invoke(nameof(BgBlockDes), 1f);
    }

    private void BgBlockDes()
    {
        DestroyEvent(0);
        animObj.SetActive(false);
        BlockerStatus(false);
    }


    private void DestroyEvent(int num)
    {
        if (!isEventInvoked)
        {
            if (BoardManager.Instance.gameDataManager.GameTypeCode == num && (int)blockType > 6)
            {
                GameManager.BlockDes.Invoke(blockType, BlockType.None, Gem_Type.none, transform.position);
                isEventInvoked = true;
            }
        }
    }
        

    private void BlockerStatus(bool isActive)
    {
        collider.enabled = isActive;
        obstacleObj.gameObject.SetActive(isActive);
        animator.SetBool("obstacle", isActive);
    }

    private void SetUpCrack()
    {
        float cur = (float)currentHitpoint;
        float quater = (float)totalHitpoint/ 4f;
        if (cur <= quater * 3)
        {
            crackSprite.gameObject.SetActive(true);
            Sprite crack = crackIcon[0];
            if (cur <= quater * 2)
            {
                crack = crackIcon[1];
                if (cur <= quater)
                {
                    crack = crackIcon[2];
                    if (cur == 1f)
                    {
                        crack = crackIcon[3];
                    }
                }
            }
            crackSprite.sprite = crack;
        }
        else
        {
            crackSprite.gameObject.SetActive(false);
        }
    }

    private void SetUpSpecialObs()
    {
        if (isSpecialObj)
        {
            specialObsObject.SetActive(true);
            specialIndex = (int)blockType - 13;

            if (isShield)
            {
                colliderRadius = 5;
                int num = UnityEngine.Random.Range(2, 10);
                InvokeRepeating(nameof(ShieldActive), (float)num, shieldTime);
            }
            else
            {
                colliderRadius = 7 - (1 * specialIndex);
            }

            specialCollider.tag = blockType.ToString();
        }
    }

    private void ShieldActive()
    {
        if (!isDestroyed)
        {
            shieldStatus = !shieldStatus;
            specialTileEffects[2].SetActive(shieldStatus);
            if (shieldStatus)
            {
                CheckForAbilityStatus();
                specialCollider.DOScale(Vector3.one * colliderRadius, 0.5f);
            }
            else
            {
                specialCollider.DOScale(Vector3.zero, 0.5f);

            }
        }
        else
        {
            CancelInvoke(nameof(ShieldActive));
        }
    }


    private void HealObs()
    {
        isHealActive = true;
        specialTileEffects[specialIndex].SetActive(true);
        if (isSpecialObj && !isShield)
        {
            specialCollider.DOScale(Vector3.one * colliderRadius, 1f).OnComplete(() =>
            {
                CheckForAbilityStatus();
                specialCollider.DOScale(Vector3.zero, 0.5f);
                Invoke(nameof(DisableSpecialTileEffect), 1f);
            });
        }
    }

    private void DisableSpecialTileEffect()
    {
        specialTileEffects[specialIndex].SetActive(false);
        if (isSpecialObj && !isShield)
        {
            isHealActive = false;
        }
    }


    private void DamageTaken(Normal_Block_Type type)
    {
        if(!isHealActive && !isDestroyed && isSpecialObj && !isShield)
            Invoke(nameof(HealObs), 0.5f);
    }

    private void CheckForAbilityStatus()
    {
        bool blockInside = false;
        RaycastHit2D[] hit = Physics2D.CircleCastAll(transform.position, colliderRadius, Vector2.zero, 0, layerMask);
        int count = 0;
        foreach (var item in hit)
        {
            if (item.collider != null)
            {
                if (item.collider.gameObject.tag == "Disturb")
                {
                    count++;
                }
            }
            if(count >= 2)
            {
                blockInside = true;
                break;
            }
        }
        if(!blockInside)
        {
            if (!isDestroyed)
                DestroyBgTile();
        }
    }

    public void DestroyObj()
    {
        Destroy(gameObject);
    }
}
