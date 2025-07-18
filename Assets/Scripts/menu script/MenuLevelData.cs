using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[Serializable]
public class LevelUpdateData
{
    public int levelNumber;
    public int speed;
    public int moveCount;
    public int starValue;
    public int abilityValue1;
    public int abilityValue2;
}
[Serializable]
public class LevelDataList
{
    public List<LevelUpdateData> data;
}

public class MenuLevelData : MonoBehaviour
{
    [SerializeField] private LevelDataInfo[] levelDataInfos;
    [SerializeField] private LevelShow levelShowPrefab;

    private List<LevelShow> levelShow = new List<LevelShow>();
    [SerializeField] private RectTransform levelHolderRect;
    private GameDataManager gameDataManager;
    private int currentLevel;
    private int previousLevel;
    [SerializeField] private Text levelPlayText;
    [SerializeField] private LevelMetaFill levelMetaFill;

    [SerializeField] private ScrollRect scrollRect;
    private bool isScroll = false;
    float scrollSpeed = 0.05f;
    private int setPosition;
    private bool isTriggered = false;

    [SerializeField] private RectTransform levelFadeTrans;
    private string savePathJson = "D:\\Block Tag Repo" + "/LevelDataSaveFile.json";
    private string savePath;
    [SerializeField] private LevelDataList LevelUpdateDatas;
    public struct userAttributes { }
    public struct appAttributes { }

    public Image[] bgImages;

    //private void OnValidate()
    //{
    //    ////Level Speed Data Set
    //    //List<double> speedDatas = LevelBlockSpeedData.blockSpeedDatas;
    //    //print(speedDatas.Count);

    //    //for (int i = 0; i < levelDataInfos.Length; i++)
    //    //{
    //    //    var item = levelDataInfos[i];
    //    //    if (i < speedDatas.Count && item != null)
    //    //    {
    //    //        item.levelData.moveSpeed = (float)speedDatas[i];
    //    //    }
    //    //}

    //    ////Level BG images position Set
    //    //for (int i = 1; i < bgImages.Length; i++)
    //    //{
    //    //    float posy = bgImages[i - 1].anchoredPosition.y + 1601;
    //    //    bgImages[i].anchoredPosition = new Vector2(0, posy);
    //    //}
    //}

    private void ChangeLevelBgImages()
    {
        foreach (var item in bgImages)
        {
            if (item != null)
            {
                Sprite sp = item.sprite;
                Image im = item.transform.GetChild(0).GetComponent<Image>();
                im.sprite = sp;
                item.color = new Color(1f, 1f, 1f, 1f);
                im.color = Color.white;
                im.fillAmount = 0;
            }
        }
    }

    private void SetLevelSpeed()
    {
        //Level Speed Data Set
        List<double> speedDatas = LevelBlockSpeedData.blockSpeedDatas;
        print(speedDatas.Count);

        for (int i = 0; i < levelDataInfos.Length; i++)
        {
            var item = levelDataInfos[i];
            if (i < speedDatas.Count && item != null)
            {
                item.levelData.moveSpeed = (float)speedDatas[i];
            }
        }
    }

    //private void OnValidate()
    //{
    //    //SetLevelSpeed();
    //    SetLevelRemoteConfig();
    //    //ChangeLevelBgImages();
    //}

    private void SetLevelRemoteConfig()
    {
        bool isUpdate = true;
        if (File.Exists(savePathJson))
        {
            // Read the JSON data from the file
            string jsonData = File.ReadAllText(savePathJson);
            LevelUpdateDatas = JsonUtility.FromJson<LevelDataList>(jsonData);
            isUpdate = false;
        }

        List<LevelUpdateData> data = new List<LevelUpdateData>();
        foreach (var item in levelDataInfos)
        {
            if (item.levelData != null)
            {
                LevelUpdateData dt = new LevelUpdateData()
                {
                    levelNumber = item.levelData.levelNumber,
                    speed = (int)(item.levelData.moveSpeed * 1000f),
                    moveCount = item.levelData.moveCount,
                    starValue = item.levelData.totalStarValue,
                    abilityValue1 = item.levelData.ability1Value,
                    abilityValue2 = item.levelData.ability2Value
                };
                data.Add(dt);
            }
        }
        if (isUpdate)
        {
            LevelUpdateDatas.data = data;
        }
        else
        {
            for (int i = 0; i < LevelUpdateDatas.data.Count; i++)
            {

                if (i < data.Count)
                {
                    LevelUpdateData dt = LevelUpdateDatas.data[i];
                    LevelUpdateData dt2 = data[i];
                    if (dt.levelNumber == dt2.levelNumber)
                    {
                        if (dt.speed == dt2.speed || dt.moveCount == dt2.moveCount || dt.starValue == dt2.starValue
                            || dt.abilityValue1 == dt2.abilityValue1 || dt.abilityValue2 == dt2.abilityValue2)
                        {
                            LevelUpdateDatas.data[i] = data[i];
                        }
                    }
                }
            }
        }

        string jsonDatasave = JsonUtility.ToJson(LevelUpdateDatas);

        // Write the JSON data to a file
        File.WriteAllText(savePathJson, jsonDatasave);
    }

    //public async void CheckForGameLevelUpdates()
    //{ 
    //    RemoteConfigService.Instance.FetchCompleted += ApplyRemoteConfig;
    //    await RemoteConfigService.Instance.FetchConfigsAsync(new userAttributes(), new appAttributes());
    //}
    //void ApplyRemoteConfig(ConfigResponse response)
    //{
    //    savePath = Application.persistentDataPath + "/LevelDataSaveFile.json";
    //    int versionCode = RemoteConfigService.Instance.appConfig.GetInt("Version_Code");
    //    if(versionCode != gameDataManager.GetSaveResources(8)) 
    //    {
    //        string jsonString = RemoteConfigService.Instance.appConfig.GetString("Level_Config_Data");
    //        LevelUpdateDatas = JsonConvert.DeserializeObject<LevelDataList>(jsonString);
    //        gameDataManager.SetSaveValues(8, versionCode);
    //        string jsonDatasave = JsonUtility.ToJson(LevelUpdateDatas);
    //        File.WriteAllText(savePath, jsonDatasave);
    //    }
    //    else
    //    {
    //        if (File.Exists(savePath))
    //        {
    //            string jsonData = File.ReadAllText(savePath);
    //            LevelUpdateDatas = JsonUtility.FromJson<LevelDataList>(jsonData);
    //        }
    //    }
    //}

    private void SetLevelValues(int index)
    {
        if (index < LevelUpdateDatas.data.Count)
        {
            var item = levelDataInfos[index];
            LevelUpdateData dt = new LevelUpdateData()
            {
                levelNumber = item.levelData.levelNumber,
                speed = (int)(item.levelData.moveSpeed * 1000f),
                moveCount = item.levelData.moveCount,
                starValue = item.levelData.totalStarValue,
                abilityValue1 = item.levelData.ability1Value,
                abilityValue2 = item.levelData.ability2Value
            };
            LevelUpdateData dt2 = LevelUpdateDatas.data[index];
            if (dt.levelNumber == dt2.levelNumber)
            {
                if(dt.speed != dt2.speed)
                {
                    levelDataInfos[index].levelData.moveSpeed = (float)dt2.speed/1000;
                }
                if (dt.moveCount != dt2.moveCount)
                {
                    levelDataInfos[index].levelData.moveCount = dt2.moveCount;
                }
                if (dt.starValue != dt2.starValue)
                {
                    levelDataInfos[index].levelData.totalStarValue = dt2.starValue;
                }
                if (dt.abilityValue1 != dt2.abilityValue1)
                {
                    levelDataInfos[index].levelData.ability1Value = dt2.abilityValue1;
                }
                if (dt.abilityValue2 != dt2.abilityValue2)
                {
                    levelDataInfos[index].levelData.ability2Value = dt2.abilityValue2;
                }
            }
        }

    }

    void OnDestroy()
    {
        //RemoteConfigService.Instance.FetchCompleted -= ApplyRemoteConfig;
    }



    //private float levelShowScrollValue;
    private void Start()
    {
        gameDataManager = BlockManager.Instance.gameDataManager;
        currentLevel = gameDataManager.currentLevel;
        previousLevel = gameDataManager.previousLevel;
        CheckLevelPosY();
        scrollRect.content.anchoredPosition = new Vector3(0, 0, 0);
        for (int i = 0; i < levelHolderRect.childCount; i++)
        {
            // Get the Transform component of the child at index 'i'
            Transform childTransform = levelHolderRect.GetChild(i);

            LevelShow lvshow = childTransform.GetComponent<LevelShow>();
            lvshow.Set_Level_Index(i);
            levelShow.Add(lvshow);

            if (i <= currentLevel + 15 && i > currentLevel - 15)
            {
                SetLevelValues(i);
                SetUpLevelShow(lvshow);
            }
        }
        levelPlayText.text = "Level " + (currentLevel + 1).ToString();
        
    }

    private void CheckLevelPosY()
    {
        setPosition = gameDataManager.GetSaveValues(9);
        int num = PlayerPrefs.GetInt(gameDataManager.LevelPosY, 0);
        if(num != 0 && setPosition != num)
        {
            if (setPosition != 0)
            {
                if (num > setPosition)
                {
                    setPosition = num;
                }
                else
                {
                    PlayerPrefs.SetInt(gameDataManager.LevelPosY, setPosition);
                }
            }
            else
            {
                setPosition = num;
            }
        }
    }


    private void SetUpLevelShow(LevelShow lv)
    {
        int index = lv.Get_Level_Index;
        LevelData levelData = null;
        Level_Star_Data data = null;
        if (levelDataInfos.Length > index)
        {
            levelData = levelDataInfos[index].levelData;
            if (gameDataManager.GetSaveDataListlength(0) > index)
            {
                data = gameDataManager.LevelStarData(index); 
            }
            else
            {
                gameDataManager.AddLevelStarData();
            }
        }
        
        
        bool isUnlock = false;
        int starCount = 0;
        if(data != null)
        {
            isUnlock = data.isUnlock;
            starCount = data.StarCount;
        }
        
        lv.SetUp(isUnlock, starCount, index, levelData);
    }

    public void ScrollToPosition(bool isShow, bool isLevelShowed = true)
    {
        if (isShow)
        {
            //StartCoroutine(ScrollAnimCO(isLevelShowed));
        }
    }

    //IEnumerator ScrollAnimCO(bool isLevelShowed)
    //{
        //if (!isLevelShowed)
        //{
        //    float num = 0;
        //    int maxiter = 350;
        //    float addVAlue = levelShowScrollValue / (float)maxiter;
        //    while (maxiter > 0 && addVAlue > 0f)
        //    {
        //        if (num < levelShowScrollValue)
        //        {
        //            maxiter--;
        //            num += addVAlue;
        //        }
        //        else
        //        {
        //            yield break;
        //        }
        //        //ScrollPos(num);
        //        yield return new WaitForSeconds(0.02f);
        //    }
        //}
        //ScrollPos(levelShowScrollValue);
    //}

    private void SetScrollStatus(bool status)
    {
        if (isScroll)
        {
            setPosition = (int)scrollRect.content.anchoredPosition.y;
            gameDataManager.SetSaveValues(9, setPosition);
            PlayerPrefs.SetInt(gameDataManager.LevelPosY, setPosition);
        }
        SetLevelFadeImage(currentLevel, isScroll);
        isScroll = status;
    }

    public void ScrollPos()
    {
        SetLevelFadeImage(previousLevel);
        levelMetaFill.GetAllMetaFill(currentLevel + 1);
        scrollRect.content.anchoredPosition = new Vector3(0, setPosition, 0);
        if (currentLevel > 5)
        {
            Invoke(nameof(SetScroll), 0.3f);
        }
        
    }

    private void SetLevelFadeImage(int level, bool isDoMove = false)
    {
        if (level < 5)
        {
            level = 5;
            isDoMove = false;
        }
        MoveBgFade(level, isDoMove);
    }
    private void MoveBgFade(int level, bool isDoMove = false)
    {
        if (level < levelShow.Count && levelShow[level] != null)
        {
            levelFadeTrans.SetParent(levelShow[level].transform);
            levelFadeTrans.anchoredPosition = Vector3.zero;
            Vector3 fadepos = new Vector3(0, 200);
            float time = 0f;
            if (isDoMove)
            {
                time = 0.5f;
            }
            levelFadeTrans.DOAnchorPos(fadepos, time);
        }
    }
    private void ChangeLevelFadeParent()
    {
        levelFadeTrans.parent = scrollRect.content.transform;
    }

    public bool MetaFill()
    {
        bool isFillEffect = false;
        if (levelMetaFill.HasMetaFill)
        {
            isFillEffect = true;
            StartCoroutine(DelayForMetaFill());
        }
        return isFillEffect;
    }

    private IEnumerator DelayForMetaFill()
    {
        yield return new WaitForSeconds(0.4f);
        int maxitter = 100;
        while (true)
        {
            if (isScroll && !isTriggered && maxitter > 0)
            {
                maxitter--;
                yield return new WaitForSeconds(0.2f);
            }
            else
            {
                DelayCall();
                yield break;
            }
        }
    }
    private void DelayCall()
    {
        Transform origin = levelShow[0].transform;
        bool isGem = false;
        if (currentLevel > 0)
        {
            origin = levelShow[currentLevel - 1].transform;
            isGem = CheckForGemLevel(currentLevel - 1);

        }
        levelMetaFill.CheckForMetaFill(currentLevel + 1, origin, isGem);
    }

    private bool CheckForGemLevel(int index)
    {
        bool isGem = false;
        TargetData[] target = levelDataInfos[index].levelData.targetData;
        foreach (TargetData targetData in target)
        {
            if(targetData.gemType != Gem_Type.none)
            {
                isGem = true;
                break;
            }
        }
        return isGem;
    }

    private void SetScroll()
    {
        if (!isTriggered)
        {
            ChangeLevelFadeParent();
            isScroll = true;
            Invoke(nameof(DisableScroll), 3f);
        }
    }
    private void DisableScroll()
    {
        SetScrollStatus(false);
        isTriggered = true;
    }

    private void FixedUpdate()
    {
        if (isScroll && !isTriggered)
        {
            scrollRect.verticalNormalizedPosition += scrollSpeed * Time.deltaTime;
        }
    }


    public void PlayLevelShow()
    {
        levelShow[currentLevel].Playlevel();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("LevelSlide") && !isTriggered)
        {
            int level = GetLevelIndex(collision.gameObject);
            if (level + 1 >= currentLevel)
            {
                SetScrollStatus(false);
                isTriggered = true;
            }
        }

        if (collision.gameObject.CompareTag("Finish"))
        {
            int level = GetLevelIndex(collision.gameObject);
            SetLevelData(level);
            levelMetaFill.CheckForLevelSlider(level);
        }
    }


    private int GetLevelIndex(GameObject obj)
    {
        int level = 0;
        if (obj != null)
        {
            LevelShow lvshow = obj.GetComponentInParent<LevelShow>();
            level = lvshow.Get_Level_Index;
        }
        return level;
    }


    private void SetLevelData(int index)
    {
        int lastPos = index - 15;

        for (int i = index; i > lastPos; i--)
        {
            if(i >= 0)
            {
                SetLevelValues(i);
                SetUpLevelShow(levelShow[i]);
            }
        }
    }
}
