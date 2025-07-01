using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public Slider energyBar;
    public GameObject loseOverlay;
    public float totalTime = 120f; // total countdown time in seconds, set in Inspector

    private float remainingTime;
    private bool isCounting = true;

    void Start()
    {
        remainingTime = totalTime;
        loseOverlay.SetActive(false);
        UpdateTimerDisplay();
        UpdateEnergyBar();
    }

    void Update()
    {
        if (!isCounting) return;

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0)
                remainingTime = 0;

            UpdateTimerDisplay();
            UpdateEnergyBar();
        }
        else
        {
            isCounting = false;
            loseOverlay.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void UpdateEnergyBar()
    {
        energyBar.value = (remainingTime / totalTime) * 100f;
    }

}