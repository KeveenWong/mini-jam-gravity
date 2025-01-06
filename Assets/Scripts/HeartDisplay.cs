using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HeartDisplay : MonoBehaviour
{
  public GameObject heartPrefab; 
  public Transform heartContainer; 
  public float heartSpacing = 30f; 


  private List<GameObject> hearts = new List<GameObject>();

  public void InitializeHearts(int maxLives)
  {
    // Clear existing hearts
    foreach (GameObject heart in hearts)
    {
      Destroy(heart);
    }
    hearts.Clear();

    // Instantiate new hearts
    for (int i = 0; i < maxLives; i++)
    {
      GameObject heart = Instantiate(heartPrefab, heartContainer);
      heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(i * heartSpacing + 400, 350);
      hearts.Add(heart);
    }
  }

  public void UpdateHearts(int lives)
  {
    for (int i = 0; i < hearts.Count; i++)
    {
      Image heartImage = hearts[i].GetComponent<Image>();
      if (i < lives)
      {
        heartImage.color = Color.white; // Full heart
      }
      else
      {
        heartImage.color = Color.black; // Empty heart
      }
    }
  }
}