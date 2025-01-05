using UnityEngine;

public class MusicTriggerZone : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource; // Reference to our AudioSource component
    [SerializeField] private float fadeInDuration = 1f; // How long it takes for the music to fade in
    
    private bool hasBeenTriggered = false; // Tracks if the trigger has been activated
    private float currentVolume = 0f; // Keeps track of current volume during fade
    
    private void Start()
    {
        // If no AudioSource was assigned in the inspector, we'll create one
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Set up our AudioSource with the proper configuration
        musicSource.loop = true; // Enable looping so the music plays continuously
        musicSource.playOnAwake = false; // We don't want the music to start until triggered
        musicSource.volume = 0f; // Start with no volume - we'll fade it in
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only respond to the player, and only if we haven't been triggered before
        if (other.CompareTag("Player") && !hasBeenTriggered)
        {
            hasBeenTriggered = true; // Mark that we've been triggered
            musicSource.Play(); // Start playing the music
            StartCoroutine(FadeInMusic()); // Begin the fade-in effect
        }
    }

    private System.Collections.IEnumerator FadeInMusic()
    {
        float elapsedTime = 0f;
        
        // Gradually increase the volume over fadeInDuration seconds
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            musicSource.volume = newVolume;
            yield return null;
        }
        
        // Ensure we end exactly at full volume
        musicSource.volume = 0.75f;
    }
}