using UnityEngine;
using MaskTransitions;

public class ClickToSwitchScene : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoadName;
    public float transitionTime = 2f;

    private SceneSwitch sceneSwitch;

    private void Start()
    {
        // Create the SceneSwitch component dynamically
        GameObject switchObj = new GameObject("ClickSceneSwitch");
        sceneSwitch = switchObj.AddComponent<SceneSwitch>();
        sceneSwitch.sceneToLoadName = sceneToLoadName;
        sceneSwitch.totalTransitionTime = transitionTime;
    }

    private void OnMouseDown()
    {
        sceneSwitch.SwitchScene();
    }
}
