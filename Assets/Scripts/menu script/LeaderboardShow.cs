using System.Collections;
using System.Collections.Generic;
using Unity.Services.Leaderboards.Models;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardShow : MonoBehaviour
{
    [SerializeField] private Text serialNumText;
    [SerializeField] private Image medalImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Text scoreText;
    [SerializeField] private Image currentPlayerShow;
    

    public void SetUp(LeaderboardEntry player, Sprite icon, Sprite medal, bool iscurrentPlayer)
    {
        string rank = (player.Rank + 1).ToString();
        if(player.Rank >= 99)
        {
            rank = "+100";
        }

        serialNumText.text = rank;
        string trimmedName = player.PlayerName.Split('#')[0];
        nameText.text = trimmedName;
        scoreText.text = player.Score.ToString();
        medalImage.sprite = medal;
        iconImage.sprite = icon;
        currentPlayerShow.gameObject.SetActive(iscurrentPlayer);
    }
}
