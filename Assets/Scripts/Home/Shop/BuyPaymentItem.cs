using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BuyPaymentItem : MonoBehaviour
{
    [SerializeField] GameObject buyButton;
    [SerializeField] TextMeshProUGUI productText;
    [SerializeField] string user_id;
    [SerializeField] string product_id;
    [SerializeField] GameObject buySuccessPanel;
    [SerializeField] GameObject PaymentShopPanel;
    string productStr;

    private void Start()
    {
        user_id = Users.Get().user_id;
        SetProductText();
    }

    public void PushBuyButton()
    {
        buyButton.SetActive(false);
        List<IMultipartFormSection> buyform = new();
        buyform.Add(new MultipartFormDataSection("uid", user_id));
        buyform.Add(new MultipartFormDataSection("pid", product_id));
        Action afterAction = new(() => SuccessBuy());
        StartCoroutine(CommunicationManager.ConnectServer(GameUtil.Uri.Buy_Currency, buyform, afterAction));
        Wallets.Get(user_id);
        Invoke("DisplayButton", 2);
    }

    void DisplayButton()
    {
        buyButton.SetActive(true);
    }

    void SuccessBuy()
    {
        HomeManager homeManager = FindObjectOfType<HomeManager>();
        if (homeManager != null)
        {
            homeManager.RefreshWalletsText();
        }
        else
        {
            Debug.LogWarning("HomeManager が見つかりませんでした。ウォレット表示の更新ができません。");
        }

        buySuccessPanel.SetActive(true);
        Invoke("HideBuySuccessPanel", 2);
    }

    void HideBuySuccessPanel()
    {
        buySuccessPanel.SetActive(false);
    }

    public void CloseShopPanel()
    {
        PaymentShopPanel.SetActive(false);
    }

    void SetProductText()
    {
        PaymentShopModel product = PaymentShops.GetShopData(int.Parse(product_id));
        productStr = string.Format("商品名:{0}\n{1}円\n有償分:{2}個\n無償分{3}個", product.product_name, product.price, product.paid_currency, product.bonus_currency);
        productText.text = productStr;
    }
}
