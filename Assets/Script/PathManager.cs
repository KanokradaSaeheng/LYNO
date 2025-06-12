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
            if (piece != null && piece.IsVisuallyAligned(mainCamera))
            {
                GameObject conn = Instantiate(connectorPrefab);
                Vector3 mid = (piece.transform.position + piece.nextPiece.transform.position) / 2f;
                conn.transform.position = mid;

                Vector3 dir = (piece.nextPiece.transform.position - piece.transform.position);
                conn.transform.rotation = Quaternion.LookRotation(dir);
                conn.transform.localScale = new Vector3(0.2f, 0.2f, dir.magnitude); // bridge thickness
                currentConnectors.Add(conn);
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