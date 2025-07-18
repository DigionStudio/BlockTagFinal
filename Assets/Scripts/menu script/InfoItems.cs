using System;
using UnityEngine;
using UnityEngine.UI;

public class InfoItems : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Image showImage;
    [SerializeField] private Text countText;
    private Text addValueText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;

    [SerializeField] private GameObject lockObj;
    [SerializeField] private Text lockValueText;



    [SerializeField] private GameObject unlimitedObj;
    [SerializeField] private Text timeText;

    private Button infoButton;
    private int itemIndex = -1;
    private MenuManager menuManager;
    private bool isInfoPanel;
    private bool isAbilityPanel;


    public void EnableUnlimitedEffect(bool status)
    {
        if(unlimitedObj != null)
        {
            unlimitedObj.SetActive(status);
        }
    }
    public void LockStatus(bool status)
    {
        if(lockObj != null)
        {
            lockObj.SetActive(status);
        }
    }

    public void DetailsLockStatus(bool status, int level)
    {
        if (status)
        {
            EnableUnlimitedEffect(false);
        }
        LockStatus(status);
        if(lockValueText != null)
        {
            lockValueText.text = "Unlock at Level " + level.ToString();
        }
    }

    public void AbilityUnlimitedTimer(int index, int time)
    {
        if(itemIndex >= 0 && index >= 0 && index == itemIndex && isInfoPanel && isAbilityPanel)
        {
            unlimitedObj.SetActive(true);
            if (timeText != null)
            {
                timeText.text = FormatTime(time);
            }
        }
    }
    private string FormatTime(int timeInSec)
    {
        int hours = Mathf.FloorToInt(timeInSec / 3600);
        int minutes = Mathf.FloorToInt((timeInSec % 3600) / 60);
        int seconds = Mathf.FloorToInt(timeInSec % 60);

        string timeText = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        if (minutes <= 0)
        {
            timeText = string.Format("{0:00}", seconds);
        }
        else if (hours <= 0)
        {
            timeText = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        return timeText;
    }

    public void SetUpInfoItemData(Sprite icon, int count,int addValue, string name, string description, int itemndex, bool isability)
    {
        isInfoPanel = true;
        isAbilityPanel = isability;
        if (menuManager == null)
            menuManager = MenuManager.Instance;

        countText.gameObject.SetActive(true);
        iconImage.gameObject.SetActive(isability);
        showImage.gameObject.SetActive(!isability);
        if (icon != null)
        {
            iconImage.sprite = icon;
            showImage.sprite = icon;
        }
        if (nameText)
            nameText.text = name;
        if (descriptionText)
            descriptionText.text = description;
        itemIndex = itemndex;
        if (countText != null)
        {
            if (addValue == -1)
            {
                countText.gameObject.SetActive(false);
            }
            else
            {
                countText.text = "x" + count.ToString();
                if (addValueText == null)
                {
                    addValueText = countText.transform.GetChild(0).GetComponent<Text>();
                }
                if (addValueText != null)
                    addValueText.text = "+" + addValue.ToString();
            }
        }
    }
    

    public void SetUpItemDataForStartUp(Sprite icon,int count, string name, string description,int itemndex, bool isInfo,bool isability, bool isHide = false)
    {
        isInfoPanel = isInfo;
        isAbilityPanel = isability;

        if (menuManager == null)
            menuManager = MenuManager.Instance;
        if (iconImage && icon != null && !isHide)
        {
            iconImage.sprite = icon;
            if (showImage != null)
            {
                showImage.enabled = false;
            }
        }
        if(nameText)
            nameText.text = name;
        if(descriptionText) 
            descriptionText.text = description;
        itemIndex = itemndex;
        if(countText != null)
        {
            if(count >= 0)
                countText.text = "x"+ count.ToString();
            else
                countText.text = "";
        }
        if (isInfo)
        {
            if (infoButton == null)
            {
                infoButton = GetComponent<Button>();
                if (infoButton != null)
                {
                    infoButton.onClick.AddListener(InfoPanelActive);
                }
            }
        }
    }

    private void InfoPanelActive()
    {
        if (menuManager && itemIndex >= 0)
        {
            menuManager.InfoDetailsPanel(itemIndex, isAbilityPanel);
        }
    }
}
