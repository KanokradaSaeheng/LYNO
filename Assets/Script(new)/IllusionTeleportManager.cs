using UnityEngine;
using System.Collections.Generic;

public class IllusionTeleportManager : MonoBehaviour
{
    public static IllusionTeleportManager Instance;
    private List<IllusionTeleportByViewIndex> teleportZones = new List<IllusionTeleportByViewIndex>();

    void Awake()
    {
        Instance = this;
        Debug.Log("✅ IllusionTeleportManager initialized.");
    }

    public void RegisterZone(IllusionTeleportByViewIndex zone)
    {
        if (!teleportZones.Contains(zone))
            teleportZones.Add(zone);
    }

    public void UnregisterZone(IllusionTeleportByViewIndex zone)
    {
        teleportZones.Remove(zone);
    }

    public IllusionTeleportByViewIndex GetActiveTeleportZone()
    {
        foreach (var zone in teleportZones)
        {
            if (zone.IsPlayerInZone()) // ✅ Capital "I"
                return zone;
        }
        return null;
    }
}