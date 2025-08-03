using UnityEngine;

public class ChangeHomeMode : MonoBehaviour
{
    enum HomeMode { Home, Shop, Bag }
    HomeMode currentMode = HomeMode.Home;

    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject bagPanel;
    [SerializeField] GameObject gachaPanel;
    [SerializeField] HomeManager homeManager;

    GachaManager gachaManager;

    void Awake()
    {
        currentMode = HomeMode.Home;
        shopPanel.SetActive(false);
        bagPanel.SetActive(false);

        gachaManager = FindObjectOfType<GachaManager>();
    }

    // ホームが選択された時
    public void ChoiceHome()
    {
        homeManager.GetHomeData();
        currentMode = HomeMode.Home;
        shopPanel.SetActive(false);
        bagPanel.SetActive(false);
    }

    // ショップが選択された時
    public void ChoiceShop()
    {
        homeManager.GetHomeData();
        currentMode = HomeMode.Shop;
        shopPanel.SetActive(true);
        bagPanel.SetActive(false);
        gachaPanel.SetActive(false);
        gachaManager.CloseCurrencyPanel();
    }

    // バッグが選択された時
    public void ChoiceBag()
    {
        homeManager.GetHomeData();
        currentMode = HomeMode.Bag;
        bagPanel.SetActive(true);
        shopPanel.SetActive(false);
    }
}
