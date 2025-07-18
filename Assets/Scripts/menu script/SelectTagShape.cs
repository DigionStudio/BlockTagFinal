using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTagShape : MonoBehaviour
{

    public Transform shapeHolder;
    public TagSelection tagPrefab;
    private RectTransform rectTransform;
    private int[] shapeSelectHeight = new int[3] {550, 685, 800};
    private int width = 720;
    private bool isSelectShape;
    private List<TagSelection> tagShow = new List<TagSelection>();
    private List<int> shapeValue = new List<int>();
    private int currentSelectedTagIndex = -1;

    public GameObject tagShowSelectPanel;
    public Transform showHolder;
    public TagSelection tagSelectPrefab;
    public Button crossButton;
    private List<TagSelection> tagSelectShow = new List<TagSelection>();

    public GameObject[] abilityObjectStatus;
    public Image[] abilityObjectStatusImage;
    public Sprite[] abilityObjectStatusSprite;
    public Button[] unlockButtons;
    public Button[] lockButtons;
    public GameObject[] unlockvalueObj;
    public Text[] unlockvalueText;
    public GameObject[] unlocklevelObj;
    public Text[] statusText;
    public Image[] selectedImage;
    public Color unSelectedColor;
    public GameObject[] tagAbilityActiveEffect;

    private string statusType = "Don't have Enough Coins";
    private string statusType1 = "Level 15 to Unlock";
    private bool tagStatus1 = false;
    private bool tagStatus2 = false;
    private int levelNumber;

    private bool buttonstatus1;
    private bool buttonstatus2;
    private MenuManager menuManager;
    private bool isTagSelect;
    private int totalBuyValue = 50;
    private int buyValue;
    private int buyValue2;

    private bool isUnlock1;
    private bool isUnlock2;

    private void Start()
    {
        menuManager = MenuManager.Instance;
        crossButton.onClick.AddListener(CrossButton);
        unlockButtons[0].onClick.AddListener(ActiveTagBomb);
        unlockButtons[1].onClick.AddListener(ActiveTagAbility);
        lockButtons[0].onClick.AddListener(LockAbilityBomb);
        lockButtons[1].onClick.AddListener(LockAbilityTag);
        DisableLevelLockObj1();
        DisableLevelLockObj2();
    }

    public void SetUpShapeSelect(bool isSelec, ShapeData[] shapeCodes)
    {
        isTagSelect = isSelec;
        if (isSelec)
        {
            SetUp(shapeCodes);
        }
        else
        {
            DestroyTag();
        }
    }
    private void SetUp(ShapeData[] shapeCodes)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        shapeHolder.gameObject.SetActive(false);
        tagShowSelectPanel.SetActive(false);
        shapeValue.Clear();
        foreach (var tag in tagShow) {
            tag.gameObject.SetActive(false);
        }
        int num = 0;
        if (shapeCodes.Length > 0)
        {
            isSelectShape = true;
            num = 1;
            if (shapeCodes.Length > 4)
                num = 2;
        }
        else isSelectShape = false;
        if (isSelectShape)
        {
            shapeHolder.gameObject.SetActive(true);

            for (int i = 0; i < shapeCodes.Length; i++)
            {
                ShapeData shapeData = shapeCodes[i];
                if (i < tagShow.Count)
                {
                    tagShow[i].SetUp(i, shapeData.code, shapeData.isSelectable, true, this);
                    tagShow[i].gameObject.SetActive(true);
                }
                else
                {
                    InstaTagSelection(i, shapeData, tagPrefab, shapeHolder, true);
                }
                shapeValue.Add(shapeData.code);
            }
        }
        rectTransform.sizeDelta = new Vector2(width, shapeSelectHeight[num]);
    }

    public void AbilityStatus(bool isBombUnlocked, bool isAbilityUnlocked, int level)
    {
        buyValue = totalBuyValue;
        buyValue2 = totalBuyValue;
        unlockvalueObj[0].SetActive(!isBombUnlocked);
        unlockvalueObj[1].SetActive(!isAbilityUnlocked);
        buyValue = (totalBuyValue * ((level / totalBuyValue) + 3));
        buyValue2 = (totalBuyValue * ((level / totalBuyValue) + 2));
        levelNumber = level;
        unlockvalueText[0].text = buyValue.ToString();
        unlockvalueText[1].text = buyValue2.ToString();
    }

    private void StatusTextSetUp(int index, string type)
    {
        if (index >= 0)
        {
            statusText[index].text = type;
        }
        else
        {
            foreach (var item in statusText)
            {
                item.text = type;
            }
        }
    }

    public void AbilitySetUp(bool isUnlockable1,bool ismatch1, bool isUnlockable2,bool ismatch2, bool initialLock)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (!isTagSelect)
        {
            rectTransform.sizeDelta = new Vector2(width, shapeSelectHeight[0]);
            shapeHolder.gameObject.SetActive(false);
        }
        int coin = BlockManager.Instance.gameDataManager.GetSaveValues(0);
        tagStatus1 = false;
        tagStatus2 = false;
        buttonstatus1 = false;
        buttonstatus2 = false;
        isUnlock1 = isUnlockable1;
        isUnlock2 = isUnlockable2;
        StatusTextSetUp(-1, statusType);
        SetUpStatusImage(initialLock);
        if (!initialLock)
        {
            unlockvalueObj[0].SetActive(!ismatch1);
            unlockvalueObj[1].SetActive(!ismatch2);
            if (!isUnlock1 && !ismatch1)
            {
                if (coin < buyValue)
                {
                    buttonstatus1 = true;
                    StatusTextSetUp(0, statusType);
                }
            }
            else
            {
                tagStatus1 = true;
            }
            if (!isUnlock2 && !ismatch2)
            {
                if (coin < buyValue2)
                {
                    buttonstatus2 = true;
                    StatusTextSetUp(1, statusType);

                }
            }
            else
            {
                tagStatus2 = true;
            }
        }
        else
        {
            unlockvalueObj[0].SetActive(false);
            unlockvalueObj[1].SetActive(false);
            buttonstatus1 = true;
            buttonstatus2 = true;
            StatusTextSetUp(-1, statusType1);
        }
        TagStatus(0, isUnlock1);
        TagStatus(1, isUnlock2);
    }
    private void ActiveTagBomb()
    {
        if (buttonstatus1)
        {
            unlocklevelObj[0].SetActive(true);
            unlockButtons[0].gameObject.SetActive(false);
            Invoke(nameof(DisableLevelLockObj1), 2f);
        }
        else 
        {
            if (!tagStatus1)
            {
                MenuManager.Instance.ChangeTagStatus(true, true, buyValue);
            }
            else
            {
                MenuManager.Instance.ChangeTagStatus(false, true, buyValue, true);
            }
            ActiveEffect1();
        }
    }

    private void ActiveTagAbility()
    {
        if (buttonstatus2)
        {
            unlocklevelObj[1].SetActive(true);
            unlockButtons[1].gameObject.SetActive(false);
            Invoke(nameof(DisableLevelLockObj2), 2f);
        }
        else
        {
            if (!tagStatus2)
            {
                MenuManager.Instance.ChangeTagStatus(true, false, buyValue2);
            }
            else
            {
                MenuManager.Instance.ChangeTagStatus(false, false, buyValue2, true);
            }

            ActiveEffect2();
        }

    }

    private void DisableLevelLockObj1()
    {
        unlocklevelObj[0].SetActive(false);
        unlockButtons[0].gameObject.SetActive(true);

    }
    private void DisableLevelLockObj2()
    {
        unlocklevelObj[1].SetActive(false);
        unlockButtons[1].gameObject.SetActive(true);

    }

    private void TagStatus(int code, bool status)
    {
        GameObject obj = abilityObjectStatus[code];
        GameObject obj2 = unlockButtons[code].gameObject;
        GameObject obj3 = lockButtons[code].gameObject;
        if (obj != null)
        {
            obj.SetActive(!status);
        }

        if (obj2 != null)
        {
            obj2.SetActive(!status);
        }

        if (obj3 != null)
        {
            obj3.SetActive(status);
        }
        Color col = unSelectedColor;
        if (status)
            col = Color.green;
        selectedImage[code].color = col;
    }

    private void SetUpStatusImage(bool initialLock)
    {
        Sprite sp = abilityObjectStatusSprite[0];
        if (!initialLock)
        {
            sp = abilityObjectStatusSprite[1];
        }
        foreach (var item in abilityObjectStatusImage)
        {
            item.sprite = sp;
        }
    }

    private void LockAbilityBomb()
    {
        MenuManager.Instance.ChangeTagStatus(false, true, buyValue);
        ActiveEffect1();
    }

    private void LockAbilityTag()
    {
        MenuManager.Instance.ChangeTagStatus(false, false, buyValue2);
        ActiveEffect2();
    }

    private void InstaTagSelection(int index, ShapeData data, TagSelection prefab,Transform parent, bool isShow)
    {
        TagSelection tag = Instantiate(prefab, parent);
        tag.transform.parent = parent.transform;
        tag.SetUp(index, data.code, data.isSelectable, isShow, this);
        if(isShow)
            tagShow.Add(tag);
        else
            tagSelectShow.Add(tag);

    }

    private void DestroyTag()
    {
        foreach (TagSelection tag in tagShow)
        {
            Destroy(tag.gameObject);
        }
        tagShow.Clear();
    }

    public void ShowTags(int index)
    {
        currentSelectedTagIndex = index;
        TagShowSelect(true);
    }

    private void TagShowSelect(bool status)
    {
        if (status)
        {
            int num = 0;
            int count = tagSelectShow.Count;
            for (int i = 0; i < 12; i++)
            {
                if (!shapeValue.Contains(i))
                {
                    ShapeData shapeData = new ShapeData();
                    shapeData.code = i;
                    shapeData.isSelectable = true;
                    if (num < count)
                    {
                        tagSelectShow[num].SetUp(num, shapeData.code, shapeData.isSelectable, false, this);
                        tagSelectShow[num].gameObject.SetActive(true);
                        num++;
                    }
                    else
                    {
                        InstaTagSelection(i, shapeData, tagSelectPrefab, showHolder, false);
                    }
                }
            }
        }
        else{
            foreach (var item in tagSelectShow)
            {
                item.gameObject.SetActive(false);
            }

        }
        tagShowSelectPanel.SetActive(status);
    }

    public void SelectTagToUse(int value)
    {
        if(currentSelectedTagIndex >= 0 && currentSelectedTagIndex < 14)
        {
            ChangeTagShow(currentSelectedTagIndex, value);
        }
        CrossButton();
    }

    private void ChangeTagShow(int index, int value)
    {
        tagShow[index].SetUp(index, value,true, true, this);
        shapeValue[index] = value;
        menuManager.ChangeTagIndex(index, value);
    }


    private void CrossButton()
    {
        currentSelectedTagIndex = -1;
        TagShowSelect(false);
    }

    public void ActiveEffect(bool status)
    {
        if (status)
        {
            ActiveEffect1();
            ActiveEffect2();
        }
        else
        {
            tagAbilityActiveEffect[0].SetActive(false);
            tagAbilityActiveEffect[1].SetActive(false);
        }
        
    }
    private void ActiveEffect1()
    {
        tagAbilityActiveEffect[0].SetActive(isUnlock1);
    }

    private void ActiveEffect2()
    {
        tagAbilityActiveEffect[1].SetActive(isUnlock2);
    }
}
