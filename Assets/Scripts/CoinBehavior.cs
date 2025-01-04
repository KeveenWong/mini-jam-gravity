using UnityEngine;

public class CoinBehavior : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 100f; // Degrees per second
    [SerializeField] private Vector3 rotationAxis = new Vector3(1f, 1f, 0f); // Rotate around both X and Y axes

    [Header("Coin Settings")]
    [SerializeField] private int scoreValue = 1; // How many points this coin is worth

    [Header("Sound Settings")]
    [SerializeField] private AudioClip pickupSound; // Drag your sound file here
    [SerializeField] private float volume = 1f; // Adjust volume in inspector

    private void Update()
    {
        // Rotate the coin around its axis
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // Play pickup sound
            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
            }

            // Add score through the GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddScore(scoreValue);
            }
            
            // Destroy the coin
            Destroy(gameObject);
        }
    }
}
