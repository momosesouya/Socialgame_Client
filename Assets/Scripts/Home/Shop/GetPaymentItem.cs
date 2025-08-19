using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetPaymentItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI paymentText;

    void Start()
    {
        var id = PaymentShops.GetShopData(5001);
        Debug.Log(id.product_name);

        PaymentShopModel[] lists = PaymentShops.GetShopDataAll();
        Debug.Log(lists.Length);
        int count = 0;
        foreach (var element in lists)
        {
            if (count > 0)
            {
                PaymentShopModel paymentShopModel = lists[count];
                string itemName = string.Format("product_id:{0}", paymentShopModel.product_id);
                Debug.Log(itemName);
            }
            count++;
        }
    }
}
