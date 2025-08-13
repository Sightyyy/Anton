using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public int LevelsCompleted { get; private set; }
    public int TotalDeaths { get; private set; }
    public int TotalEnemiesKilled { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadPlayerData();
    }

    public void CompleteLevel()
    {
        LevelsCompleted++;
        SavePlayerData();
        Debug.Log($"Level {LevelsCompleted} completed!");
    }

    public void RecordDeath()
    {
        TotalDeaths++;
        SavePlayerData();
    }

    public void RecordEnemyKill()
    {
        TotalEnemiesKilled++;
        SavePlayerData();
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("All levels completed!");
            // Trigger game completion logic here
        }
    }

    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("LevelsCompleted", LevelsCompleted);
        PlayerPrefs.SetInt("TotalDeaths", TotalDeaths);
        PlayerPrefs.SetInt("TotalEnemiesKilled", TotalEnemiesKilled);
        PlayerPrefs.Save();
    }

    private void LoadPlayerData()
    {
        LevelsCompleted = PlayerPrefs.GetInt("LevelsCompleted", 0);
        TotalDeaths = PlayerPrefs.GetInt("TotalDeaths", 0);
        TotalEnemiesKilled = PlayerPrefs.GetInt("TotalEnemiesKilled", 0);
    }

    public void ResetProgress()
    {
        LevelsCompleted = 0;
        TotalDeaths = 0;
        TotalEnemiesKilled = 0;
        PlayerPrefs.DeleteAll();
    }
}