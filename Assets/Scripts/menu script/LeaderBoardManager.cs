using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Leaderboards.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class IconsReference
{
    public Sprite[] iconSprite;
}

[Serializable]
public class LeaderboardButtons
{
    public Button useButton;
    public Image buttonImage;
    public Text buttontext;
}

public class LeaderBoardManager : MonoBehaviour
{
    public static LeaderBoardManager Instance;
    public GameObject[] holders;
    public LeaderboardButtons[] leaderBoardButtons;
    public Color SelectedColor;
    public Color DisableColor;
    private LeaderboardButtons currentButton;

    public ScrollRect currentScrollRect;
    public RectTransform boardContainerRect;
    public LeaderboardShow leaderboardShowPrefab;
    public List<LeaderboardShow> leaderboardShows = new List<LeaderboardShow>();

    private Dictionary<int, List<LeaderboardEntry>> LeaderboardCategory = new Dictionary<int, List<LeaderboardEntry>>();
    private bool hascurrentPlayer;
    private bool hasCurrentPlayerInstantiated;
    private LeaderboardShow thisPlayerShow;


    public InitializeUnityServices InitializeUnityServices;
    private GameDataManager gameDataManager;

    private int currentIndex = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        AdsLeaderboardManager.ApplicationNetworkConnectivity += ApplicationStatus;
    }

    private void Start()
    {
        if (gameDataManager == null)
            gameDataManager = BlockManager.Instance.gameDataManager;

        leaderBoardButtons[0].useButton.onClick.AddListener(WeeklyButton);
        leaderBoardButtons[1].useButton.onClick.AddListener(GlobalButton);
        leaderBoardButtons[2].useButton.onClick.AddListener(MapButton);
        bool status = false;
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            status = true;
        }
        ApplicationStatus(status);
    }

    public void SetUpPlayerID(string ID)
    {
        if (gameDataManager == null)
            gameDataManager = BlockManager.Instance.gameDataManager;
        gameDataManager.SetStringData(15, ID);
    }

    public void UpdateLeaderboardsDataDictionary(List<LeaderboardEntry> leaderboardscore, int key, int index)
    {
        if (!LeaderboardCategory.ContainsKey(key))
        {
            LeaderboardCategory[key] = new List<LeaderboardEntry>();
        }
        else
        {
            LeaderboardCategory[key].Clear();
        }
        for (int i = 0; i < leaderboardscore.Count; i++)
        {
            LeaderboardEntry newEntry = leaderboardscore[i];
            LeaderboardCategory[key].Add(newEntry);
        }
        if(index >= 0 && index == currentIndex)
        {
            currentIndex = -1;
            ShowLeaderBoard(index);
        }
    }

    public void ResetLeaderBoard()
    {
        foreach (var show in leaderboardShows)
        {
            if (show != null && show.gameObject.activeInHierarchy)
                show.gameObject.SetActive(false);
        }
    }

    private void WeeklyButton()
    {
        ShowLeaderBoard(0);
    }
    private void GlobalButton()
    {
        ShowLeaderBoard(1);
    }
    private void MapButton()
    {
        ShowLeaderBoard(2);
    }

    private void SelectButton(int index)
    {
        if (currentButton != null)
        {
            CurrentButtonStatus(true);
        }
        currentButton = leaderBoardButtons[index];
        CurrentButtonStatus(false);
    }

    private void CurrentButtonStatus(bool status)
    {
        Color col = SelectedColor;
        Color textCol = Color.white;
        int size = 60;
        if (status)
        {
            col = DisableColor;
            textCol = Color.gray;
            size = 50;
        }
        //currentButton.useButton.interactable = status;
        currentButton.buttontext.color = textCol;
        currentButton.buttontext.fontSize = size;
        currentButton.buttonImage.color = col;
    }

    public void ShowLeaderBoard(int index = -1)
    {
        if (index >= 0 && index != currentIndex)
        {
            currentScrollRect.verticalNormalizedPosition = 1f;
            SelectButton(index);
            ResetLeaderBoard();
            currentIndex = index;
            if (LeaderboardCategory.ContainsKey(currentIndex) && LeaderboardCategory[currentIndex].Count > 0)
                ShowAllTotalScoreLeader();
        }
        else
        {
            InitializeUnityServices.UpdateAllLeaderData(index);
        }
    }


    private void ShowAllTotalScoreLeader()
    {
        hascurrentPlayer = false;
        SetHolderSize(20, true);
        if (LeaderboardCategory.TryGetValue(currentIndex, out List<LeaderboardEntry> playerList))
        {
            for (int i = 0; i < leaderboardShows.Count; i++)
            {
                LeaderboardShow show = leaderboardShows[i];
                if (i < playerList.Count)
                {
                    LeaderboardEntry player = playerList[i];
                    if (player != null)
                    {
                        ShowLeaderboardItem(show, player, currentIndex);
                        show.gameObject.SetActive(true);
                        SetHolderSize(140, false);
                    }
                    
                }
            }
            CheckForHavingCurrentPlayer(currentIndex);
        }
    }

    private void ShowLeaderboardItem(LeaderboardShow show, LeaderboardEntry player, int index)
    {
        PlayerGlobalData data = new PlayerGlobalData() {
            Rank = player.Rank,
            Name = player.PlayerName,
            scoreValue = player.Score,

        };
        show.SetUp(data, index, CheckCurrentPlayer(player.PlayerId));
    }

    private bool CheckCurrentPlayer(string id)
    {
        bool isCurrent = false;
        string playerID = gameDataManager.GetStringData(15);
        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(playerID) && string.Equals(id, playerID, StringComparison.OrdinalIgnoreCase))
        {
            isCurrent = true;
            hascurrentPlayer = true;
        }
        return isCurrent;
    }

    private void CheckForHavingCurrentPlayer(int index)
    {
        bool status = false;
        LeaderboardEntry player = InitializeUnityServices.thisPlayerdata(index);

        if (!hascurrentPlayer && player != null)
        {
            if (!hasCurrentPlayerInstantiated)
            {
                hasCurrentPlayerInstantiated = true;
                thisPlayerShow = Instantiate(leaderboardShowPrefab, boardContainerRect);
            }
            status = true;
            if(thisPlayerShow != null)
            {
                ShowLeaderboardItem(thisPlayerShow, player, index);
            }
            SetHolderSize(140, false);
        }
        if (thisPlayerShow != null && thisPlayerShow != null)
        {
            thisPlayerShow.gameObject.SetActive(status);
        }
        
        
    }

    private void SetHolderSize(float num, bool isReset)
    {
        Vector2 size = boardContainerRect.sizeDelta;
        if (isReset)
        {
            size.y = num;
        }
        else
        {
            size.y += num;
        }
        boardContainerRect.sizeDelta = size;
    }

    private void ApplicationStatus(bool status)
    {
        holders[0].SetActive(status);
        holders[1].SetActive(!status);
    }

   

    public void SetNewName(string name)
    {
        if (InitializeUnityServices.isInisialize)
        {
            InitializeUnityServices.UpdatePlayerName(name);
        }
    }
    private void OnDisable()
    {
        AdsLeaderboardManager.ApplicationNetworkConnectivity -= ApplicationStatus;
    }

}
