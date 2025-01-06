using UnityEngine;

public class GameSettings : MonoBehaviour
{
    private static GameSettings instance;
    public static GameSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameSettings>();
                if (instance == null)
                {
                    GameObject go = new GameObject("GameSettings");
                    instance = go.AddComponent<GameSettings>();
                    DontDestroyOnLoad(go);
                }
            }
            return instance;
        }
    }

    public float baseDashCooldown = 2f; // Set this to match your default dashCooldownReset
    public float currentDashCooldown;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize with base cooldown
        currentDashCooldown = baseDashCooldown;
    }

    public void UpdateDashCooldown()
    {
        float reduction = PlayerInventory.Instance.GetPurchaseCount("Dash Cooldown") * 0.5f;
        currentDashCooldown = Mathf.Max(baseDashCooldown - reduction, 0.1f);
    }
}
