using UnityEngine;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public Sprite itemImage;
    public int cost;
    public string description;
    public int maxPurchases = 1; // Default to 1 for single-purchase items
}
