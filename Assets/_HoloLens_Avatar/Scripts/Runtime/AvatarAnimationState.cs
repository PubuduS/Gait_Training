using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class is used to control the avartar animation state.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AvatarAnimationState : MonoBehaviour 
{
        
    /// Reference to the animator    
    private Animator m_Animator;

    /// Reference to the SetNoise Script 
    private SetNoise m_SetNoise;

    private MoveBetweenPoints m_MoveBetweenPoints;

    /// <summary>
    /// Gets called when script is loaded
    /// cached the references to Animator and NavMesh agent
    /// </summary>
    void Awake() 
    {
        m_Animator = GetComponent<Animator>();       
        m_SetNoise = GameObject.Find("NoiseController").GetComponent<SetNoise>();
        m_MoveBetweenPoints = GetComponent<MoveBetweenPoints>();


    }

    /// <summary>
    /// Gets called per each frame
    /// If navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
    /// In addition to that it will also set the  gait patterns choosen by the user.
    /// </summary>
    void Update() 
    {
        // if navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
        var playerMoving = m_MoveBetweenPoints.m_IsPlayerMoving;
        m_Animator.SetBool("IsWalking", playerMoving);
        m_Animator.SetInteger("NoiseLvl", (int)m_SetNoise.GetNoisePattern());        
    }
}
