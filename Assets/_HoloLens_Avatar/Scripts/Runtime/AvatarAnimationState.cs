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

    /// This flag is used to lock down the animation till it played out.
    private bool m_IsAnimationLocked = false;

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
            m_IsAnimationLocked = false;
            m_Animator.speed = 1.0f;
        }
        
        // TODO Future work is needed to change the avatar speed.
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
            HandleNoise( "Pink", m_ScaledNoisePatterns.ScaledPinkNoise.Count, m_ScaledNoisePatterns.ScaledPinkNoise[m_NoiseIndex] );
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_Iso") )
        {
             if( m_IsAnimationLocked == false )
             {
                m_NoisePatternLbl = "ISO";
                float number = Mathf.Abs( m_ScaledNoisePatterns.PreferredWalkingSpeed );
                ApplyNoiseToAnimator( number );                
            }
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_WGN") )
        {
            HandleNoise( "White", m_ScaledNoisePatterns.WhiteNoise.Count, m_ScaledNoisePatterns.WhiteNoise[m_NoiseIndex] );
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") )
        {
            float number = 1.0f;
            ApplyNoiseToAnimator( number );            
        }
        else
        {
            Debug.Log(" Animation is unknown ");
        }
    }

    /// <summary>
    /// Apply noise frequency to animator
    /// </summary>
    /// <param name="number"></param>
    private void ApplyNoiseToAnimator( float number = 1.0f )
    {
        m_Animator.speed = number;
        m_NoiseDataPanelTitle.text = m_NoisePatternLbl + " Noise = " + number;
    }

    /// <summary>
    /// Lockdown the animation until it played out.
    /// Animation will unlock after it complete the current loop.
    /// This event is mapped to animation event.
    /// </summary>
    private void LockdownAnimation()
    {        
        m_IsAnimationLocked = true;
    }

    /// <summary>
    /// Unlock the animation after it played out.
    /// Mapped to animation event.
    /// </summary>
    private void UnlockAnimation()
    {        
        m_IsAnimationLocked = false;
    }

    /// <summary>
    /// Apply the White and Pink noise to expand and shrink the gait cycle.
    /// </summary>
    /// <param name="noiseName"></param>
    /// <param name="count"></param>
    /// <param name="number"></param>
    private void HandleNoise( string noiseName, int count, float number )
    {
        if( m_NoiseIndex < count )
        {
            m_NoisePatternLbl = noiseName;            
            ApplyNoiseToAnimator( Mathf.Abs(number) );
            // Debug.Log("Noise" + number);                

            if ( m_IsAnimationLocked == false )
            {
                m_NoiseIndex++;
            }

        }
        else
        {
            m_NoiseIndex = 0;
        }
    }

}
