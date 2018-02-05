using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugManager : MonoBehaviour
{
    void Awake()
    {
        if (Debug.isDebugBuild)
        {
            if (!FindObjectOfType<TimeManager>()) Debug.LogError("No Time Manager in scene!");
            if (!FindObjectOfType<GameManager>()) Debug.LogError("No Game Manager in scene!");
            if (SceneManager.GetActiveScene().buildIndex != 0){
                if (!FindObjectOfType<Arena>()) Debug.LogError("No Arena in scene!");
            }
        }
    }
}
