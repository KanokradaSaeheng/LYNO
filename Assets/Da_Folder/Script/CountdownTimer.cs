using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MaskTransitions; // Your transition manager
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public Slider energyBar;
    public GameObject warningImage;

    [Header("Timer Settings")]
    public float totalTime = 120f;

    [Header("Transition Settings")]
    public string nextSceneName;
    public float transitionTime = 1.5f;

    [Header("Audio")]
    public AudioSource warningSound; // <-- Assign in inspector

    private float remainingTime;
    private bool isCounting = true;
    private bool hasTriggered = false;
    private bool isWarningActive = false;

    void Start()
    {
        remainingTime = totalTime;
        UpdateTimerDisplay();
        UpdateEnergyBar();

        if (warningImage != null)
            warningImage.SetActive(false);
    }

    void Update()
    {
        if (!isCounting || hasTriggered) return;

        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0)
                remainingTime = 0;

            UpdateTimerDisplay();
            UpdateEnergyBar();

            if (!isWarningActive && remainingTime <= 10f && remainingTime > 0f)
            {
                ActivateWarning();
            }
        }
        else
        {
            hasTriggered = true;
            TransitionManager.Instance.LoadLevel(nextSceneName);
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

    void ActivateWarning()
    {
        isWarningActive = true;

        if (warningImage != null)
        {
            warningImage.SetActive(true);
            StartCoroutine(VibrateWarning());
        }

        if (warningSound != null)
        {
            warningSound.Play();
        }
    }

    IEnumerator VibrateWarning()
    {
        Vector3 originalPos = warningImage.transform.localPosition;

        while (remainingTime > 0)
        {
            float shakeAmount = 5f;
            warningImage.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            yield return new WaitForSeconds(0.05f);
        }

        warningImage.transform.localPosition = originalPos;
    }
}
