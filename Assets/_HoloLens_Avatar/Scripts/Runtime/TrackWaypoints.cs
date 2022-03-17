using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWaypoints : MonoBehaviour
{

    public static int g_LastWaypointIndex;

    public void ResetToFirstWaypoint()
    {
        g_LastWaypointIndex = 0;
    }
}
