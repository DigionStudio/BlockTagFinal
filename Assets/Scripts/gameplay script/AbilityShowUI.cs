using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AbilityShowUI : MonoBehaviour
{

    [SerializeField] private Button AbilityButton;
    [SerializeField] private Image abilityIocn;
    [SerializeField] private GameObject coubtBg;
    [SerializeField] private GameObject cointHolder;
    [SerializeField] private Text countText;
    [SerializeField] private GameObject infiniteObj;
    [SerializeField] private GameObject buyAbilityObj;
    [SerializeField] private GameObject disableAbility;
    [SerializeField] private GameObject lockImage;

    [SerializeField] private RectTransform unlockStatusRect;
    [SerializeField] private Text infoText;
    [SerializeField] private GameObject[] disableObj;

    private Game_Value_Status abilityCountStatus;
    private AbilityData thisData;
    private bool isAbilityLocked;
    private bool HasAbilityActive = false;
    private bool isBuyAbility = false;
    private VariableTypeCode thisAbilityType;
    private void Start()
    {
        AbilityButton.onClick.AddListener(AbilityActive);
        infoText.enabled = false;
    }

    public void SetUp(AbilityData data, Game_Value_Status valueStatus, VariableTypeCode type, bool isLock)
    {
        thisAbilityType = type;
        isAbilityLocked = isLock;
        thisData = data;
        abilityIocn.sprite = data.iconSprite;
        abilityCountStatus = valueStatus;
        unlockStatusRect.gameObject.SetActive(false);
        lockImage.gameObject.SetActive(isLock);
        coubtBg.SetActive(!isLock);
        disableAbility.SetActive(false);
        CheckAbilityCount();
        if(thisAbilityType == VariableTypeCode.Bomb || thisAbilityType == VariableTypeCode.Hammer)
        {
            disableObj[0].SetActive(true);
            disableObj[1].SetActive(false);
        }
        else
        {
            disableObj[0].SetActive(true);
            disableObj[1].SetActive(false);
        }
        
    }

    private void AbilityActive()
    {
        if (!isAbilityLocked)
        {
            if (abilityCountStatus.value > 0 || abilityCountStatus.status)
            {
                CheckAbilityStatus(!HasAbilityActive);
            }
            else
            {
                if (isBuyAbility && !AbilityManager.Instance.HasAbilityUse)
                {
                    BuyAbility();
                }
                else
                {
                    UnableToUse();
                }
            }
        }
        else
        {
            AbilityInfo("Unlock at Level " + thisData.unLockvalue.ToString());
        }
    }

    public void AbilityEnabled(bool status)
    {
        HasAbilityActive = status;
        AbilityButton.enabled = !HasAbilityActive;
        DisableAbilityUI(status);
    }

    private void CheckAbilityStatus(bool status)
    {
        AbilityManager.Instance.AbilityStatus(status, thisAbilityType);
        CheckAbilityCount();
    }
    public void DisableAbilityUI(bool status, bool isPopUp = false)
    {
        if (!isPopUp && (thisAbilityType == VariableTypeCode.Bomb || thisAbilityType == VariableTypeCode.Hammer))
            AbilityUseDetail(status);
        disableAbility.SetActive(status);
        coubtBg.SetActive(!status);
    }

    public void AbilityUpdated(Game_Value_Status abilityStatus)
    {
        abilityCountStatus = abilityStatus;
        CheckAbilityCount();
    }

    private void CheckAbilityCount()
    {
        bool isInfinite = abilityCountStatus.status;
        isBuyAbility = false;
        if (!isInfinite)
        {
            if(abilityCountStatus.value == 0)
                isBuyAbility = true;
            else
            {
                ShowAbilityCount();
            }
        }
        else
        {
            infiniteObj.SetActive(isInfinite);
            countText.gameObject.SetActive(!isInfinite);
        }
        buyAbilityObj.SetActive(isBuyAbility);
        cointHolder.gameObject.SetActive(!isBuyAbility);
    }


    private void BuyAbility()
    {
        BuyPanel.Instance.SetUpAbilityBuyPanel(true, thisData, thisAbilityType);
    }

    private void ShowAbilityCount()
    {
        if(abilityCountStatus.value >= 0)
            countText.text = abilityCountStatus.value.ToString();
    }

    private void AbilityUseDetail(bool status)
    {
        unlockStatusRect.sizeDelta = Vector2.zero;
        unlockStatusRect.gameObject.SetActive(true);
        infoText.enabled = status;

        infoText.text = "Press on the Block to Use";
        unlockStatusRect.DOAnchorPos(Vector2.zero, 0f);
        Vector2 size = new Vector2(240, 40);
        float posY = 80;
        if (!status)
        {
            posY = 0;
            size = Vector2.zero;
        }
        unlockStatusRect.DOSizeDelta(size, 0.3f);
        unlockStatusRect.DOAnchorPosY(posY, 0.3f).OnComplete(() =>
        {
            unlockStatusRect.gameObject.SetActive(status);
        });
    }

    public void UnableToUse()
    {
        print("sdfsdfsdf");
        string info = "Currently Unable TO Use";
        AbilityInfo(info);
    }

    private void AbilityInfo(string info)
    {
        AbilityButton.enabled = false;
        unlockStatusRect.sizeDelta = Vector2.zero;
        unlockStatusRect.gameObject.SetActive(true);
        infoText.enabled = true;

        infoText.text = info;
        unlockStatusRect.DOAnchorPos(Vector2.zero, 0f);
        unlockStatusRect.DOAnchorPosY(80, 0.3f);
        unlockStatusRect.DOSizeDelta(new Vector2(240, 40), 0.3f).OnComplete(() =>
        {
            Invoke(nameof(DiactiveInfo), 1f);
        });

    }
    private void DiactiveInfo()
    {
        infoText.enabled = false;
        unlockStatusRect.DOAnchorPosY(0, 0.3f);
        unlockStatusRect.DOSizeDelta(Vector2.zero, 0.3f).OnComplete(() =>
        {
            unlockStatusRect.gameObject.SetActive(false);
            AbilityButton.enabled = true;


        });
    }
}
