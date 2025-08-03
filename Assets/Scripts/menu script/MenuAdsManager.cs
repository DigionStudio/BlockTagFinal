using DG.Tweening;
using UnityEngine;

public class MenuAdsManager : MonoBehaviour
{
    [SerializeField] private RectTransform holder;
    private bool isOnGoing;
    public bool isRewardShowing;
    private int rewardCode;
    private MenuManager menuManager;
    private AdsLeaderboardManager adsLeaderboardManager;
    public bool isOnline { get; private set; }


    private void OnEnable()
    {
        AdsLeaderboardManager.ApplicationNetworkConnectivity += OnlineStatus;
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            isOnline = true;
        }

    }

    private void Start()
    {
        holder.DOAnchorPosY(0, 0);
        menuManager = MenuManager.Instance;
        adsLeaderboardManager = AdsLeaderboardManager.Instance;
        adsLeaderboardManager.GratifyRewards.AddListener(RewardPlayer);
        adsLeaderboardManager.RewardAdsLoaded.AddListener(CheckRewardButtons);
    }

    public void SetUpOffLine()
    {
        if (!isOnGoing)
        {
            isOnGoing = true;
            holder.DOScale(0, 0);
            holder.gameObject.SetActive(true);
            holder.DOScale(1, 1);
            holder.DOAnchorPosY(180, 1);
            Invoke(nameof(DisableOffline), 1f);
        }
    }

    private void DisableOffline()
    {
        isOnGoing = false;
        holder.DOScale(0, 1);
        holder.DOAnchorPosY(0, 1).OnComplete(() =>
        {
            if (!isOnGoing)
                holder.gameObject.SetActive(false);

        });
    }

    public void LoadRewardedAds()
    {
        if(adsLeaderboardManager == null)
            adsLeaderboardManager = AdsLeaderboardManager.Instance;
        bool isRVReady = adsLeaderboardManager.HasRVReady();
        if (!isRVReady)
        {
            adsLeaderboardManager.CacheRewarded();
        }
    }


    public bool Load_Reward_Ads()
    {
        bool isRVReady = adsLeaderboardManager.HasRVReady();
        return isRVReady;
    }

    public void Show_Reward_Ads(int id)
    {
        MenuTutorialManager.Instance.AdsGameSountSetup(true);
        rewardCode = id;
        isRewardShowing = true;
        adsLeaderboardManager.ShowRewarded();
    }


    private void RewardPlayer()
    {
        isRewardShowing = false;
        LoadRewardedAds();
        if (rewardCode > 0 && rewardCode <= 2)
        {
            menuManager.ClaimAdsReward(rewardCode);
            rewardCode = 0;
        }
        MenuTutorialManager.Instance.AdsGameSountSetup(false);
    }
    private void CheckRewardButtons()
    {
        if(rewardCode != 3)
            menuManager.CheckClaimButtons();
    }

    private void OnlineStatus(bool status)
    {
        isOnline = status;
    }



    private void OnDisable()
    {
        adsLeaderboardManager.GratifyRewards.RemoveListener(RewardPlayer);
        adsLeaderboardManager.RewardAdsLoaded.RemoveListener(CheckRewardButtons);
        AdsLeaderboardManager.ApplicationNetworkConnectivity -= OnlineStatus;

    }


}
