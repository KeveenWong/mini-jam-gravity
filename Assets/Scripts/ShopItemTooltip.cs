using UnityEngine;
using UnityEngine.EventSystems;

public class ShopItemTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private string itemDescription;

    public void SetDescription(string description)
    {
        itemDescription = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(itemDescription))
        {
            TooltipSystem.Show(itemDescription, transform.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Hide();
    }
}
