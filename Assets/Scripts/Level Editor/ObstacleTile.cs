using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObstacleTile : MonoBehaviour
{
    public Button button;
    private Image buttonImage;
    public Text countText;
    public Image selectedIcon;
    private int tileIndex;
    private int tileCount;
    private LevelEditManager levelEditManager;
    void Start()
    {
        
        button.onClick.AddListener(OnPressButton);
    }

    public void SetUp(int tileindex, LevelEditManager leveleditManager, Sprite sprite)
    {
        tileIndex = tileindex;
        levelEditManager = leveleditManager;
        if(buttonImage ==null)
            buttonImage = button.GetComponent<Image>();
        buttonImage.sprite = sprite;
        selectedIcon.enabled = false;
    }

    private void OnPressButton()
    {
        levelEditManager.ChangeObsTile(tileIndex);
    }

    public void SelectionStatus(bool isSelected)
    {
        selectedIcon.enabled = isSelected;

    }

    public void SetTileCount(bool isReset)
    {
        if (isReset)
        {
            tileCount = 0;
        }
        else
        {
            tileCount += 1;
        }
        countText.text = tileCount.ToString();
    }
}
