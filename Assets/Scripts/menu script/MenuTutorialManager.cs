using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;


[Serializable]
public enum VideoNameCode
{
    Row,
    Column,
    Row_Column,
    Area,
    Color,
    Chain
}


public class MenuTutorialManager : MonoBehaviour
{
    public static MenuTutorialManager Instance;


    [SerializeField] private Button startVideoButton;
    [SerializeField] private Button crossVideoButton;
    [SerializeField] private PanelTween videoShowPanel;
    [SerializeField] private Image videoShowBg;
    [SerializeField] private Button nextButton;
    [SerializeField] private Text nextButtontext;
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private Text videoText;
    private string videoFileName = "Row.mp4";
    private bool isVideoShow;
    private int videoIndex;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button crossButton;
    [SerializeField] private PanelTween settingShowPanel;
    [SerializeField] private Image settingShowBg;
    [SerializeField] private AudioSource clickaudioSource;
    private bool isSettingShow;


    [SerializeField] private PanelTween playerNameShowPanel;
    [SerializeField] private InputField nameInputField;
    [SerializeField] private Button okButton;
    private string playerName;

    private bool isplayerNameShow;


    [SerializeField] private InputField nameInput;
    [SerializeField] private Button changeButton;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text totalStarText;
    [SerializeField] private Text maxHitText;

    



    [SerializeField] private Button audioButton;
    private Image audioButtonImage;
    [SerializeField] private Button sfxButton;
    private Image sfxButtonImage;
    [SerializeField] private Sprite[] soundSprite;
    private int audioStatusInt;
    private int sfxStatusInt;
    private int voiceStatusInt;

    public static readonly string firstplayInt = "First_Play_Int";
    public static readonly string FirstPlayLevel = "First_Play_Level";
    private bool isShowed;
    private MenuManager menuManager;
    private BlockManager blockManager;
    private GameDataManager gameDataManager;
    [SerializeField] private LevelDataInfo tuteLevelData;
    private bool isFirstPlayLevel;

    [SerializeField] private GameObject levelTuteCanvas;
    [SerializeField] private Image leveltuteBg;
    [SerializeField] private GameObject tuteImageObj;

    [SerializeField] private Image detailShowBG;
    [SerializeField] private InfoItems[] itemShow;
    private List<RectTransform> showRects = new List<RectTransform>();
    private RectTransform currentDetailsShow;

    private int currentRectIndex = -1;
    private float initialValue = 1200f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        //PlayerPrefs.SetInt(GameDataManager.TutorialPref, 0);
        if(blockManager  == null)
            blockManager = BlockManager.Instance;
        if(menuManager == null)
            menuManager = MenuManager.Instance;
        if(gameDataManager == null)
            gameDataManager = blockManager.gameDataManager;

        audioStatusInt = gameDataManager.GetSaveValues(7);
        sfxStatusInt = gameDataManager.GetSaveValues(11);
        voiceStatusInt = gameDataManager.GetSaveValues(12);
        audioButtonImage = audioButton.GetComponent<Image>();
        GameSountSetup();
        sfxButtonImage = sfxButton.GetComponent<Image>();
        GameSfxSetup();

        settingButton.onClick.AddListener(SettingPanel);
        crossButton.onClick.AddListener(SettingPanel);
        audioButton.onClick.AddListener(GameSound);
        sfxButton.onClick.AddListener(SfxSound);

        okButton.onClick.AddListener(NameSetOk);
        changeButton.onClick.AddListener(EditName);
        changeButton.interactable = false;

        startVideoButton.onClick.AddListener(TotorialVideoPanel);
        crossVideoButton.onClick.AddListener(TotorialVideoPanel);
        nextButton.onClick.AddListener(NextVideo);

        int value = PlayerPrefs.GetInt(FirstPlayLevel);
        if (value == 0 && gameDataManager.currentLevel >= 4)
        {
            isFirstPlayLevel = true;
        }

        if (showRects.Count == 0)
        {
            for (int i = 0; i < itemShow.Length; i++)
            {
                showRects.Add(itemShow[i].GetComponent<RectTransform>());
            }
        }
        levelTuteCanvas.SetActive(false);
        if(gameDataManager.isMenuOpened)
            menuManager.Loading();
    }

    public void GameStart()
    {
        if (blockManager == null)
            blockManager = BlockManager.Instance;
        if (menuManager == null)
            menuManager = MenuManager.Instance;
        if (gameDataManager == null)
            gameDataManager = blockManager.gameDataManager;

        AdsLeaderboardManager.Instance.CheckAnalyticsEvent(1);
        if (showRects.Count == 0)
        {
            for (int i = 0; i < itemShow.Length; i++)
            {
                showRects.Add(itemShow[i].GetComponent<RectTransform>());
            }
        }
        int value = PlayerPrefs.GetInt(FirstPlayLevel);
        if (value == 0 && gameDataManager.currentLevel >= 4)
        {
            isFirstPlayLevel = true;
        }


        int num = PlayerPrefs.GetInt(gameDataManager.TutorialPref);
        if (num == 0)
        {
            menuManager.GameInitialize(2f, true);
            ChangeScene();
            menuManager.isTutorialActive = true;
            gameDataManager.isplayerNameSelect = true;
            AdsLeaderboardManager.Instance.CheckAnalyticsEvent(2);
        }
        else
        {
            menuManager.isTutorialActive = false;
            Inisialize();
            SetUp();
            menuManager.SetUpManager(gameDataManager.isMenuOpened);
            DailyGoalsManager.Instance.Initialize();
        }
    }
    private bool CoinCheck()
    {
        bool coins = false;
        int coin = gameDataManager.GetSaveValues(0);
        if (coin >= 5000)
        {
            coins = true;
        }
        return coins;
    }
    private void EditName()
    {
        if (CheckSettingProfileName() && CoinCheck())
        {
            changeButton.interactable = false;
            gameDataManager.CoinValueChange(1000, false);
            ChangePlayerName(nameInput.text);
        }
    }

    private bool CheckSettingProfileName()
    {
        bool status = false;
        string name = nameInput.text;
        string currentNAme = gameDataManager.GetStringData(14);
        if(!string.Equals(name, currentNAme))
        {
            status = true;
        }
        return status;
    }

    public void SettingInputFieldCheck()
    {
        bool interactable;
        string name = nameInput.text;
        if (!string.IsNullOrEmpty(name) && name.Length >= 3 && CheckSettingProfileName() && CoinCheck())
        {
            interactable = true;
        }
        else
        {
            interactable = false;
        }
        changeButton.interactable = interactable;
    }
    private void NameSetOk()
    {
        gameDataManager.isplayerNameSelect = false;
        SetPlayerName();
        menuManager.MenuShow();
        ChangePlayerName(playerName);
    }

    private void ChangePlayerName(string name)
    {
        if (!string.IsNullOrEmpty(name) && name.Length >= 3)
        {
            gameDataManager.SetStringData(14, name);
            LeaderBoardManager.Instance.SetNewName(name);
        }
    }

    public void InputFieldCheck()
    {
        bool interactable = false;
        playerName = nameInputField.text;
        if (!string.IsNullOrEmpty(playerName) && playerName.Length >= 3)
        {
            interactable = true;
        }
        okButton.interactable = interactable;
    }

    public void SetPlayerName()
    {
        float fade = 0.8f;
        clickaudioSource.Play();
        if (isplayerNameShow)
        {
            playerNameShowPanel.panel.DOAnchorPosY(playerNameShowPanel.posInitialFloat, 0.3f);
            fade = 0f;
        }
        else
        {
            playerNameShowPanel.panel.DOAnchorPosY(playerNameShowPanel.posFinalFloat, 0.3f);
            settingShowBg.gameObject.SetActive(true);
            

        }
        settingShowBg.DOFade(fade, 0.3f).OnComplete(() =>
        {
            isplayerNameShow = !isplayerNameShow;
            settingShowBg.gameObject.SetActive(isplayerNameShow);
        });
    }

    private void ChangeScene()
    {
        gameDataManager.levelData = tuteLevelData.levelData;
        gameDataManager.currentLevel = tuteLevelData.levelData.levelNumber;
        gameDataManager.GameTypeCode = 1;
        gameDataManager.isMenuOpened = true;
        Invoke(nameof(GameLoad), 0.4f);
    }

    private void GameLoad()
    {
        SceneManager.LoadScene(1);
    }

    public bool CheckFirstPlayLevel()
    {
        if (isFirstPlayLevel)
        {
            leveltuteBg.DOFade(0, 0);
            levelTuteCanvas.SetActive(true);
            tuteImageObj.gameObject.SetActive(false);
            return true;

        }
        else
        {
            return false;
        }

    }

    public void BlockTutActive(Vector3 pos)
    {
        if (isFirstPlayLevel)
        {
            tuteImageObj.gameObject.SetActive(true);
            tuteImageObj.transform.DOMove(pos, 1f);
            BlockLevelTutorial();
        }
    }
    private void BlockLevelTutorial()
    {
        leveltuteBg.DOFade(0.8f, 1);
    }

    public void LevelTuteCross()
    {
        if (isFirstPlayLevel)
        {
            levelTuteCanvas.SetActive(false);
            PlayerPrefs.SetInt(FirstPlayLevel, 10);
            isFirstPlayLevel = false;
        }
    }

    private void Inisialize()
    {
        for (int i = 0; i < showRects.Count; i++)
        {
            showRects[i].DOAnchorPosX(initialValue, 0f);
        }
    }
    private void SetUp()
    {
        detailShowBG.DOFade(0.7f, 0.3f);
        StartShow();
    }

    private void DisableUp()
    {
        detailShowBG.DOFade(0f, 0.3f);
        Inisialize();

    }
    public void CloseDetails()
    {
        if (currentDetailsShow)
            currentDetailsShow.DOAnchorPosX(-initialValue, 0.5f).SetEase(Ease.OutQuad);
        Invoke(nameof(DisableUp), 0.5f);
    }


    private void ShowAbilityDetail()
    {
        int ran = UnityEngine.Random.Range(0, 2);
        int max = menuManager.abilityData.Length;
        bool isAbility = true;
        if (ran == 1)
        {
            max = MenuManager.tileInfoShowIndex;
            if (max < 0)
            {
                max = blockManager.tileDetailsData.Length;
            }
            isAbility = false;
        }
        if (currentDetailsShow)
            currentDetailsShow.DOAnchorPosX(initialValue, 0f).SetEase(Ease.OutQuad);
        int maxitter = 20;
        int random = UnityEngine.Random.Range(0, max);
        while (true)
        {
            maxitter--;
            if (random == 0 && !isAbility)
            {
                random = UnityEngine.Random.Range(0, max);
            }
            else
            {
                break;
            }
        }

        Sprite icon = blockManager.tileDetailsData[random].iconSprite;
        string name = blockManager.tileDetailsData[random].name;
        string des = blockManager.tileDetailsData[random].working;
        itemShow[0].SetUpItemDataForStartUp(icon, 0, name, des, random, false, false);
        

        currentDetailsShow = showRects[0];
        if (isAbility)
        {
            currentDetailsShow = showRects[1];
            for (int i = 0; i < 5; i++)
            {
                if(menuManager.abilityData[random].thisType == VariableTypeCode.None || menuManager.abilityData[random].thisType == VariableTypeCode.Ability_1)
                {
                    random = UnityEngine.Random.Range(0, max);
                }
                else
                {
                    break;
                }
            }
            icon = menuManager.abilityData[random].iconSprite;
            name = menuManager.abilityData[random].name;
            des = menuManager.abilityData[random].working;
            itemShow[1].SetUpItemDataForStartUp(icon, 0, name, des, random, false, false);
        }

        if (currentDetailsShow)
        {
            currentDetailsShow.DOAnchorPosX(0, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                currentRectIndex = random;
            });
        }
    }

    private void StartShow()
    {
        ShowAbilityDetail();
    }

    private void SettingPanel()
    {
        float fade = 0.8f;
        clickaudioSource.Play();
        if (isSettingShow)
        {
            settingShowPanel.panel.DOAnchorPosY(settingShowPanel.posInitialFloat, 0.3f);
            fade = 0f;
        }
        else
        {
            settingShowPanel.panel.DOAnchorPosY(settingShowPanel.posFinalFloat, 0.3f);
            SetUpProfile();
            settingShowBg.gameObject.SetActive(true);
            changeButton.interactable = false;
        }
        menuManager.MenuPanelSetup(isSettingShow, 0.3f);
        settingShowBg.DOFade(fade, 0.3f).OnComplete(() =>
        {
            isSettingShow = !isSettingShow;
            settingShowBg.gameObject.SetActive(isSettingShow);
        });

    }

    private void SetUpProfile()
    {
        string name = gameDataManager.GetStringData(14);
        string score = gameDataManager.GetSaveValues(2).ToString();
        string totalStar = gameDataManager.GetSaveValues(13).ToString();
        string maxHit = gameDataManager.GetSaveValues(6).ToString();
        nameInput.text = name;
        scoreText.text = score;
        totalStarText.text = totalStar;
        maxHitText.text = maxHit;
    }

    public void AdsGameSountSetup(bool isAds)
    {
        int value = 0;
        if (isAds || audioStatusInt == 1)
        {
            value = -70;
        }
        audioMixer.SetFloat("Master", value);
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

    private void SoundSprite(Image image, int statuscode)
    {
        image.sprite = soundSprite[statuscode];
    }

    private void TotorialVideoPanel()
    {
        videoIndex = 0;
        float fade = 0.5f;
        clickaudioSource.Play();
        if (isVideoShow)
        {
            videoShowPanel.panel.DOAnchorPosY(videoShowPanel.posInitialFloat, 0.3f);
            fade = 0f;
        }
        else
        {
            videoShowPanel.panel.DOAnchorPosY(videoShowPanel.posFinalFloat, 0.3f);
            videoShowBg.gameObject.SetActive(true);

        }
        SettingPanelDisable(isVideoShow);
        videoShowBg.DOFade(fade, 0.3f).OnComplete(() =>
        { 
            isVideoShow = !isVideoShow;
            videoShowBg.gameObject.SetActive(isVideoShow);
            if (isVideoShow)
            {
                nextButtontext.text = "Next";
                SetUpVideo(videoIndex);
            }
        });

    }
    private void SettingPanelDisable(bool status)
    {
        float posy = settingShowPanel.posInitialFloat;
        if (status)
        {
            posy = settingShowPanel.posFinalFloat;
        }
        settingShowPanel.panel.DOAnchorPosY(posy, 0.3f);
    }
    private void NextVideo()
    {
        if(videoIndex < 5)
        {
            videoIndex++;
            if(videoIndex == 5)
                nextButtontext.text = "Close";

            SetUpVideo(videoIndex);
        }
        else
        {
            TotorialVideoPanel();
        }
    }

    private void SetUpVideo(int index)
    {
        if(index < 6)
        {
            string Name = ((VideoNameCode)index).ToString();
            videoFileName = Name + ".mp4";
            videoText.text = Name + " " + "Blast";
            string videoPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }

}
