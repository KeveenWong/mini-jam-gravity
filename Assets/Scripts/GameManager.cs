using UnityEngine;
using TMPro; // If you want to display the score in UI

public class GameManager : MonoBehaviour
{
    // Singleton pattern - allows easy access from other scripts
    public static GameManager Instance { get; private set; }

    [Header("Score")]
    private int currency = 0;
    
    // Optional: Reference to UI Text to display score
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScore(int amount)
    {
        currency += amount;
        UpdateScoreDisplay();
        Debug.Log($"Score: {currency}"); // For testing
    }

    private void UpdateScoreDisplay()
    {
        // If you have UI text referenced, update it
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currency}";
        }
    }

    public int GetScore()
    {
        return currency;
    }
}
