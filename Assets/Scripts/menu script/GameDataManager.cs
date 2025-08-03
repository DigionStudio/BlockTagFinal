using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[Serializable]
public class PlayerGlobalData
{
    public int Rank;
    public string Name;
    public double scoreValue;
}

[Serializable]
public class Ability_Status_Data
{
    public Game_Value_Status abilityValueStatus;
    public string unlimitedStartTime;
    public int totalSeconds;
}


[Serializable]
public class Level_Star_Data
{
    public int StarCount;
    public bool isUnlock;
}

[Serializable]
public class LevelDataHolder
{
    public string playerID;
    public string playerName;
    public List<Level_Star_Data> LevelStarData = new();
    public List<Block_Icon_Data> IconOpenData = new();
    public List<Ability_Status_Data> abilityCountData = new();
    public bool isNoAds;
    public bool isNewDay;
    public int levelPositionY;
    public int coinValue;
    public Ability_Status_Data lifeValue;
    public int highScoreValue;
    public int prevHighScoreValue;
    public int lastUpdatedHighScore;
    public int currentBlockIndex;
    public int currentLevelIndex;
    public int totalStar;
    public int lastUpdatedStarScore;
    public int maxHitPoint;
    public int audioStatus;
    public int sfxStatus;
    public int voiceStatus;
    public int previousLevel;
    public int wheelCount;
    public int appBundleCode;
}

[Serializable]
public class Block_Icon_Data
{
    public bool isBuyed;
    public int onceUsedData;
}

[Serializable]
public class GiftData
{
    public VariableTypeCode indexCode = VariableTypeCode.None;
    public int values;
}

public class GameDataManager : MonoBehaviour
{
    private static GameDataManager Instance;
    public readonly string FirstPlayPref = "FirstPlayPref4";
    public readonly string TutorialPref = "TutorialPref13";
    public readonly string DailyGoalsPref = "Daily_Date";
    public readonly string LevelPosY = "LevelPosY";


    [SerializeField] private LevelDataHolder levelDataHolder;
    private readonly int totalIcons = 10;
    private readonly int totalAbility = 6;
    private int gameAdsCount;
    private int gameOverAdsCount;
    public bool HasDisableAds { get { return levelDataHolder.isNoAds; } }
    public bool HasNewDay { get { return levelDataHolder.isNewDay; } }


    private PlayerGlobalData playerGlobalData;
    private string savePathJson;
    private int gameTypeCode = 0;
    private int totalCoin;
    private int totalLife;
    public GiftData[] giftData;
    public bool isGifted;
    public int GameTypeCode { get { return gameTypeCode; } set { gameTypeCode = value; } }
    public LevelData levelData;


    public bool isplayerNameSelect;
    public bool isMenuOpened;
    public int currentLevel;
    public int previousLevel;
    public int rePlayCount;
    public bool isBombAiAsist;
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
        savePathJson = Application.persistentDataPath + "/GameDataSaveFile.json";
        //PlayerPrefs.SetInt(FirstPlayPref, 0);
        //PlayerPrefs.SetInt(TutorialPref, 0);
        //PlayerPrefs.SetInt(DailyGoalsPref, 0);
        //PlayerPrefs.SetInt(LevelPosY, 0);
        playerGlobalData = new PlayerGlobalData();
        rePlayCount = 0;
        isMenuOpened = true;
        LoadPlayerData();
        currentLevel = levelDataHolder.currentLevelIndex;
        previousLevel = levelDataHolder.previousLevel;
        if(previousLevel < currentLevel - 1)
        {
            previousLevel = currentLevel - 1;
            levelDataHolder.previousLevel = previousLevel;
        }

        int num = PlayerPrefs.GetInt(FirstPlayPref, 0);
        if(num == 0)
        {
            levelDataHolder.highScoreValue = 4999;
            levelDataHolder.prevHighScoreValue = 0;
            levelDataHolder.lastUpdatedHighScore = 0;
            levelDataHolder.lastUpdatedStarScore = 0;
            levelDataHolder.maxHitPoint = 200;
            PlayerPrefs.SetInt(FirstPlayPref, 5);
            levelDataHolder.playerName = GeneratePlayerName();
        }
        else
        {
            totalCoin = levelDataHolder.coinValue;
            totalLife = levelDataHolder.lifeValue.abilityValueStatus.value;
        }
    }


    private string GeneratePlayerName()
    {
        string playername = "ID";
        int num = UnityEngine.Random.Range(1000, 10000);
        playername += num.ToString();
        return playername;
    }


    public void SavePlayerData()
    {
        string jsonData = JsonUtility.ToJson(levelDataHolder);

        // Write the JSON data to a file
        File.WriteAllText(savePathJson, jsonData);
    }

    public void LoadPlayerData()
    {
        if (File.Exists(savePathJson))
        {
            // Read the JSON data from the file
            string jsonData = File.ReadAllText(savePathJson);
            levelDataHolder =  JsonUtility.FromJson<LevelDataHolder>(jsonData);
        }
        else
        {
            Debug.LogWarning("Save file not found!");
        }

        if(levelDataHolder != null)
        {
            if(levelDataHolder.IconOpenData.Count < totalIcons)
            {
                Block_Icon_Data icondata = new Block_Icon_Data();
                levelDataHolder.IconOpenData.Add(icondata);
            }

            if (levelDataHolder.abilityCountData.Count < totalAbility)
            {
                Ability_Status_Data dt = new Ability_Status_Data();

                dt.abilityValueStatus = new Game_Value_Status()
                {
                    value = 0,
                    status = false
                };
                dt.unlimitedStartTime = "";
                dt.totalSeconds = -1;
                levelDataHolder.abilityCountData.Add(dt);
            }

        }
    }

    public void UpdatePlayerData(int level,int starCount, int currentPoint, int currentHitPoint, int gameStatus)
    {
        int newStar = 0;
        if (gameTypeCode == 1)
        {
            if (gameStatus == 1)
            {
                if (level < levelDataHolder.LevelStarData.Count)
                {
                    int nextLevel = level + 1;
                    int count = levelDataHolder.LevelStarData[level].StarCount;
                    if (starCount > count)
                    {
                        levelDataHolder.LevelStarData[level].StarCount = starCount;
                        newStar = starCount - count;
                        if (newStar > 0)
                        {
                            levelDataHolder.totalStar += newStar;
                        }
                    }
                    if (nextLevel < levelDataHolder.LevelStarData.Count)
                    {
                        if (!levelDataHolder.LevelStarData[nextLevel].isUnlock)
                        {
                            levelDataHolder.LevelStarData[nextLevel].StarCount = 0;
                        }
                        levelDataHolder.LevelStarData[nextLevel].isUnlock = true;
                    }
                    if (nextLevel > levelDataHolder.currentLevelIndex)
                    {
                        previousLevel = currentLevel;
                        currentLevel = nextLevel;
                        levelDataHolder.currentLevelIndex = currentLevel;
                        levelDataHolder.previousLevel = previousLevel;
                    }
                    else
                    {
                        currentLevel = levelDataHolder.currentLevelIndex;
                        previousLevel = levelDataHolder.previousLevel;
                    }

                }
                
            }
            else
            {
                currentLevel = levelDataHolder.currentLevelIndex;
                previousLevel = levelDataHolder.previousLevel;
            }
        }
        bool ishigh = false;
        if (gameTypeCode == 0 || gameStatus == 1)
        {
            int highPoint = levelDataHolder.highScoreValue;
            if (highPoint < currentPoint)
            {
                ishigh = true;
                levelDataHolder.highScoreValue = currentPoint;
                levelDataHolder.prevHighScoreValue = highPoint;
            }
        }
        else
        {
            currentPoint = 0;
            newStar = 0;
        }
        if (currentHitPoint >= levelDataHolder.maxHitPoint)
        {
            levelDataHolder.maxHitPoint = currentHitPoint;
        }

        UpdateLeaderBoardValues(newStar, currentPoint, ishigh);
        SavePlayerData();
    }

    private void UpdateLeaderBoardValues(int newStar, int score, bool isHigh)
    {
        AdsLeaderboardManager.Instance.SubmitDataIntoLeaderboard(newStar, score, isHigh);
    }

    public bool CheckForFreeClaim(int code, bool isSet)
    {
        bool isInterect = false;
        if (isSet)
        {
            gameAdsCount = 1;
            gameOverAdsCount = 1;
        }
        else
        {
            if (code >= 0)
            {
                if (code == 0 && gameAdsCount > 0)
                {
                    isInterect = true;
                }
                else if (code == 1 && gameOverAdsCount > 0)
                {
                    isInterect = true;
                }
            }
        }
        return isInterect;
    }
    public void GameAdaClaim(int code)
    {
        if (code == 0)
        {
            if (gameAdsCount > 0)
                gameAdsCount--;
            else 
                gameAdsCount = 0;
        }
        else if (code == 1)
        {
            if (gameOverAdsCount > 0)
                gameOverAdsCount--;
            else 
                gameOverAdsCount = 0;
        }
    }


    public bool BuyIconData(int index, int val)
    {
        bool isOpen = false;
        if(totalCoin >= val && levelDataHolder.IconOpenData.Count > index)
        {
            CoinValueChange(val, false);
            levelDataHolder.IconOpenData[index].isBuyed = true;
            isOpen = true;
        }
        return isOpen;
    }


    public int GameAbilitySave(int index, bool isadd, int value)
    {
        if(index >= levelDataHolder.abilityCountData.Count)
        {
            return 0;
        }
        if (value > 0)
        {
            int count = levelDataHolder.abilityCountData[index].abilityValueStatus.value;
            bool isInfinite = levelDataHolder.abilityCountData[index].abilityValueStatus.status;
            int mul = 1;
            if (!isadd)
            {
                if (!isInfinite)
                {
                    if (count > 0 && count >= value)
                        mul = -1;
                    else
                    {
                        mul = 0;
                        value = 0;
                        levelDataHolder.abilityCountData[index].abilityValueStatus.value = 0;
                    }
                }
                else
                {
                    mul = 0;
                    value = 0;
                }
            }
            
            levelDataHolder.abilityCountData[index].abilityValueStatus.value += (value * mul);
            SavePlayerData();
        }
        return levelDataHolder.abilityCountData[index].abilityValueStatus.value;
    }

    public List<Game_Value_Status> AbilityData()
    {
        int len = levelDataHolder.abilityCountData.Count;
        List<Game_Value_Status> arr = new List<Game_Value_Status>();
        for (int i = 0; i < len; i++)
        {
            Game_Value_Status abilityValueStatus = new Game_Value_Status()
            { 
                value = levelDataHolder.abilityCountData[i].abilityValueStatus.value,
                status = levelDataHolder.abilityCountData[i].abilityValueStatus.status
            };
            arr.Add(abilityValueStatus);
        }
        return arr;
    }

    public void SetUnlimitedUseData(int index, int totalSec, string startTime, bool status)
    {
        if (index < levelDataHolder.abilityCountData.Count)
        {
            levelDataHolder.abilityCountData[index].abilityValueStatus.status = status;
            levelDataHolder.abilityCountData[index].totalSeconds = totalSec;
            levelDataHolder.abilityCountData[index].unlimitedStartTime = startTime;
        }
    }
    public int CheckForAnilityStatus(int index, string currentTime)
    {
        int timeLeft = -1;
        if(index < levelDataHolder.abilityCountData.Count && levelDataHolder.abilityCountData[index].abilityValueStatus.status)
        {
            string startTime = levelDataHolder.abilityCountData[index].unlimitedStartTime;
            int totalSec = levelDataHolder.abilityCountData[index].totalSeconds;
            if (!string.IsNullOrEmpty(startTime) && totalSec > 0 && !string.IsNullOrEmpty(currentTime))
            {
                TimeSpan timeLeftDate = DateTime.Now - DateTime.Parse(startTime);
                timeLeft = totalSec - (int)timeLeftDate.TotalSeconds;

                if (timeLeft <= 0)
                {
                    SetUnlimitedUseData(index, -1, "", false);
                    timeLeft = -1;
                }
            }
            else
            {
                SetUnlimitedUseData(index, -1, "", false);
                timeLeft = -1;
            }

        }

        return timeLeft;
    }
   
    private void OnApplicationQuit()
    {
        GiftValueClaim();
    }

    public void CoinValueChange(int value, bool isAdd)
    {
        AddCoins(value, isAdd);
        MenuManager.OnVariableUpdate.Invoke(totalCoin, true, value, isAdd);
    }
    private void AddCoins(int value, bool isAdd)
    {
        int num = 1;
        if (!isAdd)
        {
            if (totalCoin > value)
                num = -1;
            else
            {
                num = 0;
                totalCoin = 0;
            }
        }
        totalCoin += (value * num);
        levelDataHolder.coinValue = totalCoin;
    }

    public void LifeValueChange(int value, bool isAdd)
    {
        if(value > 0)
            AddLife(value, isAdd);
        MenuManager.OnVariableUpdate.Invoke(totalLife, false, value, isAdd);
    }
    private void AddLife(int value, bool isAdd)
    {
        int num = 1;
        bool isInfinite = levelDataHolder.lifeValue.abilityValueStatus.status;
        if (!isAdd)
        {
            if (!isInfinite)
            {
                if (totalLife > 0 && totalLife >= value)
                    num = -1;
                else
                {
                    num = 0;
                    value = 0;
                    totalLife = 0;
                }
            }
            else
            {
                num = 0;
                value = 0;
            }
        }
        totalLife += (value * num);
        levelDataHolder.lifeValue.abilityValueStatus.value = totalLife;
    }

    public void AbilityCountValue(int index, int addValue)
    {
        GameAbilitySave(index,true, addValue);
    }

    public void SetGiftData(GiftData[] data, int length)
    {
        giftData = new GiftData[length];
        for (int i = 0; i < data.Length; i++)
        {
            GiftData dt = new GiftData
            {
                indexCode = data[i].indexCode,
                values = data[i].values,
            };
            giftData[i] = dt;
            isGifted = true;
        }
    }

    public void GiftValueClaim()
    {
        if (giftData != null && giftData.Length > 0 && isGifted)
        {
            for (int i = 0; i < giftData.Length; i++)
            {
                if(giftData[i] != null)
                    SetResourcesValues(giftData[i].indexCode, giftData[i].values);
            }
        }
        GetGiftData(true);
        isGifted= false;
        SavePlayerData();
    }
    public void SetResourcesValues(VariableTypeCode indexCode, int values)
    {
        int abilityCount = levelDataHolder.abilityCountData.Count;
        if (values > 0 && indexCode != VariableTypeCode.None)
        {
            if (indexCode == VariableTypeCode.Coin)
            {
                CoinValueChange(values, true);

            }
            else if (indexCode == VariableTypeCode.Life)
            {
                LifeValueChange(values, true);
            }
            else if (indexCode == VariableTypeCode.Lucky_Wheel)
            {
                levelDataHolder.wheelCount += values;
            }
            else
                    {
                int index = (int)indexCode - 1;
                if (index >= 0 && index < abilityCount)
                {
                    AbilityCountValue(index, values);
                }
            }

        }
    }


    public GiftData[] GetGiftData(bool isReset)
    {
        if (isReset)
        {
            giftData = new GiftData[0];
        }
        else
        {
            if (giftData.Length > 0)
                return giftData;
        }
        return null;

    }

    //public int coinValue;
    //public int lifeValue;
    //public int highScoreValue;
    //public int prevHighScoreValue;
    //public int currentBlockIndex;
    //public int currentLevelIndex;
    //public int maxHitPoint;
    //public int audioStatus;
    //public int appBundleCode;

    /// <summary>
    ///  (index) :- 0 = coin, 1 = life, 2 =  highScoreValue, 3 = prevHighScoreValue
    /// 4 = currentBlockIndex, 6 = maxHitPoint, 7 = audioStatus,
    /// 8 = appBundleCode, 9 = levelPositionY, 10 = wheel count, 11 = sfx sound, 12 = voice sound;
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public int GetSaveValues(int index)
    {
        int value = 0;
        if (index == 0)
        {
            value = totalCoin;
        }
        else if(index == 1)
        {
            value = totalLife;
        }
        else if(index == 2)
        {
            value = levelDataHolder.highScoreValue;
        }
        else if(index == 3)
        {
            value = levelDataHolder.prevHighScoreValue;
        }
        else if(index == 4)
        {
            value = levelDataHolder.currentBlockIndex;
        }
        else if(index == 6)
        {
            value = levelDataHolder.maxHitPoint;
        }
        else if (index == 7)
        {
            value = levelDataHolder.audioStatus;
        }
        else if (index == 8)
        {
            value = levelDataHolder.appBundleCode;
        }else if(index == 9)
        {
            value = levelDataHolder.levelPositionY;
        }
        else if (index == 10)
        {
            value = levelDataHolder.wheelCount;
        }else if (index == 11)
        {
            value = levelDataHolder.sfxStatus;
        }else if(index == 12)
        {
            value = levelDataHolder.voiceStatus;
        }else if( index == 13)
        {
            value = levelDataHolder.totalStar;
        }
        else if (index == 16)
        {
            value = levelDataHolder.lastUpdatedHighScore;
        }
        else if (index == 17)
        {
            value = levelDataHolder.lastUpdatedStarScore;
        }
        return value;
    }

    public string GetStringData(int index)
    {
        string value = "";
        if(index == 14)
        {
            value = levelDataHolder.playerName;
        }else if(index == 15)
        {
            value = levelDataHolder.playerID;
        }
        return value;
    }

    public void SetStringData(int index, string value)
    {
        if(index == 14)
        {
            levelDataHolder.playerName = value;
        }
        else if (index == 15)
        {
            levelDataHolder.playerID = value;
        }
    }

    /// <summary>
    /// 
    /// (index) :- 0 = coin, 1 = life, 2 =  highScoreValue, 3 = prevHighScoreValue
    /// 4 = currentBlockIndex, 5 = currentLevelIndex, 6 = maxHitPoint, 7 = audioStatus,
    /// 8 = appBundleCode, 9 = levelPositionY, 10 = wheel count, 11 = sfx sound, 12 = voice sound;
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    public void SetSaveValues(int index, int value)
    {
        if (index == 4)
        {
            levelDataHolder.currentBlockIndex = value;
        }
        else if (index == 5)
        {
            levelDataHolder.currentLevelIndex = value;
        }
        else if (index == 7)
        {
            levelDataHolder.audioStatus = value;
        }
        else if (index == 8)
        {
            levelDataHolder.appBundleCode = value;
        }
        else if (index == 9)
        {
            levelDataHolder.levelPositionY = value;
        }else if(index == 10)
        {
            levelDataHolder.wheelCount = value;
        }else if(index == 11)
        {
            levelDataHolder.sfxStatus = value;
        }
        else if (index == 12)
        {
            levelDataHolder.voiceStatus = value;
        }
        else if (index == 13)
        {
            levelDataHolder.totalStar = value;
        }
        else if (index == 16)
        {
            levelDataHolder.lastUpdatedHighScore = value;
        }
        else if (index == 17)
        {
            levelDataHolder.lastUpdatedStarScore = value;
        }
    }

    public void SpecialData(int index, bool status = true)
    {
        if (index == 101)
        {
            levelDataHolder.isNoAds = status;
        }
        else
        {
            levelDataHolder.isNewDay = status;
        }
    }


    /// <summary>
    /// code 0 for list levelstardata, code 1 for iconopendata, else abilitycount
    /// </summary>
    /// <param name="index"></param>
    /// <param name="code"></param>
    /// <param name="islength"></param>
    /// <returns></returns>
    public int GetSaveDataListlength(int code)
    {
        int value = 0;
        if (code == 0)
        {
            value = levelDataHolder.LevelStarData.Count;
        }
        else if (code == 1)
        {
            value = levelDataHolder.IconOpenData.Count;
        }
        else
        {
            value = levelDataHolder.abilityCountData.Count;
        }
        return value;
    }

    public Level_Star_Data LevelStarData(int index)
    {
        Level_Star_Data data = new();
        if (index < levelDataHolder.LevelStarData.Count)
        {
            data = levelDataHolder.LevelStarData[index];
        }
        return data;
    }

    public void AddLevelStarData()
    {
        Level_Star_Data addData = new();
        levelDataHolder.LevelStarData.Add(addData);
    }

    public bool CheckForBlockIconOnceUsed(int result)
    {
        bool isMatch = false;
        foreach (var item in levelDataHolder.IconOpenData)
        {
            if (item.onceUsedData == result)
            {
                isMatch = true;
                break;
            }
        }
        return isMatch;
    }
    
    public Block_Icon_Data GetSaveBlockIconData(int index, int onceused = -1)
    {
        Block_Icon_Data data = new();
        if(index < levelDataHolder.IconOpenData.Count)
        {
            data = levelDataHolder.IconOpenData[index];
            if(onceused >= 0)
            {
                levelDataHolder.IconOpenData[index].onceUsedData = onceused;
            }
        }
        return data;
    }

    public PlayerGlobalData GetPlayerGlobalData()
    {
        if (string.IsNullOrEmpty(playerGlobalData.Name))
        {
            return null;
        }
        return playerGlobalData;
    }
    public void SetPlayerGlobalData(int rank, string name, double score)
    {
        playerGlobalData.Rank = rank;
        playerGlobalData.Name = name;
        playerGlobalData.scoreValue = score;
    }
}
