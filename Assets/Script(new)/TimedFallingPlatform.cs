using UnityEngine;
using System.Collections;

public class TimedFallingPlatform : MonoBehaviour
{
    [Header("Timing Settings")]
    public float fallDelay = 1.0f;      // Time after player steps before falling
    public float respawnDelay = 3.0f;   // Time before platform returns

    [Header("Optional")]
    public bool disableMesh = true;     // Should it hide the mesh?
    public bool disableCollider = true; // Should it disable the collider?

    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Rigidbody rb;
    private Collider platformCollider;
    private Renderer platformRenderer;
    private bool isFalling = false;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        rb = GetComponent<Rigidbody>();
        platformCollider = GetComponent<Collider>();
        platformRenderer = GetComponent<Renderer>();

        if (rb != null)
            rb.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFalling) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(FallAndRespawn());
        }
    }

    private IEnumerator FallAndRespawn()
    {
        isFalling = true;

        yield return new WaitForSeconds(fallDelay);

        // Enable gravity or disable components
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        if (disableCollider && platformCollider != null)
            platformCollider.enabled = false;

        if (disableMesh && platformRenderer != null)
            platformRenderer.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        // Reset platform
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        transform.position = initialPosition;
        transform.rotation = initialRotation;

        if (disableCollider && platformCollider != null)
            platformCollider.enabled = true;

        if (disableMesh && platformRenderer != null)
            platformRenderer.enabled = true;

        isFalling = false;
    }
}
