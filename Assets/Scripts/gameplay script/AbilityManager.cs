using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[Serializable]
public class AbilityData
{
    public VariableTypeCode thisType = VariableTypeCode.None;
    public Sprite iconSprite;
    public string name;
    public string working;
    public int value;
    public int count;
    public int unLockvalue;
}

public enum VariableTypeCode
{
    None, //0
    Freeze, //1
    Bomb,   //2
    Hammer, //3
    Thunder,//4
    Ability_1,//5
    Magic_Wand,//6
    Coin,       //7
    Life,       //8
    Lucky_Wheel
}

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;
    public AbilityData[] abilityData;
    [SerializeField] private AbilityShowUI[] abilityShowUI;
    [SerializeField] private LineController lineController;

    [SerializeField] private AbilityObject abilityObjectPrefab;
    [SerializeField] private AbilityUnlicked abilityUnlocked;

    [SerializeField] private LayerMask layerMaskBlock;
    [SerializeField] private LayerMask layerMaskObs;
    [SerializeField] private GameObject popupPnael;
    [SerializeField] private Text popUpText;
    private bool isPopupActive;
    private PointerEventData pointerData = new PointerEventData(EventSystem.current);
    private List<RaycastResult> raycastResults = new List<RaycastResult>();

    private AbilityObject currentAbilityObject;
    private bool isAbilityinUse;
    public bool HasAbilityUse { get { return isAbilityinUse; } }

    private List<Game_Value_Status> abilityCount = new List<Game_Value_Status>();
    private int currentLevel;
    private int prevLevel;
    private GameDataManager gameDataManager;

    private bool abilityActive;
    private VariableTypeCode curentSelectedAbilityType = VariableTypeCode.None;
    private BoardManager boardManager;

    private static readonly string AbilityTutorialPref = "AbilityTutorialPref";
    private bool isAbilityTutorial;
    private int tuteVal;

    private readonly string UnlockCode = "UnlockCode";
    private int unLockCode = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        
    }
    void Start()
    {
        //PlayerPrefs.SetInt(AbilityTutorialPref, 0);
        unLockCode = PlayerPrefs.GetInt(UnlockCode);
        unLockCode -= 1;
        tuteVal = PlayerPrefs.GetInt(AbilityTutorialPref, 0);
        HasAbilityObject();

        if (tuteVal >= 0 && tuteVal < 3)
        {
            isAbilityTutorial = true;

        }
        else
        {
            isAbilityTutorial = false;
        }
        boardManager = BoardManager.Instance;
        gameDataManager = boardManager.gameDataManager;
        currentLevel = gameDataManager.currentLevel;
        prevLevel = gameDataManager.previousLevel;
        abilityCount = gameDataManager.AbilityData();
        for (int i = 0; i < abilityShowUI.Length; i++)
        {
            bool status = CheckAbilityLock(abilityData[i].unLockvalue);
            abilityShowUI[i].SetUp(abilityData[i], abilityCount[i], (VariableTypeCode)(i + 1), status);
        }
        CheckForAbilityUnlocked();
    }

    private void CheckForAbilityUnlocked()
    {
        for (int i = 0; i < abilityShowUI.Length; i++)
        {
            int unlkockedLevel = abilityData[i].unLockvalue - 1;
            if (prevLevel < unlkockedLevel && currentLevel == unlkockedLevel && unLockCode < i)
            {
                unLockCode = i + 1;
                PlayerPrefs.SetInt(UnlockCode, unLockCode);
                abilityUnlocked.AbilityUnlocked(abilityShowUI[i].transform, abilityData[i].iconSprite, abilityData[i].name, abilityData[i].working);
                break;
            }
        }
    }

    private bool CheckAbilityLock(int num)
    {
        bool isLock = true;
        if(num - 1 <= currentLevel)
        {
            isLock = false;
        }
        return isLock;

    }

    public void AbilityUpdate(VariableTypeCode type, bool isAdd, int count)
    {
        int index = (int)type - 1;
        abilityCount[index].value = gameDataManager.GameAbilitySave(index, isAdd, count);
        if (index < abilityShowUI.Length)
        {
            abilityShowUI[index].AbilityUpdated(abilityCount[index]);
        }
    }

    private void AbilityShowUIStatus(bool status)
    {
        int index = (int)curentSelectedAbilityType - 1;
        if (index >= 0 && index < abilityShowUI.Length)
        {
            abilityShowUI[index].AbilityEnabled(status);

        }
    }

    public void AbilityStatus(bool isActive, VariableTypeCode type)
    {
        if (!isAbilityinUse) {
            boardManager.GameStatus(!isActive);
            abilityActive = isActive;
            if(curentSelectedAbilityType != VariableTypeCode.None)
            {
                if (curentSelectedAbilityType != type)
                {
                    AbilityShowUIStatus(false);
                }
            }

            curentSelectedAbilityType = type;
            AbilityShowUIStatus(abilityActive);




            if (abilityActive)
            {
                HasAbilityObject();
                if (curentSelectedAbilityType == VariableTypeCode.Hammer)
                {
                    Vector2 pos = abilityShowUI[2].transform.position;
                    currentAbilityObject.SetUp(pos);
                    CheckForAbilityTutorial();
                }
                else if (curentSelectedAbilityType == VariableTypeCode.Bomb)
                {
                    Vector2 pos = abilityShowUI[1].transform.position;
                    currentAbilityObject.SetUp(pos);
                    CheckForAbilityTutorial();
                }
                else
                {
                    ActivateAbility();
                }
                
            
            }
            else
            {
                TuteDisable();
                DestroyAbilityObject();
            }
        }
        else
        {
            TuteDisable();
            int index = (int)type - 1;
            if (index >= 0 && index < abilityShowUI.Length)
            {
                abilityShowUI[index].UnableToUse();
                if (curentSelectedAbilityType != type)
                {
                    abilityShowUI[index].DisableAbilityUI(false, true);
                }
            }
        }
    }

    private void TuteDisable()
    {
        if (isAbilityTutorial)
        {
            isAbilityTutorial = false;
            TutorialManager.Instance.AbilityTutorial(false, Vector2.zero);
            tuteVal++;
            PlayerPrefs.SetInt(AbilityTutorialPref, tuteVal);
        }
    }

    private void CheckForAbilityTutorial()
    {
        if (isAbilityTutorial)
        {
            Vector2 pos = boardManager.AbilityTutorialPos();
            TutorialManager.Instance.AbilityTutorial(isAbilityTutorial, pos);
            Invoke(nameof(TuteDisable), 7f);
        }
        
    }

    private bool CheckAbilityActive(Vector2 pos)
    {
        popUpText.text = "You Didn't Select Ability to Use";
        Vector2 currentPos = pos + Vector2.up * 1.5f;
        bool isactive = false;
        bool isInvoke = true;
        if (CheckForObs(pos, layerMaskObs))
        {
            if (abilityActive)
            {
                popUpText.text = "Unable to Use on the block";
            }

        }
        else
        {
            if (CheckForObs(pos, layerMaskBlock))
            {
                if (abilityActive)
                {
                    isactive = true;
                }
            }
            else
            {
                isInvoke = false;
            }
            

        }
        if (!isactive && !isPopupActive && isInvoke)
        {
            popupPnael.transform.position = currentPos;
            float time = 0.2f;
            if (!boardManager.isGameStarted)
                time = 0f;
            Invoke(nameof(PopUp), time);
        }
        return isactive;
    }

    private void PopUp()
    {
        if (boardManager.isGameStarted || abilityActive)
        {
            PopUpStatus(true);
        }
    }
    private void PopUpStatus(bool status)
    {
        isPopupActive = status;
        float val = 1f;
        if (isPopupActive)
        {
            popupPnael.SetActive(true);
            Invoke(nameof(DisablePopUp), 2f);
        }
        else
        {
            val = 0;
        }
        popupPnael.transform.DOScale(val, 0.5f).OnComplete(() =>
        {
            popupPnael.SetActive(isPopupActive);
        });
    }

    private void DisablePopUp()
    {
        PopUpStatus(false);
    }
    
    

    private bool CheckForObs(Vector2 origin, LayerMask mask)
    {
        RaycastHit2D obs = Physics2D.Raycast(origin, Vector2.zero,
                                          float.PositiveInfinity, mask);
        bool isObs = false;
        if (obs)
        {
            isObs = true;
        }
        return isObs;
    }


    private void ActivateAbility()
    {
        if (abilityActive)
        {
            float value = 1f;
            isAbilityinUse = true;

            if (curentSelectedAbilityType != VariableTypeCode.None)
            {
                if (curentSelectedAbilityType == VariableTypeCode.Freeze)
                {
                    if (boardManager.HasFreezed)
                    {
                        abilityShowUI[(int)VariableTypeCode.Freeze - 1].UnableToUse();
                        isAbilityinUse = false;
                    }
                    else
                    {
                        value = 2.5f;
                    }
                }
                else
                {
                    value = 3f;
                }
               
                if (isAbilityinUse)
                {
                    DestroyAbilityObject();
                    AbilityUpdate(curentSelectedAbilityType, false, 1);
                }
                Invoke(nameof(DisableAbility), value);

            }
        }
    }
    private void FreezeAbility()
    {
        boardManager.FreezeAbilityActive();
    }

    private IEnumerator ThunderStrike()
    {
        List<BlockTile> thunderBlocks = boardManager.ThunderAbility(false);
        List<GameObject> thunderObjects = new List<GameObject>();
        float time = (0.8f / (float)thunderBlocks.Count);
        yield return new WaitForSeconds(0.7f);

        int num = 0;
        for (int i = 0; i < thunderBlocks.Count; i++)
        {
            BlockTile tile = thunderBlocks[i];
            if (tile != null && !tile.HasBlockSelected && currentAbilityObject != null)
            {
                LineController line = Instantiate(lineController, transform);
                line.SetLinePoints(currentAbilityObject.transform, tile.transform.position);
                thunderObjects.Add(line.gameObject);
                num++;
                yield return new WaitForSeconds(time);
            }
        }
        boardManager.UpdatePoint(num);
        for (int i = 0; i < thunderBlocks.Count; i++)
        {
            BlockTile tile = thunderBlocks[i];
            if (tile != null && !tile.HasBlockSelected)
            {
                tile.DestroyObject();
            }
            if(thunderObjects.Count > i && thunderObjects[i] != null)
            {
                Destroy(thunderObjects[i]);
            }
        }
        boardManager.CheckForDash(1.5f);

    }

    private void DestroyAbilityObject()
    {
        if(HasAbilityObject())
        {
            if (abilityActive)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (curentSelectedAbilityType == VariableTypeCode.Hammer)
                {
                    currentAbilityObject.HammerAbilityActive(mousePos);
                }
                else if (curentSelectedAbilityType == VariableTypeCode.Bomb)
                {
                    currentAbilityObject.BombAbilityActive(mousePos);
                }else if(curentSelectedAbilityType == VariableTypeCode.Thunder)
                {
                    int num = (int)VariableTypeCode.Thunder - 1;
                    Vector2 pos = abilityShowUI[num].transform.position;
                    currentAbilityObject.ShowAbility(pos, true, abilityData[num].iconSprite);
                    StartCoroutine(ThunderStrike());

                }else if(curentSelectedAbilityType == VariableTypeCode.Freeze) 
                {
                    int num = (int)VariableTypeCode.Freeze - 1;
                    Vector2 pos = abilityShowUI[num].transform.position;
                    currentAbilityObject.ShowAbility(pos, false, abilityData[num].iconSprite);
                    Invoke(nameof(FreezeAbility), 1f);
                }
            }
            else
            {
                currentAbilityObject.DisableObject();
            }
        }
    }

    private bool HasAbilityObject()
    {
        bool isObj = false;
        if(currentAbilityObject != null)
        {
            isObj = true;
        }
        else
        {
            currentAbilityObject = Instantiate(abilityObjectPrefab, this.transform);
            currentAbilityObject.DisableObject();
        }
        return isObj;
    }

    private void DisableAbility()
    {
        isAbilityinUse = false;
        abilityActive = false;
        boardManager.GameStatus(true);
        AbilityShowUIStatus(false);
    }

    //private void DiableAbilityObject(bool status)
    //{
    //    if (currentAbilityObject != null)
    //    {
    //        currentAbilityObject.gameObject.SetActive(status);
    //    }
    //}

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAbilityinUse)
        {
            if (!IsPointerOverUIButton())
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (!isPopupActive)
                {
                    if (CheckAbilityActive(mousePos))
                    {
                        ActivateAbility();
                    }
                }
            }
            else
            {
                if (abilityActive)
                {
                    AbilityShowUIStatus(false);
                    abilityActive = false;
                    boardManager.GameStatus(true);
                }
            }

        }
    }
    private bool IsPointerOverUIButton()
    {
        Vector2 pointerPosition = Vector2.zero;
        if (Input.touchCount > 0)
        {
            pointerPosition = Input.GetTouch(0).position;
        }
        else
        {
#if UNITY_EDITOR
            pointerPosition = Input.mousePosition;
#endif
        }

        if (pointerPosition != Vector2.zero)
        {
            pointerData.position = pointerPosition;
            raycastResults.Clear();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (RaycastResult result in raycastResults)
            {
                if (result.gameObject.TryGetComponent<Button>(out _))
                    return true;
            }
        }
        return false;
    }

}
