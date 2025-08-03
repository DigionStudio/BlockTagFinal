using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BuyPanel : MonoBehaviour
{

    public static BuyPanel Instance;
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform holderRect;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private Image headingImage;
    [SerializeField] private Sprite[] headingImageSprite;
    [SerializeField] private GameObject buyPanel;

    [SerializeField] private GameObject movePanel;
    [SerializeField] private Button crossButton;
    [SerializeField] private Button[] buyButtons = new Button[3];
    [SerializeField] private int[] moveBuyValue = new int[3];

    [SerializeField] private GameObject abilityPanel;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Text abilityCountText;
    [SerializeField] private Text abilityName;
    [SerializeField] private Text abilityworking;
    [SerializeField] private Text abilityPriceText;
    [SerializeField] private Button abilityBuyButton;
    private int abilityBuyValue;
    private VariableTypeCode currentType;
    private int abilityCount;

    private int lifeCode = -1;
    [SerializeField] private GameObject lifePanel;
    [SerializeField] private Text lifePanelDetail;
    private readonly string[] detailString = new string[3] { "Re-Setup all blocks using one Life. ", "Use One Life to Stay in the Game. ", "You Don't Have Life to Use. " };
    [SerializeField] private Button lifeCrossButton;
    [SerializeField] private Text lifesCount;
    [SerializeField] private Button useButton;

    [SerializeField] private GameObject adDetail;
    [SerializeField] private Button adButtton;


    [SerializeField] private Transform lifeEffectHolder;


    private UiManager uiManager;
    private GameDataManager gameDataManager;
    private GameAdsManager gameAdsManager;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    void Start()
    {
        gameDataManager = BoardManager.Instance.gameDataManager;
        gameAdsManager = GameAdsManager.Instance;
        //JioWrapperJS.Instance.RewardAdsLoaded.AddListener(CheckAdStatus);




        lifeEffectHolder.gameObject.SetActive(false);
        panel.SetActive(false);
        holderRect.DOAnchorPosY(1600, 0f);
        uiManager = FindObjectOfType<UiManager>();
        crossButton.onClick.AddListener(BuyCross);
        buyButtons[0].onClick.AddListener(BuyMove1);
        buyButtons[1].onClick.AddListener(BuyMove2);
        buyButtons[2].onClick.AddListener(BuyMove3);


        abilityBuyButton.onClick.AddListener(BuyAbility);



        useButton.onClick.AddListener(UseLife);
        lifeCrossButton.onClick.AddListener(LefeCross);
        adButtton.onClick.AddListener(ShowAds);
    }


    public void SetUpMoveBuyPanel(bool isActive)
    {
        clickSound.Play();
        MovePanelActive(isActive);
    }
    public void GameEndMoveBuy(bool isActive)
    {
        int coin = gameDataManager.GetSaveValues(0);
        if (CheckBuyButton(coin, moveBuyValue[0]))
        {
            MovePanelActive(isActive);
        }
    }
    private void MovePanelActive(bool isActive)
    {
        headingImage.sprite = headingImageSprite[0];
        lifePanel.SetActive(false);
        movePanel.SetActive(true);
        abilityPanel.SetActive(false);
        CheckMoveButtons();
        BuyCoinPanel(isActive);
    }

    private void CheckMoveButtons()
    {
        int coin = gameDataManager.GetSaveValues(0);
        for (int i = 0; i < buyButtons.Length; i++)
        {
            buyButtons[i].interactable = CheckBuyButton(coin, moveBuyValue[i]);
        }
    }

    public void SetUpAbilityBuyPanel(bool isActive, AbilityData abilityData, VariableTypeCode type)
    {
        headingImage.sprite = headingImageSprite[0];
        clickSound.Play();
        currentType = type;
        lifePanel.SetActive(false);
        movePanel.SetActive(false);
        abilityPanel.SetActive(true);
        abilityCount = abilityData.count;
        Sprite icon = abilityData.iconSprite;
        string name = abilityData.name;
        string working = abilityData.working;
        abilityBuyValue = abilityData.value;
        CheckAbilityBuyButton(abilityBuyValue);

        abilityIcon.sprite = icon;
        abilityCountText.text = "+"+ abilityCount.ToString();
        abilityName.text = name;
        abilityworking.text = "(" + working + ")";
        abilityPriceText.text = abilityBuyValue.ToString();


        BuyCoinPanel(isActive);
    }
    private void CheckAbilityBuyButton(int value)
    {
        int coin = gameDataManager.GetSaveValues(0);
        abilityBuyButton.interactable = CheckBuyButton(coin, value);
    }

    private void BuyCoinPanel(bool isActive)
    {
        BoardManager.Instance.GameStatus(!isActive);
        DisableBuyPanel(isActive);
    }
    public void DisableBuyPanel(bool isActive)
    {
        float posy = 1600f;
        if (isActive)
        {
            panel.SetActive(true);
            buyPanel.SetActive(true);

            posy = 200f;
        }
        holderRect.DOAnchorPosY(posy, 0.3f).OnComplete(() =>
        {
            panel.SetActive(isActive);
            buyPanel.SetActive(isActive);


        });
    }


    public void SetUpLifePanel(bool isActive, int code)
    {
        lifeCode = code;
        buyPanel.SetActive(false);
        if (lifeCode >= 0)
        {
            headingImage.sprite = headingImageSprite[lifeCode];

            int lifecount = gameDataManager.GetSaveValues(1);
            if (lifecount <= 0)
            {
                bool status = CheckAdsShowButtonStatus(lifeCode);
                adButtton.interactable = status;
                ButtonCheck(true);
                lifePanelDetail.text = detailString[2];
            }
            else
            {
                ButtonCheck(false);
                lifePanelDetail.text = detailString[lifeCode];
            }
            lifesCount.text = "x" + lifecount.ToString();
        }

        float posy = 1600f;
        if (isActive)
        {
            panel.SetActive(true);
            lifePanel.SetActive(true);

            posy = 300f;
        }
        holderRect.DOAnchorPosY(posy, 0.3f).OnComplete(() =>
        {
            panel.SetActive(isActive);
            lifePanel.SetActive(isActive);
        });
    }

    private void ButtonCheck(bool isAds)
    {
        useButton.gameObject.SetActive(!isAds);
        adButtton.gameObject.SetActive(isAds);
        adDetail.gameObject.SetActive(isAds);
    }


    private void BuyCross()
    {
        clickSound.Play();
        BuyCoinPanel(false);
    }


    private void ShowAds()
    {
        adButtton.interactable = false;
        if(lifeCode >= 0)
            gameDataManager.GameAdaClaim(lifeCode);
        gameAdsManager.Show_Reward_Ads();
    }

    private bool CheckAdsShowButtonStatus(int code)
    {
        bool status = false;
        bool isAds = gameAdsManager.CheckRewardAds();
        bool adsClaim = false;
        if (code >= 0)
            adsClaim = gameDataManager.CheckForFreeClaim(code, false);
        if(isAds &&  adsClaim)
        {
            status = true;
        }
        return status;
    }

    public bool CheckForGameOverBuyPanel()
    {
        bool status = false;
        bool isAds = CheckAdsShowButtonStatus(1);
        int lifecount = gameDataManager.GetSaveValues(1);
        if(lifecount > 0 || isAds)
        {
            status = true;
        }
        return status;
    }

    private void UseLife()
    {
        clickSound.Play();
        ActiveLife(false);
    }

    public void ActiveLife(bool isAds)
    {
        if (lifeCode == 1)
        {
            if(!isAds)
                Invoke(nameof(LifeUseEffectAnim), 0.3f);
            else
            {
                Invoke(nameof(AdsActive), 1f);
            }
        }
        else if (lifeCode == 0)
        {
            if (!isAds)
                FinishGameLifeUsed();
            uiManager.ResetUp(isAds);
        }
        SetUpLifePanel(false, -1);
    }

    private void AdsActive()
    {
        BoardManager.Instance.LifeActivate();

    }

    private void LefeCross()
    {
        clickSound.Play();
        if (lifeCode == 1)
            BoardManager.Instance.UserGameEnd();
        else if(lifeCode == 0)
        {
            BoardManager.Instance.GameStatus(true);
        }
        SetUpLifePanel(false, -1);
    }

    private void BuyMove1()
    {
        MoveTaken(5, moveBuyValue[0]);
    }

    private void BuyMove2()
    {
        MoveTaken(10, moveBuyValue[1]);

    }

    private void BuyMove3()
    {
        MoveTaken(15, moveBuyValue[2]);

    }

    private void MoveTaken(int num, int coin)
    {
        clickSound.Play();
        BoardManager.Instance.AddMoves(num, coin);
        CheckMoveButtons();
    }

    private bool CheckBuyButton(int coin, int buyvalue)
    {
        bool isActive = false;
        if (coin >= buyvalue)
        {
            isActive = true;
        }
        return isActive;
    }

    private void BuyAbility()
    {
        GameManager.OnCoinUpdate.Invoke(abilityBuyValue, false);
        AbilityManager.Instance.AbilityUpdate(currentType, true, abilityCount);
        BuyCross();

    }

    //public GameObject lifeEffectHolder;
    //public Image[] lifeImges;
    //public Vector2[] centerPos;

    private void LifeUseEffectAnim()
    {
        lifeEffectHolder.gameObject.SetActive(true);
        Invoke(nameof(DisableLifeUseEffect), 1.5f);
    }

    private void DisableLifeUseEffect()
    {
        lifeEffectHolder.gameObject.SetActive(false);
        gameDataManager.LifeValueChange(1, false);
        uiManager.GetLifeCount();
        BoardManager.Instance.LifeActivate();
    }

    private void FinishGameLifeUsed()
    {
        lifeEffectHolder.gameObject.SetActive(true);
        Invoke(nameof(DisableLifeUse), 1.5f);

    }
    private void DisableLifeUse()
    {
        lifeEffectHolder.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        //JioWrapperJS.Instance.RewardAdsLoaded.RemoveListener(CheckAdStatus);

    }
}
