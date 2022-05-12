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

    /// Reference to ScaleNoisePatterns class.
    private ScaleNoisePatterns m_ScaledNoisePatterns;

    /// Noise index for traveling the noise list.
    /// At the end of the travel, it will be reset to 0.
    private int m_NoiseIndex;
        

    /// This is also use as an error message display 
    /// and current noise display.
    private TextMeshPro m_NoiseDataPanelTitle;

    /// To identify the current noise pattern
    private string m_NoisePatternLbl = "Pink";

    /// <summary>
    /// Gets called first when script is invoked
    /// Cached the references to Animator, SetNoise and NoiseDataPanelTitle
    /// </summary>
    void Awake() 
    {
        m_Animator = GetComponent<Animator>();       
        m_SetNoise = GameObject.Find("NoiseController").GetComponent<SetNoise>();
        m_NoiseDataPanelTitle = GameObject.FindGameObjectWithTag("NoiseDataPanelTitle").GetComponent<TextMeshPro>();
    }

    /// <summary>
    /// Cached a reference to ScaledNoisePatterns class.
    /// </summary>
    private void Start()
    {
        m_ScaledNoisePatterns = GameObject.Find("NoiseController").GetComponent<ScaleNoisePatterns>();
        
        m_NoiseIndex = 0;

        if ( m_ScaledNoisePatterns == null )
        {
            Debug.Log("m_ScaledNoisePatterns is null");
        }
    }

    /// <summary>
    /// Gets called per each frame
    /// The CamMovementTracker singleton instance has speed and IsMoving Flags.
    /// When we are moving, we checked the noise pattern. By default it use pink noise.
    /// If user didn't add SD or mean, it use 1 as the default time to complete the animation( one gait cycle).
    /// Otherwise, it shrinks or expands the time to complete the gait animation cycle.
    /// </summary>
    void Update() 
    {
        
        // Check the player is moving or not.
        var playerMoving = CamMovementTracker.m_CamTrackerInstance.IsMoving;       
        m_Animator.SetBool("IsWalking", playerMoving);
        m_Animator.SetInteger("NoiseLvl", (int)m_SetNoise.GetNoisePattern());

        if( m_ScaledNoisePatterns.NoiseAppliedFlag == true )
        {
            ApplyCorrectNoisePattern();
        }
        else
        {
            Debug.Log("Reset");
            StopCoroutine("WaitTillAnimationComplete");
            m_Animator.speed = 1.0f;
        }
        
        // Future work is needed to change the avatar speed.
        // m_Animator.speed = CamMovementTracker.m_CamTrackerInstance.Speed;                
    }

    /// <summary>
    /// Get the calculated noise pattern and apply that to animation speed.
    /// This noise will shrink or expand the time to complete the gait animation cycle.
    /// </summary>
    private void ApplyCorrectNoisePattern()
    {
        if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_PGN") )
        {

            if ( m_NoiseIndex < m_ScaledNoisePatterns.ScaledPinkNoise.Count )
            {
                m_NoisePatternLbl = "Pink";
                float number = Mathf.Abs( m_ScaledNoisePatterns.ScaledPinkNoise[m_NoiseIndex] );
                StopCoroutine( "WaitTillAnimationComplete" );
                StartCoroutine( "WaitTillAnimationComplete", number );

                m_NoiseIndex++;
            }
            else
            {
                m_NoiseIndex = 0;
            }

        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_Iso") )
        {
            if ( m_NoiseIndex < m_ScaledNoisePatterns.BrownNoise.Count )
            {
                m_NoisePatternLbl = "Brown";
                float number = Mathf.Abs( m_ScaledNoisePatterns.BrownNoise[m_NoiseIndex] );
                StopCoroutine( "WaitTillAnimationComplete" );
                StartCoroutine( "WaitTillAnimationComplete", number );

                m_NoiseIndex++;
            }
            else
            {
                m_NoiseIndex = 0;
            }
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_WGN") )
        {
            if ( m_NoiseIndex < m_ScaledNoisePatterns.WhiteNoise.Count )
            {
                m_NoisePatternLbl = "White";
                float number = Mathf.Abs( m_ScaledNoisePatterns.WhiteNoise[m_NoiseIndex] );
                StopCoroutine( "WaitTillAnimationComplete" );
                StartCoroutine( "WaitTillAnimationComplete", number );

                m_NoiseIndex++;
            }
            else
            {
                m_NoiseIndex = 0;
            }
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") )
        {
            float number = 1.0f;
            StopCoroutine("WaitTillAnimationComplete");
            StartCoroutine("WaitTillAnimationComplete", number);
        }
        else
        {
            Debug.Log(" Animation is unknown ");
        }
    }

    /// <summary>
    /// Here we wait till the animation cycle is completed before we begin playing the next animation cycle.
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    IEnumerator WaitTillAnimationComplete( float number )
    {
        float clipLength = m_Animator.GetCurrentAnimatorStateInfo(0).length;
        //Debug.Log("Animation Length " + clipLength);

        m_Animator.speed = number;
       
        m_NoiseDataPanelTitle.text = m_NoisePatternLbl + " Noise = " + number;        

        float waitTime = clipLength * ( 1 / number );
        yield return new WaitForSeconds( clipLength + m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime );
    }

}
