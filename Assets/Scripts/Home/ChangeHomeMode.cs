using UnityEngine;

public class ChangeHomeMode : MonoBehaviour
{
    enum HomeMode { Home, Shop }
    HomeMode currentMode = HomeMode.Home;

    [SerializeField] GameObject shopPanel;
    [SerializeField] HomeManager homeManager;

    void Awake()
    {
        currentMode = HomeMode.Home;
        shopPanel.SetActive(false);
    }

    // ホームが選択された時
    public void ChoiceHome()
    {
        homeManager.GetHomeData();
        currentMode = HomeMode.Home;
        shopPanel.SetActive(false);
    }

    // ショップが選択された時
    public void ChoiceShop()
    {
        homeManager.GetHomeData();
        currentMode = HomeMode.Shop;
        shopPanel.SetActive(true);
    }
}
