using System;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Advertisements;
public class AdsLeaderboardManager : MonoBehaviour,IUnityAdsLoadListener,IUnityAdsShowListener
{
    public static AdsLeaderboardManager Instance;

    public bool isTesting;
    public bool isAdsInitialized { get; set; }
    public bool IsAdReady { get; private set; }
    private bool isInterCalled = false;
    public bool IsRVReady { get; private set; }
    private bool isRVCalled = false;

    public bool IsRewardUser { get; private set; }
    public static Action<bool> ApplicationNetworkConnectivity = delegate { };

    public UnityEvent GratifyRewards = new();
    public UnityEvent AdsShowClose_Failed = new();
    public UnityEvent RewardAdsLoaded = new();


    
    private string android_Inter_AdUnitID = "Interstitial_Android";
    private string ios_Inter_AdUnitID = "Interstitial_iOS";
    private string android_Reward_AdUnitID = "Rewarded_Android";
    private string ios_Reward_AdUnitID = "Rewarded_iOS";




    
    private string current_Inter_AdUnitID;
    private string current_Reward_AdUnitID;
    private bool isOnline;



    private GameDataManager gameDataManager;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
#if UNITY_EDITOR
        isTesting = true;
        current_Reward_AdUnitID = android_Reward_AdUnitID;
        current_Inter_AdUnitID = android_Inter_AdUnitID;
#elif UNITY_ANDROID
        current_Reward_AdUnitID = android_Reward_AdUnitID;
        current_Inter_AdUnitID = android_Inter_AdUnitID;
#elif UNITY_IOS
        current_Reward_AdUnitID = ios_Reward_AdUnitID;
        current_Inter_AdUnitID = ios_Inter_AdUnitID;
#endif

    }
    private void Start()
    {
        gameDataManager = BlockManager.Instance.gameDataManager;
    }

    private void OnlineStatus(bool status)
    {
        if (status)
        {
            if (Advertisement.isInitialized)
            {
                CacheInterstitial();
                CacheRewarded();
            }
        }
    }


    #region LeaderBoard
    public void SubmitDataIntoLeaderboard(int newStar, int score, bool isHigh)
    {
        if (isOnline)
        {
            SubmitTotalScore(score);
            SubmitStarScore(newStar);
            SubmitHighestScore(score, isHigh);
        }
    }
    /// <summary>
    /// Submits a score to the UGS leaderboard.
    /// </summary>
    public async void SubmitTotalScore(int score)
    {
        try
        {
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(InitializeUnityServices.totalScoreBoardID, score);
            Debug.Log($"Score {score} submitted to leaderboard {InitializeUnityServices.totalScoreBoardID}.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("SubmitScore failed: " + e.Message);
        }
    }

    /// <summary>
    /// Submits a highscore to the UGS leaderboard.
    /// </summary>
    public async void SubmitHighestScore(int score, bool isHigh)
    {
        try
        {
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(InitializeUnityServices.highestScoreBoardID, score);
            Debug.Log($"Highest {score} submitted to leaderboard {InitializeUnityServices.highestScoreBoardID}.");
            if(isHigh)
                gameDataManager.SetSaveValues(16, score);
        }
        catch (System.Exception e)
        {
            Debug.LogError("SubmitScore failed: " + e.Message);
        }
    }

    /// <summary>
    /// Submits a star to the UGS leaderboard.
    /// </summary>
    public async void SubmitStarScore(int star)
    {
        try
        {
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(InitializeUnityServices.totalStarBoardID, star);
            Debug.Log($"Star {star} submitted to leaderboard {InitializeUnityServices.totalStarBoardID}.");
            int num = gameDataManager.GetSaveValues(17) + star;
            gameDataManager.SetSaveValues(17, num);
        }
        catch (System.Exception e)
        {
            Debug.LogError("SubmitScore failed: " + e.Message);
        }
    }
    #endregion

    public void AdsInisializationComplete()
    {
        print("Ads Initialized");
        isAdsInitialized = true;
        MenuManager.Instance.LoadingComplete();
        CacheInterstitial();
        CacheRewarded();
    }


    internal void CacheInterstitial()
    {
        if (!IsAdReady && !isInterCalled)
        {
            isInterCalled = true;
            Advertisement.Load(current_Inter_AdUnitID, this);
        }
    }
    internal void CacheRewarded()
    {
        if (!IsRVReady && !isRVCalled)
        {
            isRVCalled = true;
            Advertisement.Load(current_Reward_AdUnitID, this);
        }
    }


    void OnAdPrepared(string adSpotKey, bool status)
    {
        if (string.Equals(adSpotKey, current_Inter_AdUnitID))
        {
            IsAdReady = status;
            isInterCalled = false;
            Debug.Log("Ads: onAdPrepared Inter " + IsAdReady);
        }
        else if (string.Equals(adSpotKey, current_Reward_AdUnitID))
        {
            IsRVReady = status;
            isRVCalled = false;
            Debug.Log("Ads: onAdPrepared RewardedVideo " + IsRVReady);
            RewardAdsLoaded?.Invoke();
        }
        else { }
    }


    internal void ShowInterstitial()
    {
        if (IsAdReady && !isInterCalled)
        {
            isInterCalled = true;
            Advertisement.Show(current_Inter_AdUnitID, this);
        }
    }
    internal void ShowRewarded()
    {
        if (IsRVReady && !isRVCalled)
        {
            isRVCalled = true;
            Advertisement.Show(current_Reward_AdUnitID, this);
        }
    }

    void OnAdClosed(string placementId, bool pIsVideoCompleted)
    {
        AdsShowClose_Failed?.Invoke();
        if (string.Equals(placementId, current_Inter_AdUnitID))
        {
            IsAdReady = false;
            isInterCalled = false;
            Debug.Log("ADs: onAdClosed MidRoll " + IsAdReady);
        }
        else if (string.Equals(placementId, current_Reward_AdUnitID))
        {
            IsRVReady = false;
            isRVCalled = false;
            Debug.Log("ADs: onAdClosed RewardedVideo " + IsRVReady);

            if (pIsVideoCompleted)
            {
                GratifyRewards?.Invoke();
            }
        }
        else { }
    }

    private void Update()
    {
        CheckForOnline();
    }

    private void CheckForOnline()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable && isOnline)
        {
            isOnline = false;
            ApplicationNetworkConnectivity.Invoke(isOnline);
        }
        else if (Application.internetReachability != NetworkReachability.NotReachable && !isOnline)
        {
            isOnline = true;
            OnlineStatus(true);
            ApplicationNetworkConnectivity.Invoke(isOnline);
        }
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        OnAdPrepared(placementId, true);
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        OnAdPrepared(placementId, false);
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        OnAdClosed(placementId, false);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //throw new NotImplementedException();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //throw new NotImplementedException();
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (string.Equals(placementId, current_Reward_AdUnitID))
        {
            if (showCompletionState == UnityAdsShowCompletionState.COMPLETED)
                OnAdClosed(placementId, true);
        }
        else
        {
            OnAdClosed(placementId, false);
        }
    }
}
