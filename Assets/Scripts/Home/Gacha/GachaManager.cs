using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GachaManager : MonoBehaviour
{
    [SerializeField] GameObject nScreen, rScreen, gachaCanvas, provisionPanel, notEnoughCurrencyPanel;
    [SerializeField] TextMeshProUGUI fragmentText, amountText;
    [SerializeField] Image nBanner, rBanner;

    int fragmentItemNum, amountNum;

    HomeManager homeManager;

    void Start()
    {
        gachaCanvas.SetActive(false);

        homeManager = FindObjectOfType<HomeManager>();
    }

    void FixedUpdate() => UpdateNum();

    // 通貨などを更新
    void UpdateNum()
    {
        amountNum = Wallets.Get().free_amount + Wallets.Get().paid_amount;

        if (amountText != null && fragmentText != null)
        {
            amountText.text = amountNum.ToString();
            fragmentText.text = fragmentItemNum.ToString();
        }
    }

    // ノーマルガチャが押されたら表示
    // (デフォルトはノーマルガチャ)
    public void PushNormalBanner()
    {
        gachaCanvas.SetActive(true);
        nScreen.SetActive(true);
        rScreen.SetActive(false);
        //nBanner.tintColor = Color.yellow;
        //rBanner.tintColor = Color.white;
    }

    // レアガチャが押されたら表示
    public void PushRareBanner()
    {
        nScreen.SetActive(false);
        rScreen.SetActive(true);
        //nBanner.tintColor = Color.white;
        //rBanner.tintColor = Color.yellow;
    }

    // 戻るボタンが押されたら
    public void PushBackGachaButton()
    {
        homeManager.GetHomeData();
        gachaCanvas.SetActive(false);
    }

    // ガチャ排出確率ボタンが押されたら
    public void PushGachaEmissionProbabilityButton()
    {
        provisionPanel.SetActive(true);
    }

    // ガチャ履歴ボタンが押されたら
    public void PushGachaLogButton()
    {
        //gachaLogManager.GetGachaLog();
        //gachaLogManager.UpdateText();
        //gachaLogPanel.SetActive(true);
    }

    // ショップに遷移
    public void ShopTransition()
    {
        notEnoughCurrencyPanel.SetActive(false);
    }

    // 通貨不足パネル表示
    public void OpenCurrencyPanel()
    {
        notEnoughCurrencyPanel.SetActive(true);
    }

    // 通貨不足パネル非表示
    public void CloseCurrencyPanel()
    {
        notEnoughCurrencyPanel.SetActive(false);
    }
}
