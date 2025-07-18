using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeWheelRewards : MonoBehaviour
{
    public int colorCode;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image coinIconImage;
    [SerializeField] private Text countText;

    public void SetUpReward(int count, Sprite icon, bool isCoin)
    {
        iconImage.gameObject.SetActive(true);
        coinIconImage.gameObject.SetActive(true);
        string sign = "x";
        if(isCoin)
        {
            iconImage.gameObject.SetActive(false);
            coinIconImage.sprite = icon;
            count *= 10;
            sign = "+";
        }
        else
        {
            coinIconImage.gameObject.SetActive(false);
            iconImage.sprite = icon;
        }
        countText.text = sign + count.ToString();
    }
}
