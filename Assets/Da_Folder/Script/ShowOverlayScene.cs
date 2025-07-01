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
        // Hide all overlays under the parent
        if (overlayParent != null)
        {
            foreach (Transform child in overlayParent)
            {
                child.gameObject.SetActive(false);
            }
        }

        // Show selected overlay
        if (overlayPanel != null)
        {
            overlayPanel.SetActive(true);

            if (backgroundDimmer != null)
                backgroundDimmer.SetActive(true);

            if (pauseGame)
                Time.timeScale = 0f;
        }
        else
        {
            Debug.LogWarning("No overlayPanel assigned!");
        }
    }
}