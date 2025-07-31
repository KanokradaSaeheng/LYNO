using UnityEngine;

public class FloatLogo : MonoBehaviour
{
    public float floatStrength = 10f; // How high it moves
    public float speed = 1f; // How fast it moves

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        transform.localPosition = startPos + new Vector3(0f, Mathf.Sin(Time.time * speed) * floatStrength, 0f);
    }
}
