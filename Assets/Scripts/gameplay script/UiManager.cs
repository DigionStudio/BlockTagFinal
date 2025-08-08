using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[Serializable]
public class RankShow
{
    public RectTransform rankRect;
    public LeaderboardShow leaderboardShow;
}

[Serializable]
public class StarObject
{
    public GameObject Star;
}


[Serializable]
public class UpperObject
{
    public GameObject holder;
    public GameObject addEffect;
}


[Serializable]
public class GiftContent
{
    public GameObject holder;
    public Image image;
    public GameObject deniedImage;
    public Text valueText;
    public Text nameText;
}



[Serializable]
public class UpperPanel
{
    public GameObject holder;
    public Text valueText;
}

[Serializable]
public class PanelTween
{
    public RectTransform panel;
    public float posInitialFloat;
    public float posFinalFloat;
}

public class UiManager : MonoBehaviour
{
    [SerializeField] private Image startFadeImage;
    [SerializeField] private Button refreshButton;
    [SerializeField] private GameObject disableRefreshButton;
    [SerializeField] private Image disableRefreshButtonFill;
    [SerializeField] private Button rotateButton;
    [SerializeField] private GameObject disableRotateButton;
    [SerializeField] private Image disableRotateButtonFill;
    [SerializeField] private Button pauseButton;
    private Image pauseIcon;
    private bool isPaused;
    [SerializeField] private Sprite pausedSprite;
    [SerializeField] private Sprite playSprite;
    private ShapeCreator shapeCreator;

    [SerializeField] private UpperPanel[] coinAddObject;
    [SerializeField] private Text[] cointext;
    private UpperPanel currentCoinobject;
    private Text currentCoinText;


    [SerializeField] private UpperObject[] upperHolderType;
    [SerializeField] private UpperPanel[] panel1;
    [SerializeField] private UpperPanel[] panel2;

    [SerializeField] private TargetShow[] targetUI;
    [SerializeField] private GameObject targetScore;
    public int GameTypeCode { get; private set; }

    private Text currentScore;
    private Text highScore;
    private Text moveText;
    private GameObject addEffect;
    private bool isScoreTarget;
    private int totalStarValue;

    private bool isTimer;
    private int currentScoreInt;
    private int prevScoreInt;


    private int highScoreInt;
    private int prevhighScoreInt;
    private int maxHitPoint;
    public int currentHitPoint;
    [SerializeField] private Text[] scoreEffectString;
    private Text currentScoreEffectString;

    [SerializeField] private GameObject[] gameOverHolder;

    [SerializeField] private PanelTween[] panelTween;
    [SerializeField] private Image gameOverPanel;
    [SerializeField] private UpperObject[] gameOverPanelObject;
    [SerializeField] private GameObject newHighScore;
    [SerializeField] private GameObject newHighHit;
    [SerializeField] private PanelTween[] gameOverPanelTween;
    [SerializeField] private PanelTween gameOverButtonPanelTween;
    [SerializeField] private GameOverPoints[] overPoints;
    [SerializeField] private Sprite[] gameOverTextStatus;
    [SerializeField] private Sprite[] gameOverSpriteStatus;

    [SerializeField] private StarObject[] gameStarObject;
    [SerializeField] private Image[] gameStarbgImage;
    [SerializeField] private Color[] gameStarbgColor;
    [SerializeField] private StarObject[] gameEndStarObject;
    [SerializeField] private GameObject gameEndStarPanel;
    [SerializeField] private Image barFill;
    [SerializeField] private Image barFill2;
    [SerializeField] private Button[] gameOverButton;
    [SerializeField] private AudioSource startaudioSource;
    [SerializeField] private AudioSource endaudioSource;
    [SerializeField] private AudioSource failedaudioSource;
    [SerializeField] private AudioSource clickaudioSource;
    [SerializeField] private AudioSource pointsaudioSource;
    [SerializeField] private AudioListener audioListener;
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private GameObject quitHolder;
    [SerializeField] private PanelTween settingPanel;
    [SerializeField] private Button settingsCrossButton;
    [SerializeField] private Button audioButton;
    private Image audioButtonImage;
    [SerializeField] private Button sfxButton;
    private Image sfxButtonImage;
    [SerializeField] private Button voiceButton;
    private Image voiceButtonImage;
    [SerializeField] private Sprite[] soundSprite;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button finishButton;
    [SerializeField] private Button quitButton;
    private int audioStatusInt;
    private int sfxStatusInt;
    private int voiceStatusInt;
    [SerializeField] private Button moveButton;

    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private Text levelText;
    [SerializeField] private ContentSizeFitter contentSizeFitter;
    [SerializeField] private Text lifeText;
    [SerializeField] private Transform gameButtonsPos;


    [SerializeField] private GameObject offlineHolder;
    [SerializeField] private Button tryAgainButton;
    [SerializeField] private Button menuButton;

    [SerializeField] private GameObject confetiEffect;
    [SerializeField] private GameObject confetiEffectNewHigh;
    [SerializeField] private GameObject pointEffect;
    [SerializeField] private GiftContent[] giftContent;
    private GiftData[] giftData;


    private int currentLevel;
    private int gameStatus = -1;

    [SerializeField] private InverseMaskUI upperPanel;
    [SerializeField] private Image bgPanel;
    public Sprite[] bgImageSprite;

    [SerializeField] private Image freezeImage;
    [SerializeField] private GameObject bgFill;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject rankShowObj;
    [SerializeField] private GameObject giftShowObj;
    [SerializeField] private RankShow[] rankShow;
    [SerializeField] private Text rankStatusText;
    private PlayerGlobalData rankDataCurrent;
    public static UnityEvent PlayerRankdataUpdate = new();

    [SerializeField] private Transform spTrans;
    [SerializeField] private Image spIcon;
    [SerializeField] private Text detailText;
    

    private BoardManager boardManager;
    private GameManager gameManager;
    private AbilityManager abilityManager;
    private GameDataManager gameDataManager;
    private GameAdsManager gameAdsManager;
    private bool isNoAds;
    private bool isUsergameEnd;
    void Start()
    {
        startFadeImage.DOFade(1, 0f);
        startFadeImage.DOFade(0, 1f);
        currentHitPoint = 0;
        isUsergameEnd = false;
        gameManager = FindObjectOfType<GameManager>();
        shapeCreator = ShapeCreator.Instance;
        boardManager = BoardManager.Instance;
        abilityManager = AbilityManager.Instance;
        gameAdsManager = GameAdsManager.Instance;
        gameDataManager = boardManager.gameDataManager;
        currentLevel = gameDataManager.currentLevel;
        isNoAds = gameDataManager.HasDisableAds;
        if (gameDataManager.currentLevel < 5)
        {
            isNoAds = true;
        }
        PlayerRankdataUpdate.AddListener(PlayerRankUpdate);

        audioStatusInt = gameDataManager.GetSaveValues(7); 
        sfxStatusInt = gameDataManager.GetSaveValues(11); 
        voiceStatusInt = gameDataManager.GetSaveValues(12); 
        audioButtonImage = audioButton.GetComponent<Image>();
        GameSountSetup();
        sfxButtonImage = sfxButton.GetComponent<Image>();
        GameSfxSetup();
        voiceButtonImage = voiceButton.GetComponent<Image>();
        GameVoiceSetup();

        settingsCrossButton.onClick.AddListener(PauseGame);
        restartButton.onClick.AddListener(RestartGame);
        finishButton.onClick.AddListener(ReSetUpBoard);
        audioButton.onClick.AddListener(GameSound);
        sfxButton.onClick.AddListener(SfxSound);
        voiceButton.onClick.AddListener(VoiceSound);
        quitButton.onClick.AddListener(QuitGame);
        Setting();


        pauseIcon = pauseButton.transform.GetChild(0).GetComponent<Image>();
        refreshButton.onClick.AddListener(RefreshCrush);
        rotateButton.onClick.AddListener(ResetCrushPanel);
        pauseButton.onClick.AddListener(PauseGame);
        gameOverButton[0].onClick.AddListener(ReStart);
        gameOverButton[1].onClick.AddListener(MainMenu);


        moveButton.onClick.AddListener(OpenBuyPanel);
        tryAgainButton.onClick.AddListener(TryAgain);
        menuButton.onClick.AddListener(QuitGame);

        giftData = new GiftData[2];
        confetiEffect.SetActive(false);
        confetiEffectNewHigh.SetActive(false);
        Resetgift();
        GameStatus();
        GetPanelPosY();
        DisableSpTrans();
        DisableRefresh();
        DisableRotate();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    private void ReSetUpBoard()
    {
        clickaudioSource.Play();
        isPaused = false;
        PauseManu();
        boardManager.GameStatus(false);
        BuyPanel.Instance.SetUpLifePanel(true, 0);
    }

    public void ResetUp(bool isads)
    {
        boardManager.ReSetUpBoard();
        if (!isads)
        {
            gameDataManager.LifeValueChange(1, false);
            GetLifeCount();
        }
        Invoke(nameof(ResetupBlocks), 1.7f);
    }

    private void ResetupBlocks()
    {
        boardManager.GameStatus(true);
    }

    

    private void GameSound()
    {
        if (audioStatusInt == 1)
            audioStatusInt = 0;
        else
            audioStatusInt = 1;
        gameDataManager.SetSaveValues(7, audioStatusInt);
        GameSountSetup();
    }

    private void GameSountSetup()
    {
        int value = 0;
        if (audioStatusInt == 1)
        {
            value = -70;
        }
        audioMixer.SetFloat("Master", value);
        SoundSprite(audioButtonImage, audioStatusInt);
    }
    private void SfxSound()
    {
        if (sfxStatusInt == 1)
            sfxStatusInt = 0;
        else
            sfxStatusInt = 1;
        gameDataManager.SetSaveValues(11, sfxStatusInt);
        GameSfxSetup();
    }

    private void GameSfxSetup()
    {
        int value = 0;
        if (sfxStatusInt == 1)
        {
            value = -70;
        }
        audioMixer.SetFloat("Main SFX", value);
        SoundSprite(sfxButtonImage, sfxStatusInt);
    }

    private void VoiceSound()
    {
        if (voiceStatusInt == 1)
            voiceStatusInt = 0;
        else
            voiceStatusInt = 1;
        gameDataManager.SetSaveValues(12, voiceStatusInt);
        GameVoiceSetup();
    }

    private void GameVoiceSetup()
    {
        int value = 0;
        if (voiceStatusInt == 1)
        {
            value = -70;
        }
        audioMixer.SetFloat("Voice Sfx", value);
        SoundSprite(voiceButtonImage, voiceStatusInt);
    }

    private void SoundSprite(Image image, int statuscode)
    {
        image.sprite = soundSprite[statuscode];
    }

    public void AdsGameSountSetup(bool isAds)
    {
        int value = -70;
        if (!isAds && audioStatusInt == 0)
        {
            value = 0;
        }
        audioMixer.SetFloat("Master", value);
    }

    private void RestartGame()
    {
        DisableSettings();
        Invoke(nameof(Restart), 0.3f);
        isUsergameEnd = true;
        //if (!isNoAds)
        //{
        //    isUsergameEnd = true;
        //    gameAdsManager.Show_Reward_Ads();
        //}
        //else
        //{
        //    RestartActive();
        //}
    }

    private void Restart()
    {
        if(!isUsergameEnd && GameTypeCode == 1)
            gameDataManager.rePlayCount++;
        SceneManager.LoadScene(1);
    }

    private void MainMenu()
    {
        clickaudioSource.Play();
        ChangePanelPos(false, gameOverPanelTween, false, 0.5f);
        TweenPanel(false, gameOverButtonPanelTween, 0.5f);
        gameManager.GameEndValues();
        Invoke(nameof(ToMainMenu), 0.5f);
    }

    private void ToMainMenu()
    {
        if (GameTypeCode == 1)
            SceneManager.LoadScene(0);
        else
        {
            Quit();
        }
    }

    private void GetPanelPosY()
    {
        for (int i = 0; i < panelTween.Length; i++)
        {
            var panel = panelTween[i];
            if (panel != null)
            {
                if (i != 0)
                    panel.posFinalFloat = panel.panel.anchoredPosition.y;
                else
                {
                    panel.posFinalFloat = WorldToAnchoredPosition(gameButtonsPos.position, panel.panel);
                }
            }

        }
        newHighScore.SetActive(false);
        newHighHit.SetActive(false);
        ChangePanelPos(false, panelTween, true);
        ChangePanelPos(false, gameOverPanelTween, false);
        TweenPanel(false, gameOverButtonPanelTween, 0);
    }
    float WorldToAnchoredPosition(Vector3 worldPosition, RectTransform rectTransform)
    {
        // Convert world position to local position of the RectTransform's parent
        Vector3 localPosition = rectTransform.parent.InverseTransformPoint(worldPosition);

        // Get the pivot offset
        Vector2 pivotOffset = new Vector2(
            rectTransform.rect.width * rectTransform.pivot.x,
            rectTransform.rect.height * rectTransform.pivot.y
        );

        // Adjust the local position with the pivot offset to get the anchored position
        Vector2 anchoredPosition = new Vector2(
            localPosition.x + pivotOffset.x,
            localPosition.y + pivotOffset.y
        );

        return anchoredPosition.y;
    }

    private void ReStart()
    {
        clickaudioSource.Play();
        ChangePanelPos(false, gameOverPanelTween,false, 0.5f);
        TweenPanel(false, gameOverButtonPanelTween, 0.5f);
        if(GameTypeCode == 1)
        {
            AdsLeaderboardManager.Instance.CheckAnalyticsEvent(6, currentLevel);
        }
        Invoke(nameof(Restart), 0.5f);

    }

    public void StartGame(int upperUIIndex, LevelData lvData)
    {
        horizontalLayoutGroup.enabled = false;
        contentSizeFitter.enabled = true;
        levelText.text = (lvData.levelNumber + 1).ToString();
        Invoke(nameof(LevelShowSetUp), 0.3f);
        GetLifeCount();
        currentScoreInt = 0;
        ScoreText(0);
        startaudioSource.Play();
        ChangePanelPos(true, panelTween, true);
        gameOverPanel.gameObject.SetActive(false);
        SetUpUpperPanel(upperUIIndex, lvData);

    }

    private void LevelShowSetUp()
    {
        horizontalLayoutGroup.enabled = true;
        contentSizeFitter.enabled = false;
    }

    private void SetUpUpperPanel(int upperUIIndex, LevelData lvData)
    {
        totalStarValue = lvData.totalStarValue;
        isScoreTarget = lvData.isScoreTarget;
        TargetData[] targetdata = lvData.targetData;
        GameTypeCode = upperUIIndex;
        foreach (var item in upperHolderType)
        {
            item.holder.SetActive(false);
        }
        foreach (var item in upperHolderType)
        {
            item.addEffect.SetActive(false);
        }
        upperHolderType[upperUIIndex].holder.SetActive(true);
        addEffect = upperHolderType[upperUIIndex].addEffect;
        addEffect.SetActive(false);
        currentScoreEffectString = scoreEffectString[upperUIIndex];

        if (upperUIIndex == 0)
        {
            currentScore = panel1[0].valueText;
            highScore = panel1[1].valueText;
        }
        else
        {
            GameStarSetUp();
            currentScore = panel2[0].valueText;
            moveText = panel2[1].valueText;
            targetScore.SetActive(false);

            foreach (var item in targetUI)
            {
                item.gameObject.SetActive(false);
            }
            if (!isScoreTarget)
            {
                for (int i = 0; i < targetdata.Length; i++)
                    targetUI[i].SetUp(targetdata[i]);
            }
            else
            {
                ScoreText(0);
                targetScore.SetActive(true);
            }

        }
    }

    private void GameStarSetUp()
    {

        foreach (var item in gameStarObject)
        {
            item.Star.SetActive(false);
        }

        foreach (var item in gameStarbgImage)
        {
            item.color = gameStarbgColor[0];
        }

        foreach (var item in gameEndStarObject)
        {
            item.Star.SetActive(false);
        }

        barFill.fillAmount = 0;
        barFill2.fillAmount = 0;
    }

    public void StarBarFill(float num, int index, int gamestatus)
    {
        if (gamestatus != 0)
        {
            if (index == 0)
                barFill.DOFillAmount(num, 0.5f);
            else
                barFill2.DOFillAmount(num, 0.5f);
        }
    }

    public void StarSetUp(int num, bool isGame, int gameCode, int gamestatus)
    {
        if (gamestatus != 0)
        {
            if (isGame)
            {
                if (gameCode == 0)
                {
                    ActiveStars(num, gameStarObject);
                }
                else
                {
                    ChangeStarBgColor(num);
                }
            }
            else
            {
                ActiveStars(num, gameEndStarObject);
            }
        }
    }

    private void ChangeStarBgColor(int num)
    {
        if (num > 0 && num <= 3)
        {
            for (int i = 0; i < num; i++)
            {
                gameStarbgImage[i].color = gameStarbgColor[1];
            }
        }
    }

    private void ActiveStars(int num, StarObject[] starObject)
    {
        if (num > 0 && num <= 3)
        {
            for (int i = 0; i < num; i++)
            {
                starObject[i].Star.SetActive(true);
            }
        }
    }
    public void ChangetargetCount(int count, int index, bool change)
    {
        if(change)
            StartCoroutine(ChangeTargetCountCO(count, index, change));
        
    }
    IEnumerator ChangeTargetCountCO(int count, int index, bool change)
    {
        targetUI[index].CountUpdateAnim(true);
        yield return new WaitForSeconds(1f);
        targetUI[index].UpdateTargetCount(count);
    }

    public void DisableSettings()
    {
        isPaused = false;
        Setting();
    }

    public void GameEndsetup()
    {
        TweenPanel(false, panelTween[0], 0.5f);
        TweenPanel(false, panelTween[2], 0.5f);
    }
    
    public void GameEnd(int gamestatus)
    {
        
        isPaused = false;
        PauseManu();
        newHighScore.SetActive(false);
        newHighHit.SetActive(false);
        GameEndObjectSetup(4, 0);
        gameStatus = gamestatus;
        if(GameTypeCode == 1)
        {
            if(gamestatus == 0)
                LoseGiftSetUp();
        }
        else
        {
            SetUpRankData();
        }

        //gameOverPanelImage.DOFade(1, 0.5f);

        prevhighScoreInt = gameDataManager.GetSaveValues(3);
        highScoreInt = gameDataManager.GetSaveValues(2);
        maxHitPoint = gameDataManager.GetSaveValues(6);
        PanelAnimationDelay();
    }

    private void PanelAnimationDelay()
    {
        if (!isNoAds && gameAdsManager.CkeckInterAds())
            gameAdsManager.Show_Inter_Ads(1);
        else
        {
            GameEndPanelSetUp();
        }
    }

    private void GameEndObjectSetup(int num, int gameState)
    {
        if(num < 2)
        {
            
            if (gameState == 1)
            {
                foreach (Button bt in gameOverButton)
                {
                    bt.gameObject.SetActive(false);
                }
                gameOverButton[gameState].gameObject.SetActive(true);
            }
            
            var item = gameOverPanelObject[num];
            if (item.holder)
                item.holder.SetActive(true);
            if (item.addEffect)
                item.addEffect.SetActive(true);
            if(num == 1)
            {
                gameEndStarPanel.SetActive(true);
                Image itemImage = item.holder.GetComponent<Image>();
                itemImage.sprite = gameOverSpriteStatus[gameState];
                item.addEffect.GetComponent<Image>().sprite = gameOverTextStatus[gameState];
                if (gameState == 0)
                    failedaudioSource.Play();
                else
                {
                    endaudioSource.Play();
                }

            }
            else
            {
                endaudioSource.Play();
            }
        }
        else
        {
            foreach (var item in gameOverPanelObject)
            {
                if (item != null)
                {
                    if (item.holder)
                        item.holder.SetActive(false);
                    if (item.addEffect)
                        item.addEffect.SetActive(false);
                }
            }
            gameEndStarPanel.SetActive(false);
        }
    }

    public void SetUpAfterAds()
    {
        Invoke(nameof(GameEndPanelSetUp), 0.5f);
    }
    private void GameEndPanelSetUp()
    {
        gameOverPanel.gameObject.SetActive(true);
        ChangePanelPos(false, panelTween, true, 0.3f);
        bool isNewHIgh = ChekforNewHighScore();
        if (isNewHIgh)
        {
            gameOverPanel.color = new Color(0,0,0,0);
            foreach (var item in gameOverHolder)
                item.SetActive(true);
            Invoke(nameof(GameOverSetup), 4f);
        }
        else
        {
            GameOverSetup();
        }
    }

    private void GameOverSetup()
    {
        foreach (var item in gameOverHolder)
            item.SetActive(false);
        gameOverPanel.DOFade(0.8f, 0.3f);
        GameEndObjectSetup(GameTypeCode, gameStatus);
        ChangePanelPos(true, gameOverPanelTween, false, 0.5f);
        pointsaudioSource.Play();
        Invoke(nameof(SetUpGameOverPoints), 0.5f);
    }


    private bool CheckNewHit()
    {
        bool status = false;
        if (gameStatus == 1 || gameDataManager.GameTypeCode == 0)
        {
            if (currentHitPoint >= maxHitPoint)
            {
                maxHitPoint = currentHitPoint;
                status = true;
            }
        }
        return status;
    }

    private bool ChekforNewHighScore()
    {
        bool status = false;
        if (gameStatus == 1 || gameDataManager.GameTypeCode == 0)
        {
            if (currentScoreInt >= highScoreInt)
            {
                status = true;
            }
        }
        else
        {
            currentScoreInt = 0;
        }
        return status;
    }
    private void SetUpGameOverPoints()
    {
        int num = highScoreInt;
        if (gameStatus == 1 || gameDataManager.GameTypeCode == 0)
        {
            bool newScore = ChekforNewHighScore();
            newHighScore.SetActive(newScore);
            if(newScore)
            {
                num = prevhighScoreInt;
            }
            bool newHit = CheckNewHit();
            newHighHit.SetActive(newHit);
            confetiEffect.SetActive(true);
            confetiEffectNewHigh.SetActive(newScore);
        }
        overPoints[0].SetUp(currentScoreInt);
        overPoints[1].SetUp(highScoreInt, num);
        overPoints[2].SetUp(currentHitPoint);
        AdsLeaderboardManager.Instance.CheckAnalyticsEvent(4);

        if (gameDataManager.GameTypeCode == 0)
        {
            GetNewPlayerData();
        }
        else
        {
            TweenPanel(true, gameOverButtonPanelTween, 0.5f);
        }
    }

    private void ChangePanelPos(bool isfinalpos, PanelTween[] tweenPanel,bool isGame, float duration = 0)
    {
        int num = tweenPanel.Length;
        if(TutorialManager.Instance != null && TutorialManager.Instance.isTutorial && isfinalpos && isGame) 
        {
            num = 1;
        }
        for (int i = 0; i < num; i++)
        {
            PanelTween panel = tweenPanel[i];
            TweenPanel(isfinalpos, panel, duration);
        }
    }

    private void TweenPanel(bool isfinalpos, PanelTween panel, float duration = 0)
    {
        if(panel != null)
        {
            float posy = panel.posInitialFloat;
            float time = duration;
            if (isfinalpos)
            {
                posy = panel.posFinalFloat;
                time = 0.3f;
            }
            panel.panel.DOAnchorPosY(posy, time)
        .SetEase(Ease.OutQuad);
        }
    }

    private void RefreshCrush()
    {
        if (!boardManager.isGameStarted && boardManager.HasTurn) return;
        shapeCreator.ResetCrushTiles();
    }

    private void ResetCrushPanel()
    {
        if (!boardManager.HasTurn && !boardManager.isGameStarted) return;
        shapeCreator.RotateCurrentCrushTile();
    }

    private void Setting()
    {
        float posy = settingPanel.posInitialFloat;
        if (isPaused)
        {
            quitHolder.SetActive(isPaused);
            posy = settingPanel.posFinalFloat;
        }
        settingPanel.panel.DOAnchorPosY(posy, 0.3f).OnComplete(() =>
        {
            quitHolder.SetActive(isPaused);

        });
    }
    private void PauseGame()
    {
        clickaudioSource.Play();
        isPaused = !isPaused;
        
        boardManager.GameStatus(!isPaused);
        PauseManu();
    }

    private void PauseManu()
    {
        Setting();
        GameStatus();
    }

    private void GameStatus()
    {
        Sprite thisSprite = pausedSprite;
        if (isPaused) {
            thisSprite = playSprite;
        }
        pauseIcon.sprite = thisSprite;
    }

    public void SetUpHighScore(int val)
    {
        if(highScore != null)
            highScore.text = val.ToString();
        
    }
    public void SetUpCurrentScore(int val)
    {
        if (val > 0)
        {
            isTimer = false;
            prevScoreInt = currentScoreInt;
            currentScoreInt += val;
            ScoreText(prevScoreInt);
            currentScoreEffectString.text = "+" + val.ToString();
            addEffect.SetActive(true);
            isTimer = true;
            Invoke(nameof(ScoreEfectDisable), 1f);
        }
    }
    private void ScoreEfectDisable()
    {
        prevScoreInt = currentScoreInt;
        addEffect.SetActive(false);
    }

    private void FixedUpdate()
    {
        if(isTimer)
        {
            if (prevScoreInt < currentScoreInt)
            {
                prevScoreInt += 5;
            }
            else
            {
                prevScoreInt = currentScoreInt;
                isTimer = false;
            }
            ScoreText(prevScoreInt);
        }
    }

    
    public void GameEndScoreSet(int val)
    {
        isTimer = false;
        ScoreText(val);
    }

    private void ScoreText(int num)
    {
        if(GameTypeCode == 1 && isScoreTarget)
        {
            num = totalStarValue - num;
            if(num < 0)
            {
                num = 0;
            }
        }
        if(currentScore != null)
            currentScore.text = num.ToString();

    }


    public void CoinTextSetup(int num, int value)
    {
        currentCoinobject = coinAddObject[num];
        currentCoinText = cointext[num];
        SetLevelPanelValueText(value);
    }

    public void AddCoin(int val, bool isadd)
    {
        if (currentCoinobject != null && val > 0)
        {
            string symbol = "+";
            if(!isadd)
                symbol = "-";
            currentCoinobject.valueText.text = symbol + val.ToString();
            currentCoinobject.holder.SetActive(true);
            Invoke(nameof(CoinUI), 1f);
        }
    }
    private void CoinUI()
    {
        currentCoinobject.holder.SetActive(false);

    }

    public void SetLevelPanelValueText(int coin)
    {
        if (currentCoinText != null)
            currentCoinText.text = coin.ToString();
    }
    

    public void MoveCount(int count)
    {
        if(moveText != null)
            moveText.text = count.ToString();
    }

    private void QuitGame()
    {
        gameDataManager.SavePlayerData();
        if (!isNoAds && gameAdsManager.CkeckInterAds())
        {
            gameAdsManager.Show_Inter_Ads(4);
        }
        else
        {
            Quit();
        }
    }

    public void Quit()
    {
        gameManager.GameEndValues();
        gameDataManager.isMenuOpened = true;
        SceneManager.LoadScene(0);
    }


    private void OpenBuyPanel()
    {
        clickaudioSource.Play();
        BuyPanel.Instance.SetUpMoveBuyPanel(true);
    }

    public int GetLifeCount()
    {
        int life = gameDataManager.GetSaveValues(1);
        lifeText.text = life.ToString();
        return life;
    }

    public void OpenOfflinePanel(bool status)
    {
        offlineHolder.SetActive(status);
    }

    private void TryAgain()
    {
        gameManager.CheckGameConnectivity();
    }
    private void GetNewPlayerData()
    {
        AdsLeaderboardManager.Instance.GetCurrentPlayerTopScoreData();
    }
    private void PlayerRankUpdate()
    {
        PlayerGlobalData data = gameDataManager.GetPlayerGlobalData();
        rankShowObj.gameObject.SetActive(true);
        if (data != null && !string.IsNullOrEmpty(data.Name))
        {
            StartCoroutine(RankShowCO(data));
            
        }
        else
        {
            DisplayDefaultRank();
        }
    }

    IEnumerator RankShowCO(PlayerGlobalData data)
    {
        yield return new WaitForSeconds(0.5f);
        int diff = rankDataCurrent.Rank - data.Rank;
        rankStatusText.gameObject.SetActive(true);
        if (diff > 0)
        {
            
            rankStatusText.text = "+" + diff + " Rank Up";
            rankShow[1].leaderboardShow.SetUp(data, 1, true, 1);
            yield return new WaitForSeconds(0.5f);
            rankShow[1].rankRect.gameObject.SetActive(true);
            rankShow[0].rankRect.DOAnchorPosY(-170f, 0.5f);
            rankShow[0].leaderboardShow.UIElementVisibility(0, 0.5f);
            rankShow[1].rankRect.DOAnchorPosY(0f, 0.5f);
            rankShow[1].leaderboardShow.UIElementVisibility(1, 0.5f);
            yield return new WaitForSeconds(1f);
            rankShow[0].rankRect.gameObject.SetActive(false);
            TweenPanel(true, gameOverButtonPanelTween, 0.5f);
            yield return new WaitForSeconds(2f);
            rankStatusText.gameObject.SetActive(false);
            rankShow[1].leaderboardShow.ResetBgColor();
        }
        else
        {
            rankStatusText.text = "Kept The Rank";
            rankShow[0].rankRect.gameObject.SetActive(true);
            rankShow[0].leaderboardShow.SetUp(data, 1, true);
            TweenPanel(true, gameOverButtonPanelTween, 0.5f);
            yield return new WaitForSeconds(2f);
            rankStatusText.gameObject.SetActive(false);
        }
        
    }

    private void SetUpRankData()
    {
        rankShowObj.gameObject.SetActive(true);
        PlayerGlobalData dta = gameDataManager.GetPlayerGlobalData();
        if (dta != null && !string.IsNullOrEmpty(dta.Name))
        {
            PlayerGlobalData dt = new PlayerGlobalData()
            {
                Rank = dta.Rank,
                Name = dta.Name,
                scoreValue = dta.scoreValue
            };
            rankDataCurrent = dt;
            rankStatusText.gameObject.SetActive(false);
            rankShow[0].rankRect.gameObject.SetActive(true);
            rankShow[0].leaderboardShow.SetUp(rankDataCurrent, 1, true);
            rankShow[0].leaderboardShow.UIElementVisibility(1, 0.5f);
        }
        else
        {
            DisplayDefaultRank();
        }
    }
    private void DisplayDefaultRank()
    {
        rankStatusText.gameObject.SetActive(true);
        rankStatusText.text = "Data Not Available";
        PlayerGlobalData data = new PlayerGlobalData()
        {
            Rank = 999,
            Name = "......",
            scoreValue = 0000
        };
        rankShow[0].rankRect.gameObject.SetActive(true);
        rankShow[0].leaderboardShow.SetUp(data, 1, true);
        rankShow[0].leaderboardShow.UIElementVisibility(1, 0.5f);
        TweenPanel(true, gameOverButtonPanelTween, 0.5f);
    }

    private void Resetgift()
    {
        foreach (var item in giftContent)
        {
            item.holder.SetActive(false);
            item.deniedImage.SetActive(false);
            item.valueText.color = Color.green;
        }
        pointEffect.SetActive(false);
        giftShowObj.gameObject.SetActive(false);
        rankShowObj.gameObject.SetActive(false);
        rankStatusText.gameObject.SetActive(false);
        for (int i = 0; i < rankShow.Length; i++)
        {
            int posy = i * 170;
            rankShow[i].rankRect.DOAnchorPosY((float)posy, 0f);
            rankShow[i].rankRect.gameObject.SetActive(false);
            rankShow[i].leaderboardShow.UIElementVisibility(0, 0);
        }

    }

    public void GiftAbilityPanel(List<GiftData> data)
    {
        giftShowObj.gameObject.SetActive(true);
        int count = data.Count;
        giftData = new GiftData[count];
        for (int i = 0; i < count; i++)
        {
            GiftData dt = new GiftData()
            {
                indexCode = data[i].indexCode,
                values = data[i].values,
            };

            if (i < giftContent.Length) {
                giftContent[i].deniedImage.SetActive(false);

                if (i > 0)
                {
                    int index = (int)data[i].indexCode - 1;
                    if (index >= 0 && index < abilityManager.abilityData.Length)
                    {
                        AbilityData abData = abilityManager.abilityData[index];
                        giftContent[i].image.sprite = abData.iconSprite;
                        giftContent[i].valueText.text = "+" + data[i].values.ToString();
                        giftContent[i].nameText.text = abData.name;
                        giftContent[i].holder.SetActive(true);
                    }
                }
                else
                {
                    giftContent[i].holder.SetActive(true);
                    giftContent[i].valueText.text = "+" + data[i].values.ToString();
                }
            }
            giftData[i] = dt;
        }
        pointEffect.SetActive(true);
        GiftPanelOK();
    }

    private void LoseGiftSetUp()
    {
        giftShowObj.gameObject.SetActive(true);
        for (int i = 0; i < giftContent.Length; i++)
        {
            giftContent[i].deniedImage.SetActive(true);
            giftContent[i].holder.SetActive(true);
            giftContent[i].valueText.color = Color.red;
            if (i == 0)
            {
                giftContent[i].valueText.text = "+50";
            }
            else if(i == 1)
            {
                AbilityData abData = abilityManager.abilityData[8];
                int rand = UnityEngine.Random.Range(1, 5);
                giftContent[i].image.sprite = abData.iconSprite;
                giftContent[i].valueText.text = "+" + rand.ToString();
                giftContent[i].nameText.text = abData.name;
            }
            else
            {
                int rand = UnityEngine.Random.Range(0, 4);
                AbilityData abData = abilityManager.abilityData[rand];
                giftContent[i].image.sprite = abData.iconSprite;
                giftContent[i].valueText.text = "+1";
                giftContent[i].nameText.text = abData.name;
            }
        }
    }
    
    private void GiftPanelOK()
    {
        gameDataManager.SetGiftData(giftData, giftData.Length);
    }

    public void ChangeBgPanelsSprite(int index)
    {
        if (bgImageSprite.Length > 0)
        {
            Sprite sp = bgImageSprite[0];
            if (bgImageSprite.Length > index)
            {
                sp = bgImageSprite[index];
            }
            upperPanel.sprite = sp;
            bgPanel.sprite = sp;
        }
    }

    //public Image freezeImage;
    //public GameObject bgFill;
    //public Image fillImage;

    public void SetUpFreeze()
    {
        freezeImage.fillAmount = 0;
        freezeImage.DOFade(0, 0);
        freezeImage.gameObject.SetActive(true);
        freezeImage.DOFillAmount(1, 1.5f);
        freezeImage.DOFade(1, 1.5f);

        fillImage.fillAmount = 1;
        bgFill.SetActive(true);
    }
    public void FreezeTimer(float percent)
    {
        fillImage.fillAmount = (1 - percent);
    }

    public void EndFreeze()
    {
        freezeImage.fillAmount = 0;
        freezeImage.DOFade(0, 0);
        freezeImage.gameObject.SetActive(false);
        fillImage.fillAmount = 1;
        bgFill.SetActive(false);
    }

    public void SetUpSpObjectPanel(Vector2 pos, Sprite icon, string detail)
    {
        DisableSpTrans();
        spIcon.sprite = icon;
        detailText.text = detail;
        spTrans.position = pos;
        spTrans.gameObject.SetActive(true);
        Invoke(nameof(DisableSpTrans), 3f);
    }

    private void DisableSpTrans()
    {
        spTrans.gameObject.SetActive(false);
    }

    private Tween refreshFillTween;
    public void ActivateDisableRefresh(float time)
    {
        disableRefreshButtonFill.fillAmount = 1;
        StopCoroutine(DisableRefreshCo(time));
        if(refreshFillTween != null)
            refreshFillTween.Kill();
        refreshFillTween = disableRefreshButtonFill.DOFillAmount(0, time);
        StartCoroutine(DisableRefreshCo(time));
    }
    IEnumerator DisableRefreshCo(float time)
    {
        disableRefreshButton.SetActive(true);
        refreshButton.enabled = false;
        yield return new WaitForSeconds(time);
        DisableRefresh();
    }

    private void DisableRefresh()
    {
        disableRefreshButton.SetActive(false);
        refreshButton.enabled = true;
    }

    private Tween rotateFillTween;
    public void ActivateDisableRotate(float time)
    {
        disableRotateButtonFill.fillAmount = 1;
        StopCoroutine(DisableRotateCo(time));
        if (rotateFillTween != null)
            rotateFillTween.Kill();
        rotateFillTween = disableRotateButtonFill.DOFillAmount(0, time);
        StartCoroutine(DisableRotateCo(time));
    }
    IEnumerator DisableRotateCo(float time)
    {
        disableRotateButton.SetActive(true);
        rotateButton.enabled = false;
        yield return new WaitForSeconds(time);
        DisableRotate();
    }

    private void DisableRotate()
    {
        disableRotateButton.SetActive(false);
        rotateButton.enabled = true;
    }

    private void OnDisable()
    {
        PlayerRankdataUpdate.AddListener(PlayerRankUpdate);
    }
}
