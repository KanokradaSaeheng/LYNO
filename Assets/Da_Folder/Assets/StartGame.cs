using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public string sceneToLoad = "Da_Scene"; 

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
