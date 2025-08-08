using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusGiftManager : MonoBehaviour
{
    public Image giftPanelBg;
    public GameObject panelObj;
    public PanelTween[] panelTween;
    public Text headingText;
    private string[] heading = new string[2] { "Welcome", "Welcome Back" };

    public Text mainText;
    private string[] main = new string[2] { "Block Tag Welcomes You With Some Amazing Bonus And Abilities.",
        "Welcomes Back to Block Tag here Your Amazing Bonus And Abilities." };


    public Button claimButton;
    public GameObject confettiObj;

    public GameObject[] giftContainer;
    private List<GiftData> giftData = new List<GiftData>();
    private static bool isWelcomeBonus = false;
    private static bool isWelcomeBackBonus;
    private GameDataManager gameDataManager;
    public void OnWelcomeData()
    {
        if(gameDataManager == null)
            gameDataManager = BlockManager.Instance.gameDataManager;

        int code = gameDataManager.GetSaveValues(18);
        if(code > 0)
        {
            if(code == 2)
                isWelcomeBonus = true;
            else
                isWelcomeBackBonus = true;
        }

    }
    private void Start()
    {
        if (gameDataManager == null)
            gameDataManager = BlockManager.Instance.gameDataManager;
        confettiObj.SetActive(false);
        claimButton.onClick.AddListener(ClaimBonus);
        foreach (var item in panelTween)
        {
            TweenPanel(false, item);
        }
    }
    public bool CheckForBonus()
    {
        OnWelcomeData();
        bool status = false;
        if (isWelcomeBonus)
        {
            giftPanelBg.color = new Color(0, 0, 0, 0);
            giftPanelBg.gameObject.SetActive(true);
            GiftData dt = new()
            {
                indexCode = VariableTypeCode.Coin,
                values = 500,
            };
            giftData.Add(dt);

            for (int i = 1; i < 4; i++)
            {
                int abilityindex = i;
                if(i == 3)
                {
                    abilityindex++;
                }
                GiftData data = new()
                {
                    indexCode = (VariableTypeCode)abilityindex,
                    values = 1,
                };
                giftData.Add(data);
            }

            GiftData dt2 = new()
            {
                indexCode = VariableTypeCode.Life,
                values = 5,
            };
            giftData.Add(dt2);


            headingText.text = heading[0];
            mainText.text = main[0];
            giftContainer[0].SetActive(true);
            giftContainer[1].SetActive(false);
            if (isWelcomeBackBonus)
            {
                headingText.text = heading[1];
                mainText.text = main[1];

                giftContainer[0].SetActive(false);
                giftContainer[1].SetActive(true);
                giftData[3].indexCode = VariableTypeCode.Magic_Wand;
                giftData[0].values = 200;
                giftData[giftData.Count - 1].values = 1;
            }
            isWelcomeBonus = false;
            status = true;
            Invoke(nameof(SetUpBonusPanle), 1f);
        }
        else
        {
            ResetGiftPanel();
        }
        return status;
    }

    private void SetUpBonusPanle()
    {
        panelObj.SetActive(true);
        giftPanelBg.DOFade(0.9f, 0.5f).OnComplete(() =>
        {
            Invoke(nameof(ConfettiEffect), 0.8f);
        });
        foreach (var item in panelTween)
        {
            TweenPanel(true, item, 0.7f);
        }
        
    }
    private void ConfettiEffect()
    {
        confettiObj.SetActive(true);
    }
    private void TweenPanel(bool isfinalpos, PanelTween panel, float duration = 0)
    {
        if (panel != null)
        {
            float posy = panel.posInitialFloat;
            float time = duration;
            if (isfinalpos)
            {
                posy = panel.posFinalFloat;
                time = 0.3f;
            }
            panel.panel.DOAnchorPosY(posy, time)
        .SetEase(Ease.OutQuad);
        }
    }
    private void ClaimBonus()
    {
        int length = giftData.Count;
        GiftData[] data = new GiftData[length];
        for (int i = 0; i < length; i++)
        {
            GiftData dt = new()
            {
                indexCode = giftData[i].indexCode,
                values = giftData[i].values,
            };
            data[i] = dt;
        }
        gameDataManager.SetGiftData(data, length);

        giftPanelBg.DOFade(0f, 0.5f).OnComplete(() =>
        {
            ResetGiftPanel();
            gameDataManager.SetSaveValues(18, 0);
            MenuManager.Instance.MenuShow();
        });
        foreach (var item in panelTween)
        {
            TweenPanel(false, item, 0.5f);
        }
    }

    private void ResetGiftPanel()
    {
        confettiObj.SetActive(false);
        panelObj.SetActive(false);
        giftPanelBg.color = new Color(0, 0, 0, 0.7f);
        giftPanelBg.gameObject.SetActive(false);
    }

    public void GiftPanelForMetaFill(bool status)
    {
        ResetGiftPanel();
        if (status)
        {
            giftPanelBg.gameObject.SetActive(true);
            giftPanelBg.color = new Color(0, 0, 0, 1f);
            giftPanelBg.DOFade(0, 0.7f);
        }
    }


}
