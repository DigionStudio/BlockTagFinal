using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using System;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine.Advertisements;
using Unity.Services.Analytics;

public class InitializeUnityServices : MonoBehaviour, IUnityAdsInitializationListener
{
    public bool isInisialize = false;
    public static readonly string highestScoreBoardID = "HighestScoreLeaderboard";
    public static readonly string totalScoreBoardID = "TotalScoreLeaderboard";
    public static readonly string totalStarBoardID = "TotalStarLeaderboard";
    private readonly string FirstPlayPref = "FirstPlayPref";
    private bool[] UpdateStatus = new bool[7] {false, false, false,false, false,false,false};

    private LeaderboardEntry[] thisPlayerData = new LeaderboardEntry[3];
    private bool initializeActive = false;
    private bool isOnline = false;



    private string andoidGameID = "5718827";
    private string iosGameID = "5718826";
    private string gameID;
    private bool adsInitializing;
    private bool isGameStart;


    private GameDataManager gameDataManager;
    private LeaderBoardManager leaderBoardManager;
    private AdsLeaderboardManager adsLeaderboardManager;
    private void OnEnable()
    {
        AdsLeaderboardManager.ApplicationNetworkConnectivity += OnlineStatus;
#if UNITY_EDITOR
        gameID = andoidGameID;
#elif UNITY_ANDROID
        gameID = andoidGameID;
#elif UNITY_IOS
        gameID = iosGameID;
#endif
    }
    void Start()
    {
        gameDataManager = BlockManager.Instance.gameDataManager;
        leaderBoardManager = LeaderBoardManager.Instance;
        adsLeaderboardManager = AdsLeaderboardManager.Instance;
        isGameStart = false;
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            isOnline = true;
        }
        ReInitialize();
    }

    private void OnlineStatus(bool status)
    {
        isOnline = status;
        if (isOnline == true)
        {
            if(!isInisialize)
                Invoke(nameof(ReInitialize), 2f);

            if (!Advertisement.isInitialized && isInisialize)
            {
                Initialize_Ads();
            }
        }
    }
    private void ReInitialize()
    {
        Initialize();
    }


    public async void Initialize()
    {
        if (!initializeActive && isOnline)
        {
            for (int i = 0; i < UpdateStatus.Length; i++)
            {
                UpdateStatus[i] = false;
            }
            isInisialize = false;
            initializeActive = true;
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                {
                    await UnityServices.InitializeAsync();
                    AnalyticsService.Instance.StartDataCollection();
                }
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log("Signed in anonymously as: " + AuthenticationService.Instance.PlayerId);
                    leaderBoardManager.SetUpPlayerID(AuthenticationService.Instance.PlayerId);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (UnityServices.State == ServicesInitializationState.Initialized && AuthenticationService.Instance.IsSignedIn)
            {
                isInisialize = true;
            }
            if (isInisialize)
            {
                if(adsLeaderboardManager == null)
                    adsLeaderboardManager = AdsLeaderboardManager.Instance;

                if(gameDataManager == null)
                    gameDataManager = BlockManager.Instance.gameDataManager;
                Initialize_Ads();
            }
            initializeActive = false;
            GameStart();
        }
        else
        {
            GameStart();
        }
    }

    private void GameStart()
    {
        if (!isGameStart)
        {
            isGameStart = true;
            MenuTutorialManager.Instance.GameStart();
        }
    }

    private void LeaderboardCall()
    {
        int num = PlayerPrefs.GetInt(FirstPlayPref, 0);
        if (num == 0)
        {
            adsLeaderboardManager.SubmitHighestScore(0, true);
            adsLeaderboardManager.SubmitTotalScore(0);
            adsLeaderboardManager.SubmitStarScore(0);
            PlayerPrefs.SetInt(FirstPlayPref, 5);
        }
        UpdatePlayerHighestboardData();
        UpdatePlayerStarData();
        UpdateAllLeaderData(-1);
    }


    private void CheckPlayerName()
    {
        string name = gameDataManager.GetStringData(14);
        UpdatePlayerName(name);
    }

    public async void UpdatePlayerName(string playerName)
    {
        string cloudPlayername = AuthenticationService.Instance.PlayerName;
        if (!string.Equals(playerName, cloudPlayername))
        {
            try
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }

    private void UpdatePlayerHighestboardData()
    {
        int score = gameDataManager.GetSaveValues(2);
        int prescore = gameDataManager.GetSaveValues(3);
        int lastUpdatedScore = gameDataManager.GetSaveValues(16);
        if (score > lastUpdatedScore && prescore >= 4999)
        {
            adsLeaderboardManager.SubmitHighestScore(score, true);
        }
    }

    private void UpdatePlayerStarData()
    {
        int star = gameDataManager.GetSaveValues(13);
        int lastUpdatedStar = gameDataManager.GetSaveValues(17);
        int diff = star - lastUpdatedStar;
        if (diff > 0)
        {
            adsLeaderboardManager.SubmitStarScore(diff);
        }
    }

    public void UpdateAllLeaderData(int index)
    {
        if (isInisialize)
        {
            if (isOnline)
            {
                leaderBoardManager.ResetLeaderBoard();
                if (index < 0)
                {
                    ReUpdateData(index);
                }
                else
                {
                    if(index == 0)
                    {
                        GetThisPlayerHighestScoreData();
                        GetTopHighestScore(index);
                    }else if(index == 1)
                    {
                        GetThisPlayerTotalScoreData();
                        GetTopTotalScore(index);
                    }
                    else
                    {
                        GetThisPlayerStarScoreData();
                        GetTopTotalStarScore(index);
                    }
                }
            }
        }
        else
        {
            Initialize();
        }
    }

    private void ReUpdateData(int index)
    {
        UpdateStatus[0] = true;
        for (int i = 1; i < UpdateStatus.Length; i++)
        {
            UpdateStatus[i] = false;
        }
        CheckPlayerName();
        GetTopTotalScore(index);
        GetTopHighestScore(index);
        GetTopTotalStarScore(index);

        GetThisPlayerTotalScoreData();
        GetThisPlayerStarScoreData();
        GetThisPlayerHighestScoreData();
    }

    /// <summary>
    /// Fetches the top N entries from the leaderboard.
    /// </summary>
    public async void GetTopTotalScore(int index, int limit = 50)
    {
        List<LeaderboardEntry> leaderboardsTotalScore = new List<LeaderboardEntry>();
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(totalScoreBoardID, new GetScoresOptions { Limit = limit });
            for (int i = 0; i < scores.Results.Count; i++)
            {
                var entry = scores.Results[i];
                leaderboardsTotalScore.Add(entry);
            }
            CheckForUpdateStatus(1, true, index);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetTopScores failed: " + e.Message);
            CheckForUpdateStatus(1, false, index);

        }
        leaderBoardManager.UpdateLeaderboardsDataDictionary(leaderboardsTotalScore, 1, index);
    }

    /// <summary>
    /// Fetches the top N entries from the leaderboard.
    /// </summary>
    public async void GetTopHighestScore(int index, int limit = 50)
    {
        List<LeaderboardEntry> leaderboardsHighestScore = new List<LeaderboardEntry>();
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(highestScoreBoardID, new GetScoresOptions { Limit = limit });
            for (int i = 0; i < scores.Results.Count; i++)
            {
                var entry = scores.Results[i];
                leaderboardsHighestScore.Add(entry);
            }
            CheckForUpdateStatus(2, true, index);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetTopScores failed: " + e.Message);
            CheckForUpdateStatus(2, false, index);
        }
        leaderBoardManager.UpdateLeaderboardsDataDictionary(leaderboardsHighestScore, 0, index);
    }

    /// <summary>
    /// Fetches the top N entries from the leaderboard.
    /// </summary>
    public async void GetTopTotalStarScore(int index, int limit = 50)
    {
        List<LeaderboardEntry> leaderboardsTopScore = new List<LeaderboardEntry>();
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(totalStarBoardID, new GetScoresOptions { Limit = limit });
            for (int i = 0; i < scores.Results.Count; i++)
            {
                var entry = scores.Results[i];
                leaderboardsTopScore.Add(entry);
            }
            CheckForUpdateStatus(3, true, index);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetTopScores failed: " + e.Message);
            CheckForUpdateStatus(3, false, index);
        }
        leaderBoardManager.UpdateLeaderboardsDataDictionary(leaderboardsTopScore, 2, index);
    }
    private async void GetThisPlayerTotalScoreData()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetPlayerScoreAsync(totalScoreBoardID);
            thisPlayerData[1] = scores;
            CheckForUpdateStatus(4, true);
            if(gameDataManager != null)
                gameDataManager.SetPlayerGlobalData(scores.Rank, scores.PlayerName, scores.Score);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetPlayerTopScores failed: " + e.Message);
            adsLeaderboardManager.SubmitTotalScore(0);
            CheckForUpdateStatus(4, false);
        }
    }

    private async void GetThisPlayerHighestScoreData()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetPlayerScoreAsync(highestScoreBoardID);
            thisPlayerData[0] = scores;
            CheckForUpdateStatus(5, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetPlayerHighestScores failed: " + e.Message);
            adsLeaderboardManager.SubmitHighestScore(0, false);
            CheckForUpdateStatus(5, false);
        }
    }

    private async void GetThisPlayerStarScoreData()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetPlayerScoreAsync(totalStarBoardID);
            thisPlayerData[2] = scores;
            CheckForUpdateStatus(6, true);
        }
        catch (System.Exception e)
        {
            Debug.LogError("GetPlayerHighestScores failed: " + e.Message);
            adsLeaderboardManager.SubmitStarScore(0);
            CheckForUpdateStatus(6, false);

        }
    }

    public LeaderboardEntry thisPlayerdata(int index)
    {
        if(index < thisPlayerData.Length)
        {
            return thisPlayerData[index];
        }
        else
        {
            return null;
        }
    }

    private void CheckForUpdateStatus(int index, bool status, int code = -1)
    {
        if(index >= 1 && index < UpdateStatus.Length)
        {
            UpdateStatus[index] = status;
        }
        if (!status && isOnline)
        {
            CheckForAllUpdateData(code);
        }
    }

    private void CheckForAllUpdateData(int index)
    {
        if (!UpdateStatus[0])
        {
            ReUpdateData(index);
        }
        else if (!UpdateStatus[1])
        {
            GetTopTotalScore(index);
        }
        else if (!UpdateStatus[2])
        {
            GetTopHighestScore(index);
        }
        else if (!UpdateStatus[3])
        {
            GetTopTotalStarScore(index);
        }
        else if (!UpdateStatus[4])
        {
            GetThisPlayerTotalScoreData();
        }
        else if (!UpdateStatus[5])
        {
            GetThisPlayerStarScoreData();
        }
        else if (!UpdateStatus[6])
        {
            GetThisPlayerHighestScoreData();
        }
    }

    public void Initialize_Ads()
    {
        if (!adsInitializing && !adsLeaderboardManager.isAdsInitialized)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                adsLeaderboardManager.isAdsInitialized = false;
            }
            else
            {
                if (!Advertisement.isInitialized)
                {
                    adsLeaderboardManager.isAdsInitialized = false;
                    adsInitializing = true;
                    if (Advertisement.isSupported && isOnline)
                    {
                        print("ads Initialize start");
                        Advertisement.Initialize(gameID, adsLeaderboardManager.isTesting, this);
                    }
                }
                else
                {
                    LeaderboardCall();
                    adsInitializing = true;
                    adsLeaderboardManager.isAdsInitialized = true;
                    if (gameDataManager.isMenuOpened)
                        MenuManager.Instance.LoadingComplete();
                }
            }
        }
    }



    //public void OnInitializationComplete()
    //{
    //    adsInitializing = true;
    //    adsLeaderboardManager.AdsInisializationComplete();
    //}

    //public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    //{
    //    adsLeaderboardManager.isAdsInitialized = false;
    //    adsInitializing = false;
    //}




    private void OnDisable()
    {
        AdsLeaderboardManager.ApplicationNetworkConnectivity -= OnlineStatus;
    }

    public void OnInitializationComplete()
    {
        LeaderboardCall();
        adsInitializing = true;
        adsLeaderboardManager.AdsInisializationComplete();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        adsLeaderboardManager.isAdsInitialized = false;
        adsInitializing = false;
    }
}
