using UnityEngine;
using System.Collections;
using MaskTransitions;

public class SceneCollisionLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoadName;
    public float transitionTime = 2f;

    private SceneSwitch sceneSwitch;

    private void Awake()
    {
        // Create a SceneSwitch object programmatically
        GameObject switchObject = new GameObject("AutoSceneSwitch");
        sceneSwitch = switchObject.AddComponent<SceneSwitch>();
        sceneSwitch.sceneToLoadName = sceneToLoadName;
        sceneSwitch.totalTransitionTime = transitionTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LevelEnd"))
        {
            // Just switch scene directly — it already handles transition
            sceneSwitch.SwitchScene();
        }
    }
}
