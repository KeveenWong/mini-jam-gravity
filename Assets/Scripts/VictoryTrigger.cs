using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    // Reference to our Audio Source component
    [SerializeField] private AudioSource victorySound;
    
    // Flag to ensure sound only plays once
    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        // Check if this is the player AND we haven't played the sound yet
        if (other.CompareTag("Player") && !hasPlayed)
        {
            // Make sure we have a reference to the audio source
            if (victorySound != null)
            {
                victorySound.Play();
                hasPlayed = true;
            }
            else
            {
                Debug.LogError("Victory Sound not assigned! Please assign in inspector.");
            }
        }
    }
}