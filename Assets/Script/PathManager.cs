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
                    Vector3 mid = (piece.transform.position + linked.transform.position) / 2f;
                    conn.transform.position = mid;

                    Vector3 dir = linked.transform.position - piece.transform.position;
                    conn.transform.rotation = Quaternion.LookRotation(dir);

                    // Get prefab's X (or Y) to use as square width/height
                    float squareSize = connectorPrefab.transform.localScale.x;
                    conn.transform.localScale = new Vector3(squareSize, squareSize, dir.magnitude);

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