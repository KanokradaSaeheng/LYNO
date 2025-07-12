using UnityEngine;

public class IllusionTeleportByViewIndex : MonoBehaviour
{
    [System.Serializable]
    public class ViewTeleportEntry
    {
        public int viewIndex;
        public string controlSide; // "Left", "Right", "Top", "Bottom"
        public Transform targetPosition;
    }

    [Header("Teleport Config")]
    public ViewTeleportEntry[] teleportEntries;
    public string playerTag = "Player";
    public bool onlyOnce = true;

    private bool isPlayerInZone = false;
    private Transform cachedPlayer;
    private bool hasTeleported = false;
    private string readyControlSide = "";
    private Vector3 teleportTarget;
    private int currentViewIndex;

    private void Start()
    {
        IllusionTeleportManager.Instance?.RegisterZone(this);
    }

    private void OnDestroy()
    {
        IllusionTeleportManager.Instance?.UnregisterZone(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (onlyOnce && hasTeleported) return;

        cachedPlayer = other.transform;
        currentViewIndex = GetCurrentViewIndex();

        foreach (var entry in teleportEntries)
        {
            if (entry.viewIndex == currentViewIndex)
            {
                isPlayerInZone = true;
                readyControlSide = entry.controlSide;
                teleportTarget = entry.targetPosition.position;
                return;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        isPlayerInZone = false;
        cachedPlayer = null;
        readyControlSide = "";
    }

    public bool ShouldBlockControl(int viewIndex, string controlSide)
    {
        if (!isPlayerInZone || hasTeleported) return false;

        foreach (var entry in teleportEntries)
        {
            if (entry.viewIndex == viewIndex && entry.controlSide == controlSide)
                return true;
        }

        return false;
    }

    public void TryTeleport(int viewIndex, string attemptedControlSide)
    {
        if (!isPlayerInZone || hasTeleported || cachedPlayer == null) return;

        foreach (var entry in teleportEntries)
        {
            if (entry.viewIndex == viewIndex && entry.controlSide == attemptedControlSide)
            {
                cachedPlayer.position = entry.targetPosition.position;
                hasTeleported = true;
                isPlayerInZone = false;
                cachedPlayer = null;
                return;
            }
        }
    }

    public bool IsPlayerInZone() => isPlayerInZone;

    private int GetCurrentViewIndex()
    {
        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
        return cam != null ? cam.currentViewIndex : -1;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        foreach (var entry in teleportEntries)
        {
            if (entry.targetPosition != null)
            {
                Gizmos.DrawLine(transform.position, entry.targetPosition.position);
                Gizmos.DrawSphere(entry.targetPosition.position, 0.2f);
            }
        }
    }
#endif
}
