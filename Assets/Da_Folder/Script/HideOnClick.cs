using UnityEngine;

public class HideOnClick : MonoBehaviour
{
    [Header("Objects to hide")]
    public GameObject[] objectsToHide;

    public void HideObjects()
    {
        foreach (GameObject obj in objectsToHide)
        {
            if (obj != null)
                obj.SetActive(false);
        }
    }
}
