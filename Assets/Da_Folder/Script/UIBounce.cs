using UnityEngine;

public class UIBounce : MonoBehaviour
{
    public enum BounceDirection { Vertical, Horizontal }
    public BounceDirection bounceDirection = BounceDirection.Vertical;

    public float bounceSpeed = 2f;
    public float bounceAmount = 10f;
    public float phaseOffset = 0f; // Add this to make left/right opposite

    private RectTransform rectTransform;
    private Vector2 originalPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float bounce = Mathf.Sin(Time.time * bounceSpeed + phaseOffset) * bounceAmount;

        if (bounceDirection == BounceDirection.Vertical)
        {
            rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + bounce);
        }
        else
        {
            rectTransform.anchoredPosition = new Vector2(originalPosition.x + bounce, originalPosition.y);
        }
    }
}
