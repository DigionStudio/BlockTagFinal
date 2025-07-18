using UnityEngine;

public class GameAdsManager : MonoBehaviour
{
    public static GameAdsManager Instance;
    [SerializeField] private UiManager uiManager;
    private bool isAdsDisable;
    private int code;
    private GameDataManager gameDataManager;
    private AdsLeaderboardManager adsLeaderboardManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

    }

    private void Start()
    {
        gameDataManager = BoardManager.Instance.gameDataManager;
        adsLeaderboardManager = AdsLeaderboardManager.Instance;
        adsLeaderboardManager.CacheInterstitial();
        isAdsDisable = gameDataManager.HasDisableAds;
        adsLeaderboardManager.AdsShowClose_Failed.AddListener(CodeCheck);
        adsLeaderboardManager.GratifyRewards.AddListener(Reward);
    }
    public bool CkeckInterAds()
    {
        bool isAdsReady = adsLeaderboardManager.IsAdReady;
        return isAdsReady;
    }

    public bool CheckRewardAds()
    {
        bool isAdsReady = adsLeaderboardManager.IsRVReady;
        return isAdsReady;
    }
    public void Show_Inter_Ads(int id)
    {
        code = id;
        if (!isAdsDisable)
        {
            if (adsLeaderboardManager.IsAdReady)
            {
                uiManager.AdsGameSountSetup(true);
                adsLeaderboardManager.ShowInterstitial();
            }
            else
            {
                CodeCheck();
            }
        }
        else
        {
            CodeCheck();
        }
    }

    public void Show_Reward_Ads()
    {
        if (!isAdsDisable)
        {
            if (adsLeaderboardManager.IsRVReady)
            {
                code = 0;
                uiManager.AdsGameSountSetup(true);
                adsLeaderboardManager.ShowRewarded();
            }
        }
        else
        {
            Reward();
        }
    }

    private void Reward()
    {
        uiManager.AdsGameSountSetup(false);
        BuyPanel.Instance.ActiveLife(true);
        if(gameDataManager.GameTypeCode == 1 && CheckForRewardAds())
            adsLeaderboardManager.CacheRewarded();
    }

    private bool CheckForRewardAds()
    {
        bool status = false;
        bool ads1 = gameDataManager.CheckForFreeClaim(0, false);
        bool ads2 = gameDataManager.CheckForFreeClaim(1, false);
        if(ads1 || ads2)
        {
            status = true;
        }
        return status;
    }


    private void CodeCheck()
    {
        uiManager.AdsGameSountSetup(false);
        if (code > 0)
        {
            if (code == 1)
            {
                uiManager.SetUpAfterAds();
            }
            else if (code == 4)
            {
                uiManager.Quit();
            }
            code = 0;
        }
    }
    private void OnDisable()
    {
        adsLeaderboardManager.AdsShowClose_Failed.RemoveListener(CodeCheck);
        adsLeaderboardManager.GratifyRewards.RemoveListener(Reward);
    }
}
