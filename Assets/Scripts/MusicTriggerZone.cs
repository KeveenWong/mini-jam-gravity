using UnityEngine;

public class MusicTriggerZone : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float maxVolume = 0.3f; // Adjust this value in inspector to set max volume
    
    private static bool musicStarted = false; // Static so it persists even if trigger is re-entered
    
    private void Start()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = musicStarted ? maxVolume : 0f; // Keep playing at max volume if already started
        
        if (musicStarted && !musicSource.isPlaying)
        {
            musicSource.volume = maxVolume;
            musicSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !musicStarted)
        {
            musicStarted = true;
            musicSource.Play();
            StartCoroutine(FadeInMusic());
        }
    }

    private System.Collections.IEnumerator FadeInMusic()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(0f, maxVolume, elapsedTime / fadeInDuration);
            musicSource.volume = newVolume;
            yield return null;
        }
        musicSource.volume = maxVolume;
    }
}