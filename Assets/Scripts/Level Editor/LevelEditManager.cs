using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class LevelEditManager : MonoBehaviour
{
    public LevelDataInfo[] levelDataInfos;
    private int currentLevelIndex;


    public LevelData thisLevelDataInfo;

    public Toggle isScoreToggle;
    public InputField targetCount;
    public Toggle isTagToggle;
    public Sprite[] obsSprites;
    public bgButton[] bgButtons;
    public Button removeObsButton;
    private int bgtileIndex;
    private ObstacleTile currentObsTile;
    public ObstacleTile[] obsSelectButtons = new ObstacleTile[6];
    public LevelTargetShow[] levelTargetHolders = new LevelTargetShow[3];
    public InputField levelNumber;
    public InputField moveCount;
    public InputField totalStar;
    public InputField refreshCount;
    public InputField ability1;
    public InputField ability2;

    public Button prevButton;
    public Button resetButton;
    public Button saveButton;
    public Button nextButton;

    private int levelOffset;

    void Start()
    {
        levelOffset = levelDataInfos[0].levelData.levelNumber;
        removeObsButton.onClick.AddListener(RemoveAllObs);



        isScoreToggle.onValueChanged.AddListener(HasScoreTargetChange);
        levelNumber.onValueChanged.AddListener(OnLevelNumberChange);

        moveCount.onValueChanged.AddListener(OnMoveCountChange);
        totalStar.onValueChanged.AddListener(OnTotalStarChange);
        refreshCount.onValueChanged.AddListener(OnRefreshChange);
        ability1.onValueChanged.AddListener(OnAbility1Change);
        ability2.onValueChanged.AddListener(OnAbility2Change);


        prevButton.onClick.AddListener(PrevButton);
        resetButton.onClick.AddListener(ResetButton);
        saveButton.onClick.AddListener(SaveButton);
        nextButton.onClick.AddListener(NextButton);

        for (int i = 0; i < 99; i++)
        {
            bgButtons[i].SetUp(i, false, this);
        }
        for (int i = 0; i < obsSelectButtons.Length; i++)
        {
            ObstacleTile item = obsSelectButtons[i];
            if (item != null)
                item.SetUp(i, this, obsSprites[i]);

            if(i == bgtileIndex)
            {
                item.SelectionStatus(true);
            }
            currentObsTile = item;
        }

        SetThisLevelDataInfo(levelDataInfos[currentLevelIndex]);
        targetCount.onValueChanged.AddListener(OnTargetCountChange);
    }

    private void OnTargetCountChange(string value)
    {
        int num = 0;
        if (thisLevelDataInfo.targetData != null)
            num = thisLevelDataInfo.targetData.Length;
        if (!string.IsNullOrEmpty(value) && num > 0)
        {
            int count = int.Parse(value);
            List<TargetData> targetdata = new List<TargetData>();
            if (count != num)
            {
                foreach (var item in thisLevelDataInfo.targetData)
                {
                    targetdata.Add(item);
                }
                thisLevelDataInfo.targetData = new TargetData[count];

                for (int i = 0; i < thisLevelDataInfo.targetData.Length; i++)
                {
                    if (i < targetdata.Count)
                    {
                        thisLevelDataInfo.targetData[i] = targetdata[i];
                    }
                }
                TargetUISetUpCo(count);
            }
            
        }
    }
    private void TargetUISetUpCo(int count)
    {
        for (int i = 0; i < 3; i++)
        {
            LevelTargetShow leveltargetHolder = levelTargetHolders[i];
            if (i < count)
            {
                leveltargetHolder.gameObject.SetActive(true);
                leveltargetHolder.SetUp(i, thisLevelDataInfo, this);
            }
            else
            {
                leveltargetHolder.gameObject.SetActive(false);
            }
        }
    }
    private void OnLevelNumberChange(string value)
    {
        int count = int.Parse(value);
        int index = count - levelOffset;
        if (index < 0)
        {
            index = 0;
        }
        if(index != currentLevelIndex)
        {
            currentLevelIndex = index;
            ResetButton();
        }
    }

    public void ChangeObsTile(int tileindex)
    {
        currentObsTile.SelectionStatus(false);
        bgtileIndex = tileindex;
        currentObsTile = obsSelectButtons[bgtileIndex];
        currentObsTile.SelectionStatus(true);
    }
    private void HasScoreTargetChange(bool isOn)
    {
        thisLevelDataInfo.isScoreTarget = isOn;
        if (isOn)
        {
            targetCount.text = "0";
            TargetUISetUpCo(0);
        }
        else
        {
            ResetButton();
        }
    }

   
    public void TargetDataChange(int index, int targetindex, int value)
    {
        if(targetindex == 0)
        {
            thisLevelDataInfo.targetData[index].normalBlockType = (Normal_Block_Type)value;
        }else if(targetindex == 1)
        {
            thisLevelDataInfo.targetData[index].blockType = (BlockType)value;
        }else if (targetindex == 2)
        {
            thisLevelDataInfo.targetData[index].specialObject = (Special_Object_Type)value;
        }
        else
        {
            thisLevelDataInfo.targetData[index].count = value;
        }
    }

    private void OnMoveCountChange(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            int count = int.Parse(value);
            if (count > 0)
            {
                thisLevelDataInfo.moveCount = count;
            }
        }
    }

    private void OnTotalStarChange(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            int count = int.Parse(value);
            if (count > 0)
            {
                thisLevelDataInfo.totalStarValue = count;
            }
        }
    }

    private void OnRefreshChange(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            int count = int.Parse(value);
            if (count > 0)
            {
                thisLevelDataInfo.reFreshCount = count;
            }
        }
    }

    private void OnAbility1Change(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            int count = int.Parse(value);
            if (count > 0)
            {
                thisLevelDataInfo.ability1Value = count;
            }
        }
    }
    private void OnAbility2Change(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            int count = int.Parse(value);
            if (count > 0)
            {
                thisLevelDataInfo.ability2Value = count;
            }
        }
    }

    private void PrevButton()
    {
        if (currentLevelIndex > 0)
        {
            currentLevelIndex--;
            ResetButton();
        }
    }

    private void NextButton()
    {
        if (currentLevelIndex + 1 < levelDataInfos.Length)
        {
            currentLevelIndex++;
            ResetButton();
        }
    }

    private void SaveButton()
    {
        if (levelDataInfos.Length > currentLevelIndex)
        {
            LevelData info = new LevelData();
            info = thisLevelDataInfo;
            levelDataInfos[currentLevelIndex].levelData = info;
        }
    }

    private void ResetButton()
    {
        if (levelDataInfos.Length > currentLevelIndex) 
        {
            SetThisLevelDataInfo(levelDataInfos[currentLevelIndex]);
        }
    }


    private void SetThisLevelDataInfo(LevelDataInfo data)
    {
        //AssetDatabase.OpenAsset(data);
        LevelData info = new LevelData();
        info.isScoreTarget = data.levelData.isScoreTarget;
        info.isTagAbility = data.levelData.isTagAbility;

        int shapeLength = 0;
        if(data.levelData.shapeCodes != null)
            shapeLength = data.levelData.shapeCodes.Length;
        if (shapeLength > 0) {
            info.shapeCodes = new ShapeData[shapeLength];
            for (int i = 0; i < shapeLength; i++)
            {
                info.shapeCodes[i].code = data.levelData.shapeCodes[i].code;
                info.shapeCodes[i].isSelectable = data.levelData.shapeCodes[i].isSelectable;
            }
        }

        int targetLength = 0;
        if(data.levelData.targetData != null)
            targetLength = data.levelData.targetData.Length;
        if (targetLength > 0)
        {
            info.targetData = new TargetData[targetLength];
            for (int i = 0; i < targetLength; i++)
            {
                TargetData targetdata = new TargetData();
                targetdata.normalBlockType = data.levelData.targetData[i].normalBlockType;
                targetdata.blockType = data.levelData.targetData[i].blockType;
                targetdata.specialObject = data.levelData.targetData[i].specialObject;
                targetdata.count = data.levelData.targetData[i].count;
                info.targetData[i] = targetdata;

            }
        }

        int bgTileLength = 0;
        if(data.levelData.bgTileData != null)
            bgTileLength = data.levelData.bgTileData.Length;
        if (bgTileLength > 0)
        {
            info.bgTileData = new BgTileData[bgTileLength];
            for (int i = 0; i < bgTileLength; i++)
            {
                BgTileData bgdata = new BgTileData();
                bgdata.positionIndex = data.levelData.bgTileData[i].positionIndex;
                bgdata.tileType = data.levelData.bgTileData[i].tileType;
                info.bgTileData[i] = bgdata;
            }
        }

        info.moveSpeed = data.levelData.moveSpeed;
        info.moveCount = data.levelData.moveCount;
        info.totalStarValue = data.levelData.totalStarValue;
        info.reFreshCount = data.levelData.reFreshCount;
        info.ability1Value = data.levelData.ability1Value;
        info.ability2Value = data.levelData.ability2Value;
        info.levelNumber = data.levelData.levelNumber;
        thisLevelDataInfo = info;


        isScoreToggle.isOn = thisLevelDataInfo.isScoreTarget;
        isTagToggle.isOn = thisLevelDataInfo.isTagAbility;
        targetCount.text = targetLength.ToString();

        levelNumber.text = thisLevelDataInfo.levelNumber.ToString();

        moveCount.text = thisLevelDataInfo.moveCount.ToString();
        totalStar.text = thisLevelDataInfo.totalStarValue.ToString();
        refreshCount.text = thisLevelDataInfo.reFreshCount.ToString();
        ability1.text = thisLevelDataInfo.ability1Value.ToString();
        ability2.text = thisLevelDataInfo.ability2Value.ToString();
        TargetUISetUpCo(targetLength);

        BgTileReset();
        if (bgTileLength > 0)
            SetUpObstacleBoard();
    }
    private void BgTileReset()
    {
        foreach (var item in obsSelectButtons)
        {
            if (item != null)
                item.SetTileCount(true);
        }
        foreach (var item in bgButtons)
        {
            if(item != null)
                item.SetUpSprite(obsSprites[0], false, 0);
        }
    }


    private void SetUpObstacleBoard()
    {
        foreach (var data in thisLevelDataInfo.bgTileData)
        {
            int positionIndex = data.positionIndex;
            int tileType = (int)data.tileType - 5;
            if(tileType > 0 && obsSelectButtons[tileType] != null)
            {
                obsSelectButtons[tileType].SetTileCount(false);
            }
            foreach (var item in bgButtons)
            {
                if(item != null && item.ThisIndex == positionIndex && tileType >= 0)
                {
                    item.SetUpSprite(obsSprites[tileType], true, tileType);
                    break;
                }
            }
        }
        
    }

    private void RemoveAllObs()
    {
        BgTileReset();
        thisLevelDataInfo.bgTileData = new BgTileData[0];
    }

    public void CheckForBgTile(int index,int tileindex, bool status)
    {
        List<int> indexList = new List<int>();
        if(thisLevelDataInfo.bgTileData != null && thisLevelDataInfo.bgTileData.Length > 0)
        {
            foreach(var item in thisLevelDataInfo.bgTileData)
            {
                int num = item.positionIndex;
                indexList.Add(num);
            }
            int code = bgtileIndex + 5;
            Normal_Block_Type type = (Normal_Block_Type)code;

            if (!indexList.Contains(index))
            {
                indexList.Add(index);
                int count = indexList.Count;
                List<BgTileData> bgtiledata = new List<BgTileData>();
                foreach (var item in thisLevelDataInfo.bgTileData)
                {
                    bgtiledata.Add(item);
                }

                BgTileData bgdata = new BgTileData();
                bgdata.positionIndex = index;
                
                bgdata.tileType = type;
                bgtiledata.Add(bgdata);


                thisLevelDataInfo.bgTileData = new BgTileData[count];
                for (int i = 0; i < count; i++)
                {
                    if (i < bgtiledata.Count)
                    {
                        thisLevelDataInfo.bgTileData[i] = bgtiledata[i];
                    }
                }
            }
            else
            {
                if(bgtileIndex != tileindex && bgtileIndex != 0)
                {
                    for (int i = 0; i < thisLevelDataInfo.bgTileData.Length; i++)
                    {
                        int val = thisLevelDataInfo.bgTileData[i].positionIndex;
                        if (val == index)
                        {
                            thisLevelDataInfo.bgTileData[i].tileType = type;
                            break;
                        }
                    }
                }
                else
                {
                    if (status && bgtileIndex == 0)
                    {
                        List<BgTileData> bgtiledata2 = new List<BgTileData>();
                        foreach (var item in thisLevelDataInfo.bgTileData)
                        {
                            int val = item.positionIndex;
                            if (val != index)
                            {
                                bgtiledata2.Add(item);
                            }
                        }
                        int count2 = bgtiledata2.Count;
                        thisLevelDataInfo.bgTileData = new BgTileData[count2];
                        for (int i = 0; i < count2; i++)
                        {
                            if (i < bgtiledata2.Count)
                            {
                                thisLevelDataInfo.bgTileData[i] = bgtiledata2[i];
                            }
                        }
                    }
                }
            }
        }
        else
        {
            AddBgTileData(index);
        }

        BgTileReset();
        SetUpObstacleBoard();
    }

    private void AddBgTileData(int index)
    {
        thisLevelDataInfo.bgTileData = new BgTileData[1];
        BgTileData bgdata = new BgTileData();
        bgdata.positionIndex = index;
        bgdata.tileType = (Normal_Block_Type)bgtileIndex;
        thisLevelDataInfo.bgTileData[0] = bgdata;
    }
}
