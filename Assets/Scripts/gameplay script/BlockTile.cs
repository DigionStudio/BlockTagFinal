using DG.Tweening;
using UnityEngine;


public class BlockTile : MonoBehaviour
{
    public bool HasBlockSelected { get { return isBlockSelected; } private set { } }
    private bool isBlockSelected;
    public bool isColorBlockSelected;
    public Transform rayPoint;
    [SerializeField] private SpriteRenderer thisSprite;
    public BlockType ThisBlockType { get { return thisBlockType; } private set { } }
    private BlockType thisBlockType;
    [SerializeField] private GameObject blockerObj;
    [SerializeField] private GameObject highlighterObj;
    [SerializeField] private GameObject modEffect;
    [SerializeField] private AudioSource realignSfx;

    public int RowValue { get { return rowValue; } }
    public int ColumnValue { get { return columnValue; } }
    public int ColorCode { get { return colorCode; } }
    public bool HasAsists { get { return isAIAsists; } }
    private bool isAIAsists;
    private int rowValue;
    private int columnValue;
    private int colorCode;
    private Color thisColor = new Color(1, 1, 1, 1f);
    private Color thisSelectedColor = new Color(1, 1, 1, 0.5f);
    private GameObject effectObj;
    private GameObject abilityShowObject;
    private GameObject abilityEffectObj;
    private GameObject coinObject;
    private BlockTileData thisTileData;
    private BoardManager boardManager;

    private void Start()
    {
        blockerObj.SetActive(false);
        modEffect.SetActive(false);
        boardManager = BoardManager.Instance;
    }

    public void BlockTileSetUp(BlockTileData blockData, GameObject blockEffect, BlockType type, GameObject abilityShow, GameObject abilityEffect, GameObject coinobject, int row, int col, int colorcode)
    {
        isBlockSelected = false;
        blockerObj.SetActive(false);
        coinObject = coinobject;
        thisTileData = blockData;
        rowValue = row;
        columnValue = col;
        colorCode = colorcode;
        effectObj = blockEffect;
        thisSprite.sprite = thisTileData.blockIconSprite;
        effectObj.SetActive(false);
        NormalColor();
        AbilityEffectsSetUp(type, abilityShow, abilityEffect);
        ActiveAbilities(false);
        AbilityShow(true);
        
    }

    public void ChangeBlockTypeSetUp(bool isasists, BlockTileData blockData, GameObject blockEffect, BlockType type, GameObject abilityShow, GameObject abilityEffect, int colorcode)
    {
        isAIAsists = isasists;
        thisTileData = blockData;
        thisSprite.sprite = thisTileData.blockIconSprite;

        if(effectObj)
            Destroy(effectObj);
        effectObj = blockEffect;
        effectObj.SetActive(false);

        if (abilityShowObject)
            Destroy(abilityShowObject);

        if (abilityEffectObj)
            Destroy(abilityEffectObj);

        AbilityEffectsSetUp(type, abilityShow, abilityEffect);

        colorCode = colorcode;
        NormalColor();
        ActiveAbilities(false);
        AbilityShow(true);
    }

    public void AbilityEffectsSetUp(BlockType type, GameObject abilityShow, GameObject abilityEffect)
    {
        thisBlockType = type;
        abilityShowObject = abilityShow;
        abilityEffectObj = abilityEffect;
        if (abilityEffectObj != null)
        {
            ParticleColor pcolor = abilityEffectObj.GetComponent<ParticleColor>();
            if (pcolor != null)
            {
                pcolor.ChangeParticleColor(thisTileData.effectColor);
            }
        }
    }

    public void SelectedColor()
    {
        thisSprite.color = thisSelectedColor;
    }

    public void NormalColor()
    {
        thisSprite.color = thisColor;
    }

    public void ColorBombMod()
    {
        if(thisBlockType == BlockType.Normal_Block && !HasBlockSelected)
        {
            int num = Random.Range(-1, 3);
            if(num > 0)
            {
                BlockManager.Instance.ColorBombMod(num, this);
            }
            modEffect.SetActive(true);
        }
        Invoke(nameof(DestroyObject), 0.5f);
    }

    public void DestroyObject()
    {
        if (!isBlockSelected)
        {
            isBlockSelected = true;
            transform.parent = null;

            modEffect.SetActive(false);
            blockerObj.SetActive(true);
            DisableCoin();
            AbilityShow(false);
            ActiveAbilities(false);
            effectObj.SetActive(true);
            thisSprite.gameObject.SetActive(false);
            if (thisBlockType != BlockType.Normal_Block && thisBlockType != BlockType.None)
            {

                ActiveAbilities(true);

                if (thisBlockType == BlockType.Row_Col_Crusher)
                {
                    boardManager.Row_Crush(rowValue);
                    boardManager.Col_Crush(columnValue);
                }
                else if (thisBlockType == BlockType.Row_Crusher)
                {
                    boardManager.Row_Crush(rowValue);
                }
                else if (thisBlockType == BlockType.Col_Crusher)
                {
                    boardManager.Col_Crush(columnValue);
                }
                else if (thisBlockType == BlockType.Color_Bomb)
                {
                    boardManager.Color_Bomb(colorCode, transform.position);
                    CrushTileCreator.ColorBombType.Invoke(thisBlockType);
                }
                else if (thisBlockType == BlockType.Area_Crush)
                {
                    boardManager.Area_Crush(rowValue, columnValue);
                }
            }
            GameManager.BlockDes.Invoke(thisTileData.block, thisBlockType, Special_Object_Type.none, transform.position);
            Invoke(nameof(DisableObj), 1f);
        }
    }

    public void GameOverSplash(bool isAbility = false)
    {
        isBlockSelected = true;
        ActiveAbilities(isAbility);
        AbilityShow(false);
        effectObj.SetActive(true);
        thisSprite.gameObject.SetActive(false);
        Invoke(nameof(DisableObj), 0.5f);

    }

    private void AbilityShow(bool status)
    {
        if(abilityShowObject != null)
        {
            abilityShowObject.SetActive(status);
        }
    }

    private void ActiveAbilities(bool status)
    {
        if (abilityEffectObj != null)
        {
            abilityEffectObj.SetActive(status);
        }
    }

    private void DisableCoin()
    {
        if(coinObject != null)
        {
            coinObject.SetActive(false);
        }
    }

    private void DisableObj()
    {
        if (gameObject != null)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }

    //private void FixedUpdate()
    //{
    //    if (isBlockSelected)
    //    {
    //        SelectedColor();
    //    }
    //    else
    //    {
    //        NormalColor();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Limit") && thisSprite.gameObject.activeInHierarchy)
        {
            BoardManager.Instance.GameEnd(1);
        }
        if (collision.gameObject.CompareTag("Destroy") && thisSprite.gameObject.activeInHierarchy)
        {
            GameOverSplash(true);
        }

        if (collision.gameObject.CompareTag("Destruction") && thisSprite.gameObject.activeInHierarchy)
        {
            DestroyObject();
        }
        if (collision.gameObject.CompareTag("Disturb") && thisSprite.gameObject.activeInHierarchy)
        {
            if (!highlighterObj.activeInHierarchy)
            {
                highlighterObj.transform.localScale = Vector3.zero;
                highlighterObj.SetActive(true);
                highlighterObj.transform.DOScale(Vector2.one * 1.1f, 0.7f).OnComplete(() =>
                {
                    Invoke(nameof(HighlighterDisable), 1f);

                });
            }
        }
    }
    private void HighlighterDisable()
    {
        highlighterObj.SetActive(false);
    }

    //private void OnMouseDown()
    //{
    //    //if (AbilityManager.Instance.CheckAbilityActive(transform.position))
    //    //{
    //    //    print("Ability activate");
    //    //    AbilityManager.Instance.ActivateAbility();
    //    //}
    //}



    public void Re_Oreder(Vector2 pos, int row, int col, bool isMagnet = false)
    {
        transform.position = pos;
        rowValue = row;
        columnValue = col;
        if(!isMagnet)
            realignSfx.Play();
    }
}
