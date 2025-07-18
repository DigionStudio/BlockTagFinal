using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Level_Info", menuName = "Level_Stats")]
public class LevelDataInfo : ScriptableObject
{
    public LevelData levelData;
    //private void OnValidate()
    //{
    //    string name = "";
    //    string currentName = this.name;
    //    if(levelData != null && currentName.Length < 10)
    //    {
    //        if (levelData.isScoreTarget)
    //        {
    //            name = currentName + " " + "(S-" + levelData.totalStarValue.ToString() + "-" + levelData.moveCount.ToString() + ")";
    //        }
    //        else
    //        {
    //            name = currentName + " " + TargetCode();
    //        }
    //        this.name = name;
    //    }
    //}


    //private string TargetCode()
    //{
    //    string code = "";
    //    string codefirst = "";
    //    string codelast = "";
    //    int count = levelData.targetData.Length;
    //    if (levelData != null && count > 0)
    //    {
    //        for (int i = 1; i < count; i++)
    //        {
    //            codefirst += "(";
    //            codelast += ")";
    //        }
    //        for (int i = 0; i < count; i++)
    //        {
    //            codefirst += (TargetElementCode(i));
    //        }
    //        code = codefirst + "-" + levelData.moveCount.ToString() + codelast;

    //    }
    //    return code;
    //}

    //private string TargetElementCode(int index)
    //{
    //    string code = string.Empty;
    //    TargetData data = levelData.targetData[index];

    //    int blockType = (int)data.normalBlockType;
    //    string blocktype = data.normalBlockType.ToString();
    //    int abilityType = (int)data.blockType;
    //    string abilitytype = data.blockType.ToString();


    //    if (blockType >= 0 && blockType < 5)
    //    {
    //        if (abilityType != 6)
    //        {
    //            code += "N";
    //        }
    //        code += blocktype[6];
    //    }
    //    else
    //    {
    //        if (blockType == 6)
    //        {
    //            code += "IN";
    //        }
    //        else if ((blockType >= 7 && blockType < 9) || blockType == 11)
    //        {
    //            code += blocktype[0];
    //        }
    //        else if (blockType == 9)
    //        {
    //            code += "RK";
    //        }
    //        else if (blockType == 10)
    //        {
    //            code += "WA";
    //        }
    //    }
        
    //    if ((abilityType >= 1 && abilityType < 3) || abilityType == 4)
    //    {
    //        code += abilitytype[0];
    //    }else if(abilityType == 3)
    //    {
    //        code += "RC";
    //    }
    //    else if (abilityType == 5)
    //    {
    //        code += "CR";
    //    }

    //    code += "-" + data.count.ToString();


    //    return "(" + code + ")";
    //}

}
