using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    private ScaleNoisePatterns m_ScaledNoisePatterns;

    private int m_NoiseIndex;
        
    private TextMeshPro m_NoiseDataPanelTitle;

    /// <summary>
    /// Gets called when script is loaded
    /// cached the references to Animator and NavMesh agent
    /// </summary>
    void Awake() 
    {
        m_Animator = GetComponent<Animator>();       
        m_SetNoise = GameObject.Find("NoiseController").GetComponent<SetNoise>();
        m_NoiseDataPanelTitle = GameObject.FindGameObjectWithTag("NoiseDataPanelTitle").GetComponent<TextMeshPro>();
    }

    private void Start()
    {
        m_ScaledNoisePatterns = GameObject.Find("Clip").GetComponent<ScaleNoisePatterns>();
        
        m_NoiseIndex = 0;

        if ( m_ScaledNoisePatterns == null )
        {
            Debug.Log("m_ScaledNoisePatterns is null");
        }
    }

    /// <summary>
    /// Gets called per each frame
    /// If navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
    /// In addition to that it will also set the  gait patterns choosen by the user.
    /// </summary>
    void Update() 
    {
        
        // if navmesh velocity magnitude is greater than 0 then play moving animation otherwise idle animation
        var playerMoving = CamMovementTracker.m_CamTrackerInstance.IsMoving;       
        m_Animator.SetBool("IsWalking", playerMoving);

        if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_PGN") )
        {         

            if ( m_NoiseIndex < m_ScaledNoisePatterns.ScaledPinkNoise.Count )
            {
                float number = Mathf.Abs( m_ScaledNoisePatterns.ScaledPinkNoise[m_NoiseIndex] );
                StartCoroutine( "WaitTillAnimationComplete", number );

                m_NoiseIndex++;
            }
            else
            {
                m_NoiseIndex = 0;
            }

        }

        m_Animator.SetInteger("NoiseLvl", (int)m_SetNoise.GetNoisePattern());
        // m_Animator.speed = CamMovementTracker.m_CamTrackerInstance.Speed;                
    }

    IEnumerator WaitTillAnimationComplete( float number )
    {
        float clipLength = m_Animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log("Animation Length " + clipLength);

        m_Animator.speed = number;
        // Debug.Log("Joel_PGN is Playing " + number);
        m_NoiseDataPanelTitle.text = "Pink Noise = " + number;

        //Debug.Log("NomralizedTime " + m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        float waitTime = clipLength * ( 1 / number );
        yield return new WaitForSeconds( clipLength + m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime );
    }

}
