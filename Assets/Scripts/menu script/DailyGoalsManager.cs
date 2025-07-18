using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class DailyTargetData
{
    public bool isActive;
    public bool isClaimed;
    public Normal_Block_Type normalBlockType = Normal_Block_Type.none;
    public BlockType abilityType = BlockType.None;
    public int totalCount;
    public int currentCount;
    public int coins;
}
[Serializable]
public class DailyTarget
{
    public DailyTargetData[] dailyTargetData = new DailyTargetData[4];
    public int goalsClainCount;
    
}

public class DailyGoalsManager : MonoBehaviour
{
    public static DailyGoalsManager Instance;

    private int[] count1 = new int[3] { 50, 100, 150};
    private int[] count2 = new int[3] { 5, 10, 15 };
    private int[] count3 = new int[3] { 3, 6, 9};
    private int[] count4 = new int[3] { 7, 14, 21 };
    private int[] claimValues = new int[2] { 100, 1 };


    private int[] coinsCount = new int[3] { 100, 150, 200};
    private List<int> coinIndex = new List<int>();

    private DailyTarget dailyTarget;
    private bool isDataLoaded;
    private string savePathJson;
    private GameDataManager gameDataManager;
    public bool isNewLevel { get; set; }
    public DailyTarget DailyTargetData()
    {
        return dailyTarget;
    }

    private void Awake()
    {
        savePathJson = Application.persistentDataPath + "/GameGoalDataFile.json";
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        //PlayerPrefs.SetString(dailyDate, "");
    }
    private void Start()
    {
        GameManager.BlockDes += OnBlockDdestroy;
    }

    public void Initialize()
    {
        gameDataManager = BlockManager.Instance.gameDataManager;
        string dateData = PlayerPrefs.GetString(gameDataManager.DailyGoalsPref, "");
        DateTime today = DateTime.Now.Date;
        if (!string.IsNullOrEmpty(dateData))
        {
            DateTime lastSavedDate = DateTime.Parse(dateData);
            TimeSpan difference = today - lastSavedDate;
            int dayDifference = difference.Days;

            if (!isDataLoaded)
            {
                if (dayDifference > 0)
                {
                    PlayerPrefs.SetString(gameDataManager.DailyGoalsPref, today.ToString());
                    SetUpDailyTargets();
                    gameDataManager.SpecialData(1);
                }
                else
                {
                    LoadDailyTargets();
                }
            }
            if(dayDifference > 4)
            {
                BonusGiftManager.OnWelcomeData(true);
            }

        }
        else
        {
            isDataLoaded = false;
            PlayerPrefs.SetString(gameDataManager.DailyGoalsPref, today.ToString());
            SetUpDailyTargets();
            gameDataManager.SpecialData(1);
            BonusGiftManager.OnWelcomeData(false);
        }
        StartCoroutine(SetUpGoals());
    }

    public bool CheckForDataLoad()
    {
        bool isSet = false;
        if (!isDataLoaded)
        {
            LoadDailyTargets();
            isSet = true;
        }
        else
        {
            bool isdataloadFailed = false;
            if (dailyTarget != null && dailyTarget.dailyTargetData != null && dailyTarget.dailyTargetData.Length > 0)
            {
                foreach (var item in dailyTarget.dailyTargetData)
                {
                    if (item.totalCount == 0)
                    {
                        isdataloadFailed = true;
                        break;
                    }

                }
            }
            else
            {
                isdataloadFailed = true;
                SetUpDailyTargets();
            }
            isSet = !isdataloadFailed;
        }
        return isSet;
    }


    private IEnumerator SetUpGoals()
    {
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++)
        {
            if (CheckForDataLoad())
            {
                yield break;
            }
            else
            {
                SetUpDailyTargets();
                yield return new WaitForSeconds(0.1f);
            }
        }
        MenuManager.Instance.SetUpGoalsPanel();
    }


    private void OnDisable()
    {
        GameManager.BlockDes -= OnBlockDdestroy;
        SaveDailyTargets();

    }
    private void OnBlockDdestroy(Normal_Block_Type arg1, BlockType arg2,Gem_Type gemtype, Vector3 arg3)
    {
        if (isNewLevel)
        {
            if (arg2 != BlockType.None)
            {
                if (arg1 == dailyTarget.dailyTargetData[2].normalBlockType && arg2 == dailyTarget.dailyTargetData[2].abilityType)
                {
                    if (dailyTarget.dailyTargetData[2].isActive)
                    {
                        if (dailyTarget.dailyTargetData[2].totalCount > dailyTarget.dailyTargetData[2].currentCount)
                        {
                            dailyTarget.dailyTargetData[2].currentCount++;
                        }
                        if (dailyTarget.dailyTargetData[2].totalCount <= dailyTarget.dailyTargetData[2].currentCount)
                        {
                            dailyTarget.dailyTargetData[2].isActive = false;
                        }
                    }
                }
                else if (arg1 == dailyTarget.dailyTargetData[0].normalBlockType && arg2 == BlockType.Normal_Block)
                {
                    if (dailyTarget.dailyTargetData[0].isActive)
                    {
                        if (dailyTarget.dailyTargetData[0].totalCount > dailyTarget.dailyTargetData[0].currentCount)
                        {
                            dailyTarget.dailyTargetData[0].currentCount++;
                        }
                        if (dailyTarget.dailyTargetData[0].totalCount <= dailyTarget.dailyTargetData[0].currentCount)
                        {
                            dailyTarget.dailyTargetData[0].isActive = false;
                        }
                    }
                }
                else if (arg2 == dailyTarget.dailyTargetData[1].abilityType && dailyTarget.dailyTargetData[1].normalBlockType == Normal_Block_Type.none)
                {
                    if (dailyTarget.dailyTargetData[1].isActive)
                    {
                        if (dailyTarget.dailyTargetData[1].totalCount > dailyTarget.dailyTargetData[1].currentCount)
                        {
                            dailyTarget.dailyTargetData[1].currentCount++;
                        }
                        if(dailyTarget.dailyTargetData[1].totalCount <= dailyTarget.dailyTargetData[1].currentCount)
                        {
                            dailyTarget.dailyTargetData[1].isActive = false;
                        }
                    }
                }
            }
            else
            {
                int index = (int)arg1;
                if(index > 6)
                {
                    if (dailyTarget.dailyTargetData[3].abilityType == BlockType.None && dailyTarget.dailyTargetData[3].normalBlockType == arg1)
                    {
                        if (dailyTarget.dailyTargetData[3].isActive)
                        {
                            if (dailyTarget.dailyTargetData[3].totalCount > dailyTarget.dailyTargetData[3].currentCount)
                            {
                                dailyTarget.dailyTargetData[3].currentCount++;
                            }
                            if (dailyTarget.dailyTargetData[3].totalCount <= dailyTarget.dailyTargetData[3].currentCount)
                            {
                                dailyTarget.dailyTargetData[3].isActive = false;
                            }
                        }
                    }
                }
            }
        }
    }

    private int GetIndex()
    {
        int num = 0;
        int index = UnityEngine.Random.Range(0, coinIndex.Count);
        num = coinIndex[index];
        coinIndex.RemoveAt(index);
        return num;

    }

    private void SetUpDailyTargets()
    {
        coinIndex = new List<int> { 0, 1, 0, 2 };
        dailyTarget = new DailyTarget();
        dailyTarget.dailyTargetData = new DailyTargetData[4];
        for (int i = 0; i < dailyTarget.dailyTargetData.Length; i++)
        {
            DailyTargetData data = new DailyTargetData()
            {
                normalBlockType = Normal_Block_Type.none,
                abilityType = BlockType.None,
                totalCount = 0,
                currentCount = 0,
                coins = 0,
                isActive = true,
                isClaimed = false
            };
            dailyTarget.dailyTargetData[i] = data;
        }
        
        dailyTarget.goalsClainCount = 1;

        int blockColorIndex = UnityEngine.Random.Range(0, 5);
        int countIndex = GetIndex();
        Normal_Block_Type blocktype = (Normal_Block_Type)blockColorIndex;
        dailyTarget.dailyTargetData[0].normalBlockType = blocktype;
        dailyTarget.dailyTargetData[0].abilityType = BlockType.None;
        dailyTarget.dailyTargetData[0].totalCount = count1[countIndex];
        dailyTarget.dailyTargetData[0].coins = coinsCount[countIndex];


        int abilitynumIndex = UnityEngine.Random.Range(1, 6);
        int countIndex1 = GetIndex();
        BlockType blocktype1 = (BlockType)abilitynumIndex;
        dailyTarget.dailyTargetData[1].abilityType = blocktype1;
        dailyTarget.dailyTargetData[1].normalBlockType = Normal_Block_Type.none;
        dailyTarget.dailyTargetData[1].totalCount = count2[countIndex1];
        dailyTarget.dailyTargetData[1].coins = coinsCount[countIndex1];

        int blockcolorIndex2 = UnityEngine.Random.Range(0, 5);
        int maxitter = 50;
        while(maxitter > 0)
        {
            maxitter--;
            if (blockColorIndex == blockcolorIndex2)
            {
                blockcolorIndex2 = UnityEngine.Random.Range(0, 5);
            }
            else
            {
                break;
            }
        }
        
        int abilitynumIndex2 = UnityEngine.Random.Range(1, 6);
        maxitter = 50;
        while (maxitter > 0)
        {
            maxitter--;
            if (abilitynumIndex == abilitynumIndex2)
            {
                abilitynumIndex2 = UnityEngine.Random.Range(1, 6);
            }
            else
            {
                break;
            }
        }

        int countIndex2 = GetIndex();
        Normal_Block_Type blocktype21 = (Normal_Block_Type)blockcolorIndex2;
        BlockType blocktype2 = (BlockType)abilitynumIndex2;
        dailyTarget.dailyTargetData[2].abilityType = blocktype2;
        dailyTarget.dailyTargetData[2].normalBlockType = blocktype21;
        dailyTarget.dailyTargetData[2].totalCount = count3[countIndex2];
        dailyTarget.dailyTargetData[2].coins = coinsCount[countIndex2];


        int countIndex3 = GetIndex();
        int obsType = UnityEngine.Random.Range(7, 12);
        if(gameDataManager.currentLevel < 30 && obsType > 7)
        {
            obsType = 7;
        }
        else if (gameDataManager.currentLevel < 120 && obsType > 8)
        {
            obsType = UnityEngine.Random.Range(7, 9);
        }
        else if(gameDataManager.currentLevel < 250 && obsType > 9)
        {
            obsType = UnityEngine.Random.Range(7, 10);
        }
        else if (gameDataManager.currentLevel < 400 && obsType > 9)
        {
            obsType = UnityEngine.Random.Range(7, 12);
        }
        for (int i = 0; i < 15; i++)
        {
            if(obsType == 10)
            {
                obsType = UnityEngine.Random.Range(7, 12);
            }
            else
            {
                break;
            }
        }
        Normal_Block_Type blocktype3 = (Normal_Block_Type)obsType;
        dailyTarget.dailyTargetData[3].abilityType = BlockType.None;
        dailyTarget.dailyTargetData[3].normalBlockType = blocktype3;
        dailyTarget.dailyTargetData[3].totalCount = count4[countIndex3];
        dailyTarget.dailyTargetData[3].coins = coinsCount[countIndex3];


        isDataLoaded = true;
        SaveDailyTargets();
    }

    private void SaveDailyTargets()
    {
        string jsonData = JsonUtility.ToJson(dailyTarget);
        // Write the JSON data to a file
        File.WriteAllText(savePathJson, jsonData);
    }

    private void LoadDailyTargets()
    {
        if (File.Exists(savePathJson))
        {
            if (!isDataLoaded)
            {
                dailyTarget = new DailyTarget();
                dailyTarget.dailyTargetData = new DailyTargetData[3];
                string jsonData = File.ReadAllText(savePathJson);
                dailyTarget = JsonUtility.FromJson<DailyTarget>(jsonData);
                isDataLoaded = true;
            }
        }
        else
        {
            isDataLoaded = false;
            Debug.LogWarning("Save file not found!");
        }
    }

    public void GoalCoinClaimed(int index)
    {
        dailyTarget.dailyTargetData[index].isActive = false;
        dailyTarget.dailyTargetData[index].isClaimed = true;
        SaveDailyTargets();

    }

    public void ClaimFree(bool isCoin)
    {
        if (dailyTarget.goalsClainCount > 0)
            dailyTarget.goalsClainCount--;
        else
        {
            dailyTarget.goalsClainCount = 0;
        }
        if (isCoin)
        {
            gameDataManager.CoinValueChange(claimValues[0], true);
        }
        else
        {
            gameDataManager.LifeValueChange(claimValues[1], true);
        }
    }

    public bool CheckForGoalsFreeClaim(int code)
    {
        bool isInterect = false;
        if (code == 2 && dailyTarget.goalsClainCount > 0)
        {
            isInterect = true;
        }
        return isInterect;
    }

    

    public bool HasGoalsAchived()
    {
        bool status = false;
        foreach (var item in dailyTarget.dailyTargetData)
        {
            if(!item.isActive && !item.isClaimed)
            {
                status = true;
                break;
            }
        }
        return status;
    }

}
