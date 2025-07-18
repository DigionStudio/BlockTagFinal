using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BlockAbilityImage
{
    public Image blockIcon;
    public Image abilityIcon;
}

public class TileShow : MonoBehaviour
{
    [SerializeField] private Image SelectedImage;
    [SerializeField] private Image[] blockIcons;
    [SerializeField] private BlockAbilityImage[] blockAbilityIcon;
    [SerializeField] private GameObject buttonHolder;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button freeButton;


    [SerializeField] private GameObject selected;
    [SerializeField] private GameObject notEnough;
    [SerializeField] private GameObject comingSoonObj;
    [SerializeField] private Sprite soonIcon;
    private BlockManager blockManager;
    private GameDataManager gameDataManager;
    private int showIndex;
    private bool isSetUped;
    private Sprite blockShowSprite;
    public static Action<int> OnBlockTypeUpdate = delegate { };


    private void Start()
    {
        buyButton.onClick.AddListener(BuyBlockIcon);
        selectButton.onClick.AddListener(SelectBlockIcon);
        freeButton.onClick.AddListener(FreeBlock);
        notEnough.SetActive(false);
        if (blockManager == null)
        {
            blockManager = BlockManager.Instance;
            gameDataManager = blockManager.gameDataManager;
        }

    }

    private void OnDisable()
    {
        OnBlockTypeUpdate -= OnBlockType;

    }
    public void SetUp(int index, int selectInt, bool isBuyed, bool isFree)
    {
        comingSoonObj.SetActive(false);
        showIndex = index;
        if (blockManager == null)
        {
            blockManager = BlockManager.Instance;
            gameDataManager = blockManager.gameDataManager;
        }
        buyButton.gameObject.SetActive(false);
        selectButton.gameObject.SetActive(false);
        freeButton.gameObject.SetActive(false);
        Selected(false);
        if (!isBuyed)
        {
            if (isFree)
            {
                freeButton.gameObject.SetActive(true);
            }else
                buyButton.gameObject.SetActive(true);
        }
        else
        {
            selectButton.gameObject.SetActive(true);
        }
        BlockTypeData blockType = blockManager.AllTileData[index];
        blockShowSprite = blockType.blockTileSprite[0];

        for (int i = 0; i < blockIcons.Length; i++)
        {
            blockIcons[i].sprite = blockType.blockTileSprite[i];
        }

        blockAbilityIcon[0].blockIcon.sprite = blockType.blockTileSprite[0];
        blockAbilityIcon[0].abilityIcon.sprite = blockType.abilityTileSprite[0];

        blockAbilityIcon[1].blockIcon.sprite = blockType.blockTileSprite[1];
        blockAbilityIcon[1].abilityIcon.sprite = blockType.abilityTileSprite[1];

        blockAbilityIcon[2].blockIcon.sprite = blockType.blockTileSprite[2];
        blockAbilityIcon[2].abilityIcon.sprite = blockType.abilityTileSprite[3];
        OnBlockType(selectInt);

    }


    public void CommingSoonPanel(int index)
    {
        showIndex = index;
        SelectedImage.enabled = false;

        comingSoonObj.SetActive(true);
        for (int i = 0; i < blockIcons.Length; i++)
        {
            blockIcons[i].sprite = soonIcon;
        }

        blockAbilityIcon[0].blockIcon.sprite = soonIcon;
        blockAbilityIcon[0].abilityIcon.enabled = false;

        blockAbilityIcon[1].blockIcon.sprite = soonIcon;
        blockAbilityIcon[1].abilityIcon.enabled = false;

        blockAbilityIcon[2].blockIcon.sprite = soonIcon;
        blockAbilityIcon[2].abilityIcon.enabled = false;
        buyButton.gameObject.SetActive(true);
        selectButton.gameObject.SetActive(false);
    }
    


    private void SelectBlockIcon()
    {
        blockManager.SelectBlockIcon(showIndex);
    }

    private void BuyBlockIcon()
    {
        bool isBuyed = gameDataManager.BuyIconData(showIndex, 1000);
        ButtonSetUp(isBuyed);
        if(!isBuyed)
        {
            NotEnoughCoin();
        }
    }

    private void ButtonSetUp(bool isBuyed)
    {
        buyButton.gameObject.SetActive(!isBuyed);
        selectButton.gameObject.SetActive(isBuyed);
        freeButton.gameObject.SetActive(false);

    }

    private void OnEnable()
    {
        OnBlockTypeUpdate += OnBlockType;
        if (isSetUped)
        {
            CheckTileShow();
        }
    }

    private void CheckTileShow()
    {
        int currentIndex = gameDataManager.GetSaveValues(4);
        OnBlockType(currentIndex);
        bool isBuyed = gameDataManager.GetSaveBlockIconData(showIndex).isBuyed;
        buyButton.gameObject.SetActive(!isBuyed);
        selectButton.gameObject.SetActive(isBuyed);
    }

    private void OnBlockType(int num)
    {
        bool isSelec = false;
        if(num == showIndex)
        {
            SelectedImage.enabled = true;
            isSelec = true;
            MenuManager.Instance.SetBlockTypeImage(blockShowSprite);
        }
        Selected(isSelec);
    }

    private void Selected(bool isSelected)
    {
        SelectedImage.enabled = isSelected;
        buttonHolder.SetActive(!isSelected);
        selected.SetActive(isSelected);
    }

    private void NotEnoughCoin()
    {
        buyButton.interactable = false;
        notEnough.SetActive(true);
        Invoke(nameof(DisableEnoughCoin), 2f);
    }

    private void DisableEnoughCoin()
    {
        notEnough.SetActive(false);
        buyButton.interactable = true;

    }

    private void FreeBlock()
    {
        SelectBlockIcon();
        System.DateTime dateTime = DateTime.Now;
        int result = int.Parse(dateTime.ToString("yyyyMMdd"));
        gameDataManager.GetSaveBlockIconData(showIndex, result);
        gameDataManager.SavePlayerData();
    }
}
