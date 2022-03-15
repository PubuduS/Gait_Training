using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetweenPoints : MonoBehaviour
{
    public List<Transform> wayPointsRef;

    private Transform targetWayPoint;
    private int targetWayPointIndex = 0;
    private float minDistance = 1f;
    private int lastWayPointIndex;
    private SpawnWaypoints waypointScript;

    private Vector3 m_LastPlayerPosition;
    private Quaternion m_LastPlayerRotation;

    private float movementSpeed = 4.0f;
    private float rotationSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        waypointScript = GameObject.Find("WaypointSpawner").GetComponent<SpawnWaypoints>();
        wayPointsRef = waypointScript.m_AllWaypoints;
        targetWayPoint = wayPointsRef[targetWayPointIndex];
        lastWayPointIndex = (wayPointsRef.Count - 1);       
        m_LastPlayerPosition = Camera.main.transform.position;
        m_LastPlayerRotation = Camera.main.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {

        movementSpeed = CamMovementTracker.m_CamTrackerInstance.Speed;   
        float movementStep = movementSpeed * Time.deltaTime;
        float rotationSteps = rotationSpeed * Time.deltaTime;

        Vector3 directionToTarget = targetWayPoint.position - transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation( directionToTarget );
        
        transform.rotation = Quaternion.Slerp( transform.rotation, rotationToTarget, rotationSteps );

        float distance = Vector3.Distance( transform.position, targetWayPoint.position );
        CheckDistanceToWaypoint( distance );

        if( CamMovementTracker.m_CamTrackerInstance.IsMoving == true )
        {
            transform.position = Vector3.MoveTowards( transform.position, targetWayPoint.position, movementStep );            
        }

    }

    private void CheckDistanceToWaypoint(float currentDistance)
    {
        if (currentDistance <= minDistance)
        {
            targetWayPointIndex++;
            UpdateTargerWaypoint();
        }
    }

    private void UpdateTargerWaypoint()
    {
        if (targetWayPointIndex > lastWayPointIndex)
        {
            targetWayPointIndex = 0;
        }

        targetWayPoint = wayPointsRef[targetWayPointIndex];
    }

}
