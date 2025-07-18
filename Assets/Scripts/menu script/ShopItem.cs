using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    private ShopItemData shopItemData;
    [SerializeField] public Button buyButton;
    [SerializeField] public Text priceText;
    [SerializeField] public Text symbolText;
    [SerializeField] public HorizontalLayoutGroup horizontalLayoutGroup;
    [SerializeField] public ContentSizeFitter sizeFitter;
    private ShopManager shopManager;

    public void SetUp(ShopManager shopmanager, ShopItemData shopitemdata)
    {
        horizontalLayoutGroup.enabled = false;
        sizeFitter.enabled = true;
        shopManager = shopmanager;
        shopItemData = shopitemdata;
        buyButton.onClick.AddListener(BuyItem);
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
        sizeFitter.enabled = true;
        horizontalLayoutGroup.enabled = false;
        Invoke(nameof(Change), 0.3f);
    }

    private void Change()
    {
        sizeFitter.enabled = false;
        horizontalLayoutGroup.enabled = true;
    }
}
