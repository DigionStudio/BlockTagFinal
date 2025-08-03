using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    private LevelData levelData;
    private int gameTypeCode = 1;
    private int totalStarValue;
    private bool isScoreTarget;
    [SerializeField] private LevelDataInfo[] allLeveldata;


    private TargetEffect targetEffect;
    private TargetData[] targetData;
    private BgTileData[] bgTileData;
    private int moveCount;
    private int currentMoveCount;

    private int moveCountDiff = 5;
    private int lastLevelBg = 0;
    private int totalBgDesCount;

    private int starCount;
    private int starCount2;
    private int point = 10;
    private int abilityPoint = 20;
    private int currentPoint = 0;
    private int highPoint;
    private bool isAbility;
    public bool HasAbility { set { isAbility = value; } }

    
    public static Action OnMoveTaken = delegate { };
    public static Action<Normal_Block_Type, BlockType,Gem_Type, Vector3> BlockDes = delegate { };
    public static Action<int, bool> OnCoinUpdate = delegate { };
    public static int gameStatus = -1;
    private bool gameWin;
    private int totalCoin;
    private int totalCoinDes;
    private int currentHitPoint;


    private float adInterval;
    private float timeSinceLastAd;
    private bool isAdsShow;

    private bool isGamedataUpdated;

    [SerializeField] private UiManager uiManager;
    private BoardManager boardManager;
    private GameDataManager gameDataManager;
    private ShapeCreator shapeCreator;

    
    private GameAdsManager gameAdsManager;
    void Start()
    {
        
        gameStatus = -1;
        currentHitPoint = 0;
        boardManager = BoardManager.Instance;
        gameDataManager = boardManager.gameDataManager;
        targetEffect = TargetEffect.Instance;
        gameAdsManager = GameAdsManager.Instance;
        shapeCreator = ShapeCreator.Instance;
        //uiManager = FindObjectOfType<UiManager>();
        BlockDes += OnBlockDdestroy;
        OnMoveTaken += MoveCount;
        OnCoinUpdate += CoinShow;
        gameTypeCode = gameDataManager.GameTypeCode;
        adInterval = UnityEngine.Random.Range(60, 90);
        isAdsShow = !gameDataManager.HasDisableAds;
        GameStart();
    }


    private void OnDisable()
    {
        BlockDes -= OnBlockDdestroy;
        OnMoveTaken -= MoveCount;
        OnCoinUpdate -= CoinShow;
    }

    private void GameStart()
    {
        levelData = gameDataManager.levelData;
        int targetLength = 0;
        if (levelData.targetData != null)
            targetLength = levelData.targetData.Length;

        int bgtileLength = 0;
        if(levelData.bgTileData != null)
            bgtileLength = levelData.bgTileData.Length;


        isGamedataUpdated = false;
        targetData = new TargetData[targetLength];
        bgTileData = new BgTileData[bgtileLength];
        totalStarValue = gameDataManager.levelData.totalStarValue;
        isScoreTarget = gameDataManager.levelData.isScoreTarget;
        BgImageSpriteSetUP();
        MenuStart();
    }

    private void BgImageSpriteSetUP()
    {
        int num = levelData.levelNumber / 20;
        int totalIndex = uiManager.bgImageSprite.Length;
        int currentNum = num;
        int maxiter = 50;
        while (true)
        {
            maxiter--;
            if(maxiter < 0) break;
            else
            {
                if(currentNum >= 0 && currentNum < totalIndex)
                {
                    break;
                }
                else
                {
                    if(currentNum > totalIndex)
                    {
                        currentNum -= totalIndex;
                    }
                    else
                    {
                        currentNum *= -1;
                    }
                }
            }
                 
        }
        uiManager.ChangeBgPanelsSprite(currentNum);
    }

    private void MenuStart()
    {
        highPoint = gameDataManager.GetSaveValues(2);
        starCount = 0;
        starCount2 = 0;
        currentPoint = 0;
        StartGame();
    }

    private void StartGame()
    {
        
        for (int i = 0; i < targetData.Length; i++)
        {
            TargetData data = new TargetData();
            data.normalBlockType = levelData.targetData[i].normalBlockType;
            data.blockType = levelData.targetData[i].blockType;
            data.gemType = levelData.targetData[i].gemType;
            data.count = levelData.targetData[i].count;
            targetData[i] = data;

        }
        for (int i = 0; i < bgTileData.Length; i++)
        {
            BgTileData data = new BgTileData();
            data.positionIndex = levelData.bgTileData[i].positionIndex;
            data.tileType = levelData.bgTileData[i].tileType;
            bgTileData[i] = data;

        }
        bool isSlime = false;
        if(gameTypeCode == 1)
        {
            foreach (var item in targetData)
            {
                if(item.normalBlockType == Normal_Block_Type.Slime)
                {
                    isSlime = true;
                }
            }
        }
        
        moveCount = levelData.moveCount;
        currentMoveCount = 0;
        TutorialManager.Instance.IntroPanel();
        uiManager.StartGame(gameTypeCode, levelData);
        boardManager.StartGame(gameTypeCode, bgTileData, levelData.ability1Value, levelData.ability2Value, isSlime, levelData.moveSpeed);
        GameAIManager.Instance.GameStarted(targetData, levelData.moveCount);
        shapeCreator.SetUpShapeCreator(gameTypeCode, levelData.isBombAbility, levelData.isTagAbility, levelData.shapeCodes);
        Invoke(nameof(StartBoardSetUp), 1f);
    }
    public void MoveCountsetUp()
    {
        currentMoveCount = moveCount - 5;
        uiManager.MoveCount(moveCount - currentMoveCount);
    }
    private void StartBoardSetUp()
    {
        HighPoint();
        AddMoves(0, 0);
        boardManager.BoardPosStart();
        totalCoin = gameDataManager.GetSaveValues(0);
        uiManager.CoinTextSetup(gameTypeCode, totalCoin);

    }

    public void CalculatePoints(int num, bool isgame)
    {
        int pointVAl = point;
        if (isAbility)
            pointVAl = abilityPoint;
        int cPoint = (num * pointVAl);

        currentPoint += cPoint;
        uiManager.SetUpCurrentScore(cPoint);
        SetStar(true, gameStatus);
        if(currentHitPoint < cPoint && isgame)
        {
            currentHitPoint = cPoint;
            uiManager.currentHitPoint = currentHitPoint;
        }
    }
    private void HighPoint()
    {
        uiManager.SetUpHighScore(highPoint);
    }
    public void GameEndFailed()
    {
        print("Game Over Failed");
        gameStatus = 0;
        boardManager.GameOver(false);
    }

    public void GameEndSuccess()
    {
        print("Game Over success");
        gameStatus = 1;
        
        Invoke(nameof(GameWin), 1f);
    }
    private void GameWin()
    {
        boardManager.GameOver(true);
        if (HasFirstPlay())
        {
            int per = moveCount / 4;
            List<GiftData> data = new List<GiftData>();

            int num = 3;
            if (gameDataManager.rePlayCount <= 0 && starCount == 3) {

                for (int i = 3; i >= 0; i--)
                {
                    if (currentMoveCount >= i * per)
                    {
                        num = i;
                        break;
                    }
                }
            }
            int cointCount = 4 - num;

            GiftData dt = new GiftData()
            {
                indexCode = VariableTypeCode.Coin,
                values = cointCount * 50,
            };
            data.Add(dt);

            int wheelCount = 3 - num;
            if(wheelCount == 0)
            {
                wheelCount = 1;
            }
            GiftData dt2 = new GiftData()
            {
                indexCode = VariableTypeCode.Lucky_Wheel,
                values = wheelCount,
            };
            data.Add(dt2);


            if (gameDataManager.rePlayCount <= 0 && starCount == 3 && currentMoveCount <= per * 3)
            {
                // ability gift
                int giftIndex = UnityEngine.Random.Range(1, 7);
                for (int i = 0; i < 15; i++)
                {
                    if (giftIndex == 5 || (gameDataManager.isBombAiAsist && giftIndex == 6))
                    {
                        giftIndex = UnityEngine.Random.Range(1, 7);
                    }
                    else
                    {
                        if (giftIndex == 3)
                        {
                            int ran = UnityEngine.Random.Range(0, 3);
                            if (ran == 1)
                            {
                                break;
                            }
                            else
                            {
                                giftIndex = UnityEngine.Random.Range(1, 7);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (giftIndex > 0)
                {
                    GiftData dt3 = new GiftData()
                    {
                        indexCode = (VariableTypeCode)giftIndex,
                        values = 1,
                    };
                    data.Add(dt3);
                }
            }
            uiManager.GiftAbilityPanel(data);

        }
    }

    public void GameOver()
    {
        FollowCursor.OnMousePressed.Invoke(false);
        SetStar(false, gameStatus);
        GameEndValues();
        uiManager.GameEnd(gameStatus);
        if (targetData.Length > 0)
            Array.Clear(targetData, 0, targetData.Length);
    }
    public void UpdatePointToLeaderboard()
    {
        AdsLeaderboardManager.Instance.UpdateScore(currentPoint);
    }

    public void GameEndValues()
    {
        if (!isGamedataUpdated)
        {
            isGamedataUpdated = true;
            gameDataManager.UpdatePlayerData(levelData.levelNumber, starCount, currentPoint, currentHitPoint, gameStatus);
            currentPoint = 0;
        }
    }

    public void gameEndPaneldisable(bool status)
    {
        uiManager.DisableSettings();
        if (status)
        {
            BuyPanel.Instance.SetUpLifePanel(true, 1);
        }
        else
        {
            BuyPanel.Instance.DisableBuyPanel(false);
        }
    }

    private void OnBlockDdestroy(Normal_Block_Type type, BlockType abilityType,Gem_Type gemType, Vector3 pos)
    {
        if (gameTypeCode == 1)
        {
            for (int i = 0; i < targetData.Length; i++)
            {
                bool change = false;
                var item = targetData[i];
                if (item != null && item.count > 0)
                {
                    if (item.gemType == Gem_Type.none)
                    {
                        if (item.normalBlockType == type && item.blockType == abilityType)
                        {
                            StartEffect(pos, type, abilityType, i);
                            item.count--;
                            change = true;
                        }
                        else if (item.normalBlockType == Normal_Block_Type.none && item.blockType == abilityType)
                        {
                            StartEffect(pos, Normal_Block_Type.none, abilityType, i);
                            item.count--;
                            change = true;
                        }
                        else if (item.normalBlockType == type && item.blockType == BlockType.Normal_Block)
                        {
                            StartEffect(pos, type, BlockType.Normal_Block, i);
                            item.count--;
                            change = true;
                        }
                    }
                    else
                    {
                        if (item.gemType == gemType) 
                        {
                            targetEffect.SetUpGemTarget(pos, gemType, i);
                            item.count--;
                            change = true;
                        }
                    }
                    uiManager.ChangetargetCount(targetData[i].count, i, change);
                }
            }
        }
        else
        {
            int num = (int)type;
            if (type == Normal_Block_Type.none && abilityType == BlockType.None)
            {
                totalCoinDes++;
                CoinAddEffect(pos);
            }
            else if (num >= 7)
            {
                if (totalBgDesCount > 0)
                {
                    totalBgDesCount--;
                }
            }
        }
    }

    private void StartEffect(Vector3 pos, Normal_Block_Type type, BlockType abilityType, int targetCode)
    {
        if(gameTypeCode == 1)
        {
            targetEffect.SetUpEffect(pos, type, abilityType, targetCode);
        }
    }


    public void IncreaseSlimeTileCount()
    {
        if (gameTypeCode == 1)
        {
            bool change = false;
            int index = -1;
            for (int i = 0; i < targetData.Length; i++)
            {
                var item = targetData[i];
                if (item != null && item.count > 0)
                {
                    if (item.gemType == Gem_Type.none)
                    {
                        if (item.normalBlockType == Normal_Block_Type.Slime && item.count < levelData.targetData[i].count)
                        {
                            item.count++;
                            change = true;
                            index = i;
                            break;
                        }
                    }
                    


                }
            }
            if(index >= 0)
                uiManager.ChangetargetCount(targetData[index].count, index, change);
        }
    }
    

    public void CheckGameStatus()
    {
        
        if (gameStatus < 0)
        {
            gameWin = TargetStatus();
            if (gameWin)
            {
                if (isScoreTarget)
                {
                    uiManager.GameEndScoreSet(totalStarValue);
                }
                GameEndSuccess();
            }
            else
            {
                if (moveCount - currentMoveCount == 2)
                {
                    BuyPanel.Instance.GameEndMoveBuy(true);
                }
            }
        }
        if (currentMoveCount >= moveCount)
        {
            boardManager.GameEnd(0);
        }

    }
    public bool TargetStatus()
    {
        bool gameStatus = true;
        if (!isScoreTarget)
        {
            for (int i = 0; i < targetData.Length; i++)
            {
                var item = targetData[i];
                if (item != null && item.count > 0)
                {
                    gameStatus = false;
                    break;
                }
            }
        }
        else
        {
            if(currentPoint < totalStarValue)
            {
                gameStatus = false;
            }
        }
        return gameStatus;
    }

    public bool CheckMoveCount()
    {
        bool isMove = true;
        if (gameTypeCode == 1 && currentMoveCount >= moveCount)
            isMove = false;
        return isMove;
    }

    public void GameEndMoveDecrese()
    {
        CalculatePoints((moveCount - currentMoveCount) * 50, false);
        StartCoroutine(DecreseMoveEfectCo());
    }

    private IEnumerator DecreseMoveEfectCo()
    {
        int movecount = currentMoveCount;
        float diff = (float)(moveCount - movecount);
        float time = 1f/ diff;
        int maxitter = 100;
        while(maxitter > 0)
        {
            if (movecount < moveCount)
                movecount++;
            else
            {
                movecount = moveCount;
            }
            uiManager.MoveCount(moveCount - movecount);
            yield return new WaitForSeconds(time);
        }
    }

    private void MoveCount()
    {
        if (currentMoveCount < moveCount || gameTypeCode == 0)
            currentMoveCount++;

        if (gameTypeCode == 1)
        {
            uiManager.MoveCount(moveCount - currentMoveCount);
            CheckGameStatus();
        }
        else if (gameTypeCode == 0 && !TutorialManager.Instance.isTutorial)
        {
            if (totalBgDesCount <= 0)
            {
                if (currentMoveCount >= moveCountDiff)
                {
                    currentMoveCount = 0;

                    foreach (LevelDataInfo data in allLeveldata)
                    {
                        if (data.levelData.levelNumber > lastLevelBg && data.levelData.bgTileData.Length > 0 && data.levelData.bgTileData[0].tileType != Normal_Block_Type.Invisible)
                        {
                            lastLevelBg = data.levelData.levelNumber;
                            bgTileData = data.levelData.bgTileData;
                            totalBgDesCount = bgTileData.Length;
                            break;
                        }
                    }
                    Invoke(nameof(ChangeBgTileData), 1f);
                }
            }
            else
            {
                currentMoveCount = 0;
                moveCountDiff = UnityEngine.Random.Range(3, 7);
            }
            CheckGameConnectivity();
        }
    }

    private void ChangeBgTileData()
    {
        boardManager.ChangeBgBlockStatus(bgTileData);
    }

    private void SetStar(bool inGame, int gameStatus)
    {
        if (gameTypeCode == 1)
        {
            if (inGame)
            {
                GetStar(gameStatus);
            }
            else
            {
                uiManager.StarSetUp(starCount, false, 0, gameStatus);

            }
        }
    }

    private bool HasFirstPlay()
    {
        bool isFirst = false;
        int nextLevel = levelData.levelNumber + 1;
        if (nextLevel < gameDataManager.GetSaveDataListlength(0))
        {
            if (!gameDataManager.LevelStarData( nextLevel).isUnlock)
            {
                isFirst = true;
            }
        }
        return isFirst;

    }

    private void GetStar(int gamestatus)
    {
        float fillAmount = 0;
        int gameCd = 0;
        int starcount = 0;
        if (starCount < 3)
        {

            starcount = CalculateStarCount(levelData.totalStarValue, currentPoint);
            fillAmount = (float)currentPoint / (float)levelData.totalStarValue;
            if (fillAmount >= 1f)
                fillAmount = 1f;
            starCount = starcount;
        }
        else
        {
            gameCd = 1;
            starcount = CalculateStarCount((highPoint - levelData.totalStarValue), (currentPoint - levelData.totalStarValue));
            fillAmount = (float)(currentPoint - levelData.totalStarValue) / (float)(highPoint - levelData.totalStarValue);
            starCount2 = starcount;
        }
        uiManager.StarSetUp(starcount, true, gameCd, gamestatus);
        uiManager.StarBarFill(fillAmount, gameCd, gamestatus);
    }

    private int CalculateStarCount(int total, int current)
    {
        float multiplier = 0.33f;
        int starcount = 0;
        for (float i = 1; i < 4; i++)
        {
            int starMul = (int)(i * multiplier * total);
            if (starcount == 2)
                starMul = total;


            if (starMul > 0 && current >= starMul)
            {
                starcount = (int)i;
                if (starcount > 3)
                {
                    starcount = 3;
                }
            }
        }
        return starcount;
    }

    public void CoinAddEffect(Vector2 pos)
    {
        targetEffect.SetUpCoinShow(pos);
    }
    public void TotalCoinDesEffect()
    {
        if (gameTypeCode == 0)
        {
            CoinShow(totalCoinDes, true);
            totalCoinDes = 0;
        }
    }

    private void CoinShow(int val, bool add)
    {
        int num = 1;
        if (!add)
            num = -1;
        totalCoin += (val * num);
        gameDataManager.CoinValueChange(val, add);
        uiManager.AddCoin(val, add);
        uiManager.SetLevelPanelValueText(totalCoin);
    }

    public void AddMoves(int num, int coin)
    {
        moveCount += num;
        uiManager.MoveCount(moveCount - currentMoveCount);
        if(coin > 0)
            CoinShow(coin, false);
    }


    


    //private void Update()
    //{
    //    if(gameTypeCode == 0 && gameStatus < 0)
    //    {
    //        AdsShowTimer();
    //    }
    //}

    private void AdsShowTimer()
    {
        if (boardManager.isGameStarted && isAdsShow)
        {
            timeSinceLastAd += Time.deltaTime;

            if (timeSinceLastAd >= adInterval)
            {
                ShowAd();
                timeSinceLastAd = 0f; // Reset the timer after showing the ad
                adInterval = UnityEngine.Random.Range(60, 90);
            }
        }
    }

    private void ShowAd()
    {
        //gameAdsManager.ShowOnGameAds();
    }

    public void CheckGameConnectivity()
    {
        if (gameTypeCode == 0 && !gameDataManager.HasDisableAds)
        {
            if (!AdsLeaderboardManager.Instance.HasOnline)
            {
                uiManager.OpenOfflinePanel(true);
                boardManager.GameStatus(false);
            }
            else
            {
                uiManager.OpenOfflinePanel(false);
                boardManager.GameStatus(true);
            }
        }
    }

    //public void InsializeAds()
    //{
    //    CheckGameConnectivity();
    //}
}
