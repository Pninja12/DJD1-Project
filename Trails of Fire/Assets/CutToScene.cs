using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Debug log to check which object enters the trigger zone
        Debug.Log("Collider entered: " + other.gameObject.name);

        // Check if the collider that entered is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            // Debug log to confirm player entry
            Debug.Log("Player entered the trigger zone");

            // Load the scene called "Cutscene"
            SceneManager.LoadSceneAsync("Cutscene");
        }
    }
}
