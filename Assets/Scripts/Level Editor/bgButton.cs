using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class bgButton : MonoBehaviour
{
    private bool isSelected;
    public int ThisIndex { get {  return thisIndex; } }
    private int thisIndex;
    private int tileIndex;
    public Image obsImage;
    private Button button;
    private LevelEditManager levelEditManager;
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPress);
    }

    public void SetUp(int index, bool isselected, LevelEditManager leveleditManager)
    {
        thisIndex = index;
        isSelected = isselected;
        levelEditManager = leveleditManager;
    }

    public void SetUpSprite(Sprite sprite, bool status, int tileindex)
    {
        isSelected = status;
        obsImage.gameObject.SetActive(status);
        obsImage.sprite = sprite;
        tileIndex = tileindex;
    }

    private void OnButtonPress()
    {
        levelEditManager.CheckForBgTile(thisIndex, tileIndex, isSelected);
    }
}
