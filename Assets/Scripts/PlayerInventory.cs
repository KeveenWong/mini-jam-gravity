using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory instance;
    public static PlayerInventory Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerInventory>();
                if (instance == null)
                {
                    GameObject go = new GameObject("PlayerInventory");
                    instance = go.AddComponent<PlayerInventory>();
                }
            }
            return instance;
        }
    }

    private Dictionary<string, int> purchasedItems = new Dictionary<string, int>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddItem(string itemName)
    {
        if (!purchasedItems.ContainsKey(itemName))
        {
            purchasedItems[itemName] = 1;
        }
        else
        {
            purchasedItems[itemName]++;
        }
    }

    public bool HasItem(string itemName)
    {
        return purchasedItems.ContainsKey(itemName) && purchasedItems[itemName] > 0;
    }

    public int GetPurchaseCount(string itemName)
    {
        return purchasedItems.ContainsKey(itemName) ? purchasedItems[itemName] : 0;
    }
}
