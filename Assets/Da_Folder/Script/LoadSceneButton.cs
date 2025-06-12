using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [Tooltip("Scene index to load from Build Settings")]
    public int sceneIndex;

    public void OnButtonClick()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}