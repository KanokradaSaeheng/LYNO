using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    public List<PathPiece> allPieces;
    public Camera mainCamera;
    public GameObject connectorPrefab;

    private List<GameObject> currentConnectors = new List<GameObject>();

    void Update()
    {
        ClearConnectors();

        foreach (PathPiece piece in allPieces)
        {
            if (piece == null) continue;

            foreach (PathPiece linked in piece.illusionLinkedPieces)
            {
                if (linked == null || piece == linked) continue;

                if (piece.IsAlignedWith(linked, mainCamera))
                {
                    Debug.Log("🔗 Spawning connector between: " + piece.name + " → " + linked.name);

                    GameObject conn = Instantiate(connectorPrefab);

                    // Start and end of illusion connection
                    Vector3 start = piece.transform.position;
                    Vector3 end = linked.transform.position;

                    // Offset the ends to prevent overlap with block center
                    Vector3 dir = (end - start).normalized;
                    float offset = 0.5f;

                    Vector3 p1 = start + dir * offset;
                    Vector3 p2 = end - dir * offset;

                    Vector3 mid = (p1 + p2) / 2f;
                    float length = Vector3.Distance(p1, p2);

                    conn.transform.position = mid;
                    conn.transform.rotation = Quaternion.LookRotation(dir);
                    conn.transform.localScale = new Vector3(1f, 1f, length); // square connector

                    // Add collider dynamically
                    if (!conn.TryGetComponent<BoxCollider>(out var col))
                        col = conn.AddComponent<BoxCollider>();

                    col.isTrigger = false;
                    col.size = Vector3.one;
                    col.center = new Vector3(0f, 0f, 0.5f); // forward push for stretching

                    currentConnectors.Add(conn);
                }
            }
        }
    }

    void ClearConnectors()
    {
        foreach (GameObject g in currentConnectors)
        {
            Destroy(g);
        }
        currentConnectors.Clear();
    }
}
