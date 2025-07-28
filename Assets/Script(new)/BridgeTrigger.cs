using UnityEngine;

public class BridgeTrigger : MonoBehaviour
{
    [Header("Bridge Settings")]
    public GameObject bridgePrefab;
    public Transform bridgeSpawnPoint;

    [Header("Light Replacement")]
    public GameObject objectToReplace;
    public GameObject replacementPrefab;

    [Header("Optional")]
    public bool activateOnStart = false;

    private bool hasActivated = false;

    void Start()
    {
        if (activateOnStart)
        {
            ActivateBridge();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasActivated) return;

        if (other.CompareTag("Player"))
        {
            ActivateBridge();
        }
    }

    public void ActivateBridge()
    {
        if (hasActivated) return;
        hasActivated = true;

        // Spawn bridge
        if (bridgePrefab != null && bridgeSpawnPoint != null)
        {
            Instantiate(bridgePrefab, bridgeSpawnPoint.position, bridgeSpawnPoint.rotation);
            Debug.Log("🌉 Bridge spawned at " + bridgeSpawnPoint.position);
        }

        // Replace light
        if (objectToReplace != null && replacementPrefab != null)
        {
            Vector3 pos = objectToReplace.transform.position;
            Quaternion rot = objectToReplace.transform.rotation;
            Destroy(objectToReplace);
            Instantiate(replacementPrefab, pos, rot);
            Debug.Log("💡 Light replaced at " + pos);
        }
    }
}