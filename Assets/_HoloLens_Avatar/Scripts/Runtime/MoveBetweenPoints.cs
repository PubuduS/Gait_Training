using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    public List<Transform> wayPointsRef;

    private Transform targetWayPoint;    
    private float minDistance = 1f;
    private int lastWayPointIndex;
    private SpawnWaypoints waypointScript;

    private float movementSpeed = 4.0f;
    private float rotationSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        waypointScript = GameObject.Find("WaypointSpawner").GetComponent<SpawnWaypoints>();
        wayPointsRef = waypointScript.m_AllWaypoints;        
        targetWayPoint = wayPointsRef[TrackWaypoints.g_LastWaypointIndex];
        lastWayPointIndex = (wayPointsRef.Count - 1);
    }

    // Update is called once per frame
    void Update()
    {

        movementSpeed = CamMovementTracker.m_CamTrackerInstance.Speed;   
        float movementStep = movementSpeed * Time.deltaTime;
        float rotationSteps = rotationSpeed * Time.deltaTime;

        Debug.Log("movementStep " + movementStep);

        Vector3 directionToTarget = targetWayPoint.position - transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation( directionToTarget );
        
        transform.rotation = Quaternion.Slerp( transform.rotation, rotationToTarget, rotationSteps );

        float distance = Vector3.Distance( transform.position, targetWayPoint.position );
        CheckDistanceToWaypoint( distance );

        if( CamMovementTracker.m_CamTrackerInstance.IsMoving == true )
        {
            movementStep = movementStep / 20.0f;
            transform.position = Vector3.MoveTowards( transform.position, targetWayPoint.position, movementStep );
        }

    }

    private void CheckDistanceToWaypoint(float currentDistance)
    {
        if (currentDistance <= minDistance)
        {
            TrackWaypoints.g_LastWaypointIndex++;
            UpdateTargerWaypoint();
        }
    }

    private void UpdateTargerWaypoint()
    {
        if ( TrackWaypoints.g_LastWaypointIndex > lastWayPointIndex )
        {
            TrackWaypoints.g_LastWaypointIndex = 0;
        }

        targetWayPoint = wayPointsRef[TrackWaypoints.g_LastWaypointIndex];
    }

}
