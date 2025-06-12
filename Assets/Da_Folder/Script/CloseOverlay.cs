using UnityEngine;

public class CloseOverlay : MonoBehaviour
{
    [Tooltip("Canvas group or panel to hide")]
    public GameObject overlayPanel;

    [Tooltip("Should the game be unpaused when overlay is hidden?")]
    public bool unpauseGame = true;

    public void OnButtonClick()
    {
        if (overlayPanel != null)
        {
            overlayPanel.SetActive(false);

            if (unpauseGame)
            {
                Time.timeScale = 1f; 
            }
        }
        else
        {
            Debug.LogWarning("No overlay panel assigned in inspector!");
        }
    }
}