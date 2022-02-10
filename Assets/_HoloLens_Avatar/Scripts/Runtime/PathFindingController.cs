using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This Script defines the logic a NavMesh agent in a unity navmesh
/// This script required NavMesh Agent and RigidBody component.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class PathFindingController : MonoBehaviour
{

    /// Without the distance, avatar will not walk 
    [SerializeField] private float m_Distance = 5f;

    /// Define the rotation angle
    [SerializeField, Range(0f, 90f)] private float m_MaxAngle = 90f;

    /// NavMesh Agent
    private NavMeshAgent m_Agent;

    /// Rigidbody
    private Rigidbody m_Rigidbody;

    /// Store the avatar's last position
    private Vector3 m_LastPosition;

    /// Store the avatar's last rotation
    private Quaternion m_LastRotation;

    /// When sets to true avatar will to the gaze point.
    private bool m_GoToGazeLocation = false;

    /// <summary>
    /// Gets called when the script is loaded.
    /// Cached NavMesh agent and Rigidbody
    /// </summary>
    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();        
        MoveAgent();
    }

    /// <summary>
    /// Gets called at each frame
    /// </summary>
    private void Update()
    {
        MoveWhenPlayerIsNear();

        if(m_Agent.velocity.magnitude <= 0f)
        {
            LookAtPlayer();
        }
    }

    /// Enable update coroutine when object or component is active
    private void OnEnable() => StartCoroutine(UpdateRoutine());

    /// Stop all coroutines just in case
    private void OnDisable() => StopAllCoroutines();

    /// <summary>
    /// Update destination every second.
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpdateRoutine()
    {
        while (true)
        {
            UpdateMove();
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// The agent will move wherever the main camera is gazing at.
    /// </summary>
    private void UpdateMove()
    {
        if (!m_Agent.enabled || !m_Agent.isOnNavMesh)
        {
            Debug.LogError("Avatar is not active or not in the NavMesh.");
            return;
        }
            

        var targetDir = m_Agent.transform.position - Camera.main.transform.position;
        var angle = Vector3.Angle(targetDir, Camera.main.transform.forward);

        if ( (angle < m_MaxAngle) && (m_GoToGazeLocation == false) )
        {
            return;
        }           

        // Prevents update when camera is not moving
        if (m_LastPosition == Camera.main.transform.position && m_LastRotation == Camera.main.transform.rotation)
        {
            return;
        }            

        m_LastPosition = Camera.main.transform.position;
        m_LastRotation = Camera.main.transform.rotation;

        if(m_GoToGazeLocation)
        {
            WalkToGazeLocation();
        }
        else
        {
            MoveAgent();
        }
        
    }

    /// <summary>
    /// Move the agent to forward direction.
    /// </summary>
    private void MoveAgent() 
    {        
        m_Rigidbody.isKinematic = true;
        var direction = Camera.main.transform.TransformDirection(Vector3.forward);
        var targetPosition = Camera.main.transform.position + (direction.normalized * m_Distance);
        m_Agent.SetDestination(targetPosition);
    }

    /// <summary>
    /// The avatar will start to move when the player get closer to the avatar.
    /// </summary>
    private void MoveWhenPlayerIsNear()
    {
        float distance = Vector3.Distance(Camera.main.transform.position, m_Agent.transform.position);

        if (distance < 10.0f)
        {
            MoveAgent();
        }
        else
        {
            return;
        }
    }

    /// <summary>
    /// The avatar will look at player's direction when not moving.
    /// </summary>
    private void LookAtPlayer()
    {       
        var direction = (Camera.main.transform.position - m_Agent.transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    /// Toggle the gaze walking flag.    
    public void ToggleGazeWalk()
    {
        m_GoToGazeLocation = !m_GoToGazeLocation;
    }

    /// When m_GoToGazeLocation flag sets to true, avatar will walk to the point where user is currently looking.
    private void WalkToGazeLocation()
    {
        if (Physics.Raycast(m_LastPosition, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity))
        {
            // When NavAgent is working, we need to decouple the RigidBody.
            // Otherwise race condition will occur between NavAgent and RigidBody
            m_Rigidbody.isKinematic = true;
            m_Agent.SetDestination(hit.point);
        }
        else
        {
            return;
        }
    }

}
