using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class is used to control the avartar animation state.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class AvatarAnimationState : MonoBehaviour 
{
    
    
    /// Reference to the animator    
    private Animator m_Animator;

    /// Reference to the NavMesh Agent 
    private NavMeshAgent m_NavMesh;

    /// <summary>
    /// Gets called when script is loaded
    /// cached the references to Animator and NavMesh agent
    /// </summary>
    void Awake() 
    {
        m_Animator = GetComponent<Animator>();
        m_NavMesh = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Gets called per each frame
    /// If navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
    /// </summary>
    void Update() 
    {
        // if navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
        var playerMoving = m_NavMesh.velocity.magnitude > 0f;
        m_Animator.SetBool("IsWalking", playerMoving);
    }
}
