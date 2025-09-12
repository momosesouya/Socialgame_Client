using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class StaminaResponse
{
    public int item_id;
    public int last_stamina;
    public int hasItem;
}

public class StaminaConsumption : MonoBehaviour
{
    [SerializeField] Slider staminaBar;
    [SerializeField] TextMeshProUGUI staminaText, consumptionText, itemText, errorText;
    [SerializeField] GameObject questCompletePanel;
    UsersModel usersModel;

    int currentStamina;
    int has_enhancement_item;
    string consumptionStr = "スタミナを5消費しました";
    string notConsumptionStr = "スタミナが足りません";
    string getItemStr = "武器強化アイテムを獲得しました";
    const int usedStamina = 5;

    void Start()
    {
        usersModel = Users.Get();
        has_enhancement_item = Items.GetItemData(1002).has_enhancement_item;

        questCompletePanel.SetActive(false);
    }

    void Update()
    {
        currentStamina = usersModel.last_stamina;
    }

    // クエストボタン押下時
    public void StaminaConsumptionMove()
    {
        if (currentStamina >= usedStamina)
        {
            StartCoroutine(ConsumptionProcess());
            consumptionText.enabled = true;
            itemText.enabled = true;
            errorText.enabled = false;
            consumptionText.text = string.Format(consumptionStr);
            itemText.text = string.Format(getItemStr);
        }
        else
        {
            errorText.text = string.Format(notConsumptionStr);
            errorText.enabled = true;
            consumptionText.enabled = false;
            itemText.enabled = false;
        }
        StartCoroutine(QuestPanel(1.5f));
    }


    // スタミナ消費処理
    IEnumerator ConsumptionProcess()
    {
        List<IMultipartFormSection> postData = new List<IMultipartFormSection>
        {
            new MultipartFormDataSection("uid", usersModel.user_id)
        };

        UnityWebRequest request = UnityWebRequest.Post(GameUtil.Uri.Stamina_Consumption, postData);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }

        string json = request.downloadHandler.text;

        // JSON をオブジェクトに変換
        StaminaResponse response = JsonUtility.FromJson<StaminaResponse>(json);

        if (response != null)
        {
            // 取得したスタミナとUIの更新
            currentStamina = response.last_stamina;
            staminaBar.value = currentStamina;
            staminaText.text = currentStamina.ToString();

            // アイテムの保存
            ItemsModel item = Items.GetItemData(1002);
            int updatedNum = response.hasItem;
            Items.UpdateItemData(1002, updatedNum);
        }
    }

    IEnumerator QuestPanel(float seconds)
    {
        questCompletePanel.SetActive(true);
        yield return new WaitForSeconds(seconds);
        questCompletePanel.SetActive(false);
    }
}