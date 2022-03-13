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

    public bool m_IsPlayerMoving = false;

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

        movementSpeed = GetSpeedOfPlayer();        
        float movementStep = movementSpeed * Time.deltaTime;
        float rotationSteps = rotationSpeed * Time.deltaTime;

        Vector3 directionToTarget = targetWayPoint.position - transform.position;
        Quaternion rotationToTarget = Quaternion.LookRotation( directionToTarget );
        
        transform.rotation = Quaternion.Slerp( transform.rotation, rotationToTarget, rotationSteps );

        float distance = Vector3.Distance( transform.position, targetWayPoint.position );
        CheckDistanceToWaypoint( distance );

        if( m_IsPlayerMoving )
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

    private float GetSpeedOfPlayer()
    {
        float speed = 0.0f;

        Vector3 playerCurrentPos = Camera.main.transform.position;

        if( ( m_LastPlayerPosition == playerCurrentPos ) && ( m_LastPlayerRotation == Camera.main.transform.rotation ) )
        {
            m_IsPlayerMoving = false;
            return 0.0f;
        }
        else
        {
            m_IsPlayerMoving = true;
        }

        speed = ( playerCurrentPos - m_LastPlayerPosition ).magnitude / Time.deltaTime;
        speed = Mathf.Abs( speed );
        m_LastPlayerPosition = playerCurrentPos;
        m_LastPlayerRotation = Camera.main.transform.rotation;

        return speed;
    }
}
