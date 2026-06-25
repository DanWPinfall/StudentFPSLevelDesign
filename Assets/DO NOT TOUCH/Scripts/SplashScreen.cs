using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public LevelSelectManager levelSelectManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelSelectManager.enabled = false;
    }

    // Update is called once per frame

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            CloseSplashScreen();
        }
    }

    public void CloseSplashScreen()
    {
        levelSelectManager.enabled = true;
        gameObject.SetActive(false);
    }
}
