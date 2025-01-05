using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSystem : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject interactionPrompt;
    public GameObject shopMenu;
    public TextMeshProUGUI currencyText;
    public Transform itemContainer;  // Parent object for shop items
    public GameObject shopItemPrefab;  // Prefab for each shop item

    [Header("Shop Items")]
    public ShopItem[] availableItems;

    [Header("Interaction Settings")]
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;

    private bool isPlayerInRange = false;
    private bool isShopOpen = false;

    private void Start()
    {
        // Ensure UI elements start hidden
        if (interactionPrompt != null) interactionPrompt.SetActive(false);
        if (shopMenu != null) shopMenu.SetActive(false);
        
        // Create shop items
        CreateShopItems();
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            ToggleShop();
        }

        // Update currency display when shop is open
        if (isShopOpen && currencyText != null)
        {
            UpdateCurrencyDisplay();
        }
    }

    private void CreateShopItems()
    {
        if (itemContainer == null || shopItemPrefab == null) return;

        // Clear existing items
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        // Create new items
        foreach (ShopItem item in availableItems)
        {
            GameObject newItem = Instantiate(shopItemPrefab, itemContainer);
            
            // Set up item UI
            Image itemImage = newItem.transform.Find("ItemImage")?.GetComponent<Image>();
            TextMeshProUGUI itemName = newItem.transform.Find("ItemName")?.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI itemCost = newItem.transform.Find("ItemCost")?.GetComponent<TextMeshProUGUI>();
            Button buyButton = newItem.transform.Find("BuyButton")?.GetComponent<Button>();

            if (itemImage != null) itemImage.sprite = item.itemImage;
            if (itemName != null) itemName.text = item.itemName;
            if (itemCost != null) itemCost.text = $"{item.cost} coins";
            
            if (buyButton != null)
            {
                // Store item reference for the button click
                ShopItem capturedItem = item;
                buyButton.onClick.AddListener(() => TryPurchaseItem(capturedItem));
            }
        }
    }

    private void TryPurchaseItem(ShopItem item)
    {
        if (GameManager.Instance.GetCurrency() >= item.cost)
        {
            GameManager.Instance.SpendCurrency(item.cost);
            UpdateCurrencyDisplay();  // Update currency after purchase
            // TODO: Add logic to give the item to the player
            Debug.Log($"Purchased {item.itemName}");
        }
        else
        {
            Debug.Log("Not enough coins!");
        }
    }

    private void UpdateCurrencyDisplay()
    {
        if (currencyText != null)
        {
            currencyText.text = $"Coins: {GameManager.Instance.GetCurrency()}";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
            if (isShopOpen)
            {
                ToggleShop();
            }
        }
    }

    public void ToggleShop()
    {
        isShopOpen = !isShopOpen;
        if (shopMenu != null)
        {
            shopMenu.SetActive(isShopOpen);
            if (isShopOpen)
            {
                UpdateCurrencyDisplay();  // Update currency when shop opens
            }
            
            // Handle cursor visibility
            Cursor.visible = isShopOpen;
            Cursor.lockState = isShopOpen ? CursorLockMode.None : CursorLockMode.Locked;
            
            // You might want to pause the game or disable player movement when shop is open
            Time.timeScale = isShopOpen ? 0f : 1f;
        }
    }
}