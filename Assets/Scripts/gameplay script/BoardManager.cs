using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

[Serializable]
public enum BlockType
{
    Normal_Block,
    Row_Crusher,
    Col_Crusher,
    Row_Col_Crusher,
    Area_Crush,
    Color_Bomb,
    None
}





public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance;
    private int gameTypeCode = 1;

    [SerializeField] private BgTile bgBlockPrefab;
    [SerializeField] private Transform bgHolder;
    [SerializeField] private LayerMask bgObsLayer;
    private float bgDiff = 2.5f;
    private Vector2 bgPos = Vector2.zero;
    private int bgWidth = 9;
    private int bgHeight = 12;
    private List<BgTile> bgTiles = new List<BgTile>();
    private BgTileData[] bgTileData;


    private BlockManager blockManager;
    public GameDataManager gameDataManager;
    [SerializeField] private Transform holderObj;
    private int gemIndex = -1;
    private int gemTargetIndex = -1;
    private readonly float diff = 2.5f;
    private readonly float width = 9;
    private float height = 0;
    private int index = 0;
    private int count = 0;
    private float posY = 6;
    private float currentWidth = 0;
    private bool isTurn;
    private int totalCount = 4;
    private int currentCount;
    private Vector2 dashMovePos;
    private Tween dashTween;
    public bool HasTurn { get { return isTurn; } }
    private int totalBlockDes = 0;
    public int TotalBlockDes { set { totalBlockDes += value; } }
    private int shapeIndex;

    private bool isMove;
    private bool isDashMove;
    private List<BlockTile> blockList = new List<BlockTile>();
    private Transform startBlockTrans;
    private Transform blockTrans;
    private List<GemTile> gemList = new List<GemTile>();
    //private float speed = f;
    //private float currentSpeed;
    private bool isBlockSetUp;

    private int currentColorCode;

    private int abilityDiff1 = 5;
    private int abilityDiff2 = 5;
    private bool isMoving;
    private float moveSpeed = 0.15f;
    private bool isFreezeMove;
    public bool HasFreezed { get { return isFreezeMove; } }
    private float totalFreezeTime = 20f;
    private float currentFreezeTime;
    private bool isRandom;


    private bool isGameEnd;
    private bool isGameWin;
    private int desCount;
    private int gameEndID = -1;


    //public Transform limitTrans;
    //public RectTransform limitRectTrans;

    [SerializeField] private AudioSource dragSFX;
    [SerializeField] private GameManager gameManager;
    private ShapeCreator shapeCreator;
    [SerializeField] private UiManager uiManager;
    [SerializeField] private TextEffect textEffect;
    private Vector2 textEffectPos;
    private int showTextIndex;
    private bool isEfectActive;
    public static Action<Vector2> OnCrushTileDisable = delegate { };
    public bool isGameStarted { get; private set; }


    public AudioSource blockSlideAbilitySFX;

    //slime tile values
    private bool isSlimeHit;
    private int totalValues = 4;
    private int currentUnHitValue;
    public LayerMask layerMaskTile;

    public ColorBombEffect colorbombEffect;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (BlockManager.Instance)
        {
            gameDataManager = BlockManager.Instance.gameDataManager;
        }
    }

    public List<Transform> CurrentBgTile()
    {
        List<Transform> bgtileTranns = new List<Transform>();
        int count = 10;
        for (int i = bgTiles.Count - 1; i >= 0 ; i--)
        {
            BgTile bg = bgTiles[i];
            if (count > 0)
            {
                if (bg != null)
                {
                    int typeCode = (int)bg.BlockTypeCode;
                    if (!bg.HasDestroyed && typeCode > 6 && typeCode <= 12)
                    {
                        bgtileTranns.Add(bg.transform);
                        count--;
                    }
                }
            }
            else
            {
                break;
            }
        }
        foreach (var item in blockList)
        {
            if (count > 0)
            {
                if (item != null && item.gameObject.activeInHierarchy)
                {
                    bgtileTranns.Add(item.transform);
                    count--;
                }
            }
            else
            {
                break;
            }
        }
        
        return bgtileTranns;
    }

    public void ChangeBgBlockStatus(BgTileData[] bgtileData)
    {
        bgTileData = bgtileData;

        for (int i = 0; i < bgTiles.Count; i++)
        {
            BgTileData tiledata = null;
            for (int d = 0; d < bgTileData.Length; d++)
            {
                if (bgTileData[d].positionIndex == i)
                {
                    tiledata = bgTileData[d];
                    break;
                }
            }
            bgTiles[i].SetUpNewBgTileData(tiledata);
        }
    }
    private void InstaBgBlocks()
    {
        bgPos = bgHolder.position;
        int count = 0;
        bool isLimit;

        for (int j = bgHeight - 1; j >= 0; j--)
        {
            if (j == 0 && gameDataManager.GetSaveValues(5) >= 0)
            {
                isLimit = true;
            }
            else
            {
                isLimit = false;
            }
           

            for (int i = 0; i < bgWidth; i++)
            {
                Vector2 pos = bgPos + new Vector2(bgDiff * i, bgDiff * j);
                BgTile bg = Instantiate(bgBlockPrefab, bgHolder);
                bg.transform.position = pos;

                BgTileData tiledata = null;
                if (gameTypeCode == 1)
                {
                    for (int d = 0; d < bgTileData.Length; d++)
                    {
                        if (bgTileData[d].positionIndex == count)
                        {
                            tiledata = bgTileData[d];
                            break;
                        }
                    }
                }
                bg.SetUpTile(isLimit, tiledata);
                bgTiles.Add(bg);
                count++;
            }
        }
        
    }

    private void BgTileSetUp(bool status)
    {
        if(bgTiles.Count <= 0)
        {
            InstaBgBlocks();
        }
        foreach (var item in bgTiles)
        {
            item.TileStatus(status);
        }
    }

    
    private void OnEnable()
    {
        OnCrushTileDisable += GetCrushPos;
        BgTile.OnDamageTaken += SlimeHitCheck;
    }

    void Start()
    {
        shapeCreator = ShapeCreator.Instance;
        blockManager = BlockManager.Instance;
    }

    public void StartGame(int gametypecode, BgTileData[] bgtileData, int abilityvalue1, int abilityvalue2, bool isslime, float movespeed)
    {
        isRandom = TutorialManager.Instance.isTutorial;
        bgTileData = bgtileData;
        gameTypeCode = gametypecode;
        BgTileSetUp(true);

        isSlimeHit = isslime;
        currentUnHitValue = totalValues;
        isDashMove = true;
        currentCount = 0;
        shapeCreator.isSetUp = true;
        isTurn = false;
        holderObj.position = Vector2.up * 17f;
        isBlockSetUp = true;
        posY = holderObj.position.y;
        blockList.Clear();
        gemList.Clear();
        isMoving = true;
        
        if (gameTypeCode == 1)
        {
            abilityDiff1 = abilityvalue1;
            abilityDiff2 = abilityvalue2;
            moveSpeed = movespeed;
            if (movespeed == 0)
            {
                isMoving = false;
            }
        }
        InvokeRepeating(nameof(BoardInsta), 0, 0.03f);
        Invoke(nameof(ShowAllBlockAnim), 1f);
        

    }

    public void BoardPosStart()
    {
        GameStatus(true);
        textEffect.StartGame();
        MoveHolder();
    }
    
    public void GameStatus(bool status)
    {
        isGameStarted = status;
        isMove = status;
    }

    public void ChangeMoveStatus(bool status)
    {
        if(isGameStarted)
            isMove = status;
    }

    public void LifeActivate()
    {
        count = 0;
        ClearEmptyBlockTile();
        CancelInvoke(nameof(GameEndBlockDes));
        GameStatus(true);
        if(gameEndID == 0)
            gameManager.MoveCountsetUp();
        gameEndID = -1;
    }
    
    public void GameEnd(int code)
    {
        if (gameEndID != code)
        {
            count = 0;
            gameEndID = code;
            if (gameTypeCode == 1)
            {
                bool gameStatus = gameManager.TargetStatus();
                if (!gameStatus)
                {
                    
                    GameStatus(false);

                    bool status = BuyPanel.Instance.CheckForGameOverBuyPanel();
                    if (!status)
                    {
                        UserGameEnd();
                    }
                    else
                    {
                        desCount = CalculateBoardTiles(0);
                        DestroyGems();
                        if (gameEndID == 1)
                            InvokeRepeating(nameof(GameEndBlockDes), 0, 0.05f);
                    }
                    gameManager.gameEndPaneldisable(status);
                }
            }
            else
            {
                UserGameEnd();
            }
        }
    }

    public void UserGameEnd()
    {
        CancelInvoke(nameof(GameEndBlockDes));
        index = UnityEngine.Random.Range(0, 5);
        gameManager.GameEndFailed();
    }

    public void GameOver(bool isGamewin)
    {
        TutorialManager.Instance.CheckTutorialStatus();
        GameAIManager.Instance.DisableEffectObject();
        isGameWin = isGamewin;
        isDashMove = false;
        count = 0;
        textEffect.GameEnd();
        isBlockSetUp = false;
        StopAllCoroutines();
        shapeCreator.isSetUp = false;
        shapeCreator.GameReset(false);
        uiManager.GameEndsetup();
        GameStatus(false);
        isGameEnd = true;
        desCount = CalculateBoardTiles(0);
        if (gameTypeCode == 1)
        {
            gameManager.GameEndMoveDecrese();

        }
        else
        {
            Invoke(nameof(GameEnded), 3f);

        }
        if((isGamewin && gameTypeCode == 1) || gameTypeCode == 0)
            gameManager.UpdatePointToLeaderboard();
        InvokeRepeating(nameof(GameEndBlockDes), 0, 0.05f);
    }
    void GameEndBlockDes()
    {
        for (int i = 0; i < blockList.Count; i++)
        {
            BlockTile item = blockList[i];
            if (count < desCount)
            {
                if (!isGameEnd)
                {
                    if (item != null && item.transform.position.y < 3 && !item.HasBlockSelected)
                    {
                        item.GameOverSplash();
                        count++;
                        break;
                    }
                }
                else
                {
                    if (item != null && !item.HasBlockSelected)
                    {
                        if (isGameWin)
                        {
                            item.DestroyObject();
                        }
                        else
                        {
                            item.GameOverSplash();
                        }
                        count++;
                        break;
                    }
                }
            }
            else
            {
                CancelInvoke(nameof(GameEndBlockDes));
                DestroyGems();
                if (isGameEnd)
                    Invoke(nameof(GameEnded), 1f);
                break;
            }
        }
    }
    private void DestroyBgBlocks()
    {
        foreach (var bg in bgTiles)
        {
            bg.DestroyObj();
        }
        bgTiles.Clear();
    }

    private void GameEnded()
    {
        if (!isGameEnd) return;
        print("ended");
        DestroyBgBlocks();
        GameStatus(false);
        count = 0;
        currentCount = 0;
        isGameEnd = false;
        gameManager.GameOver();
        CancelInvoke(nameof(GameEndBlockDes));
        foreach (var item in blockList)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        blockList.Clear();


        foreach (var item in gemList)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        gemList.Clear();
    }
    private void OnDisable()
    {
        OnCrushTileDisable -= GetCrushPos;
        BgTile.OnDamageTaken -= SlimeHitCheck;

    }

    private void GetCrushPos(Vector2 pos)
    {
        textEffectPos = pos;
    }

    private void ShowAllBlockAnim()
    {
        shapeCreator.SetUp();
        TutorialManager.Instance.ButtonPressStart(0);
        //MaskSetUp();
        Invoke(nameof(StopblockSetUp), 1.5f);
    }
    private void StopblockSetUp()
    {
        isBlockSetUp = false;
    }

    private void BlockBoardSetUp()
    {
        isBlockSetUp = true;
        InvokeRepeating(nameof(BoardInsta), 0, 0.05f);
        Invoke(nameof(StopblockSetUp), 2f);
    }
    private void BlockReInstantiate()
    {
        isBlockSetUp = false;
    }

    private void BoardInsta()
    {
        if (startBlockTrans != null)
        {
            posY = startBlockTrans.position.y;
            posY += diff;
        }
        int random = UnityEngine.Random.Range(0, 3);
        if (isBlockSetUp)
        {
            if (random != 1 || isRandom)
            {
                float posX;
                int colorIndex = 0;
                gemIndex = -1;
                posX = diff * currentWidth;
                Vector2 pos = new Vector2(posX, posY);
                if ((!CheckGem() || height == 0))
                {
                    colorIndex = BlockTypeColorSetUp(false);
                    int index = BlockAbilitySetUp((int)height, (int)currentWidth, colorIndex);
                    if (colorIndex == 5)
                    {
                        index = 6;
                    }
                    BlockTile tile = blockManager.InstatiateBlockObject(pos, holderObj, colorIndex, index, (int)height, (int)currentWidth);
                    blockList.Add(tile);
                    blockTrans = tile.transform;
                }
                else
                {
                    InstaGem(pos, gemIndex, (int)height, (int)width);
                }
            }
            if (currentWidth < width - 1)
            {
                currentWidth++;
            }
            else
            {
                if(blockTrans != null)
                    startBlockTrans = blockTrans;
                currentWidth = 0;
                height++;
            }
        }
        else
        {
            GameAIManager.Instance.AiAssists((int)height - 1, blockList, gemList);
            CancelInvoke(nameof(BoardInsta));
        }
    }
    public void InstaGem(Vector2 pos, int gemindex, int height, int width)
    {
        GemTile tile = blockManager.InstantiateGemTile(pos, holderObj, gemindex, height, width);
        gemList.Add(tile);
    }

    private int BlockTypeColorSetUp(bool isMod)
    {
        int num = UnityEngine.Random.Range(0, 6);
        index = num;
        if (num == 5 && (gameTypeCode == 1 || isMod))
        {
            index = UnityEngine.Random.Range(0, 5);
        }
        
        return index;
    }

    private int BlockAbilitySetUp(int row, int column, int code)
    {
        int num = 0;
        if (code == 5)
        {
            return num;
        }
        int randomIndex = UnityEngine.Random.Range(0, 120);
        if(randomIndex >= 0 && randomIndex < abilityDiff2 ) {
            num = 3;
           
        }
        else if (randomIndex >= 20 && randomIndex < abilityDiff1 + 20)
        {
            num = 1;

        }
        else if (randomIndex >= 40 && randomIndex < abilityDiff1 + 40)
        {
            num = 2;
        }
        else if (randomIndex >= 60 && randomIndex < abilityDiff2 + 60)
        {
            num = 4;
            
        }
        else if (randomIndex >= 80 && randomIndex < abilityDiff2 + 80)
        {
            num = 5;
        }
        return num;

    }

    private bool CheckGem()
    {
        bool isActive = false;
        int number = UnityEngine.Random.Range(0, 300);
        if (number >= 0 && number < 100)
        {
            if (gameDataManager.levelData.gemTileData != null && gameDataManager.levelData.gemTileData.Length > 0 && gameTypeCode == 1)
            {
                for (int i = 0; i < gameDataManager.levelData.gemTileData.Length; i++)
                {
                    var item = gameDataManager.levelData.gemTileData[i];
                    if (number >= item.spawnpercentmin && number < item.spawnpercentmax)
                    {
                        isActive = true;
                        gemIndex = (int)item.gemType - 1;
                        gemTargetIndex = i;
                    }
                }
            }
        }
        return isActive;

    }

    

    private BlockTile AbilityTile(int row,int col)
    {
        BlockTile thisTile = blockList[0];
        foreach (var item in blockList)
        {
            if (item.RowValue == row && item.ColumnValue == col)
            {
                thisTile = item;
                break;
            }
        }
        return thisTile;
    }

    //private void MaskSetUp()
    //{
    //    // Convert the position from RectTransform to Transform
    //    Vector3 transformPosition = limitRectTrans.TransformPoint(limitRectTrans.rect.center);
    //    // Optionally, you can convert rotation and scale as well
    //    Quaternion transformRotation = limitRectTrans.rotation;
    //    Vector3 transformScale = limitRectTrans.lossyScale;
    //    limitTrans.position = transformPosition;
    //    // Now you can use transformPosition, transformRotation, and transformScale
    //    // for the corresponding Transform.
    //}

    

    public void BoardTurn(int index)
    {
        shapeIndex = index;
        Turn();
    }

    private void BlockAvailableCheck()
    {
        if (!isBlockSetUp)
        {
            int availablecount = CalculateBoardTiles(1);
            if (availablecount < 7)
            {
                BlockBoardSetUp();
            }
        }
    }

    private void MoveHolder()
    {
        isEfectActive = false;
        int count = CalculateBoardTiles(3);
        float num = 0f;
        if (count == 0)
        {
            num = 10f;
        }
        else
        {
            if (count > 0 && count <= 7)
            {
                if (count > 0 && count <= 2)
                {
                    num = 6f;
                }
                else if (count > 2 && count <= 4)
                {
                    num = 4f;
                }
                else
                {
                    num = 1.5f;
                }
                if (isFreezeMove && !CheckForBlockAvaiable())
                {
                    num = 0;
                }
                currentCount = 0;
            }
            else
            {
                if (currentCount < totalCount)
                {

                    currentCount++;
                }
                else
                {
                    currentCount = 0;
                    if (gameTypeCode == 0)
                    {
                        moveSpeed += 0.01f;
                    }
                }

            }
        }
        DashMove(num);
    }

    private bool CheckForBlockAvaiable()
    {
        bool ismove = false;
        List<BlockTile> availableBlocks = BlockElements(-1, 3);
        int count = 0;
        foreach (BlockTile blockTile in availableBlocks)
        {
            if (!CollderStatus(blockTile.transform.position))
            {
                count++;
            }
        }
        if(count < 4)
        {
            ismove = true;
        }
        return ismove;
    }

    private void DashMove(float num)
    {
        if (isDashMove)
        {
            isDashMove = false;

            if (num > 0f)
                blockSlideAbilitySFX.Play();
            dashMovePos = (Vector2)holderObj.position - Vector2.up * num;
            
        }
        else
        {
            dashTween.Kill();
            dashMovePos -= Vector2.up * num;

        }
        dashTween = holderObj.DOMove(dashMovePos, 1f).OnComplete(() =>
        { 
            isDashMove = true;
            dashMovePos = Vector2.zero;

        });
    }

    //private void SpeedUp()
    //{
    //    float addValue = 0.0003f;
    //    if (gameTypeCode == 0)
    //    {
    //        addValue = 0.0005f;
    //    }
    //    if (currentSpeed < 0.01f)
    //        currentSpeed += addValue;
    //}

    private void FixedUpdate()
    {
        if (isMoving && !isFreezeMove) 
        {
            if (isMove && isGameStarted)
            {
                holderObj.Translate(-Vector2.up * moveSpeed * Time.deltaTime);
                if (!isGameEnd)
                {
                    if (currentFreezeTime <= 3)
                    {
                        currentFreezeTime += Time.deltaTime;
                    }
                    else
                    {
                        currentFreezeTime = 0;
                        BlockAvailableCheck();
                    }
                }
            }

        }
        else
        {
            if (isFreezeMove)
            {
                if (currentFreezeTime <= totalFreezeTime)
                {
                    currentFreezeTime += Time.deltaTime;
                    FreezeStart();
                }
                else
                {
                    isFreezeMove = false;
                    FreezeOver();
                }
            }
        }
    }

    private void Turn()
    {
        if (isTurn == true) return;
        List<BlockTile> availableBlocks = BlockElements(-1, 3);
        GameAIManager.Instance.CheckForBombAIAsist(availableBlocks);
        BlockAvailableCheck();
        isTurn = true;

    }

    public void CallUnturn()
    {
        Invoke(nameof(UnTurn), 0.8f);
    }

    private void UnTurn()
    {
        if (isTurn == false) return;
        isTurn = false;
        ShowEffect(totalBlockDes);
        UpdatePoint(totalBlockDes);
        totalBlockDes = 0;
        gameManager.TotalCoinDesEffect();
        gameManager.HasAbility = false;
        shapeCreator.CreateTile(shapeIndex);
        GameManager.OnMoveTaken.Invoke();

        //SpeedUp();
        Invoke(nameof(MoveHolder), 0.5f);
        Invoke(nameof(ClearEmptyBlockTile), 1f);
        if(gameTypeCode == 1)
        {
            CheckSlimeHitStatus();
        }

    }

    public void UpdatePoint(int num)
    {
        gameManager.CalculatePoints(num, true);
    }

    public bool HasMove()
    {
        return gameManager.CheckMoveCount();
    }

    private int CalculateBoardTiles(int code)
    {
        int count = 0;
        int startrow = 0;
        int endrow = 0;
        int loopCount = 0;

        if (code == 3 || code == 1)
        {
            int maxitter = 1;
            int blockcount = blockList.Count;
            while (maxitter < 15)
            {
                if(blockList[blockcount - maxitter] == null)
                {
                    maxitter++;
                }
                else
                {
                    startrow = blockList[blockcount - maxitter].RowValue;
                    endrow = blockList[blockcount - maxitter].RowValue;
                    break;
                }
            }
            for (int i = 0; i < blockList.Count; i++)
            {
                var item = blockList[i];
                if (item != null && item.gameObject.activeInHierarchy && !item.HasBlockSelected)
                {
                    if (loopCount < 10)
                    {
                        if (code == 1)
                        {
                            if (item.transform.position.y > 16)
                            {
                                loopCount++;
                                if (item.RowValue < startrow)
                                {
                                    loopCount = 0;
                                    startrow = item.RowValue;
                                }
                            }
                            else
                            {
                                loopCount = 0;
                            }

                        }
                        else
                        {
                            if (item.transform.position.y <= 16)
                            {
                                loopCount = 0;
                                if (item.RowValue < startrow)
                                {
                                    startrow = item.RowValue;
                                }
                            }
                            else
                            {
                                loopCount++;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    
                }
            }
        }

        if (code != 1)
        {
            loopCount = 0;
            endrow = 0;
            for (int i = 0; i < blockList.Count; i++)
            {
                var item = blockList[i];
                if (item != null && item.gameObject.activeInHierarchy && !item.HasBlockSelected)
                {
                    if (loopCount < 10)
                    {
                        if (code == 0)
                        {
                            if (item.transform.position.y <= 16)
                            {
                                count++;
                                loopCount = 0;
                            }
                            else
                            {
                                loopCount++;
                            }
                        }else if (code == 4)
                        {
                            if (item.transform.position.y <= 16)
                            {
                                if(i > count)
                                    count = i;
                                loopCount = 0;
                            }
                            else
                            {
                                loopCount++;
                            }
                        }
                        else
                        {
                            if(item.transform.position.y <= 16)
                            {
                                loopCount = 0;
                                if (item.RowValue > endrow)
                                {
                                    endrow = item.RowValue;
                                }
                            }
                            else
                            {
                                loopCount++;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }

                }
            }
        }
        if(code == 0 || code == 4)
        {
            return count;
        }
        else if (code == 2)
        {
            return endrow;
        }
        else
        {
            int num = endrow - startrow;
            if(num < 0)
            {
                num = 0;
            }
            return num;
        }
    }

    private int DestroyGems(bool isIndex = false)
    {
        int index = 0;
        for (int i = 0; i < gemList.Count; i++)
        {
            var item = gemList[i];
            if (item != null && item.gameObject.activeInHierarchy)
            {
                if (item.transform.position.y < 16)
                {
                    if (isIndex)
                    {
                        if (i > index)
                        {
                            index = i;
                        }
                    }
                    else
                    {
                        item.DestroyGem();
                    }
                }
                else
                {
                    break;
                }
            }
        }
        return index;
    }



    public void Row_Crush(int value)
    {
        List<BlockTile> rowElements = BlockElements(value, 0);
        foreach (var item in rowElements)
        {
            if (item != null)
            {
                item.DestroyObject();
            }
        }
    }

    public void Col_Crush(int value)
    {
        List<BlockTile> colElement = BlockElements(value, 1);
        foreach (var item in colElement)
        {
            if (item != null)
            {
                item.DestroyObject();
            }
        }
    }

    public void Color_Bomb(int value, Vector2 pos)
    {
        List<BlockTile> elements = BlockElements(value, 2);
        if (elements.Count > 0)
        {
            foreach (var item in elements)
            {
                if (item != null)
                {
                    ColorBombEffect effect = Instantiate(colorbombEffect, transform);
                    effect.transform.position = new Vector3(pos.x, pos.y, 0);
                    effect.TweenMove(item.transform.position, item);
                }
            }
        }
    }

    public void Area_Crush(int row, int col)
    {
        List<BlockTile> elements = AreaBlock(row, col);
        foreach (var item in elements)
        {
            if (item != null)
            {
                item.DestroyObject();
            }
        }
    }

    private void ShowEffect(int num)
    { 
        if (isEfectActive)
            return;
        isEfectActive = true;
        if (num >= 12)
        {
            int maxitter = 50;
            int code = UnityEngine.Random.Range(0, 5);
            while (true)
            {
                if (maxitter <= 0) break;
                else
                {

                    if (code == showTextIndex)
                    {
                        code = UnityEngine.Random.Range(0, 5);
                        maxitter--;
                    }
                    else
                    {
                        showTextIndex = code;
                        break;
                    }

                }
            }
            if (showTextIndex >= 0)
                textEffect.ShowEffect(showTextIndex, textEffectPos);
        }
    }

    private List<BlockTile> BlockElements(int value, int code)
    {
        var list = new List<BlockTile>();
        int totalCount = 100;
        if(blockList.Count < 100)
        {
            totalCount = blockList.Count;
        }
        int loopCount = 0;
        for (int i = 0; i < totalCount; i++)
        {
            var item = blockList[i];
            if (item != null && !item.HasBlockSelected && item.gameObject.activeInHierarchy && !item.isColorBlockSelected)
            {
                if (loopCount < 10)
                {
                    if (item.transform.position.y <= 16)
                    {
                        loopCount = 0;
                        if (value >= 0)
                        {
                            if (code == 0 && item.RowValue == value)
                            {
                                list.Add(item);
                            }
                            else if (code == 1 && item.ColumnValue == value)
                            {
                                list.Add(item);
                            }
                            else if (code == 2 && item.ColorCode == value)
                            {
                                list.Add(item);
                                item.isColorBlockSelected = true;
                            }
                        }
                        else
                        {
                            list.Add(item);
                        }
                    }
                    else
                    {
                        loopCount++;
                    }
                }
                else
                {
                    break;
                }
            }
        }
        if (code < 3)
        {
            gameManager.HasAbility = true;
            if (isTurn)
                totalBlockDes += list.Count;
        }
        return list;
    }

    private List<BlockTile> AreaBlock(int row, int column)
    {
        var list = new List<BlockTile>();
        int colmin = 0;
        int colmax = 8;
        if(column > 1 && column < 8)
        {
            colmin = column - 2;
            colmax = column + 2;
        }
        else
        {
            if(column <= 1)
            {
                colmax = column + 3;
            }
            if(column >= 7)
            {
                colmin = column - 3;
            }
        }
        for (int i = colmin; i <= colmax; i++)
        {
            for (int j = row - 2; j <= row + 2; j++)
            {
                foreach (var item in blockList)
                {
                    
                    if (item != null && !item.HasBlockSelected && item.gameObject.activeInHierarchy)
                    {
                        if (item.transform.position.y < 15)
                        {
                            if (item.ColumnValue == i && item.RowValue == j)
                            {
                                list.Add(item);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                
            }
        }
        gameManager.HasAbility = true;
        if (isTurn)
            totalBlockDes += list.Count;
        return list;
    }

    private void ClearEmptyBlockTile()
    {
        for (int i = 0; i < 100; i++)
        {
            if (!isGameEnd && blockList.Count > i)
            {
                var item = blockList[i];
                if (item == null)
                {
                    blockList.RemoveAt(i);
                }
            }
            else
            {
                break;
            }
        }
    }

    public void AddMoves(int num, int coin)
    {
        gameManager.AddMoves(num, coin);
    }

    public void Re_Alignr_Ability()
    {
        StartCoroutine(GetStarting());
    }

    private IEnumerator GetStarting()
    {
        int index = CalculateBoardTiles(4);

        dragSFX.Play();
        yield return new WaitForSeconds(0.01f);
        GameStatus(false);
        for (int j = index; j > 0; j--)
        {
            BlockTile tile = blockList[j];
            if (tile != null)
            {
                Vector2 currentTilePos = new Vector2(tile.transform.position.x, tile.transform.position.y);
                int width = tile.ColumnValue;
                int height = tile.RowValue;
                int nextWidth = width - 1;
                int nextHeight = height;
                float Xmul = -1;
                float Ymul = 0;
                if (width == 0 || nextWidth < 0)
                {
                    nextWidth = 8;
                    Xmul = 8;
                    if (nextHeight > 0)
                    {
                        nextHeight--;
                        Ymul = -1;
                    }
                }

                int maxitter = 10;
                while(maxitter > 0)
                {
                    
                    Vector2 pos = currentTilePos + new Vector2(diff * (float)Xmul, diff * Ymul);
                    if (CheckGemPos(pos))
                    {
                        nextWidth -= 1;
                        Xmul -= 1;
                        if (nextWidth < 0)
                        {
                            nextWidth = 8;
                            Xmul = 8;
                            if (nextHeight > 0)
                            {
                                nextHeight--;
                                Ymul = -1;
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                    maxitter--;
                }

                Vector2 nextTileCurentPos = currentTilePos + new Vector2(diff * (float)Xmul, diff * Ymul);

                int nextTile = j - 1;
                BlockTile nexttile = blockList[nextTile];
                int maxitter2 = 30;
                while (true)
                {
                    if (maxitter2 > 0 && nexttile == null)
                    {
                        if (nextTile > 0)
                        {
                            nextTile--;
                        }
                        nexttile = blockList[nextTile];
                    }
                    else
                    {
                        break;
                    }
                    maxitter2--;
                }
                Vector2 nextTilePos = Vector2.zero;
                bool iscalculate = false;
                if (nexttile != null)
                {
                    nextTilePos = new Vector2(nexttile.transform.position.x, nexttile.transform.position.y);
                    iscalculate = true;
                }

                float distance = Vector2.Distance(nextTileCurentPos, nextTilePos);
                if (distance > 2 && iscalculate)
                {
                    nexttile.Re_Oreder(nextTileCurentPos, nextHeight, nextWidth);
                    yield return new WaitForSeconds(0.03f);
                }
            }
        }
        StartCoroutine(GemRe_Position());
        yield return new WaitForSeconds(0.2f);
        GameStatus(true);
        //CheckForDash(0.5f);

    }

    private int StartIndex()
    {
        int num = 0;
        int index = CalculateBoardTiles(4);
        int maxrowval = CalculateBoardTiles(2);
        for (int i = 0; i < index; i++)
        {
            BlockTile tile = blockList[i];
            if (tile != null)
            {
                Vector2 pos = tile.transform.position;
                bool isGem = CheckGemPos(pos);
                if (!CollderStatus(pos) && !isGem && tile.ColumnValue >= 0 && tile.ColumnValue < 6 && tile.RowValue < maxrowval)
                {
                    num = i;
                    break;
                }
            }
            
            
        }
        return num;

    }

    private bool CollderStatus(Vector2 pos)
    {
        bool isCollider = false;
        RaycastHit2D hit = Physics2D.CircleCast(pos, 0.3f, Vector2.zero, 0, bgObsLayer);
        if(hit.collider != null)
        {
            isCollider = true;
        }
        return isCollider;
    }
    public Vector2 AbilityTutorialPos()
    {
        Vector2 pos = Vector2.zero;
        int index = StartIndex();
        if (blockList[index] != null)
        {
            pos = blockList[index].transform.position;
        }
        return pos;
    }

    public void MagnaticAbility()
    {
        StartCoroutine(MagneticAbility());
    }
    private IEnumerator MagneticAbility()
    {
        int startindex = CalculateBoardTiles(4);
        for (int i = startindex; i >= 0; i--)
        {
            BlockTile block = blockList[i];
            if(block != null)
            {

                RaycastHit2D obs = Physics2D.Raycast(block.rayPoint.position, Vector3.up,
                                          float.PositiveInfinity, layerMaskTile);
                if (obs && obs.distance > 2)
                {
                    if (obs.transform.TryGetComponent<BlockTile>(out var blockTile))
                    {
                        Vector2 pos;
                        int currentRow = blockTile.RowValue;
                        int val = -1;
                        pos = (Vector2)blockTile.transform.position + new Vector2(0, diff * val);
                        int maxitte = 50;
                        while (maxitte > 0)
                        {
                            maxitte--;
                            if (CheckGemPos(pos))
                            {
                                val--;
                                pos = (Vector2)blockTile.transform.position + new Vector2(0, diff * val);
                            }
                            else
                            {
                                break;
                            }
                        }
                        block.Re_Oreder(pos, currentRow + val, block.ColumnValue);
                        yield return new WaitForSeconds(0.03f);
                    }
                }
            }
        }
        StartCoroutine(GemRe_Position());
    }

    private Vector2 BlockPosY(int row, int starting)
    {
        Vector2 pos = Vector2.zero;
        for (int i = starting; i < blockList.Count; i++)
        {
            if (blockList[i] != null && blockList[i].RowValue == row)
            {
                pos = blockList[i].transform.position;
            }
        }
        return pos;
    }

    private IEnumerator GemRe_Position()
    {
        yield return new WaitForSeconds(0.03f);
        int index = DestroyGems(true);
        if (gemList.Count > 0)
        {
            for (int i = index; i >= 0; i--)
            {
                GemTile gem = gemList[i];
                if (gem != null)
                {
                    RaycastHit2D obs = Physics2D.Raycast(gem.transform.position, Vector3.up,
                                              float.PositiveInfinity, layerMaskTile);
                    if (obs && obs.distance > 2)
                    {
                        if (obs.transform.TryGetComponent<BlockTile>(out var blockTile))
                        {
                            int row = blockTile.RowValue;
                            int column = blockTile.ColumnValue;
                            int val = -1;
                            Vector2 pos = (Vector2)blockTile.transform.position + new Vector2(0, diff * val);
                            int maxitte = 50;
                            while (maxitte > 0)
                            {
                                maxitte--;
                                if (CheckGemPos(pos))
                                {
                                    val--;
                                    pos = (Vector2)blockTile.transform.position + new Vector2(0, diff * val);
                                }
                                else
                                {
                                    break;
                                }
                            }
                            gem.transform.position = pos;
                            gem.Re_Pos(row + val, column);
                            yield return new WaitForSeconds(0.02f);
                        }
                    }
                }
            }
        }
    }
    

    private bool CheckGemPos(Vector2 pos)
    {
        bool isGem = false;
        RaycastHit2D hit = Physics2D.CircleCast(pos, 0.5f, Vector2.zero);
        if(hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Gem")
            {
                isGem = true;
            }
        }
        return isGem;
    }

    public void CheckForDash(float value)
    {
        Invoke(nameof(DashInvoke), value);
    }

    private void DashInvoke()
    {
        BlockAvailableCheck();
        MoveHolder();
        if (gameTypeCode == 1)
            gameManager.CheckGameStatus();
    }


    public void FreezeAbilityActive()
    {
        isFreezeMove = true;
        currentFreezeTime = 0;
        uiManager.SetUpFreeze();

    }
    private void FreezeStart()
    {
        float percent = currentFreezeTime / totalFreezeTime;
        uiManager.FreezeTimer(percent);
    }

    private void FreezeOver()
    {
        uiManager.EndFreeze();
    }


    public List<BlockTile> ThunderAbility(bool isTag)
    {
        List<BlockTile> thunderBlocks = new List<BlockTile>();
        count = 15;
        for (int i = 0; i < blockList.Count; i++)
        {
            if (count > 0)
            {
                BlockTile tile = blockList[i];
                if (tile != null && !tile.HasBlockSelected)
                {
                    thunderBlocks.Add(tile);
                    count--;
                }
            }
            else
            {
                break;
            }
        }
        if(isTag)
            totalBlockDes += thunderBlocks.Count;
        return thunderBlocks;

    }

    private void SlimeHitCheck(Normal_Block_Type type)
    {
        
        if (type == Normal_Block_Type.Slime)
        {
            currentUnHitValue = totalValues;
        }
        
    }

    private void CheckSlimeHitStatus()
    {
        if (isSlimeHit)
        {
            if (currentUnHitValue > 0)
            {
                currentUnHitValue--;
            }
            if (currentUnHitValue == 0)
            {
                isSlimeHit = false;
                currentUnHitValue = totalValues;
                ActiveNewSlimeTile();
            }
        }
    }
    private void ActiveNewSlimeTile()
    {
        foreach (var item in bgTiles)
        {
            if(item.BlockTypeCode == Normal_Block_Type.Slime && item.HasDestroyed)
            {
                item.ResetBgTile();
                gameManager.IncreaseSlimeTileCount();
                break;
            }
        }
    }

    public void ReSetUpBoard()
    {
        StartCoroutine(ReSetUpBoardCO());
    }
    IEnumerator ReSetUpBoardCO()
    {
        float value = holderObj.transform.position.y;
        holderObj.DOMove(Vector2.up * 17f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        foreach (var item in blockList)
        {
            if (item != null && !item.HasAsists && item.ColorCode < 5)
            {
                int colorIndex = BlockTypeColorSetUp(true);
                int index = BlockAbilitySetUp(item.RowValue, item.ColumnValue, colorIndex);
                if (colorIndex == 5)
                {
                    index = 6;
                }
                blockManager.ChangeBlockType(item, colorIndex, index, false);
            }
        }
        yield return new WaitForSeconds(0.5f);
        blockSlideAbilitySFX.Play();
        holderObj.DOMove(Vector2.up * value, 0.5f);
    }
}
