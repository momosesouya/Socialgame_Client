using TMPro;
using UnityEngine;

public class GetPaymentItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI paymentText;
    const int productId = 5001;

    void Start()
    {
        var id = PaymentShops.GetShopData(productId);

        PaymentShopModel[] lists = PaymentShops.GetShopDataAll();
        int count = 0;
        foreach (var element in lists)
        {
            if (count > 0)
            {
                PaymentShopModel paymentShopModel = lists[count];
                string itemName = string.Format("product_id:{0}", paymentShopModel.product_id);
            }
            count++;
        }
    }
}
