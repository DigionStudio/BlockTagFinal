using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushTile : MonoBehaviour
{
    public BlockTile blockSelected;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private LayerMask layerMaskObs;
    private BoxCollider2D collider2D;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool isTriggerEnter;
    private bool isRaycast;
    private BoardManager boardManager;
    private bool isNormalSize = true;
    private Sprite crushTileSprite;
    private CrushTileType thisCrushTileType = CrushTileType.Normal; 
    private bool isObstacle;
    private bool isModBomb;
    [SerializeField] private GameObject effectHolder;
    private List<BlockTile> blockSelectedList = new List<BlockTile>();
    private List<string> bombBlockList = new List<string>();
    public int BlockSelectedListCount { get { return blockSelectedList.Count; } }
    private CircleCollider2D circleCollider;
    [SerializeField] private GameObject bombBlast;
    [SerializeField] private GameObject freezeEffect;
    [SerializeField] private GameObject magnetEffect;
    [SerializeField] private GameObject realignEffect;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform abilityEffectShow;
    [SerializeField] private SpriteRenderer abilitySpriteRen;
    [SerializeField] private AudioSource bombAudioSource;
    [SerializeField] private Sprite[] abilityIcon;
    [SerializeField] private GameObject modEffect;

    void Start()
    {
        animator.enabled = false;
        boardManager = BoardManager.Instance;
        circleCollider = GetComponent<CircleCollider2D>();
        collider2D = GetComponent<BoxCollider2D>();
        crushTileSprite = BlockManager.Instance.CurrentCrushTile();
        spriteRenderer.sprite = crushTileSprite;
        SetUpCollider(false);
        bombBlast.SetActive(false);
        freezeEffect.SetActive(false);
        magnetEffect.SetActive(false);
        realignEffect.SetActive(false);
        SetUp();

    }

    private void SetUp()
    {
        if(collider2D == null)
        {
            collider2D = GetComponent<BoxCollider2D>();
        }
        if (thisCrushTileType == CrushTileType.Normal)
        {
            circleCollider.enabled = false;
            collider2D.enabled = true;
            effectHolder.SetActive(false);

        }
        else
        {
            spriteRenderer.enabled = false;
            collider2D.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Disturb"))
        {
            isObstacle = true;
            DisableCrush(collision.gameObject);
        }
        else
        {
            if (collision.gameObject.CompareTag("Tile") && !boardManager.HasTurn && (!isObstacle || thisCrushTileType != CrushTileType.Normal))
            {
                TriggerEnter(collision.gameObject);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Disturb"))
        {
            isObstacle = false;
        }

        if (collision.gameObject.CompareTag("Tile") && !boardManager.HasTurn)
        {
            DisableCrush(collision.gameObject);
        }
    }
    private void DisableCrush(GameObject obj)
    {
        if (thisCrushTileType != CrushTileType.Normal)
        {
            if (obj.TryGetComponent<BlockTile>(out var block))
            {
                if (block != null)
                {
                    AddRemoveBlockTile(block, false);
                    ChangeBombTileColor(block, 3);
                }
            }

        }
        else
        {
            ChangeTileColor(obj, 3);
            isTriggerEnter = false;
            if (!isNormalSize)
                ChangeColor(2);
        }
    }
    public bool SizeStatus(bool isReset)
    {
        isNormalSize = isReset;
        if (isNormalSize)
        {
            isObstacle = false;
        }
        return isNormalSize;
    }

    private void TriggerEnter(GameObject obj)
    {
        
        if(thisCrushTileType != CrushTileType.Normal)
        {
            if (obj.TryGetComponent<BlockTile>(out var block))
            {
                AddRemoveBlockTile(block, true);
                ChangeBombTileColor(block, 0);
            }
        }
        else
        {
            if (!isObstacle)
            {
                isTriggerEnter = true;
                ChangeTileColor(obj, 0);
                ChangeColor(1);
            }
        }
    }

    private void AddRemoveBlockTile(BlockTile tile, bool isadd)
    {
        string code = tile.RowValue.ToString() + tile.ColumnValue.ToString();
        if (!isadd)
        {
            if (bombBlockList.Contains(code))
            {
                bombBlockList.Remove(code);
                blockSelectedList.Remove(tile);
            }
        }
        else
        {
            if (!bombBlockList.Contains(code))
            {
                blockSelectedList.Add(tile);
                bombBlockList.Add(code);
            }
        }
    }
    private void ChangeBombTileColor(BlockTile tile, int code)
    {
        if (tile != null)
        {
            if (code == 0)
            {
                tile.SelectedColor();

            }
            else
            {
                tile.NormalColor();
            }
        }
    }

    private void ChangeTileColor(GameObject tile, int code)
    {
        if (tile.TryGetComponent<BlockTile>(out var block)) {
            blockSelected = block;
        }
        if (blockSelected != null)
        {
            if (code == 0)
            {
                blockSelected.SelectedColor();
            }
            else
            {
                blockSelected.NormalColor();
                blockSelected = null;
            }
        }
    }

    public void ChangeColor(int code)
    {
        Color col = Color.red;
        if(code == 1)
        {
            col = Color.white;
        }
        spriteRenderer.color = col;
    }
    public void ChangeAlpha()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.5f);
    }

    public void DestroyBlock()
    {
        modEffect.SetActive(false); 
        if (thisCrushTileType == CrushTileType.Freeze)
        {
            effectHolder.SetActive(false);
            freezeEffect.SetActive(true);
            ChangeToNormal();
        }
        else if (thisCrushTileType == CrushTileType.ReAlign)
        {
            realignEffect.SetActive(true);
            abilityEffectShow.gameObject.SetActive(false);
            ChangeToNormal();
        }
        else if (thisCrushTileType == CrushTileType.Bomb)
        {
            if (isModBomb)
            {
                GameAIManager.Instance.ChangeBombAsisted();
            }
            animator.enabled = true;
            animator.SetBool("isBomb", true);
            bombAudioSource.Play();
            spriteRenderer.enabled = false;
            Invoke(nameof(BombBlast), 0.7f);
        }
        else if (thisCrushTileType == CrushTileType.Magnet)
        {
            magnetEffect.SetActive(true);
            abilityEffectShow.gameObject.SetActive(false);
            ChangeToNormal();
        }
        else
        {
            blockSelected.DestroyObject();
        }
    }

    private void ChangeToNormal()
    {
        foreach (var item in blockSelectedList)
        {
            item.NormalColor(); ;
        }
    }
    private void BombBlast()
    {
        if(thisCrushTileType == CrushTileType.Bomb)
        {
            effectHolder.SetActive(false);
            bombBlast.SetActive(true);
            DestroySelected();
        }
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(transform.position, 5);
    //}


    private void DestroySelected()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 5, Vector2.zero, 0, layerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if(hit.transform.TryGetComponent<BlockTile>(out var block))
            {
                string code = block.RowValue.ToString() + block.ColumnValue.ToString();
                if (!bombBlockList.Contains(code))
                {
                    blockSelectedList.Add(block);
                    bombBlockList.Add(code);
                }
            }
        }

        RaycastHit2D[] hits2 = Physics2D.CircleCastAll(transform.position, 5, Vector2.zero, 0, layerMaskObs);

        foreach (RaycastHit2D hit in hits2)
        {
            BgTile bgtile = hit.transform.GetComponentInParent<BgTile>();
            if (bgtile != null)
            {
                bgtile.DoDamage();
            }
        }



        foreach (var item in blockSelectedList)
        {
            if (item != null)
            {
                item.DestroyObject();
            }

        }
    }


    private void FixedUpdate()
    {
        if (!isTriggerEnter && !boardManager.HasTurn && isRaycast)
        {
            RaycastHit2D obs = Physics2D.Raycast(transform.position, Vector3.forward,
                                          float.PositiveInfinity, layerMaskObs);
            if (!obs)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.forward,
                                               float.PositiveInfinity, layerMask);
                if (hit && !isObstacle)
                {
                    TriggerEnter(hit.transform.gameObject);
                }
            }
            else
            {
                isObstacle = true;
                DisableCrush(obs.transform.gameObject);
            }
        }
    }

    public void SetUpCollider(bool status)
    {
        isRaycast = status;
        collider2D.enabled = status;
    }

    public void SpecialCrushSetUp(CrushTileType crushtype, bool isMod = false)
    {
        circleCollider = GetComponent<CircleCollider2D>();
        thisCrushTileType = crushtype;
        isModBomb = isMod;

        abilityEffectShow.gameObject.SetActive(false);
        effectHolder.SetActive(true);
        circleCollider.enabled = false;
        bombBlast.SetActive(false);
        freezeEffect.SetActive(false);
        magnetEffect.SetActive(false);
        realignEffect.SetActive(false);
        modEffect.SetActive(false);
        if (thisCrushTileType != CrushTileType.Normal)
        {
            float time = 0;
            int spriteIndex = (int)thisCrushTileType - 1;
            Sprite sp = abilityIcon[spriteIndex];
            abilitySpriteRen.sprite = sp;
            if (isModBomb)
                time = 1.5f;

            abilityEffectShow.gameObject.SetActive(true);
            circleCollider.enabled = true;
            Invoke(nameof(SpecialCrushEffect), time);

        }
        else
        {
            effectHolder.SetActive(false);

        }
        SetUp();
    }

    private void SpecialCrushEffect()
    {
        modEffect.SetActive(true);
    }

    public void BombCollider(bool status)
    {
        if (circleCollider && thisCrushTileType == CrushTileType.Bomb)
        {
            circleCollider.enabled = status;
        }
    }
}