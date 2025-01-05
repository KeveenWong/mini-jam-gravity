using UnityEngine;

public class TextureScroller : MonoBehaviour
{
    // Speed of scrolling for X and Y directions
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.5f, 0f);
    
    // Reference to the material we'll be manipulating
    private Material material;
    
    // Store the offset values
    private Vector2 textureOffset = Vector2.zero;
    
    void Start()
    {
        // Get the material from the renderer
        // We use sharedMaterial if we want all objects with this material to scroll together
        // Use material if you want this specific instance to scroll independently
        material = GetComponent<Renderer>().material;
        
        // Store initial offset if there is one
        textureOffset = material.mainTextureOffset;
    }
    
    void Update()
    {
        // Calculate new offset based on time and speed
        // Time.time gives us the total time since game start
        // We multiply by scrollSpeed to control the movement rate
        textureOffset += scrollSpeed * Time.deltaTime;
        
        // The following line ensures our values stay between 0 and 1
        // This prevents potential floating-point precision issues over time
        textureOffset = new Vector2(textureOffset.x % 1, textureOffset.y % 1);
        
        // Apply the new offset to the material
        material.mainTextureOffset = textureOffset;
    }
    
    void OnDestroy()
    {
        // Clean up: Reset the texture offset when the object is destroyed
        if (material != null)
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }
}