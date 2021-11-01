using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This Script defines the logic a NavMesh agent in a unity navmesh
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class PathFindingController : MonoBehaviour
{

    [SerializeField] private float m_Distance = 5f;
    [SerializeField, Range(0f, 90f)] private float m_MaxAngle = 90f;

    private NavMeshAgent m_Agent;
    private Rigidbody m_Rigidbody;
    private Vector3 m_LastPosition;
    private Quaternion m_LastRotation;    

    void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();        
        MoveAgent();
    }

    private void Update()
    {
        MoveWhenPlayerIsNear();

        if(m_Agent.velocity.magnitude <= 0f)
        {
            LookAtPlayer();
        }
    }

    // Enable update coroutine when object or component is active
    void OnEnable() => StartCoroutine(UpdateRoutine());

    // Stop all coroutines just in case
    void OnDisable() => StopAllCoroutines();

    /// <summary>
    /// Update destination every second.
    /// </summary>
    /// <returns></returns>
    IEnumerator UpdateRoutine()
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
    void UpdateMove()
    {
        if (!m_Agent.enabled || !m_Agent.isOnNavMesh)
        {
            Debug.LogError("Avatar is not active or not in the NavMesh.");
            return;
        }
            

        var targetDir = m_Agent.transform.position - Camera.main.transform.position;
        var angle = Vector3.Angle(targetDir, Camera.main.transform.forward);

        if (angle < m_MaxAngle)
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

        MoveAgent();
    }

    /// <summary>
    /// Move the agent to forward direction.
    /// </summary>
    void MoveAgent() 
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

        if (distance < 2.0f)
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

}
