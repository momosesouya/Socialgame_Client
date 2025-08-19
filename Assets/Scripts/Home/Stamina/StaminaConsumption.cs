using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

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
    [SerializeField] TextMeshProUGUI staminaText;
    UsersModel usersModel;

    int currentStamina;
    int has_enhancement_item;
    string user_id;
    string consumptionStr = "スタミナを5消費しました";
    string notConsumptionStr = "スタミナが足りません";
    string getItemStr = "武器強化アイテムを獲得しました";

    void Start()
    {
        usersModel = Users.Get();
        has_enhancement_item = Items.GetItemData(1002).has_enhancement_item;
    }

    void Update()
    {
        currentStamina = usersModel.last_stamina;
    }

    // クエストボタン押下時
    public void StaminaConsumptionMove()
    {
        if (currentStamina >= 5)
        {
            StartCoroutine(ConsumptionProcess());
        }
        else
        {
            Debug.Log(notConsumptionStr);
        }
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
            Debug.LogError("通信エラー: " + request.error);
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
            int updatedNum = item.has_enhancement_item + response.hasItem;
            Items.UpdateItemData(1002, updatedNum);

            Debug.Log($"スタミナを{5}消費。残り: {currentStamina}、獲得アイテムID: {response.item_id} x{response.hasItem}");

            Debug.Log(consumptionStr);
            Debug.Log(getItemStr);
        }
        else
        {
            Debug.LogWarning("レスポンスの解析に失敗しました");
        }
    }
}