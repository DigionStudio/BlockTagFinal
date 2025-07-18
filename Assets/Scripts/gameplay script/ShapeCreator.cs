using System.Collections.Generic;
using UnityEngine;


public class ShapeCreator : MonoBehaviour
{

    public static ShapeCreator Instance;
    public bool isSetUp { get; set; }
    public Transform[] shapePos;
    public GameObject[] shapeSelect;
    public CrushTileCreator tileCreator;
    public GameObject showObject;
    private List<int> shapeIds = new List<int>() {-5,-5,-5,-5};
    private GameObject currentTileHilighter;
    private CrushTileCreator currrentCrushTile;
    private CrushTileCreator[] crushTiles = new CrushTileCreator[4];
    private bool[] SelectedCrush = new bool[4] {false, false, false, false};
    private List<int> shapeCodes = new List<int>();
    private List<int> shapeCodesSelected = new List<int>();
    private bool isNormalAbilityTile;
    private int maxRand = 7;

    public bool CheckSelected(int index)
    {
        bool status = false;
        if (index < 0)
        {
            foreach (bool stats in SelectedCrush)
            {
                if (stats)
                {
                    status = true;
                    break;
                }
            }
        }
        else
        {
            status = SelectedCrush[index];
        }
        return status;
    }
    public void HasCrushTileSelected(int index, bool status)
    {
        SelectedCrush[index] = status;
    }




    public int ShapeCode(int index)
    {
        int shapecode = shapeIds[index];
        return shapecode;
    }
    public int ShapeCode2(int index)
    {
        int shapecode2 = crushTiles[index].ShapeCode2;
        return shapecode2;
    }
    public float RotValue(int index)
    {
        float rot = crushTiles[index].RotateValue;
        return rot;
    }

    public void SetHasTutorial(int index)
    {
        crushTiles[index].HasTutorial = true;
    }

    public void TutorialStatusSet(int index = -1)
    {
        foreach(var tile in crushTiles)
        {
            if(tile != null)
                tile.SetUpTilesTutorials(false);
        }
        if(index >= 0 && index < crushTiles.Length && crushTiles[index] != null)
            crushTiles[index].SetUpTilesTutorials(true);

    }


    public void ResetCrushTileTutorial()
    {
        foreach (var tile in crushTiles)
        {
            if (tile != null)
                tile.MouseDownTutorial();
        }
    }


    private int selectedShapeIndex;
    public int SelectedShapeIndex { get { return selectedShapeIndex; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        //MaskSetUp();
        ShapeSelectReset();
    }
    
    public void SetUpShapeCreator(int gametype, bool isbomb, bool istag, ShapeData[] shapecodes)
    {
        shapeCodes = new List<int>();
        shapeCodesSelected = new List<int>();
        bool isTagAbility = istag;
        bool isBombAbility = isbomb;
        int shapecodelength = 0;
        if(shapecodes != null)
        {
            shapecodelength = shapecodes.Length;
        }
        if (shapecodelength > 0 && gametype == 1)
        {
            for (int i = 0; i < shapecodelength; i++)
            {
                int code = shapecodes[i].code;
                shapeCodes.Add(code);
            }
        }
        else
        {
            int num = 12;
            for (int i = 0; i < num; i++)
            {
                shapeCodes.Add(i);
            }
        }
        isNormalAbilityTile = false;
        if (gametype == 1 && (isTagAbility || isBombAbility))
        {
            if (isBombAbility)
            {
                shapeCodes.Add(12);
            }
            if (isTagAbility)
            {
                shapeCodes.Add(13);
            }
        }
        else
        {
            if (!TutorialManager.Instance.isTutorial)
            {
                shapeCodes.Add(12);
                shapeCodes.Add(13);
                isNormalAbilityTile = true;
            }
        }
        if (shapeCodes != null && shapeCodes.Count > 0)
        {
            PutShapeIntoSelected();
        }
        if(gametype == 0)
        {
            maxRand = 2;
        }
    }

    public void SetUp()
    {
        showObject.SetActive(true);
        bool status = TutorialManager.Instance.isTutorial;
        for (int i = 0; i < shapePos.Length; i++)
        {
            MakeCrushTile(i, status);
        }
    }

    public void CreateTile(int tileIndex)
    {
        MakeCrushTile(tileIndex);
    }
    private void MakeCrushTile(int tileIndex, bool isStart = false)
    {
        int value = GetShapeCode();
        int maxitter = 100;
        while (maxitter > 0)
        {
            if ((shapeIds.Contains(value) || CheckAbility(value)))
            {
                value = GetShapeCode();
                maxitter--;
            }
            else
            {
                shapeIds[tileIndex] = value;
                break;
            }
        }
        InstaCrushTile(tileIndex, value, isStart);
    }

    private bool CheckAbility(int index)
    {
        bool isAbility = false;
        if(TutorialManager.Instance.isTutorial && (index == 11 || index == 12 ||  index == 13 || index == 1 || index == 0)) 
        {
            isAbility = true;
        }
        else
        {
            if (isNormalAbilityTile && index >= 12)
            {
                int num = Random.Range(0, maxRand);
                if (num != 1)
                {
                    isAbility = true;
                }
            }
            else
            {
                if (shapeIds.Contains(index))
                {
                    isAbility = true;
                }
            }
        }

        return isAbility;
    }

    private void PutShapeIntoSelected()
    {
        for (int i = 0; i < shapeCodes.Count; i++)
        {
            shapeCodesSelected.Add(shapeCodes[i]);
        }
    }

    private int GetShapeCode()
    {
        if(shapeCodesSelected.Count == 0)
        {
            PutShapeIntoSelected();
            foreach (int id in shapeIds)
            {
                shapeCodesSelected.Remove(id);
            }
        }
        int shapecode = -1;
        int index = Random.Range(0, shapeCodesSelected.Count);
        int value = shapeCodesSelected[index];
        int maxitter = 50;
        while (maxitter > 0)
        {
            maxitter--;
            if (!shapeCodesSelected.Contains(value))
            {
                index = Random.Range(0, shapeCodesSelected.Count);
                value = shapeCodesSelected[index];
            }
            else
            {
                shapeCodesSelected.RemoveAt(index);
                shapecode = value;
                break;
            }
        }
        return shapecode;
    }

    public void ChangerToAiBomb(int tileindex, int shapeCode)
    {
        shapeCodesSelected.Add(shapeCode);
        if (shapeCodesSelected.Contains(shapeCode))
        {
            shapeCodesSelected.Remove(shapeCode);
        }
        shapeIds[tileindex] = 12;
    }

    private int CrushTileType(int shapeCode)
    {
        int num = 0;
        if (shapeCode == 0)
        {
            num = Random.Range(0, 2);
        }
        else if (shapeCode == 2 || shapeCode == 3)
        {
            num = Random.Range(0, 4);
        }
        else if (shapeCode == 4)
        {
            num = Random.Range(0, 4);
        }
        else if (shapeCode == 5)
        {
            num = Random.Range(0, 2);
        }
        else if (shapeCode == 6 || shapeCode == 7)
        {
            num = Random.Range(0, 2);

        }
        else if (shapeCode == 8)
        {
            num = Random.Range(0, 4);
        }
        else if (shapeCode == 9 || shapeCode == 10)
        {
            num = Random.Range(0, 2);
        }
        else if (shapeCode == 12)
        {
            num = Random.Range(3, 5);
            if(isNormalAbilityTile && num == 3)
            {
                num = 4;
            }
        }
        else if (shapeCode == 13)
        {
            num = Random.Range(1, 3);
            if (isNormalAbilityTile && num == 1)
            {
                num = 2;
            }
        }
        return num;
    }
    private void InstaCrushTile(int tileIndex, int shapecode, bool isStart = false)
    {
        if (isSetUp)
        {
            CrushTileCreator tilecreator = Instantiate(tileCreator, transform.position, Quaternion.identity);
            crushTiles[tileIndex] = tilecreator;
            Vector2 pos = shapePos[tileIndex].position;
            int num;
            bool isModBomb = false;
            if (GameAIManager.Instance.HasBomb(pos))
            {
                ChangerToAiBomb(tileIndex, shapecode);
                shapecode = 12;
                num = 3;
                isModBomb = true;
            }
            else
            {
                num = CrushTileType(shapecode);
            }
            tilecreator.SetUp_Shape(shapecode, tileIndex, num, isModBomb);
            tilecreator.SetPosistion(pos);
            tilecreator.InstaSize(isModBomb);
            if (!isStart)
            {
                SetCrushTile(tileIndex);
                
            }else
            {
                if ((tileIndex == 1))
                    SetCrushTile(tileIndex);
            }
        }
    }

    public void SetCrushTile(int thisTileIndex)
    {
        if(currentTileHilighter != null)
        {
            currentTileHilighter.SetActive(false);
        }
        currentTileHilighter = shapeSelect[thisTileIndex];
        currentTileHilighter.SetActive(true);
        currrentCrushTile = crushTiles[thisTileIndex];
        selectedShapeIndex = thisTileIndex;
    }


    public void RotateCurrentCrushTile()
    {
        if(currrentCrushTile != null)
        {
            currrentCrushTile.RotateObj();
            TutorialManager.Instance.ButtonPressStart(2);
            TutorialManager.Instance.RotateTile();
        }
    }

    public void ResetCrushTiles()
    {
        GameAIManager.Instance.ReSetModBomb();
        GameReset(true);
        TutorialManager.Instance.EndTutorial();
    }
    public void GameReset(bool isSetUp)
    {
        ShapeSelectReset();
        for (int i = 0; i < 4; i++)
        {
            CrushTileCreator tile = crushTiles[i];
            if (tile != null)
            {
                Destroy(tile.gameObject);
                if (isSetUp)
                {
                    MakeCrushTile(i);
                }
                else
                {
                    showObject.SetActive(false);
                }
            }
                
        }
        
    }

    public void UsedCrushTile(int index)
    {
        if (crushTiles[index] != null)
        {
            crushTiles[index] = null;
        }
    }


    private void ShapeSelectReset()
    {
        foreach(GameObject tile in shapeSelect)
        {
            tile.SetActive(false);
        }
    }
}
