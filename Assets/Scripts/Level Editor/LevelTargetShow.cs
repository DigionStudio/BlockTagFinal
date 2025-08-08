using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTargetShow : MonoBehaviour
{
    public Dropdown normalBlock;
    public Dropdown blockType;
    public Dropdown gemType;
    public InputField valueCount;
    private int thisIndex;
    private LevelEditManager levelEditManager;
    void Start()
    {
        normalBlock.onValueChanged.AddListener(OnNormalTypeChange);
        blockType.onValueChanged.AddListener(OnBlockTypeChange);
        gemType.onValueChanged.AddListener(OnGemTypeChange);
        valueCount.onValueChanged.AddListener(OnCountChange);
        List<Dropdown.OptionData> normalType = new List<Dropdown.OptionData>();
        for (int i = 0; i < 12; i++)
        {
            string type = ((Normal_Block_Type)i).ToString();
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = type;
            normalType.Add(data);
        }
        normalBlock.AddOptions(normalType);

        List<Dropdown.OptionData> blocktype = new List<Dropdown.OptionData>();
        for (int i = 0; i < 7; i++)
        {
            string type = ((BlockType)i).ToString();
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = type;
            blocktype.Add(data);
        }
        blockType.AddOptions(blocktype);

        List<Dropdown.OptionData> gemtype = new List<Dropdown.OptionData>();
        for (int i = 0; i < 6; i++)
        {
            string type = ((Special_Object_Type)i).ToString();
            Dropdown.OptionData data = new Dropdown.OptionData();
            data.text = type;
            gemtype.Add(data);
        }
        gemType.AddOptions(gemtype);
    }

    public void SetUp(int index, LevelData thisLevelDataInfo, LevelEditManager manager)
    {
        thisIndex = index;
        levelEditManager = manager;
        SetUpTargetDropDowns(index, thisLevelDataInfo);
    }

    private void SetUpTargetDropDowns(int targetindex, LevelData thisLevelDataInfo)
    {
        if (thisLevelDataInfo.targetData[targetindex] != null)
        {
            normalBlock.value = (int)thisLevelDataInfo.targetData[targetindex].normalBlockType;
            blockType.value = (int)thisLevelDataInfo.targetData[targetindex].blockType;
            gemType.value = (int)thisLevelDataInfo.targetData[targetindex].specialObject;
            valueCount.text = thisLevelDataInfo.targetData[targetindex].count.ToString();
        }
    }


    private void OnNormalTypeChange(int value)
    {
        levelEditManager.TargetDataChange(thisIndex, 0, value);
    }

    private void OnBlockTypeChange(int value)
    {
        levelEditManager.TargetDataChange(thisIndex, 1, value);

    }

    private void OnGemTypeChange(int value)
    {
        levelEditManager.TargetDataChange(thisIndex, 2, value);

    }
    private void OnCountChange(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            int count = int.Parse(value);
            if(count > 0)
            {
                levelEditManager.TargetDataChange(thisIndex, 3, count);
            }
        }
    }
}
