using System;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Advertisements;
using Unity.Services.Analytics;
using System.Collections.Generic;

public class AdsLeaderboardManager : MonoBehaviour,IUnityAdsLoadListener,IUnityAdsShowListener
{
    public static AdsLeaderboardManager Instance;

    public bool isTesting;
    public bool isAdsInitialized { get; set; }
    private bool IsAdReady;
    private bool isInterCalled = false;
    private bool IsRVReady;
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
    public bool HasOnline { get { return isOnline; } }


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

    public void UpdateScore(int score)
    {
        if (isOnline)
            SubmitTotalScore(score);
    }
    public void SubmitDataIntoLeaderboard(int newStar, int score, bool isHigh)
    {
        if (isOnline)
        {
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


    public bool HasRVReady()
    {
        bool status = false;
        if(isOnline && IsRVReady)
        {
            status = true;
        }
        return status;

    }
    public bool HasInterReady()
    {
        bool status = false;
        if (isOnline && IsAdReady)
        {
            status = true;
        }
        return status;
    }
    internal void CacheInterstitial()
    {
        if (!IsAdReady && !isInterCalled && isOnline)
        {
            isInterCalled = true;
            Advertisement.Load(current_Inter_AdUnitID, this);
        }
    }
    internal void CacheRewarded()
    {
        if (!IsRVReady && !isRVCalled && isOnline)
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

    //public void LevelWinEvent(int level)
    //{
    //    AnalyticsResult analyticsResult = new AnalyticsResult(
    //         "LevelWin", level );
    //}


    public async void GetCurrentPlayerTopScoreData()
    {
        BlockManager.Instance.gameDataManager.SetPlayerGlobalData(999, "", 000);
        if (isOnline)
        {
            try
            {
                var scores = await LeaderboardsService.Instance.GetPlayerScoreAsync(InitializeUnityServices.totalScoreBoardID);
                BlockManager.Instance.gameDataManager.SetPlayerGlobalData(scores.Rank, scores.PlayerName, scores.Score);
            }
            catch (System.Exception e)
            {
                Debug.LogError("GetPlayerTopScores failed: " + e.Message);
            }
        }
        UiManager.PlayerRankdataUpdate.Invoke();
    }

    /// <summary>
    ///  1 = Game start, 2 = tutorial start, 3 = tutorial end, 4 = Game Over, 5 = Play level, 6 = restart level
    /// </summary>
    /// <param name="code"></param>
    /// <param name="level"></param>
    public void CheckAnalyticsEvent(int code, int level = -1)
    {
        string eventName = "";
        bool restart = false;
        if(code == 1)
        {
            eventName = "Game_Start";
        }else if (code == 2)
        {
            eventName = "Totorial_Start";
        }
        else if (code == 3)
        {
            eventName = "Totorial_End";
        }
        else if (code == 4)
        {
            eventName = "Game_Over";
        }
        else if (code == 5)
        {
            eventName = "Play_Level";
        }
        else if (code == 6)
        {
            eventName = "Restart_Level";
            restart = true;
        }
        if (!string.IsNullOrEmpty(eventName) && isOnline)
        {
            UploadAnalytics(eventName, code, restart);
        }
    }

    public void UploadAnalytics(string eventName, int level, bool isRestart)
    {
        string paramName = "Level_Index";
        if (isRestart)
        {
            paramName = "Restart_Level_Index";
        }
        CustomEvent myEvent = new CustomEvent(eventName)
        {
            {paramName, level }
        };
        if(level >= 0)
        {
            AnalyticsService.Instance.RecordEvent(myEvent);
        }
        else
        {
            AnalyticsService.Instance.RecordEvent(eventName);
        }
        AnalyticsService.Instance.Flush();
    }
}
