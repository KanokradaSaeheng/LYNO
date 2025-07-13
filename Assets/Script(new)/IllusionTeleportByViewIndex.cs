using UnityEngine;

public class IllusionTeleportByViewIndex : MonoBehaviour
{
    [System.Serializable]
    public class ViewTeleportEntry
    {
        public bool is2DEntry = false; // 🔄 This entry works in 2D mode
        public int viewIndex; // Used as either 3D viewIndex or 2DIndex
        public string controlSide; // "Left", "Right", "Top", "Bottom"
        public Transform targetPosition;
    }


    [Header("Teleport Config")]
    public ViewTeleportEntry[] teleportEntries;
    public string playerTag = "Player";
    public bool onlyOnce = true;
    
    [Header("2D Support")]
    public bool is2DZone = false; // Enable for zones that work in 2D puzzles
    public int required2DIndex = 0; // Match with Movement.cs.active2DOffsetIndex


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
        int indexToCheck = is2DZone ? GetCurrent2DIndex() : GetCurrentViewIndex();

        foreach (var entry in teleportEntries)
        {
            if (entry.viewIndex == indexToCheck)
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
        if (!isPlayerInZone) return false;
        if (onlyOnce && hasTeleported) return false;

        foreach (var entry in teleportEntries)
        {
            if (entry.viewIndex == (is2DZone ? GetCurrent2DIndex() : viewIndex) &&
                entry.controlSide == controlSide) ;
        }

        return false;
    }


    public void TryTeleport(int viewIndex, string attemptedControlSide)
    {
        if (!isPlayerInZone || cachedPlayer == null) return;

        foreach (var entry in teleportEntries)
        {
            if (entry.viewIndex == viewIndex && entry.controlSide == attemptedControlSide)
            {
                cachedPlayer.position = entry.targetPosition.position;

                if (onlyOnce)
                {
                    hasTeleported = true;
                    isPlayerInZone = false;
                    cachedPlayer = null;
                }

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
