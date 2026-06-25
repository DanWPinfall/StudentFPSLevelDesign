using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelSelectManager : MonoBehaviour
{
    [Header("Levels")]
    public LevelData[] levels;

    [Header("Preview")]
    public Transform previewSpawnPoint;

    [Header("UI")]
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI creatorNameText;
    public TextMeshProUGUI completedText;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI controlsText;

    public GameObject settingsMenu;

    private int currentIndex;
    private GameObject currentPreview;

    private void Start()
    {
        SaveManager.Load();
        ShowLevel();
        controlsText.text = "A/D | SPACE TO SELECT | C FOR SETTINGS ";
    }

    private void Update()
    {
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            PreviousLevel();
        }

        if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            NextLevel();
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            LoadSelectedLevel();
        }
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            if (settingsMenu.activeInHierarchy)
            {
                HideSettings();
            }
            else
            {
                ShowSettings();
            }
        }
    }

    void NextLevel()
    {
        currentIndex++;

        if (currentIndex >= levels.Length)
            currentIndex = 0;

        ShowLevel();
    }

    void PreviousLevel()
    {
        currentIndex--;

        if (currentIndex < 0)
            currentIndex = levels.Length - 1;

        ShowLevel();
    }

    void ShowLevel()
    {
        if (currentPreview != null)
            Destroy(currentPreview);

        currentPreview = Instantiate(
            levels[currentIndex].levelPrefab,
            previewSpawnPoint.position,
            previewSpawnPoint.rotation);

        currentPreview.transform.localScale = levels[currentIndex].previewScale;
        currentPreview.transform.parent = previewSpawnPoint;


        levelNameText.text = levels[currentIndex].displayName;
        creatorNameText.text = levels[currentIndex].creatorName;

        var saveData = SaveManager.GetLevelData(levels[currentIndex].sceneName);

        if (saveData != null)
        {
            completedText.text = saveData.completed ? "Completed" : "Not Completed";

            bestTimeText.text = saveData.bestTime > 0
                ? $"{saveData.bestTime:F2}s"
                : "--";
        }
        else
        {
            completedText.text = "Not Completed";
            bestTimeText.text = "--";
        }
    }

    void LoadSelectedLevel()
    {
        SceneManager.LoadScene(levels[currentIndex].sceneName);
    }

    void ShowSettings()
    {
        settingsMenu.SetActive(true);
    }

    void HideSettings()
    {
        settingsMenu.SetActive(false);
    }
}