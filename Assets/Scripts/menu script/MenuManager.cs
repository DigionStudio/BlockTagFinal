using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

[Serializable]
public class Game_Value_Status
{
    public int value;
    public bool status;
}


[Serializable]
public class BgTileData
{
    public int positionIndex;
    public Normal_Block_Type tileType = Normal_Block_Type.none;
}

[Serializable]
public class SpecialObjectTileData:System.Object
{
    public Special_Object_Type specialObjType = Special_Object_Type.none;
    public int spawnProb;
}

[Serializable]
public class ShapeData
{
    public int code;
    public bool isSelectable;
}

[Serializable]
public class LevelData
{
    public float moveSpeed;
    public bool isScoreTarget;
    public bool isBombAbility;
    public bool isTagAbility;
    public ShapeData[] shapeCodes;
    public TargetData[] targetData;
    public BgTileData[] bgTileData;
    public SpecialObjectTileData[] specialObjectTileData;
    public int moveCount;
    public int totalStarValue;
    public int reFreshCount;
    public int ability1Value = 10;
    public int ability2Value = 7;
    public int levelNumber;
}

[Serializable]
public enum Special_Object_Type
{
    none,
    UnFresh,// 10 Sec Disable Reset Tag buttton
    UnRotate, // 15 sec disable Rotate Tag Button
    SpeedUp, // +20% speed
    Disolve, //Disable Ability 
    Disable, // 5 Sec Disable Ability Use
    Gem_4,
    Gem_5
}

[Serializable]
public enum Normal_Block_Type
{
    Block_1,    // 0
    Block_2,    // 1
    Block_3,    // 2
    Block_4,    // 3
    Block_5,    // 4
    none,       // 5
    Invisible,  // 6
    Wodden,     // 7
    Ice,        // 8
    Rock,       // 9
    Wall,       // 10
    Muddy,      // 11
    Slime,      // 12
    Treatment,
    Recover,
    Shield
}
[Serializable]
public class Combo_Block_Type
{
    public Normal_Block_Type[] comboBlockType;
}

[Serializable]
public class TargetData
{
    public Normal_Block_Type normalBlockType;
    public BlockType blockType;
    public Special_Object_Type specialObject;
    public int count;
}


[Serializable]
public class BlockTileData
{
    public Normal_Block_Type block = Normal_Block_Type.Block_1;
    public Sprite blockIconSprite;
    public GameObject blockEffect;
    public Color effectColor;

}

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;
    [SerializeField] private PanelTween[] menuTween;
    [SerializeField] private PanelTween menuHeadingTween;
    [SerializeField] private Button menuplayButton;
    [SerializeField] private GameObject levelPanel;
    [SerializeField] private Button levelplayButton;
    [SerializeField] private AudioSource clickaudioSource;
    [SerializeField] private AudioSource coinaudioSource;
    [SerializeField] private AudioSource backgroundMusic;
    private bool isBgMusicPlayed;
    private bool isMenuOpen;
    private bool isMenuPlay;
    private bool isLevelPanelActive;
    [SerializeField] private Button menuButton;


    [SerializeField] private PanelTween lowerLevelPanel;
    [SerializeField] private PanelTween leftLevelPanel;
    [SerializeField] private PanelTween rightLevelPanel;

    [SerializeField] private GameObject loadingHolder;
    [SerializeField] private GameObject loadPanel;
    [SerializeField] private Image loadingBar;
    private Tween loadTween;

    public AbilityData[] abilityData;
    [SerializeField] private Image giftHolder;

    [SerializeField] private PanelTween upperShowPanel;

    [Header("Leaderboard")]
    [SerializeField] private PanelTween leaderboardShowPanelTween;
    [SerializeField] private Image leaderBoardBG;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button leaderboardCrossButton;
    private bool isLeaderboardShowActive;


    [Header("Level Show")]
    [SerializeField] private PanelTween levelShowPanel;
    [SerializeField] private Image levelShowBg;
    [SerializeField] private GameObject[] starObject;
    [SerializeField] private TargetShow[] targetUI;
    [SerializeField] private GameObject pointShowHolder;
    [SerializeField] private Text pointCount;
    [SerializeField] private Text movetext;
    [SerializeField] private Text levelText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button crossButton;
    [SerializeField] private Button levelPlayButton;
    [SerializeField] private MenuLevelData menuLevelData;
    [SerializeField] private SelectTagShape selectTagShape;
    private LevelData levelData;
    private bool isLevelShowActive;


    [Header("Wand Show")]
    private bool isWandActive;
    [SerializeField] private Button wandActiveButton;
    [SerializeField] private Image wandSelectedImage;
    [SerializeField] private Color unSelectedColor;
    [SerializeField] private Text wandCountText;
    [SerializeField] private GameObject wandDisableObj;
    [SerializeField] private GameObject wandEnableEffectObj;


    private Game_Value_Status gameBombAbility;
    private Game_Value_Status gameTagAbility;
    private bool isbombtag;
    private bool isabilitytag;

    [Header("Upper Data Show")]
    [SerializeField] private Text heartText;
    [SerializeField] private Text coinText;
    [SerializeField] private UpperPanel currentCoinobject;
    private bool isCoindAddEffect;
    private List<Game_Value_Status> coinEffectValues = new List<Game_Value_Status>();
    [SerializeField] private Image blockShowImage;
    [SerializeField] private Button quitButton;
    public static Action<int, bool, int, bool> OnVariableUpdate = delegate { };



    [Header("Block Icon Show")]
    [SerializeField] private Button blockIconButton;
    [SerializeField] private Button blockIconCrossButton;
    [SerializeField] private Image blockTypePanel;
    [SerializeField] private PanelTween blockTypePanelTween;
    [SerializeField] private TileShow tileShowPrefab;
    [SerializeField] private RectTransform tileshowHolder;
    [SerializeField] private ScrollRect blockShowScroll;
    private bool isBlockTileSetUp;
    private bool isBlockPanelActive;
    private int currentBlockIndex;

    [Header("Goal Show")]
    [SerializeField] private Button goalPanelButton;
    [SerializeField] private Button goalPanelCrossButton;
    [SerializeField] private DailyGoalShow[] dailyGoalShows;
    [SerializeField] private Image goalPanelBg;
    [SerializeField] private PanelTween goalPanelTween;
    private bool isGoalPanelActive;
    [SerializeField] private Button freeButton;
    private RectTransform freeCoinImage;
    [SerializeField] private GameObject freeRewardHolder;
    [SerializeField] private Button freeCoinButton;
    private RectTransform coinButtonImage;
    [SerializeField] private Button freeLifeButton;
    [SerializeField] private RectTransform lifeImage;


    [Header("Fortune Wheel")]
    [SerializeField] private FortuneWheel fortuneWheel;
    [SerializeField] private Button wheelPanelButton;
    [SerializeField] private VideoPlayer wheelVideoPlayer;
    [SerializeField] private string videoFileName;
    private string videoPath;
    [SerializeField] private Button wheelPanelCrossButton;
    [SerializeField] private Image wheelPanelBg;
    [SerializeField] private PanelTween wheelPanelTween;
    private bool isFortuneWheelActive;

    [Header("Buy Panel")]
    [SerializeField] private Button buyPanelButton;
    [SerializeField] private Button buyPanelCrossButton;
    [SerializeField] private Image buyPanelBg;
    [SerializeField] private PanelTween buyPanelTween;
    [SerializeField] private ScrollRect buyPanelScroll;
    private bool isBuyPanelActive;
    [SerializeField] private ShopManager shopManager;


    [Header("info Panel")]
    [SerializeField] private Button infoPanelButton;
    [SerializeField] private Button rateButton;
    [SerializeField] private Button infoPanelCrossButton;
    [SerializeField] private Image infoPanelBg;
    [SerializeField] private PanelTween infoPanelTween;
    private bool isInfoPanelActive;
    public InfoItems[] tilesInfo;
    public InfoItems[] abilityInfo;
    [SerializeField] private Button infoGetButton;
    [SerializeField] private Button infoBuyButton;
    [SerializeField] private Text infoBuyText;
    private int infoAbilityIndex = -1;
    [SerializeField] private Button infodetailsCrossButton;
    private bool infoDetailsPanelStatus;
    [SerializeField] private Image infoDetailsPanelBg;
    [SerializeField] private PanelTween infoDetailsPanelTween;
    [SerializeField] private InfoItems detailsInfo;
    public static int tileInfoShowIndex = -1;
    private bool isInfoSetup;

    private DailyGoalsManager goalsManager;
    private MenuTutorialManager menuTutorialManager;
    [SerializeField] private MenuAdsManager menuAdsManager;
    public BonusGiftManager bonusGiftManager;

    private bool isEscapeActive = true;
    private bool isEscapeStatus = true;
    private bool isblockShowTutActive;

    private GameDataManager gameDataManager;
    private int totalCoin;
    private int totalLife;
    private TargetEffect targetEffect;
    public bool isTutorialActive;
    private bool isInitial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        isInitial = true;
        isBgMusicPlayed = false;
    }


    void Start()
    {
        if (gameDataManager == null)
            gameDataManager = BlockManager.Instance.gameDataManager;
        if (goalsManager == null)
            goalsManager = DailyGoalsManager.Instance;
        if (menuTutorialManager == null)
            menuTutorialManager = MenuTutorialManager.Instance;
        gameDataManager.rePlayCount = 0;
        if (targetEffect == null)
            targetEffect = TargetEffect.Instance;
        totalCoin = gameDataManager.GetSaveValues(0);
        totalLife = gameDataManager.GetSaveValues(1);
        UpdateCoin(totalCoin, totalLife);
        isInfoSetup = false;
        gameBombAbility = new Game_Value_Status()
        {
            value = -1,
            status = false
        };

        gameTagAbility = new Game_Value_Status()
        {
            value = -1,
            status = false
        };
        menuplayButton.onClick.AddListener(MenuPlay);
        levelplayButton.onClick.AddListener(LevelShow);
        menuButton.onClick.AddListener(LevelDisable);

        leaderboardButton.onClick.AddListener(LeaderboardPanelStatus);
        leaderboardCrossButton.onClick.AddListener(LeaderboardPanelStatus);

        blockIconButton.onClick.AddListener(BlockTilePanelStatus);
        blockIconCrossButton.onClick.AddListener(BlockTilePanelStatus);
        playButton.onClick.AddListener(LevelPlay);
        crossButton.onClick.AddListener(DisableLevelShow);
        levelPlayButton.onClick.AddListener(PlayLevel);
        quitButton.onClick.AddListener(QuitGame);
        wandActiveButton.onClick.AddListener(WandActive);


        goalPanelButton.onClick.AddListener(GoalsPanelStatus);
        goalPanelCrossButton.onClick.AddListener(GoalsPanelStatus);
        freeButton.onClick.AddListener(FreeReward);
        freeCoinImage = freeButton.GetComponent<RectTransform>();
        freeCoinButton.onClick.AddListener(FreeCoin);
        coinButtonImage = freeCoinButton.GetComponent<RectTransform>();
        freeLifeButton.onClick.AddListener(FreeLife);
        
        wheelPanelButton.onClick.AddListener(FortuneWheelPanelStatus);
        
        
        wheelPanelCrossButton.onClick.AddListener(FortuneWheelPanelStatus);
        videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);

        buyPanelButton.onClick.AddListener(BuyPanelStatus);
        buyPanelCrossButton.onClick.AddListener(BuyPanelStatus);
        infoPanelButton.onClick.AddListener(InfoPanelStatus);
        infoPanelCrossButton.onClick.AddListener(InfoPanelStatus);
        rateButton.onClick.AddListener(OpenURL);
        infoGetButton.onClick.AddListener(InfoBuyPanel);
        infoBuyButton.onClick.AddListener(BuyInfoAbility);
        infodetailsCrossButton.onClick.AddListener(InfoDetailsPanelStatus);
        OnVariableUpdate += SetLevelPanelValueText;
        ChangePanelPos(false, menuTween, 0);


        currentBlockIndex = gameDataManager.GetSaveValues(4);
        BlockTileSetUp();
        isabilitytag = false;
        isbombtag = false;

    }

    public void MenuPanelSetup(bool status, float duration)
    {
        ChangePanelPos(status, menuTween, duration);
        float posy = menuHeadingTween.posInitialFloat;
        float time = duration;
        if (status)
        {
            posy = menuHeadingTween.posFinalFloat;
            time = 0.3f;
        }
        TweenPanel(menuHeadingTween.panel, posy, time);

    }

    public void SetUpManager(bool ismenuopen)
    {
        isMenuPlay = ismenuopen;
        if (isMenuPlay)
        {
            if (!isTutorialActive)
            {
                GameInitialize(3f, false);
            }
        }
        else
        {
            LevelShow();
        }
    }
    private void OnDisable()
    {
        OnVariableUpdate -= SetLevelPanelValueText;

    }
    public void Loading()
    {
        loadingBar.fillAmount = 0;
        loadingHolder.SetActive(true);
        loadPanel.SetActive(true);
        if (loadTween == null)
            loadTween = loadingBar.DOFillAmount(0.8f, 5f);
    }

    public void GameInitialize(float time2, bool isTute)
    {
        LevelButtonShowAnim(false);
        isMenuOpen = false;
        if(loadTween != null)
            loadTween.Kill();
        loadTween = loadingBar.DOFillAmount(1f, time2).OnComplete(() =>
        { 
            if (!isTute)
            {
                Invoke(nameof(GameLoadingComplete), 0.5f);
            }

        });
    }

    public void LoadingComplete()
    {
        if (!isTutorialActive)
        {
            if (loadTween != null)
                loadTween.Kill();
            loadTween = loadingBar.DOFillAmount(1, 1f);
            Invoke(nameof(GameLoadingComplete), 1f);
        }
    }

    private void GameLoadingComplete()
    {
        if (!isMenuOpen)
        {
            isMenuOpen = true;
            menuTutorialManager.CloseDetails();
            Invoke(nameof(GameInitializeComplete), 1f);
        }

    }

    private void GameInitializeComplete()
    {
        loadPanel.SetActive(false);
        loadingHolder.SetActive(false);
        LevelDisable();

    }
    private void QuitGame()
    {
        gameDataManager.SavePlayerData();
        Application.Quit();
    }

    private void GameLoad()
    {
        SceneManager.LoadScene(1);
    }
    private void MenuPlay()
    {
        clickaudioSource.Play();
        if (menuAdsManager.isOnline)
        {
            GameMenuPlay();
            //if (gameDataManager.HasDisableAds)
            //{
            //    GameMenuPlay();
            //}
            //else
            //{
            //    menuAdsManager.Show_Inter_Ads(1);
            //}
        }
        else
        {
            menuAdsManager.SetUpOffLine();
        }
    }
    public void GameMenuPlay()
    {
        gameDataManager.levelData = gameDataManager.classicModelevelData;
        menuAdsManager.LoadRewardedAds();
        gameDataManager.CheckForFreeClaim(-1, true);
        gameDataManager.GameTypeCode = 0;
        goalsManager.isNewLevel = true;
        gameDataManager.isBombAiAsist = false;
        ChangePanelPos(false, menuTween, 0.2f);
        if (gameDataManager.isGifted)
        {
            gameDataManager.GiftValueClaim();
        }
        Invoke(nameof(GameLoad), 0.1f);
    }

    private void LevelShow()
    {
        loadPanel.SetActive(false);
        loadingHolder.SetActive(false);


        if (gameDataManager == null)
            gameDataManager = BlockManager.Instance.gameDataManager;
        if (goalsManager == null)
            goalsManager = DailyGoalsManager.Instance;
        if (menuTutorialManager == null)
            menuTutorialManager = MenuTutorialManager.Instance;
        gameDataManager.rePlayCount = 0;
        if (targetEffect == null)
            targetEffect = TargetEffect.Instance;

        isblockShowTutActive = menuTutorialManager.CheckFirstPlayLevel();

        menuAdsManager.LoadRewardedAds();

        menuLevelData.ScrollPos();
        isLevelPanelActive = true;
        clickaudioSource.Play();
        gameDataManager.GameTypeCode = 1;
        ChangePanelPos(false, menuTween, 0.3f);
        LevelLoad(true);
        LevelButtonShowAnim(true);
        GiftPanelForMetaFill(true);
        PlayBGMusic();
    }

    private bool CheckForNewlevel(int level)
    {
        bool isnewLevel = false;
        int currentlevel = gameDataManager.currentLevel;
        if (currentlevel <= level)
        {
            isnewLevel = true;
        }
        return isnewLevel;
    }

    private void LevelDisable()
    {
        isLevelPanelActive = false;
        LevelButtonShowAnim(false);
        clickaudioSource.Play();
        gameDataManager.GameTypeCode = 0;
        if (gameDataManager.isplayerNameSelect)
        {
            menuTutorialManager.SetPlayerName();
            menuTutorialManager.GameNameAnim(false);
        }
        else
        {
            MenuShow();
        }
    }

    public void MenuShow()
    {
        bool status = bonusGiftManager.CheckForBonus();
        if (!status)
        {
            ChangePanelPos(true, menuTween, 0.5f);
            LevelLoad(false);
            PlayBGMusic();
            menuTutorialManager.GameNameAnim(true);
        }
        else
        {
            menuTutorialManager.GameNameAnim(false);
        }
    }

    public void PlayBGMusic()
    {
        if (!isBgMusicPlayed)
        {
            isBgMusicPlayed = true;
            backgroundMusic.Play();
        }
    }

    private void UpperLevelShow(bool isFinal)
    {
        float num = upperShowPanel.posInitialFloat;
        if (isFinal)
        {
            num = upperShowPanel.posFinalFloat;
        }
        upperShowPanel.panel.DOAnchorPosY(num, 0.3f);
    }

    private void LevelButtonShowAnim(bool isFinal)
    {
        LowerLevelShow(isFinal);
        LeftLevelShow(isFinal);
        RightLevelShow(isFinal);
    }
    private void LowerLevelShow(bool isFinal)
    {
        float num = lowerLevelPanel.posInitialFloat;
        if (isFinal)
        {
            num = lowerLevelPanel.posFinalFloat;
        }
        lowerLevelPanel.panel.DOAnchorPosY(num, 0.2f);
    }
    private void LeftLevelShow(bool isFinal)
    {
        float xPos = leftLevelPanel.posInitialFloat;
        if (isFinal)
        {
            xPos = leftLevelPanel.posFinalFloat;
        }
        leftLevelPanel.panel.DOAnchorPosX(xPos, 0.2f);
    }

    private void RightLevelShow(bool isFinal)
    {
        float xPos = rightLevelPanel.posInitialFloat;
        if (isFinal)
        {
            xPos = rightLevelPanel.posFinalFloat;
        }
        rightLevelPanel.panel.DOAnchorPosX(xPos, 0.2f);
    }

    private void LevelLoad(bool isLevelShow)
    {
        levelPanel.SetActive(isLevelShow);
        if(isLevelShow)
        {
            if(string.IsNullOrEmpty(videoPath))
                videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);


            if (!string.IsNullOrEmpty(videoPath))
            {
                wheelVideoPlayer.url = videoPath;
                wheelVideoPlayer.Play();
            }
        }
        //menuLevelData.ScrollToPosition(isLevelShow, isLevelShowed);
    }

    private void ChangePanelPos(bool isfinalpos, PanelTween[] tweenPanel, float duration)
    {
        foreach (var panel in tweenPanel)
        {
            if (panel != null)
            {
                float posy = panel.posInitialFloat;
                float time = duration;
                if (isfinalpos)
                {
                    posy = panel.posFinalFloat;
                    time = 0.3f;
                }
                TweenPanel(panel.panel, posy, time);
                
            }
        }
    }

    private void TweenPanel(RectTransform panel,float posY, float duration)
    {
        isEscapeActive = true;
        panel.DOAnchorPosY(posY, duration).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            isEscapeActive = false;

        });
    }


    private void LevelPlay()
    {
        clickaudioSource.Play();
        gameDataManager.CheckForFreeClaim(-1, true);
        StartLevelPlay();

    }

    public void StartLevelPlay()
    {
        levelData.isBombAbility = isbombtag;
        levelData.isTagAbility = isabilitytag;
        gameDataManager.levelData = levelData;
        gameDataManager.currentLevel = levelData.levelNumber;
        gameDataManager.GameTypeCode = 1;
        gameDataManager.isMenuOpened = false;

        if (gameDataManager.currentLevel > 7 && gameDataManager.currentLevel <= 10)
        {
            gameDataManager.isBombAiAsist = true;

        }
        else
            SetWandStatus(isWandActive);

        AdsLeaderboardManager.Instance.CheckAnalyticsEvent(5, gameDataManager.currentLevel);
        goalsManager.isNewLevel = CheckForNewlevel(levelData.levelNumber);
        Invoke(nameof(GameLoad), 0.3f);
    }

    private void SetWandStatus(bool status)
    {
        if (status)
        {
            gameDataManager.isBombAiAsist = true;
            gameDataManager.GameAbilitySave((int)VariableTypeCode.Magic_Wand, false, 1);
        }
        else
        {
            gameDataManager.isBombAiAsist = false;
        }
    }

    private void PlayLevel()
    {
        menuLevelData.PlayLevelShow();
    }

    private void DisableLevelShow()
    {
        LevelShowPanelSetUp(levelData);

    }

    public void LevelShowPanelSetUp(LevelData lvlData)
    {
        SetUpLevelShow(lvlData);
        float fade = 0f;
        levelShowBg.gameObject.SetActive(true);
        LevelButtonShowAnim(isLevelShowActive);

        clickaudioSource.Play();
        if (isLevelShowActive)
        {
            levelShowPanel.panel.DOAnchorPosY(levelShowPanel.posInitialFloat, 0.3f);
        }
        else
        {
            levelShowPanel.panel.DOAnchorPosY(levelShowPanel.posFinalFloat, 0.3f);
            levelShowBg.gameObject.SetActive(true);
            fade = 0.85f;
        }
        levelShowBg.DOFade(fade, 0.3f).OnComplete(() =>
        {
            isLevelShowActive = !isLevelShowActive;
            levelShowBg.gameObject.SetActive(isLevelShowActive);
            if (isLevelShowActive)
            {
                CheckForWandActivation(lvlData.levelNumber);
            }
            else
            {
                WandStatus(false, 0);
            }
            selectTagShape.ActiveEffect(isLevelShowActive);
        });
    }

    private void SetUpLevelData(LevelData data)
    {
        LevelData info = new LevelData();
        info.isScoreTarget = data.isScoreTarget;
        info.isBombAbility = data.isBombAbility;
        info.isTagAbility = data.isTagAbility;

        int shapeLength = 0;
        if (data.shapeCodes != null)
            shapeLength = data.shapeCodes.Length;

        if (shapeLength > 0)
        {
            info.shapeCodes = new ShapeData[shapeLength];
            for (int i = 0; i < shapeLength; i++)
            {

                ShapeData shapedata = new ShapeData
                {
                    code = data.shapeCodes[i].code,
                    isSelectable = data.shapeCodes[i].isSelectable

                };
                info.shapeCodes[i] = shapedata;
            }
        }

        int targetLength = 0;
        if (data.targetData != null)
            targetLength = data.targetData.Length;
        if (targetLength > 0)
        {
            info.targetData = new TargetData[targetLength];
            for (int i = 0; i < targetLength; i++)
            {
                TargetData targetdata = new TargetData
                {
                    normalBlockType = data.targetData[i].normalBlockType,
                    blockType = data.targetData[i].blockType,
                    specialObject = data.targetData[i].specialObject,
                    count = data.targetData[i].count
                };
                info.targetData[i] = targetdata;

            }
        }

        int bgTileLength = 0;
        if (data.bgTileData != null)
            bgTileLength = data.bgTileData.Length;
        if (bgTileLength > 0)
        {
            info.bgTileData = new BgTileData[bgTileLength];
            for (int i = 0; i < bgTileLength; i++)
            {
                BgTileData bgdata = new BgTileData
                {
                    positionIndex = data.bgTileData[i].positionIndex,
                    tileType = data.bgTileData[i].tileType
                };
                info.bgTileData[i] = bgdata;
            }
        }

        int spLength = 0;
        if (data.specialObjectTileData != null)
            spLength = data.specialObjectTileData.Length;
        if (spLength > 0)
        {
            info.specialObjectTileData = new SpecialObjectTileData[spLength];
            for (int i = 0; i < spLength; i++)
            {
                SpecialObjectTileData spdata = new SpecialObjectTileData
                {
                    specialObjType = data.specialObjectTileData[i].specialObjType,
                    spawnProb = data.specialObjectTileData[i].spawnProb,
                };
                info.specialObjectTileData[i] = spdata;
            }
        }

        info.moveSpeed = data.moveSpeed;
        info.moveCount = data.moveCount;
        info.totalStarValue = data.totalStarValue;
        info.reFreshCount = data.reFreshCount;
        info.ability1Value = data.ability1Value;
        info.ability2Value = data.ability2Value;
        info.levelNumber = data.levelNumber;
        levelData = info;
    }
    private void SetUpLevelShow(LevelData lvlData)
    {
        SetUpLevelData(lvlData);
        TargetData[] targetdata = levelData.targetData;
        for (int i = 0; i < starObject.Length; i++)
        {
            starObject[i].SetActive(false);
        }
        Level_Star_Data data = gameDataManager.LevelStarData(levelData.levelNumber); 

        for (int i = 0; i < data.StarCount; i++)
        {
            starObject[i].SetActive(true);
        }
        movetext.text = levelData.moveCount.ToString();
        levelText.text = (levelData.levelNumber + 1).ToString();
        pointShowHolder.SetActive(false);
        foreach (var item in targetUI)
        {
            item.gameObject.SetActive(false);
        }
        if (!levelData.isScoreTarget)
        {
            for (int i = 0; i < targetdata.Length; i++)
                targetUI[i].SetUp(targetdata[i]);
        }
        else
        {
            pointShowHolder.SetActive(true);
            pointCount.text = levelData.totalStarValue.ToString();

        }
        if (levelData.shapeCodes != null && levelData.shapeCodes.Length > 0)
        {
            selectTagShape.SetUpShapeSelect(true, levelData.shapeCodes);
        }
        else
        {
            selectTagShape.SetUpShapeSelect(false, levelData.shapeCodes);
        }
        isbombtag = levelData.isBombAbility;
        isabilitytag = levelData.isTagAbility;
        if (levelData.levelNumber == gameBombAbility.value)
        {
            if (gameBombAbility.status)
                isbombtag = true;
        }
        if (levelData.levelNumber == gameTagAbility.value)
        {
            if (gameBombAbility.status)
                isabilitytag = true;
        }

        selectTagShape.AbilityStatus(levelData.isBombAbility, levelData.isTagAbility, levelData.levelNumber);
        ShapeTagSetUp();
    }

    private void ShapeTagSetUp()
    {
        bool initialeLock = false;
        if (levelData.levelNumber < 15)
        {
            initialeLock = true;
        }
        bool ismatch1 = false;
        bool ismatch2 = false;
        if (levelData.levelNumber == gameBombAbility.value)
        {
            if (gameBombAbility.status)
                ismatch1 = true;
        }else if (levelData.isBombAbility)
        {
            ismatch1 = true;
        }
        if (levelData.levelNumber == gameTagAbility.value)
        {
            if (gameBombAbility.status)
                ismatch2 = true;
        }
        else if (levelData.isTagAbility)
        {
            ismatch2 = true;
        }
        selectTagShape.AbilitySetUp(isbombtag, ismatch1, isabilitytag, ismatch2, initialeLock);
    }
    public void ChangeTagStatus(bool isenable, bool isBomb, int price, bool status = false)
    {
        if (isenable)
        {
            if (isBomb)
            {
                isbombtag = true;
                if (isbombtag && gameBombAbility.value != levelData.levelNumber)
                {
                    gameBombAbility.value = levelData.levelNumber;
                    gameBombAbility.status = true;
                    gameDataManager.CoinValueChange(price, false);
                }
            }
            else
            {
                isabilitytag = true;
                if (isabilitytag && gameTagAbility.value != levelData.levelNumber)
                {
                    gameTagAbility.value = levelData.levelNumber;
                    gameTagAbility.status = true;
                    gameDataManager.CoinValueChange(price, false);
                }
            }
        }
        else
        {
            if (isBomb)
            {
                isbombtag = status;
            }
            else
            {
                isabilitytag = status;
            }
        }
        ShapeTagSetUp();
    }


    public void ChangeTagIndex(int index, int value)
    {
        levelData.shapeCodes[index].code = value;
    }


    private void SetLevelPanelValueText(int value, bool isCoin, int num, bool isadd)
    {
        coinaudioSource.Play();
        if (isCoin)
        {
            AddCoin(num, isadd);
            UpdateCoin(value, -1);
        }
        else
        {
            UpdateCoin(-1, value);
        }

    }
    private void AddCoin(int num, bool isadd)
    {
        if (!isCoindAddEffect)
        {
            isCoindAddEffect = true;
            string symbol = "+";
            if (!isadd)
                symbol = "-";
            currentCoinobject.valueText.text = symbol + num.ToString();
            currentCoinobject.holder.SetActive(true);
            Invoke(nameof(CoinUI), 1f);
        }
        else
        {
            Game_Value_Status gs = new Game_Value_Status()
            {
                value = num,
                status = isadd
            };
            coinEffectValues.Add(gs);
        }
    }
    private void CoinUI()
    {
        currentCoinobject.holder.SetActive(false);
        isCoindAddEffect = false;
        InvokeCoinEffects();
    }

    private void InvokeCoinEffects()
    {
        if(coinEffectValues.Count > 0)
        {
            AddCoin(coinEffectValues[0].value, coinEffectValues[0].status);
            coinEffectValues.RemoveAt(0);
        }
    }

    private void UpdateCoin(int coin, int lifes)
    {
        if (coin >= 0)
        {
            totalCoin = coin;
            coinText.text = coin.ToString();
        }
        if (lifes >= 0)
        {
            totalLife = lifes;
            heartText.text = totalLife.ToString();
        }
    }

    string FormatCoinValue(int value, int max)
    {
        if (value >= max)
        {
            float valueInThousands = (float)value / 1000f;
            return valueInThousands.ToString("000.000") + "k";
        }
        return value.ToString();
    }

    public void SetBlockTypeImage(Sprite blockSprite)
    {
        if(blockSprite != null)
            blockShowImage.sprite = blockSprite;
        if (levelData != null && levelData.totalStarValue > 0)
            SetUpLevelShow(levelData);
    }


    private void BlockTilePanelStatus()
    {
        float fade = 1f;
        clickaudioSource.Play();
        BlockTileSetUp();
        if (isBlockPanelActive)
        {
            blockTypePanelTween.panel.DOAnchorPosY(blockTypePanelTween.posInitialFloat, 0.3f);
            fade = 0f;
            if (!isLevelShowActive && !isGoalPanelActive && !isBuyPanelActive && !isInfoPanelActive && !isFortuneWheelActive && !isLeaderboardShowActive)
            {
                LevelButtonShowAnim(true);
            }
            else
            {
                LevelButtonShowAnim(false);
            }
        }
        else
        {
            blockShowScroll.verticalNormalizedPosition = 5;
            LevelButtonShowAnim(isBlockPanelActive);
            blockTypePanelTween.panel.DOAnchorPosY(blockTypePanelTween.posFinalFloat, 0.3f);
            blockTypePanel.gameObject.SetActive(true);
            MenuTutorialManager.Instance.LevelTuteCross();
            BackButton(0.2f);

        }
        blockTypePanel.DOFade(fade, 0.3f).OnComplete(() =>
        { 
            isBlockPanelActive = !isBlockPanelActive;
            blockTypePanel.gameObject.SetActive(isBlockPanelActive);

        });
        
    }

    private void BlockTileSetUp()
    {
        if (isBlockTileSetUp) return;
        isBlockTileSetUp = true;
        int num = gameDataManager.GetSaveDataListlength(1); 
        BlockManager.Instance.SelectBlockIcon(currentBlockIndex);
        CheckForOnceUsedmatch();
        for (int i = 0; i < num; i++)
        {
            TileShow tile = Instantiate(tileShowPrefab, tileshowHolder);
            if (i < BlockManager.Instance.AllTileData.Length)
            {
                Block_Icon_Data data = gameDataManager.GetSaveBlockIconData(i);
                bool value = data.isBuyed;
                bool isfree = HasFreeBlockUsed(data.onceUsedData);
                tile.SetUp(i, currentBlockIndex, value, isfree);
            }
            else
            {
                tile.CommingSoonPanel(i);
            }
        }

        float sizeY = ((float)num / 2 + 1) * 455;
        tileshowHolder.sizeDelta = new Vector2(965, sizeY);
        
    }

    private bool HasFreeBlockUsed(int value)
    {
        bool isFree = true;
        System.DateTime dateTime = DateTime.Now;
        int result = int.Parse(dateTime.ToString("yyyyMMdd"));
        if (value > 0) {
            if (value != result)
            {
                isFree = false;
            }
        }

        return isFree;
    }

    private void CheckForOnceUsedmatch()
    {
        System.DateTime dateTime = DateTime.Now;
        int result = int.Parse(dateTime.ToString("yyyyMMdd"));
        bool isMatch = gameDataManager.CheckForBlockIconOnceUsed(result);
        if(!isMatch)
        {
            if (!gameDataManager.GetSaveBlockIconData(currentBlockIndex).isBuyed) 
            {
                currentBlockIndex = 0;
                BlockManager.Instance.SelectBlockIcon(currentBlockIndex);
            }
        }
    }


    private void LeaderboardPanelStatus()
    {
        clickaudioSource.Play();
        float fade = 0f;
        LevelButtonShowAnim(isLeaderboardShowActive);
        UpperLevelShow(isLeaderboardShowActive);

        if (isLeaderboardShowActive)
        {
            leaderboardShowPanelTween.panel.DOAnchorPosY(leaderboardShowPanelTween.posInitialFloat, 0.3f);
        }
        else
        {
            LeaderBoardManager.Instance.ShowLeaderBoard(1);
            leaderBoardBG.gameObject.SetActive(true);
            leaderboardShowPanelTween.panel.DOAnchorPosY(leaderboardShowPanelTween.posFinalFloat, 0.3f);
            fade = 0.8f;
        }
        leaderBoardBG.DOFade(fade, 0.3f).OnComplete(() =>
        { 
            isLeaderboardShowActive = !isLeaderboardShowActive;
            leaderBoardBG.gameObject.SetActive(isLeaderboardShowActive);
            UpdateLeaderboard();
        });
    }

    private void UpdateLeaderboard()
    {
        if (!isLeaderboardShowActive)
        {
            LeaderBoardManager.Instance.ShowLeaderBoard();
        }
    }


    private void GoalsPanelStatus()
    {
        clickaudioSource.Play();
        float fade = 0f;
        LevelButtonShowAnim(isGoalPanelActive);
        if (isGoalPanelActive)
        {
            goalPanelTween.panel.DOAnchorPosY(goalPanelTween.posInitialFloat, 0.3f);
        }
        else
        {
            goalPanelBg.gameObject.SetActive(true);
            goalPanelTween.panel.DOAnchorPosY(goalPanelTween.posFinalFloat, 0.3f);
            fade = 0.8f;
            DailyFreeReward(gameDataManager.HasNewDay);
        }
        goalPanelBg.DOFade(fade, 0.3f).OnComplete(() =>
        {
            isGoalPanelActive = !isGoalPanelActive;
            goalPanelBg.gameObject.SetActive(isGoalPanelActive);

        });
        SetUpGoalsPanel();
    }

    public void SetUpGoalsPanel()
    {
        DailyTarget goalTarget = goalsManager.DailyTargetData();
        if (goalTarget != null)
        {
            for (int i = 0; i < goalTarget.dailyTargetData.Length; i++)
            {
                if (dailyGoalShows[i] != null)
                {
                    dailyGoalShows[i].SetUpGoalShow(goalTarget.dailyTargetData[i], i);
                }
                else
                {
                    DisableDailyTarget(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < dailyGoalShows.Length; i++)
            {
                DisableDailyTarget(i);
            }
            freeButton.gameObject.SetActive(false);
        }
    }
    private void DisableDailyTarget(int index)
    {
        dailyGoalShows[index].DisableGoal();
    }

    private void GoalPanelPopUp()
    {
        goalPanelBg.gameObject.SetActive(true);
    }

    private void FortuneWheelPanelStatus()
    {
        clickaudioSource.Play();
        float fade = 0f;
        LevelButtonShowAnim(isFortuneWheelActive);
        if (isFortuneWheelActive)
        {
            wheelPanelTween.panel.DOAnchorPosY(wheelPanelTween.posInitialFloat, 0.3f);
        }
        else
        {
            fortuneWheel.SetUpFortuneWheel();
            wheelPanelBg.gameObject.SetActive(true);
            wheelPanelTween.panel.DOAnchorPosY(wheelPanelTween.posFinalFloat, 0.3f);
            fade = 0.9f;
        }
        wheelPanelBg.DOFade(fade, 0.3f).OnComplete(() =>
        {
            isFortuneWheelActive = !isFortuneWheelActive;
            wheelPanelBg.gameObject.SetActive(isFortuneWheelActive);

        });
        SetUpGoalsPanel();
    }
    private void FortunePanelPopUp()
    {
        wheelPanelBg.gameObject.SetActive(true);
    }

    private void BuyPanelStatus()
    {
        clickaudioSource.Play();
        float fade = 1f;
        LevelButtonShowAnim(isBuyPanelActive);
        if (isBuyPanelActive)
        {
            fade = 0f;
            buyPanelTween.panel.DOAnchorPosY(buyPanelTween.posInitialFloat, 0.3f);
        }
        else
        {
            buyPanelBg.gameObject.SetActive(true);
            shopManager.SetUp();
            buyPanelScroll.verticalNormalizedPosition = 3;
            buyPanelTween.panel.DOAnchorPosY(buyPanelTween.posFinalFloat, 0.3f);
        }
        buyPanelBg.DOFade(fade, 0.3f).OnComplete(() =>
        { 
            isBuyPanelActive = !isBuyPanelActive;
            buyPanelBg.gameObject.SetActive(isBuyPanelActive);

        });
    }
    private void InfoPanelStatus()
    {
        clickaudioSource.Play();
        float fade = 0f;
        LevelButtonShowAnim(isInfoPanelActive);
        if (isInfoPanelActive)
        {
            infoPanelTween.panel.DOAnchorPosY(infoPanelTween.posInitialFloat, 0.3f);
        }
        else
        {
            if (!isInfoSetup)
            {
                InfoPanelItemSetUp();
                isInfoSetup = true;
            }
            else
            {
                InfoAbilityDataUpdate();
            }
            infoPanelBg.gameObject.SetActive(true);
            infoPanelTween.panel.DOAnchorPosY(infoPanelTween.posFinalFloat, 0.3f);
            fade = 0.6f;

        }
        infoPanelBg.DOFade(fade, 0.3f).OnComplete(() =>
        {
            isInfoPanelActive = !isInfoPanelActive;
            infoPanelBg.gameObject.SetActive(isInfoPanelActive);

        });

    }

    private void InfoDetailsPanelStatus()
    {
        if (isInfoPanelActive)
        {

            clickaudioSource.Play();
            float fade = 0f;
            if (infoDetailsPanelStatus)
            {
                infoDetailsPanelTween.panel.DOAnchorPosY(infoDetailsPanelTween.posInitialFloat, 0.3f);
                infoAbilityIndex = -1;
            }
            else
            {
                infoDetailsPanelBg.gameObject.SetActive(true);
                infoDetailsPanelTween.panel.DOAnchorPosY(infoDetailsPanelTween.posFinalFloat, 0.3f);
                fade = 0.9f;
            }
            infoDetailsPanelBg.DOFade(fade, 0.3f).OnComplete(() =>
            {
                infoDetailsPanelStatus = !infoDetailsPanelStatus;
                infoDetailsPanelBg.gameObject.SetActive(infoDetailsPanelStatus);
            });
        }
    }

    public void InfoDetailsPanel(int index, bool isability)
    {
        infoBuyButton.interactable = false;
        if (isability)
        {
            if (detailsInfo != null)
            {
                infoBuyButton.gameObject.SetActive(true);
                infoAbilityIndex = index;
                int num = 0;
                if (abilityData[index].thisType != VariableTypeCode.None)
                {
                    num = gameDataManager.GameAbilitySave((int)abilityData[index].thisType - 1, true, 0);
                }
                bool status = CheckAbilityLock(abilityData[index].unLockvalue);
                detailsInfo.DetailsLockStatus(status, abilityData[index].unLockvalue);
                detailsInfo.SetUpInfoItemData(abilityData[index].iconSprite,num, abilityData[index].count, abilityData[index].name, abilityData[index].working, index, true);
                int price = abilityData[index].value;
                int coins = gameDataManager.GetSaveValues(0);
                if(price != 0 && coins >= price)
                {
                    infoBuyButton.interactable = true;
                }
                infoBuyText.text = price.ToString();

            }
            infoGetButton.gameObject.SetActive(false);// when shop manager active make this true
        }
        else
        {
            infoAbilityIndex = -1;
            infoBuyButton.gameObject.SetActive(false);
            infoGetButton.gameObject.SetActive(false);
            AbilityData tileData = BlockManager.Instance.tileDetailsData[index];
            if (tileData != null && detailsInfo != null)
            {
                detailsInfo.DetailsLockStatus(false, 0);
                detailsInfo.SetUpInfoItemData(tileData.iconSprite, -1, -1, tileData.name, tileData.working, index, false);
            }
        }
        InfoDetailsPanelStatus();
    }

    private void BuyInfoAbility()
    {
        
        if (infoAbilityIndex >=0 && infoAbilityIndex != 1 && infoAbilityIndex < abilityData.Length - 1)
        {
            int price = abilityData[infoAbilityIndex].value;
            int coins = gameDataManager.GetSaveValues(0);
            if (coins > price)
            {
                gameDataManager.CoinValueChange(price, false);
                gameDataManager.GameAbilitySave(infoAbilityIndex, true, abilityData[infoAbilityIndex].count);
            }
            InfoAbilityDataUpdate();
            InfoDetailsPanel(infoAbilityIndex, true);
        }
    }

    private void InfoPanelItemSetUp()
    {
        AbilityData[] tileData = BlockManager.Instance.tileDetailsData;
        for (int i = 0; i < tilesInfo.Length; i++)
        {
            if (tileData[i] != null)
            {
                if (tileInfoShowIndex < 0 || (i > 0 && i < tileInfoShowIndex))
                {
                    tilesInfo[i].SetUpItemDataForStartUp(tileData[i].iconSprite, -1, "", "", i, true, false);
                }
                else
                {
                    tilesInfo[i].SetUpItemDataForStartUp(null, 0, "", "", i, true, false, true);
                }
            }
        }
        InfoAbilityDataUpdate();
    }
    private void InfoAbilityDataUpdate()
    {
        for (int i = 0; i < abilityInfo.Length; i++)
        {
            if (i < abilityData.Length && abilityData[i] != null)
            {
                bool status = false;
                int num = 0;
                if (abilityData[i].thisType != VariableTypeCode.None)
                {
                    status = CheckAbilityLock(abilityData[i].unLockvalue);
                    num = gameDataManager.GameAbilitySave((int)abilityData[i].thisType - 1, true, 0);
                }
                bool ishide = false;
                if(abilityData[i].thisType == VariableTypeCode.Ability_1)
                {
                    ishide = true;
                }
                abilityInfo[i].SetUpItemDataForStartUp(abilityData[i].iconSprite, num, "", "", i, true, true, ishide);
                abilityInfo[i].LockStatus(status);
            }
        }
    }
    private bool CheckAbilityLock(int num)
    {
        bool isLock = true;
        if (num - 1 <= gameDataManager.currentLevel)
        {
            isLock = false;
        }
        return isLock;

    }
    private void InfoBuyPanel()
    {
        StartCoroutine(InfoBuyAbility());
    }

    private IEnumerator InfoBuyAbility()
    {
        infoDetailsPanelStatus = false;
        EscapeActive(infoDetailsPanelBg, infoDetailsPanelTween.panel, infoDetailsPanelTween.posInitialFloat, 0.2f);
        yield return new WaitForSeconds(0.25f);
        isInfoPanelActive = false;
        EscapeActive(infoPanelBg, infoPanelTween.panel, infoPanelTween.posInitialFloat, 0.2f);
        yield return new WaitForSeconds(0.25f);
        BuyPanelStatus();
    }

    private void OpenURL()
    {
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.DigionStudio.BlockTagBlockPuzzleGame");
    }

    private void CheckForAbilityUnlimitedTimer()
    {
        if (isInfoPanelActive)
        {
            if (infoDetailsPanelStatus)
            {
                DisableAllInfoEffects();
                CheckForUnlimitedInfoDetails();
            }
            else
            {
                detailsInfo.EnableUnlimitedEffect(false);
                CheckForAbilityInfoUnlimitedStatus();
            }
        }
        else
        {
            detailsInfo.EnableUnlimitedEffect(false);
        }
    }

    private void CheckForUnlimitedInfoDetails()
    {
        if(infoAbilityIndex >= 0 && infoAbilityIndex < 4)
        {
            string today = DateTime.Now.Date.ToString();
            int timeInSec = gameDataManager.CheckForAnilityStatus(infoAbilityIndex, today);
            if (timeInSec > 0)
            {
                detailsInfo.EnableUnlimitedEffect(true);
                detailsInfo.AbilityUnlimitedTimer(infoAbilityIndex, timeInSec);

            }
            else
            {
                detailsInfo.EnableUnlimitedEffect(false);
            }
        }
        else
        {
            detailsInfo.EnableUnlimitedEffect(false);
        }
    }
    private void CheckForAbilityInfoUnlimitedStatus()
    {
        for (int i = 0; i < 4; i++)
        {
            string today  = DateTime.Now.Date.ToString();
            int timeInSec = gameDataManager.CheckForAnilityStatus(i, today);
            if (timeInSec > 0)
            {
                abilityInfo[i].EnableUnlimitedEffect(true);
            }
            else
            {
                abilityInfo[i].EnableUnlimitedEffect(false);
            }
        }
    }

    private void DisableAllInfoEffects()
    {
        for (int i = 0; i < 4; i++)
        {
            if (abilityInfo[i] != null)
            {
                abilityInfo[i].EnableUnlimitedEffect(false);
            }
        }
    }


    private void Update()
    {
        CheckForAbilityUnlimitedTimer();
        if (!isEscapeActive && !isEscapeStatus && Input.GetKey(KeyCode.Escape))
        {
            if (isBlockPanelActive)
            {
                isBlockPanelActive = false;
                EscapeActive(blockTypePanel, blockTypePanelTween.panel, blockTypePanelTween.posInitialFloat, 0.2f);
            }
            else if (isLevelShowActive)
            {
                isLevelShowActive = false;
                EscapeActive(levelShowBg, levelShowPanel.panel, levelShowPanel.posInitialFloat, 0.2f);
            }
            else if (isGoalPanelActive)
            {
                isGoalPanelActive = false;
                EscapeActive(goalPanelBg, goalPanelTween.panel, goalPanelTween.posInitialFloat, 0.2f);
            }
            else if (isLeaderboardShowActive)
            {
                UpperLevelShow(isLeaderboardShowActive);
                isLeaderboardShowActive = false;
                EscapeActive(leaderBoardBG, leaderboardShowPanelTween.panel, leaderboardShowPanelTween.posInitialFloat, 0.2f);
                UpdateLeaderboard();
            }
            else if (isFortuneWheelActive)
            {
                isFortuneWheelActive = false;
                EscapeActive(wheelPanelBg, wheelPanelTween.panel, wheelPanelTween.posInitialFloat, 0.2f);
            }
            else if (isBuyPanelActive)
            {
                isBuyPanelActive = false;
                EscapeActive(buyPanelBg, buyPanelTween.panel, buyPanelTween.posInitialFloat, 0.2f);
            }
            else if (isInfoPanelActive)
            {
                if (infoDetailsPanelStatus)
                {
                    infoDetailsPanelStatus = false;
                    EscapeActive(infoDetailsPanelBg, infoDetailsPanelTween.panel, infoDetailsPanelTween.posInitialFloat, 0.2f);
                }
                else
                {
                    isInfoPanelActive = false;
                    EscapeActive(infoPanelBg, infoPanelTween.panel, infoPanelTween.posInitialFloat, 0.2f);
                }
            }
            else if (isLevelPanelActive)
            {
                if (isblockShowTutActive)
                {
                    MenuTutorialManager.Instance.LevelTuteCross();
                    isblockShowTutActive = false;
                }
                else
                {
                    LevelDisable();
                }
            }
            else
            {
                QuitGame();
            }

            if (!isBlockPanelActive && !isLevelShowActive && !isGoalPanelActive && !isBuyPanelActive && !isInfoPanelActive && !isFortuneWheelActive && !isLeaderboardShowActive && isLevelPanelActive)
            {
                LevelButtonShowAnim(true);
            }
        }
        
    }

    private void BackButton(float time)
    {
        if (isLevelShowActive)
        {
            isLevelShowActive = false;
            EscapeActive(levelShowBg, levelShowPanel.panel, levelShowPanel.posInitialFloat, time);
        }
        else if (isGoalPanelActive)
        {
            isGoalPanelActive = false;
            EscapeActive(goalPanelBg, goalPanelTween.panel, goalPanelTween.posInitialFloat, time);
        }
        else if (isLeaderboardShowActive)
        {
            isLeaderboardShowActive = false;
            UpperLevelShow(isLeaderboardShowActive);
            EscapeActive(leaderBoardBG, leaderboardShowPanelTween.panel, leaderboardShowPanelTween.posInitialFloat, time);
            UpdateLeaderboard();
        }
        else if (isFortuneWheelActive)
        {
            isFortuneWheelActive = false;
            EscapeActive(wheelPanelBg, wheelPanelTween.panel, wheelPanelTween.posInitialFloat, time);
        }
        else if (isBuyPanelActive)
        {
            isBuyPanelActive = false;
            EscapeActive(buyPanelBg, buyPanelTween.panel, buyPanelTween.posInitialFloat, time);
        }
        else if (isInfoPanelActive)
        {
            isInfoPanelActive = false;
            EscapeActive(infoPanelBg, infoPanelTween.panel, infoPanelTween.posInitialFloat, time);
        }
            
    }

    private void EscapeActive(Image bgImage ,RectTransform panel, float posY, float duration)
    {
        TweenPanel(panel, posY, duration);
        if(bgImage != null)
        {
            bgImage.DOFade(0, duration).OnComplete(() =>
            {
                bgImage.gameObject.SetActive(false);

            });
        }
    }

    public void CheckClaimButtons()
    {
        bool isAds = menuAdsManager.Load_Reward_Ads();
        bool isActive = goalsManager.CheckForGoalsFreeClaim(2);
        if(isAds && isActive)
            freeRewardHolder.SetActive(true);
        else
            freeRewardHolder.SetActive(false);

    }

    private void FreeCoin()
    {
        if (!menuAdsManager.isRewardShowing)
        {
            freeRewardHolder.SetActive(false);
            menuAdsManager.Show_Reward_Ads(1);
        }

    }
    private void FreeLife()
    {
        if (!menuAdsManager.isRewardShowing)
        {
            freeRewardHolder.SetActive(false);
            menuAdsManager.Show_Reward_Ads(2);
        }
    }
    public bool FortuneFreeSpin()
    {
        bool isActive = true;
        if (!menuAdsManager.isRewardShowing)
        {
            isActive = false;
            menuAdsManager.Show_Reward_Ads(3);
        }
        return isActive;
    }

    public bool CheckForFortune()
    {
        bool isAds = menuAdsManager.Load_Reward_Ads();
        if(isAds && menuAdsManager.isRewardShowing)
            isAds = false;
        return isAds;
    }


    public void ClaimAdsReward(int code)
    {
        if (code > 0 && code < 3)
        {
            if (code == 1)
            {
                Vector2 pos = coinButtonImage.TransformPoint(coinButtonImage.rect.center);
                targetEffect.FreeRewardEffectCoins(pos, 20);

            }
            else if (code == 2)
            {
                Vector2 pos = lifeImage.TransformPoint(lifeImage.rect.center);
                targetEffect.FreeRewardEffectLifes(pos, 1);
            }
            StartCoroutine(FreeRewardCO(code));
        }
    }

    IEnumerator FreeRewardCO(int code)
    {
        yield return new WaitForSeconds(1f);
        if (code == 1)
        {
            goalsManager.ClaimFree(true);
        }
        else if (code == 2)
        {
            goalsManager.ClaimFree(false);
        }
        CheckClaimButtons();
    }

    public void GoalsClaimButton(Vector2 pos, int count)
    {
        targetEffect.FreeRewardEffectCoins(pos, count);
    }

    private void DailyFreeReward(bool isNewDay)
    {
        if (isNewDay)
        {
            freeRewardHolder.SetActive(false);
        }else
            CheckClaimButtons();
        freeButton.gameObject.SetActive(isNewDay);
    }

    private void FreeReward()
    {
        DailyFreeReward(false);
        gameDataManager.SpecialData(1, false);
        gameDataManager.CoinValueChange(50, true);
        Vector2 pos = freeCoinImage.TransformPoint(freeCoinImage.rect.center);
        targetEffect.FreeRewardEffectCoins(pos, 10);
    }



    private IEnumerator CheckForGift()
    {
        yield return new WaitForSeconds(0.3f);
        GiftData[] data = gameDataManager.GetGiftData(false);
        int length = data.Length;
        if (gameDataManager.isGifted && data != null && length > 0)
        {
            giftHolder.gameObject.SetActive(true);
            bool status = Has_Life_Coin(data);
            Vector3 coinPos = new Vector3(0.6f, 1, 0);
            Vector3 lifepos = new Vector3(1f, 1, 0);
            Vector3 wheelpos = new Vector3(0f, -0.5f, 0);

            if (!status)
            {
                coinPos = new Vector3(0, 1f, 0);
                lifepos = new Vector3(0f, 1f, 0);

            }
            else
            {
                coinPos = new Vector3(0.7f, 0, 0);
                lifepos = new Vector3(-0.7f, 0, 0);
            }
            float Xpos = -1;
            bool isRandom = false;
            if (data.Length > 3)
            {
                isRandom = true;
            }
            float time = 1.5f;
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] != null && data[i].indexCode != VariableTypeCode.None)
                {
                    if (data[i].indexCode == VariableTypeCode.Coin)
                    {
                        int coinCount = data[i].values / 5;
                        if (coinCount > 30)
                        {
                            coinCount = 30;
                        }
                        targetEffect.FreeRewardEffectCoins(coinPos, coinCount, 0.5f);

                    }
                    else if (data[i].indexCode == VariableTypeCode.Life)
                    {
                        int lifeCount = data[i].values;
                        targetEffect.FreeRewardEffectLifes(lifepos, lifeCount);
                    }else if (data[i].indexCode == VariableTypeCode.Lucky_Wheel)
                    {
                        int wheelCount = data[i].values;
                        targetEffect.FreeRewardEffectWheel(wheelpos, wheelCount);
                    }
                    else
                    {
                        if (!isRandom)
                        {
                            Xpos = 0;
                        }

                        int index = (int)data[i].indexCode - 1;
                        if (index >= 0 && index < abilityData.Length)
                        {
                            Sprite sp = abilityData[index].iconSprite;
                            targetEffect.InstaAbilitygiftShow(sp, Xpos, length);
                            time = 3;
                        }
                        if (isRandom)
                        {
                            Xpos++;
                        }
                    }
                    yield return new WaitForSeconds(0.1f);


                }
            }
            
            gameDataManager.GiftValueClaim();
            Invoke(nameof(DisableGiftholder), time);
        }
        else
        {
            DisableGiftholder();
        }
    }

    private bool Has_Life_Coin(GiftData[] data)
    {
        bool isStatus = false;
        bool isCoin = false;
        bool isLife = false;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] != null && data[i].indexCode != VariableTypeCode.None)
            {
                if (data[i].indexCode == VariableTypeCode.Coin)
                {
                    isCoin = true;

                }
                else if (data[i].indexCode == VariableTypeCode.Life)
                {
                    isLife = true;
                }
            }
        }

        if(isCoin && isLife)
        {
            isStatus = true;
        }
        return isStatus;
    }

    private void DisableGiftholder()
    {
        giftHolder.gameObject.SetActive(false);
        gameDataManager.isGifted = false;
        isEscapeStatus = false;
        if(isblockShowTutActive)
            MenuTutorialManager.Instance.BlockTutActive(blockIconButton.transform.position);
        else
        {
            if(gameDataManager.currentLevel > 1 && isMenuPlay && isInitial)
            {
                isInitial = false;
                LevelShowPopUp();
            }
        }
    }

    public void GiftPanelForMetaFill(bool status)
    {
        if (status)
        {
            if (!menuLevelData.MetaFill())
            {
                CheckForNextEffect();
            }
            else
            {
                bonusGiftManager.GiftPanelForMetaFill(true);
                LevelButtonShowAnim(false);
                isEscapeStatus = true;
            }
        }
        else
        {
            CheckForNextEffect();
        }
    }
    private void CheckForNextEffect()
    {
        bonusGiftManager.GiftPanelForMetaFill(false);
        LevelButtonShowAnim(true);
        if (gameDataManager.isGifted)
        {
            isEscapeStatus = true;
            giftHolder.gameObject.SetActive(true);
            StartCoroutine(CheckForGift());
        }
        else
        {
            DisableGiftholder();
        }
    }


    //    private bool isWandActive;
    //    public Button wandActiveButton;
    //    public Image wandSelectedImage;
    //    public Text wandCountText;
    //    public GameObject wandDisableObj;
    //    public GameObject wandEnableEffectObj;

    private void CheckForWandActivation(int currentLevel)
    {
        wandCountText.gameObject.SetActive(false);
        int count = gameDataManager.GameAbilitySave(5, true, 0);
        if (currentLevel > 10 && count > 0)
        {
            wandCountText.gameObject.SetActive(true);
            wandActiveButton.interactable = true;
            wandDisableObj.SetActive(false);
            WandStatus(isWandActive, count);

        }
        else if (currentLevel > 7 && currentLevel <= 10)
        {
            WandStatus(true, count);
            wandActiveButton.interactable = false;
            wandDisableObj.SetActive(false);
        }
        else
        {
            wandActiveButton.interactable = false;
            wandDisableObj.SetActive(true);
            wandSelectedImage.color = unSelectedColor;
        }
    }

    private void WandActive()
    {
        int count = gameDataManager.GameAbilitySave(5, true, 0);
        if (count > 0)
        {
            isWandActive = !isWandActive;
            WandStatus(isWandActive, count);
        }
    }
    private void WandStatus(bool iswand, int count)
    {
        Color color = unSelectedColor;
        if (iswand)
        {
            color = Color.green;
        }
        wandCountText.text = count.ToString();
        wandSelectedImage.color = color;
        wandEnableEffectObj.SetActive(iswand);
    }

    private void LevelShowPopUp()
    {
        int wheelCount = gameDataManager.GetSaveValues(10);
        if (!isBlockPanelActive && !isLevelShowActive && !isGoalPanelActive && !isBuyPanelActive && !isInfoPanelActive && !isFortuneWheelActive && !isblockShowTutActive && !isLeaderboardShowActive && isLevelPanelActive)
        {
            if (gameDataManager.HasNewDay || goalsManager.HasGoalsAchived())
            {
                GoalPanelPopUp();
                Invoke(nameof(GoalsPanelStatus), 0.5f);
            }else if(wheelCount > 0)
            {
                FortunePanelPopUp();
                Invoke(nameof(FortuneWheelPanelStatus), 0.5f);
            }
        }
    }

}
