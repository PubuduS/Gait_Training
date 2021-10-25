using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class is used to control the avartar animation state.
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class AvatarAnimationState : MonoBehaviour 
{
    
    //! Reference to Animator
    private Animator m_Animator;

    //! Reference to NavMeshAgent
    private NavMeshAgent m_NavMesh;

    //! Get called when the script instance is being loaded
    //! Will cache the Animator and NavMeshAgent as soon as it loaded.
    private void Awake() 
    {
        m_Animator = GetComponent<Animator>();
        m_NavMesh = GetComponent<NavMeshAgent>();
    }

    //! Get called at every frame.
    //! if Navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
    private void Update() 
    {
        // if navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
        var playerMoving = m_NavMesh.velocity.magnitude > 0f;
        m_Animator.SetBool("IsWalking", playerMoving);
    }
}
