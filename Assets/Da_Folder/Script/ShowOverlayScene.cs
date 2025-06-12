using UnityEngine;

public class ShowOverlayScene : MonoBehaviour
{
    [Tooltip("Canvas group or panel to show as overlay")]
    public GameObject overlayPanel;

    [Tooltip("Should the game be paused when overlay is shown?")]
    public bool pauseGame = true;

    public void OnButtonClick()
    {
        if (overlayPanel != null)
        {
            overlayPanel.SetActive(true);

            if (pauseGame)
            {
                Time.timeScale = 0f; 
            }
        }
        else
        {
            Debug.LogWarning("No overlay panel assigned in inspector!");
        }
    }
}