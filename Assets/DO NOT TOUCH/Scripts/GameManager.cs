using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int enemiesRemaining;
    float levelTimer;

    void Awake()
    {
        Instance = this;
        SaveManager.Load();
    }

    void Update()
    {
        levelTimer += Time.deltaTime;
    }

    public void RegisterEnemy()
    {
        enemiesRemaining++;
    }

    public void EnemyKilled()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            LevelCompleted();
        }
    }

    void LevelCompleted()
    {
        string sceneName =
            SceneManager.GetActiveScene().name;

        SaveManager.CompleteLevel(sceneName, levelTimer);

        SceneManager.LoadScene("LevelSelect");
    }
}