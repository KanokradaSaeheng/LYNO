using UnityEngine;
using UnityEngine.UI;

public class SoundToggle : MonoBehaviour
{
    public AudioSource[] audioSources; // assign in inspector
    public Sprite soundOnSprite;       // sprite for sound ON
    public Sprite soundOffSprite;      // sprite for sound OFF

    private bool isMuted = false;
    private Image iconImage;

    private void Start()
    {
        iconImage = GetComponent<Image>();
        UpdateIcon();
    }

    public void OnClickToggleSound()
    {
        isMuted = !isMuted;

        foreach (AudioSource source in audioSources)
        {
            if (source != null)
            {
                source.mute = isMuted;
            }
        }

        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (iconImage != null)
        {
            iconImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }
    }
}
