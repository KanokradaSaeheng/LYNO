using UnityEngine;
using System.Collections;
using MaskTransitions; // Required for SceneSwitch

public class SceneTriggerHandler : MonoBehaviour
{
    public SceneSwitch sceneSwitch;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LevelEnd")) // Tag the object as "LevelEnd"
        {
            if (sceneSwitch != null)
            {
                StartCoroutine(HandleSceneTransition());
            }
            else
            {
                Debug.LogWarning("SceneSwitch reference not set!");
            }
        }
    }

    private IEnumerator HandleSceneTransition()
    {
        sceneSwitch.PlayTransition();
        yield return new WaitForSeconds(sceneSwitch.totalTransitionTime);
        sceneSwitch.SwitchScene();
    }
}
