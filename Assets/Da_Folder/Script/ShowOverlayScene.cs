using UnityEngine;

public class ShowOverlayScene : MonoBehaviour
{
    [Tooltip("The overlay panel to show when this button is clicked")]
    public GameObject overlayPanel;

    [Tooltip("The background dimmer (semi-transparent dark panel)")]
    public GameObject backgroundDimmer;

    [Tooltip("Parent object containing all overlays")]
    public Transform overlayParent;

    [Tooltip("Pause the game when this overlay is shown?")]
    public bool pauseGame = true;

    public void OnButtonClick()
    {
        // Hide all overlays first
        if (overlayParent != null)
        {
            foreach (Transform child in overlayParent)
            {
                child.gameObject.SetActive(false);
            }
        }

        bool isOverlayActive = overlayPanel != null && overlayPanel.activeSelf;

        if (overlayPanel != null)
        {
            if (isOverlayActive)
            {
                // Overlay is visible, so hide it and unpause
                overlayPanel.SetActive(false);
                if (backgroundDimmer != null)
                    backgroundDimmer.SetActive(false);

                if (pauseGame)
                {
                    Time.timeScale = 1f;
                    AudioListener.pause = false;
                }
            }
            else
            {
                // Overlay hidden, so show it and pause
                overlayPanel.SetActive(true);
                if (backgroundDimmer != null)
                    backgroundDimmer.SetActive(true);

                if (pauseGame)
                {
                    Time.timeScale = 0f;
                    AudioListener.pause = true;
                }
            }
        }
        else
        {
            Debug.LogWarning("No overlayPanel assigned!");
        }
    }
}
