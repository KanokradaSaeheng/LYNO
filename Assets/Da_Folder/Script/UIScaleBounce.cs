using UnityEngine;

public class UIScaleBounce : MonoBehaviour
{
    public float scaleSpeed = 2f;
    public float scaleAmount = 0.1f; // how much to grow/shrink

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scaleOffset = Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;
        transform.localScale = originalScale + Vector3.one * scaleOffset;
    }
}
