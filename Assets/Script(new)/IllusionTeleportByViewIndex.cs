using UnityEngine;
using System.Collections;

public class IllusionTeleportByViewIndex : MonoBehaviour
{
    [System.Serializable]
    public class ViewTeleportEntry
    {
        public bool is2DEntry = false;
        public int viewIndex;
        public string controlSide;
        public Transform targetPosition;
    }

    [Header("Teleport Config")]
    public ViewTeleportEntry[] teleportEntries;
    public string playerTag = "Player";
    public bool onlyOnce = true;

    [Header("2D Support")]
    public bool is2DZone = false; // Tells the script which index to check

    private bool isPlayerInZone = false;
    private Transform cachedPlayer;
    private bool hasTeleported = false;

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
        isPlayerInZone = true;

        Debug.Log($"🟡 Player ENTERED teleport zone: {gameObject.name}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        isPlayerInZone = false;
        cachedPlayer = null;

        Debug.Log($"⚪ Player EXITED teleport zone: {gameObject.name}");
    }

    public bool ShouldBlockControl(int viewIndex, string controlSide)
    {
        if (!isPlayerInZone) return false;
        if (onlyOnce && hasTeleported) return false;

        bool currentMode2D = DimensionManager.Is2DModeStatic;
        int currentIndex = currentMode2D ? GetCurrent2DIndex() : GetCurrentViewIndex();

        foreach (var entry in teleportEntries)
        {
            if (entry.is2DEntry != currentMode2D) continue;

            if (entry.viewIndex == currentIndex && entry.controlSide == controlSide)
            {
                Debug.Log($"🟢 Teleport condition met: [{(currentMode2D ? "2D" : "3D")}] Index={currentIndex}, Side={controlSide}");
                return true;
            }
        }

        return false;
    }


    public void TryTeleport(int viewIndex, string attemptedControlSide)
    {
        if (!isPlayerInZone || cachedPlayer == null) return;

        bool currentMode2D = DimensionManager.Is2DModeStatic;
        int currentIndex = currentMode2D ? GetCurrent2DIndex() : viewIndex;

        foreach (var entry in teleportEntries)
        {
            if (entry.is2DEntry != currentMode2D) continue;

            if (entry.viewIndex == currentIndex && entry.controlSide == attemptedControlSide)
            {
                Debug.Log($"🚀 Teleporting to: {entry.targetPosition.position}");

                cachedPlayer.position = entry.targetPosition.position;

                if (onlyOnce)
                {
                    hasTeleported = true;
                    isPlayerInZone = false;
                    cachedPlayer = null;
                }

                StartCoroutine(ForceRecheckZone());
                return;
            }
        }

        Debug.LogWarning("❌ No matching teleport entry found for ViewIndex=" + currentIndex + ", Side=" + attemptedControlSide);
    }


    private IEnumerator ForceRecheckZone()
    {
        yield return null; // wait 1 frame

        if (cachedPlayer == null)
        {
            cachedPlayer = GameObject.FindGameObjectWithTag(playerTag)?.transform;
            if (cachedPlayer == null) yield break;
        }

        Collider[] overlapping = Physics.OverlapSphere(cachedPlayer.position, 0.1f);
        foreach (var col in overlapping)
        {
            if (col.gameObject == gameObject)
            {
                Debug.Log("🔁 Force-retriggered OnTriggerEnter after teleport.");
                OnTriggerEnter(col);
                break;
            }
        }
    }

    public bool IsPlayerInZone() => isPlayerInZone;

    private int GetCurrentViewIndex()
    {
        CameraFollow cam = Camera.main?.GetComponent<CameraFollow>();
        return cam != null ? cam.currentViewIndex : -1;
    }

    private int GetCurrent2DIndex()
    {
        Movement movement = GameObject.FindGameObjectWithTag(playerTag)?.GetComponent<Movement>();
        return movement != null ? movement.active2DOffsetIndex : -1;
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
