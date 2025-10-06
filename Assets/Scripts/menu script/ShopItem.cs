using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ShopItemHolder
{
    public GameObject holderItem;
    public Image iconItem;
    public Image bgItem;
    public Text countItem;
    public ParticleSystem particleSystem;
    public void SetUp(Sprite icon, int count, Color col)
    {
        holderItem.SetActive(true);
        iconItem.sprite = icon;
        countItem.text = "x" + count.ToString();
        bgItem.color = col;
        ParticleSystem.MainModule main = particleSystem.main;
        main.startColor = new ParticleSystem.MinMaxGradient(col);

        countItem.gameObject.SetActive(true);
        if (count <= 0)
        {
            countItem.gameObject.SetActive(false);
        }
    }
}

public class ShopItem : MonoBehaviour
{
    private ShopItemData shopItemData;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text priceText;
    [SerializeField] private Text symbolText;
    [SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] private ShopItemHolder[] shopItemHolder;
    [SerializeField] private Text itemPackName;
    private ShopManager shopManager;

    public void SetUp(ShopManager shopmanager, ShopItemData shopitemdata)
    {
        DisableShopHolders();
        horizontalLayoutGroup.enabled = false;
        shopManager = shopmanager;
        shopItemData = shopitemdata;
        buyButton.interactable = false;
        //buyButton.onClick.AddListener(BuyItem);
        itemPackName.text = shopitemdata.itemName;
        for (int i = 0; i < shopitemdata.shopData.Length; i++)
        {
            var item = shopitemdata.shopData[i];
            if (item.shopItemCode != ShopItemCode.None && i < shopItemHolder.Length)
            {
                shopItemHolder[i].SetUp(item.iconImage, item.count, item.colCode);
            }
        }
        SetUpLocalizePrice("00.00", "$");
    }
    private void DisableShopHolders()
    {
        foreach (var item in shopItemHolder)
        {
            item.holderItem.SetActive(false);
        }
    }


    private void BuyItem()
    {
        shopManager.ShopBuyItem(shopItemData.itemCodeID);
    }

    public void SetUpLocalizePrice(string price, string symbol)
    {

        priceText.text = price;
        symbolText.text = symbol;

    }

    public void ResetLayout()
    {
        horizontalLayoutGroup.enabled = false;
        Invoke(nameof(Change), 0.3f);
    }

    private void Change()
    {
        horizontalLayoutGroup.enabled = true;
    }
}
