using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum ShopItemCode
{
    no_Ads,
    Coin,
    Life,
    Freeze,
    Realign,
    D_Bomb,
    Hammer,
    Thunder,
    Destruction
}




[Serializable]
public class ShopData
{
    public ShopItemCode shopItemCode;
    public int count;
}



[Serializable]
public class ShopItemData
{
    public string itemCodeID;
    public ShopData[] shopData;

}

public class ShopManager : MonoBehaviour
{
    public ShopItemData noAdsData;
    public ShopItemData[] shopItemAllData;


    public ShopItem noAdsItem;
    public ShopItem[] shopItems;
    public GameObject shopItemHolder;
    public GameObject offlineHolder;

    private bool isInitialize;
    //private IStoreController storeController;
    private GameDataManager gameDataManager;

    // Start is called before the first frame update
    void Start()
    {
        //UnityServices.InitializeAsync();
        shopItemHolder.SetActive(false);
        offlineHolder.SetActive(true);
        gameDataManager = BlockManager.Instance.gameDataManager;
        

        noAdsItem.SetUp(this, noAdsData);
        for (int i = 0; i < shopItemAllData.Length; i++)
        {
            if (shopItems[i] != null)
            {
                if(shopItemAllData[i] != null)
                    shopItems[i].SetUp(this, shopItemAllData[i]);
                else
                {
                    shopItems[i].gameObject.SetActive(false);
                }
            }
        }
        SetUpBuilder();
    }

    public void SetUp()
    {
        if (!isInitialize)
        {
            offlineHolder.SetActive(true);
            shopItemHolder.SetActive(false);
            SetUpBuilder();
        }
        else
        {
            shopItemHolder.SetActive(true);
            offlineHolder.SetActive(false);


            if (noAdsItem != null)
                noAdsItem.ResetLayout();

            for (int i = 0; i < shopItemAllData.Length; i++)
            {
                if (shopItems[i] != null)
                {
                    if (shopItemAllData[i] != null)
                        shopItems[i].ResetLayout();
                }
            }
        }
    }

    private void SetUpBuilder()
    {
        //var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        //if (!string.IsNullOrEmpty(noAdsData.itemCodeID))
        //{
        //    builder.AddProduct(noAdsData.itemCodeID, ProductType.NonConsumable);
        //}
        //foreach (var item in shopItemAllData)
        //{
        //    if (!string.IsNullOrEmpty(item.itemCodeID))
        //    {
        //        builder.AddProduct(item.itemCodeID, ProductType.Consumable);
        //    }
        //}
        //UnityPurchasing.Initialize(this, builder);
    }

    public void ShopBuyItem(string shopitemID)
    {
        //storeController.InitiatePurchase(shopitemID);
    }

    private void CheckButItem(string productID)
    {
        if (string.Equals(noAdsData.itemCodeID, productID))
        {
            if (noAdsData.shopData.Length > 0)
                ItemBuyed(noAdsData.shopData[0]);
        }
        else
        {
            foreach (var item in shopItemAllData)
            {
                if (string.Equals(item.itemCodeID, productID))
                {
                    foreach(var item2 in item.shopData)
                    {
                        ItemBuyed(item2);
                    }
                    break;
                }
            }
        }
    }


    private void ItemBuyed(ShopData shopdata)
    {
        if(shopdata.shopItemCode == ShopItemCode.no_Ads && !gameDataManager.HasDisableAds)
        {
            gameDataManager.SpecialData(101);
            noAdsItem.gameObject.SetActive(false);
            gameDataManager.SavePlayerData();
        }
        else
        {
            if (shopdata.shopItemCode == ShopItemCode.Coin)
            {
                gameDataManager.CoinValueChange(shopdata.count, true);

            }else if (shopdata.shopItemCode == ShopItemCode.Life)
            {
                gameDataManager.LifeValueChange(shopdata.count, true);
            }
            else if (shopdata.shopItemCode == ShopItemCode.Freeze)
            {
                gameDataManager.AbilityCountValue(0, shopdata.count);
            }
            else if (shopdata.shopItemCode == ShopItemCode.Realign)
            {
                gameDataManager.AbilityCountValue(1, shopdata.count);
            }
            else if (shopdata.shopItemCode == ShopItemCode.D_Bomb)
            {
                gameDataManager.AbilityCountValue(2, shopdata.count);
            }
            else if (shopdata.shopItemCode == ShopItemCode.Hammer)
            {
                gameDataManager.AbilityCountValue(3, shopdata.count);
            }
            else if (shopdata.shopItemCode == ShopItemCode.Thunder)
            {
                gameDataManager.AbilityCountValue(4, shopdata.count);
            }
            else if (shopdata.shopItemCode == ShopItemCode.Destruction)
            {
                gameDataManager.AbilityCountValue(4, shopdata.count);
            }
        }

    }

    //public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
    //{
    //}

    //public void OnInitializeFailed(InitializationFailureReason error)
    //{
    //    print("initialize failed");

    //    isInitialize = false;
    //}

    //public void OnInitializeFailed(InitializationFailureReason error, string message)
    //{
    //    print("initialize failed" + message + "/" + error);

    //    isInitialize = false;
    //}

    //public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    //{
    //    var product = purchaseEvent.purchasedProduct;
    //    CheckButItem(product.definition.id);
    //    return PurchaseProcessingResult.Complete;
    //}

    //public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    //{
    //}

    //public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    //{
    //    isInitialize = true;
    //    shopItemHolder.SetActive(true);
    //    offlineHolder.SetActive(false);

    //    storeController = controller;
    //    var product = controller.products.WithID(noAdsData.itemCodeID);
    //    if(product != null && product.hasReceipt && !gameDataManager.HasDisableAds)
    //    {
    //        gameDataManager.SpecialData(101);
    //        noAdsItem.gameObject.SetActive(false);
    //        gameDataManager.SavePlayerData();

    //    }
    //    else
    //    {
    //        if(!gameDataManager.HasDisableAds)
    //            noAdsItem.gameObject.SetActive(true);
    //        else
    //        {
    //            noAdsItem.gameObject.SetActive(false);
    //        }
    //    }
    //    SetUpLocalizePrice();
    //}
    //private void SetUpLocalizePrice()
    //{
    //    if (noAdsItem != null && !string.IsNullOrEmpty(noAdsData.itemCodeID))
    //    {
    //        string[] values = ReturnLocalizePrice(noAdsData.itemCodeID);
    //        noAdsItem.SetUpLocalizePrice(values[0], values[1]);
    //    }
    //    for (int i = 0; i < shopItemAllData.Length; i++)
    //    {
    //        var item = shopItems[i];
    //        var data = shopItemAllData[i];
    //        if (item != null && !string.IsNullOrEmpty(data.itemCodeID))
    //        {
    //            string[] values = ReturnLocalizePrice(data.itemCodeID);
    //            item.SetUpLocalizePrice(values[0], values[1]);
    //        }
    //    }
    //}

    //private string[] ReturnLocalizePrice(string id)
    //{
    //    string[] result = new string[2];
    //    if (storeController == null || storeController.products == null)
    //    {
    //        return result;
    //    }

    //    Product product = storeController.products.WithID(id);
    //    if (product == null || product.metadata == null)
    //    {
    //        return result;
    //    }

    //    string price = product.metadata.localizedPrice.ToString();
    //    result[0] = price;

    //    string code = product.metadata.isoCurrencyCode;
    //    string currencySymbol = GetCurrencySymbol(code);
    //    result[1] = currencySymbol;
    //    return result;
    //}

    //private string GetCurrencySymbol(string currencyCode)
    //{
    //    // A simple mapping of common currency codes to symbols
    //    switch (currencyCode)
    //    {
    //        case "USD": return "\u0024";
    //        case "EUR": return "\u20AC";
    //        case "GBP": return "\u00A3";
    //        case "INR": return "\u20B9";
    //        // Add more mappings as needed
    //        default: return currencyCode; // Use the currency code if no symbol is found
    //    }
    //}
}
