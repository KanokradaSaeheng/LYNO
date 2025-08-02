using UnityEngine;

public class FpsBoost : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
}
