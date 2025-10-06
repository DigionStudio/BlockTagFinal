using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class FortuneWheelSector : System.Object
{
    [Tooltip("Variable type")]
    public VariableTypeCode variableType = VariableTypeCode.None;

    public Sprite iconSprite;

    [Tooltip("Value of reward")]
    [RangeAttribute(0, 50)]
    public int RewardValue = 0;


    [Tooltip("Chance that this sector will be randomly selected")]
    [RangeAttribute(0, 100)]
    public int Probability = 100;
}

public class FortuneWheel : MonoBehaviour
{
    private bool _isStarted;                    // Flag that the wheel is spinning

    public Button turnButton; 				// This button is showed when you can turn the wheel for coins
    public Image buttonIcon;
    public Sprite[] buttonSprite;
    public Text nameText;
    public GameObject[] typeSpriteObj;


    private bool isSpinActive;
    public Text totalWheelCountText;
    public Transform wheelIconTrans;
    public GameObject wheelPopUp;                 // Pop-up with wasted fortune Wheel amount
    public Transform _finalTransform;



    public GameObject Circle; 					// Rotatable GameObject on scene with reward objects


    [Header("Params for each sector")]
    public FortuneWheelSector[] allSector;

    public FreeWheelRewards[] sectorShowUI;


    private float _finalAngle;                  // The final angle is needed to calculate the reward
    private float _startAngle;                  // The first time start angle equals 0 but the next time it equals the last final angle
    private float _currentLerpRotationTime;     // Needed for spinning animation
    private int[] sectorsAngles;

    private FortuneWheelSector[] Sectors;
    private FortuneWheelSector _finalSector;
    private int _lastSectorIndex = -1;
    private int wheelCount;
    private int spinCost = 5;



    private MenuManager menuManager;
    private GameDataManager gameDataManager;
    private BonusGiftManager bonusGiftManager;
    private TargetEffect targetEffect;
    private AdsLeaderboardManager adsLeaderboardManager;


    private void Start()
    {
        targetEffect = TargetEffect.Instance;
        menuManager = MenuManager.Instance;
        bonusGiftManager = menuManager.bonusGiftManager;
        gameDataManager = BlockManager.Instance.gameDataManager;
        adsLeaderboardManager = AdsLeaderboardManager.Instance;
        turnButton.onClick.AddListener(WheelTurnByWheelCoin);
        _finalTransform.gameObject.SetActive(false);
        adsLeaderboardManager.GratifyRewards.AddListener(Reward);
        adsLeaderboardManager.RewardAdsLoaded.AddListener(RewardButton);
        SetUp();
    }
    public void SetUpFortuneWheel()
    {
        wheelCount = gameDataManager.GetSaveValues(10);
        SetWheelCountText(wheelCount);
        SpinStatus();
        if (!isSpinActive)
            CheckRewardButtonAds();
        else
        {
            CheckRewardButton();
        }
    }

    private void SetUp()
    {
        int length = sectorShowUI.Length;
        Sectors = new FortuneWheelSector[length];
        sectorsAngles = new int[length];
        for (int i = 0; i < length; i++)
        {
            Sectors[i] = allSector[i];
            var sector = Sectors[i];
            bool isCoin = false;
            if (sector.variableType == VariableTypeCode.Coin)
            {
                isCoin = true;
            }
            sectorShowUI[i].SetUpReward(sector.RewardValue, sector.iconSprite, isCoin);
            sectorsAngles[i] = -360 / Sectors.Length * (i+1);
        }
    }

    private void WheelTurnByWheelCoin()
    {
        turnButton.interactable = false;
        if (isSpinActive)
        {
            CheckAndDecrease();
            TurnWheel();
        }
        else
        {
            RewardAds();
        }
    }

    private void RewardAds()
    {
        bool status = menuManager.FortuneFreeSpin();
        if(status)
        {
            Invoke(nameof(DisableGiftPanel), 1f);
        }
    }

    private void RewardButton()
    {
        if (!_isStarted)
        {
            turnButton.interactable = true;
        }
    }
    private void CheckRewardButton()
    {
        bool isActive = false;
        if(wheelCount > 0)
        {
            isActive = true;
        }
        turnButton.interactable = isActive;
    }

    private void CheckRewardButtonAds()
    {
        bool status = menuManager.CheckForFortune();
        turnButton.interactable = status;
    }


    private void Reward()
    {
        TurnWheel();
    }

    private void TurnWheel()
    {
        _currentLerpRotationTime = 0f;

        int randomFinalAngle = sectorsAngles[0];
        _finalSector = Sectors[0];


        int sectorIndex = FinalSector();
        print("Si " +  sectorIndex);
        _finalSector = Sectors[sectorIndex];

        if (sectorIndex > 0)
        {
            randomFinalAngle = sectorsAngles[sectorIndex - 1];
        }


        int fullTurnovers = 5;

        // Set up how many turnovers our wheel should make before stop
        _finalAngle = fullTurnovers * 360 + randomFinalAngle;

        // Stop the wheel
        _isStarted = true;

        if (isSpinActive)
        {
            // Show used wheel
            wheelPopUp.SetActive(true);
        }

        // disable for wheel
        Invoke(nameof(HideCoinsDelta), 0.8f);
    }
    private int FinalSector()
    {
        int rewardLevel = UnityEngine.Random.Range(0, 5);

        for (int i = 0; i < 50; i++)
        {
            if(rewardLevel == 3 && !adsLeaderboardManager.HasOnline)
            {
                rewardLevel = UnityEngine.Random.Range(0, 5);
            }
            else
            {
                break;
            }
        }
        print("RL " + rewardLevel);




        List<int> rewardList = new List<int>();
        if(rewardLevel == 3)
        {
            for (int i = 0; i < Sectors.Length; i++)
            {
                if(Sectors[i].variableType == VariableTypeCode.Thunder || (Sectors[i].RewardValue >= 3 && Sectors[i].variableType != VariableTypeCode.Coin) || (Sectors[i].variableType == VariableTypeCode.Coin && Sectors[i].RewardValue > 15))
                {
                    rewardList.Add(i);
                }
            }
        }else if(rewardLevel >= 1 && rewardLevel < 3)
        {
            for (int i = 0; i < Sectors.Length; i++)
            {
                if ((Sectors[i].RewardValue == 1 && Sectors[i].variableType != VariableTypeCode.Coin && Sectors[i].variableType != VariableTypeCode.Thunder) || (Sectors[i].variableType == VariableTypeCode.Coin && Sectors[i].RewardValue > 10 && Sectors[i].RewardValue <= 15))
                {
                    rewardList.Add(i);
                }
            }
        }
        else
        {
            for (int i = 0; i < Sectors.Length; i++)
            {
                if ((Sectors[i].variableType == VariableTypeCode.Coin && Sectors[i].RewardValue >= 1 && Sectors[i].RewardValue <= 10))
                {
                    rewardList.Add(i);
                }
            }
        }

        int index = UnityEngine.Random.Range(0, rewardList.Count);
        for (int i = 0; i < 50; i++)
        {
            if(_lastSectorIndex >= 0 && _lastSectorIndex == index)
            {
                index = UnityEngine.Random.Range(0, rewardList.Count);
            }
            else
            {
                _lastSectorIndex = index;
                break;
            }
        }
        int num = rewardList[index];
        return num;
    }
    private void HideCoinsDelta()
    {
        wheelPopUp.SetActive(false);
    }

    private void CheckAndDecrease()
    {
        if (wheelCount >= spinCost)
        {
            wheelCount -= spinCost;
        }
        else
        {
            wheelCount = 0;
        }
        gameDataManager.SetSaveValues(10, wheelCount);
        SetWheelCountText(wheelCount);
    }

    private void SpinStatus()
    {
        if (wheelCount >= spinCost)
        {
            isSpinActive = true;
            ChangeSpinButton("Spin", buttonSprite[0], 0);
        }
        else
        {
            isSpinActive = false;
            ChangeSpinButton("Free", buttonSprite[1], 1);

        }
    }

    private void ChangeSpinButton(string buttonName, Sprite buttonicon, int typeiconcode)
    {
        buttonIcon.sprite = buttonicon;
        nameText.text = buttonName;
        foreach (var obj in typeSpriteObj)
        {
            obj.SetActive(false);
        }
        typeSpriteObj[typeiconcode].SetActive(true);

    }
    private void SetWheelCountText(int count)
    {
        totalWheelCountText.text = count.ToString();
    }

    private void Update()
    {
        if (!_isStarted)
            return;

        // Animation time
        float maxLerpRotationTime = 4f;

        // increment animation timer once per frame
        _currentLerpRotationTime += Time.deltaTime;

        // If the end of animation
        if (_currentLerpRotationTime > maxLerpRotationTime || Circle.transform.eulerAngles.z == _finalAngle)
        {
            _currentLerpRotationTime = maxLerpRotationTime;
            _isStarted = false;
            _startAngle = _finalAngle % 360;
            //GiveAwardByAngle ();
            ShowReward();
        }
        else
        {
            // Calculate current position using linear interpolation
            float t = _currentLerpRotationTime / maxLerpRotationTime;

            // This formulae allows to speed up at start and speed down at the end of rotation.
            // Try to change this values to customize the speed
            t = t * t * t * (t * (6f * t - 15f) + 10f);

            float angle = Mathf.Lerp(_startAngle, _finalAngle, t);
            Circle.transform.eulerAngles = new Vector3(0, 0, -angle);
        }
    }

    private void ShowReward()
    {
        if(_finalSector.variableType != VariableTypeCode.None)
        {
            bonusGiftManager.GiftPanelForMetaFill(true);
            _finalTransform.gameObject.SetActive(true);
            float time = 2f;
            if(_finalSector.variableType == VariableTypeCode.Coin)
            {
                int count = _finalSector.RewardValue * 5;
                if(count > 30)
                {
                    count = count / 2;
                }
                targetEffect.FreeRewardEffectCoins(_finalTransform.position, count, 4f);
            }
            else if (_finalSector.variableType == VariableTypeCode.Lucky_Wheel)
            {
                Sprite sp = _finalSector.iconSprite;
                targetEffect.WheelAbility(sp, _finalTransform.position, wheelIconTrans.position, _finalSector.RewardValue);
            }
            else if (_finalSector.variableType == VariableTypeCode.Life)
            {
                Sprite sp = _finalSector.iconSprite;
                targetEffect.FreeRewardEffectLifes(_finalTransform.position, _finalSector.RewardValue);
            }
            else
            {
                time = 4f;
                int index = (int)_finalSector.variableType;
                if(index > 0 && index < 7)
                {
                    Sprite sp = _finalSector.iconSprite;
                    targetEffect.WheelAbility(sp, _finalTransform.position, turnButton.transform.position, _finalSector.RewardValue);
                }
            }
            Invoke(nameof(DisableGiftPanel), time);
            Invoke(nameof(GiveWheelReward), 0.5f);
        }
        else
        {
            SpinStatus();
        }
    }

    private void DisableGiftPanel()
    {
        bonusGiftManager.GiftPanelForMetaFill(false);
        _finalTransform.gameObject.SetActive(false);
        CheckTurnButton();
    }

    private void CheckTurnButton()
    {
        SpinStatus();
        if (!isSpinActive)
        {
            CheckRewardButtonAds();
        }
        else
        {
            CheckRewardButton();
        }

    }
    private void GiveWheelReward()
    {
        int values = _finalSector.RewardValue;
        if (_finalSector.variableType == VariableTypeCode.Coin)
        {
            values *= 10;
        }else if(_finalSector.variableType == VariableTypeCode.Lucky_Wheel)
        {
            wheelCount += values;
            SetWheelCountText(wheelCount);
        }
        gameDataManager.SetResourcesValues(_finalSector.variableType, values);
    }

    private void OnDisable()
    {
        adsLeaderboardManager.GratifyRewards.RemoveListener(Reward);
        adsLeaderboardManager.RewardAdsLoaded.RemoveListener(RewardButton);
    }


}
