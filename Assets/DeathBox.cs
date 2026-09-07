using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathBox : MonoBehaviour
{
    public Scene activeScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            SceneManager.LoadScene(activeScene.name);
        }
    }
}
