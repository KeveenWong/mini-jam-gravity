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
    [SerializeField] private GameObject title3DText; // Reference to the 3D title text
    [SerializeField] private GameObject instructions3DText; // Reference to the 3D instructions text
    
    private bool gameStarted = false;
    
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
    
    private void Start()
    {
        // Ensure UI is visible at the start
        if (title3DText != null) title3DText.SetActive(true);
        if (instructions3DText != null) instructions3DText.SetActive(true);

        // Freeze the entire scene
        FreezeScene();
    }
    
    private void Update()
    {
        // Listen for WASD input to start the game
        if (!gameStarted && CheckInput())
        {
            StartGame();
        }
    }

    // private bool CheckInput()
    // {
    //     return Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) ||
    //            Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D);
    // }
    
    private bool CheckInput()
    {
        return Input.GetKeyDown(KeyCode.U);
    }

    private void StartGame()
    {
        gameStarted = true;

        // Hide 3D UI elements
        if (title3DText != null) title3DText.SetActive(false);
        if (instructions3DText != null) instructions3DText.SetActive(false);

        // Allow player movement
        var playerController = FindObjectOfType<FirstPersonController>();
        if (playerController != null)
        {
            playerController.playerCanMove = true;
        }

        UnfreezeScene();

    }

    private void FreezeScene()
    {
        // Freeze time
        Time.timeScale = 0f;

        // Optional: Disable player input manually (if needed)
        var playerController = FindObjectOfType<FirstPersonController>();
        if (playerController != null)
        {
            playerController.playerCanMove = false;
        }
    }

    private void UnfreezeScene()
    {
        // Unfreeze time
        Time.timeScale = 1f;

        // Enable player input manually (if needed)
        var playerController = FindObjectOfType<FirstPersonController>();
        if (playerController != null)
        {
            playerController.playerCanMove = true;
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

    public int GetCurrency()
    {
        return currency;
    }

    public void SpendCurrency(int amount)
    {
        currency -= amount;
        UpdateScoreDisplay();
    }
}
