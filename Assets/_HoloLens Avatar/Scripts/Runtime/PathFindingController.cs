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

    //! Reference to NavMeshAgent
    private NavMeshAgent m_Agent;

    //! Reference to RigidBody
    private Rigidbody m_Rigidbody;

    //! Store the avatar's last position
    private Vector3 m_LastPosition;

    //! Store the avatar's last rotation
    private Quaternion m_LastRotation;

    //! Get called when the script instance is being loaded
    //! This will cache the NavMeshAgent and RigidBody
    void Awake() 
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    //! Enable update coroutine when object or component is active
    private void OnEnable() => StartCoroutine(UpdateRoutine());

    //! Stop all coroutines when disable just in case
    private void OnDisable() => StopAllCoroutines();


    //! Update destination every second.
    IEnumerator UpdateRoutine() 
    {
        while (true) 
        {
            MoveAgent();
            yield return new WaitForSeconds(1f);
        }
    }


    //! The agent will move wherever the main camera is gazing at.
    void MoveAgent() 
    {
        if (!m_Agent.enabled || !m_Agent.isOnNavMesh)
            return;

        // Prevents update when camera is not moving
        if (m_LastPosition == Camera.main.transform.position && m_LastRotation == Camera.main.transform.rotation)
            return;

        m_LastPosition = Camera.main.transform.position;
        m_LastRotation = Camera.main.transform.rotation;

        if (Physics.Raycast(m_LastPosition, Camera.main.transform.TransformDirection(Vector3.forward), out RaycastHit hit, Mathf.Infinity)) 
        {
            // When NavAgent is working, we need to decouple the RigidBody.
            // Otherwise race condition will occur between NavAgent and RigidBody
            m_Rigidbody.isKinematic = true;
            m_Agent.SetDestination(hit.point);
        }
    }
}
