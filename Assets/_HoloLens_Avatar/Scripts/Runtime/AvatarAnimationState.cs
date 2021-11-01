using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class is used to control the avartar animation state.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class AvatarAnimationState : MonoBehaviour 
{
    
    private Animator m_Animator;
    private NavMeshAgent m_NavMesh;

    void Awake() 
    {
        m_Animator = GetComponent<Animator>();
        m_NavMesh = GetComponent<NavMeshAgent>();
    }

    void Update() 
    {
        // if navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
        var playerMoving = m_NavMesh.velocity.magnitude > 0f;
        m_Animator.SetBool("IsWalking", playerMoving);
    }
}
