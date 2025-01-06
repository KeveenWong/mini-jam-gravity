using UnityEngine;
using TMPro;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem current;
    public TextMeshProUGUI tooltipText;
    public RectTransform tooltipRect;
    
    // Position where you want the tooltip to appear
    // public Vector2 fixedPosition = new Vector2(800, 400); // Adjust these values in the Unity Inspector

    private void Awake()
    {
        current = this;
        HideTooltip();
    }

    public static void Show(string content, Vector2 position)
    {
        if (current != null)
        {
            current.tooltipText.text = content;
            current.tooltipRect.gameObject.SetActive(true);
            
            // // Use the fixed position instead of calculating from item position
            // current.tooltipRect.position = current.fixedPosition;
        }
    }

    public static void Hide()
    {
        if (current != null)
        {
            current.HideTooltip();
        }
    }

    private void HideTooltip()
    {
        tooltipRect.gameObject.SetActive(false);
    }
}
