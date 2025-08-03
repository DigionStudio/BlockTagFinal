using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

[Serializable]
public class LeaderBoardRankObj
{
    public GameObject rankObj;
    public Text rankText;
}

public class LeaderboardShow : MonoBehaviour
{
    [SerializeField] private Image bgImage;

    [SerializeField] private LeaderBoardRankObj[] serialObj;
    [SerializeField] private Image medalImage;
    [SerializeField] private GameObject medalEffectImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image scoreBGImage;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image currentPlayerShow;
    [SerializeField] private Color[] colorCode;
    [SerializeField] private IconsReference rankIcons;
    [SerializeField] private IconsReference scoreIcons;
    private int playerRank = -1;

    public void SetUp(PlayerGlobalData player, int iconCode, bool iscurrentPlayer, int colCode = -1)
    {
        playerRank = player.Rank;
        string rank = (playerRank + 1).ToString();
        if(playerRank > 99)
        {
            rank = "+100";
        }
        ResetrankObj();
        int rankIndex = 1;
        Color col = colorCode[3];
        Sprite medal = null;
        Sprite icon = scoreIcons.iconSprite[iconCode];
        if (playerRank <= 2)
        {
            rankIndex = 0;
            if (colCode < 0)
                col = colorCode[playerRank];
            medal = rankIcons.iconSprite[playerRank];
        }
        if(colCode == 1)
        {
            col = Color.green;
        }
        bgImage.color = col;
        serialObj[rankIndex].rankObj.SetActive(true);
        serialObj[rankIndex].rankText.text = rank;
        string trimmedName = player.Name.Split('#')[0];
        nameText.text = trimmedName;
        scoreText.text = player.scoreValue.ToString();
        if(medal != null)
            medalImage.sprite = medal;
        if(icon != null)
            iconImage.sprite = icon;
        currentPlayerShow.gameObject.SetActive(iscurrentPlayer);
    }

    public void ResetBgColor()
    {
        Color col = colorCode[3];
        if (playerRank <= 2)
        {
            col = colorCode[playerRank];
        }
        bgImage.color = col;
        currentPlayerShow.gameObject.SetActive(true);
        currentPlayerShow.enabled = true;
    }

    private void ResetrankObj()
    {
        foreach(var obj in serialObj)
        {
            obj.rankObj.SetActive(false);
        }
    }

    public void UIElementVisibility(float val, float time)
    {
        bgImage.DOFade(val, time);
        nameText.DOFade(val, time);
        foreach(var obj in serialObj)
        {
            obj.rankText.DOFade(val, time);
        }
        medalImage.DOFade(val, time);
        nameText.DOFade(val, time);
        iconImage.DOFade(val, time);
        float amt = val;
        bool active = false;
        if (val == 1)
        {
            active = true;
            amt = 0.63f;
        }
        medalEffectImage.SetActive(active);
        scoreBGImage.DOFade(amt, time);
        scoreText.DOFade(val, time);
        currentPlayerShow.DOFade(val, time);

    }
}
