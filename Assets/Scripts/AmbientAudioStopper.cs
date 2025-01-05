using UnityEngine;

public class AmbientAudioStopper : MonoBehaviour
{
    // We don't need to manually assign the audio source since we'll find it automatically
    private AudioSource ambientAudioSource;

    private void Start()
    {
        // Find the AmbientAudio object in the scene and get its AudioSource component
        // This way, we don't need to manually drag anything in the Inspector
        GameObject ambientAudioObject = GameObject.Find("AmbientAudio");
        
        // Make sure we found the object before trying to use it
        if (ambientAudioObject != null)
        {
            ambientAudioSource = ambientAudioObject.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogWarning("Could not find AmbientAudio object in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if it's the player entering the trigger
        if (other.CompareTag("Player") && ambientAudioSource != null)
        {
            // Stop the ambient audio
            // ambientAudioSource.Stop();
            
            // You could also fade it out instead of stopping it immediately
            StartCoroutine(FadeOutAudio());
        }
    }

    // Optional: If you want to fade out the audio instead of stopping it immediately,
    // uncomment this coroutine and call it instead of ambientAudioSource.Stop()
    
    private System.Collections.IEnumerator FadeOutAudio()
    {
        float startVolume = ambientAudioSource.volume;
        float fadeTime = 1f; // How long the fade takes in seconds
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            ambientAudioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeTime);
            yield return null;
        }

        // Make sure we end at zero volume and stop the audio
        ambientAudioSource.volume = 0f;
        ambientAudioSource.Stop();
    }
}