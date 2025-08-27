using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    [SerializeField] GameObject nScreen, rScreen, gachaCanvas, provisionPanel, notEnoughCurrencyPanel;
    [SerializeField] Button backButton;
    [SerializeField] TextMeshProUGUI fragmentText, amountText;
    [SerializeField] Image nBanner, rBanner;

    int fragmentItemNum, amountNum;

    HomeManager homeManager;
    GachaMove gachaMove;

    void Start()
    {
        gachaCanvas.SetActive(false);

        homeManager = FindObjectOfType<HomeManager>();
        gachaMove = FindObjectOfType<GachaMove>();
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
        nBanner.color = Color.cyan;
        rBanner.color = Color.white;
    }

    // レアガチャが押されたら表示
    public void PushRareBanner()
    {
        nScreen.SetActive(false);
        rScreen.SetActive(true);
        nBanner.color = Color.white;
        rBanner.color = Color.cyan;
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

    // ショップに遷移
    public void ShopTransition()
    {
        notEnoughCurrencyPanel.SetActive(false);
    }

    // 通貨不足パネル表示
    public void OpenCurrencyPanel()
    {
        notEnoughCurrencyPanel.SetActive(true);
        backButton.interactable = false;
    }

    // 通貨不足パネル非表示
    public void CloseCurrencyPanel()
    {
        notEnoughCurrencyPanel.SetActive(false);
        backButton.interactable = true;
        gachaMove.ActiveButton();
    }
}
