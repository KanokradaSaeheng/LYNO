using UnityEngine;
using System.Collections.Generic;

public class IllusionWalkSystem : MonoBehaviour
{
    public Transform player;                 // Your player cube
    public float moveDistance = 1f;          // How far one step is in world units
    public float screenStepPixelDistance = 100f; // Approx. how far one step appears in screen space
    public float screenTolerance = 10f;      // How close a screen-space match must be to count

    public LayerMask platformLayer;          // Layer for walkable platforms
    public bool drawDebug = true;            // Toggle debug lines

    /// <summary>
    /// Call this when the player tries to move. Returns true if illusion allows the move.
    /// </summary>
    public bool CanMoveByIllusion(Vector3 moveDirection)
    {
        Vector3 playerWorld = player.position + Vector3.up * 0.5f;
        Vector3 targetWorld = player.position + moveDirection * moveDistance;

        // Project player and intended move direction to screen
        Vector3 playerScreen = Camera.main.WorldToScreenPoint(playerWorld);
        Vector3 targetScreen = Camera.main.WorldToScreenPoint(targetWorld);

        Vector2 visualStep = (targetScreen - playerScreen).normalized;
        Vector2 expectedScreen = new Vector2(playerScreen.x, playerScreen.y) + visualStep * screenStepPixelDistance;

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");

        foreach (GameObject platform in platforms)
        {
            Vector3[] testOffsets = {
                Vector3.zero,
                Vector3.up * 0.5f,
                Vector3.up * 1f
            };

            foreach (Vector3 offset in testOffsets)
            {
                Vector3 platformScreen = Camera.main.WorldToScreenPoint(platform.transform.position + offset);
                float distance = Vector2.Distance(platformScreen, expectedScreen);

                if (drawDebug)
                {
                    Debug.Log($"Platform {platform.name} test offset {offset.y:F1} → distance = {distance:F2}");
                }

                if (distance < screenTolerance)
                {
                    if (drawDebug)
                    {
                        Debug.Log($"✅ Illusion match on {platform.name} (offset {offset.y:F1})");
                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(new Vector3(playerScreen.x, playerScreen.y, 10f)),
                                       Camera.main.ScreenToWorldPoint(new Vector3(platformScreen.x, platformScreen.y, 10f)),
                                       Color.green, 1f);
                    }
                    return true;
                }
            }
        }

        if (drawDebug)
        {
            Vector3 worldA = Camera.main.ScreenToWorldPoint(new Vector3(playerScreen.x, playerScreen.y, 10f));
            Vector3 worldB = Camera.main.ScreenToWorldPoint(new Vector3(expectedScreen.x, expectedScreen.y, 10f));
            Debug.DrawLine(worldA, worldB, Color.red, 1f);
        }

        return false;
    }
}
 