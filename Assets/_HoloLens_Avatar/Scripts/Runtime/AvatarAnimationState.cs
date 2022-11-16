using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

    /// Default value of animation speed.
    private const float m_DefaultAnimationSpeed = 1.0f;

    /// This list stores timestamps of the left heel strikes.
    private List<float> m_LeftFootTimeStamps = null;

    /// This list stores timestamps of the right heel strikes.
    private List<float> m_RightFootTimeStamps = null;

    /// This stores the length of the original gait cycle animations.    
    private Dictionary<string, float> m_OriginalAnimationLength;

    /// Property to get the left foot time stamp (Read-Only)
    public List<float> LeftFootTimeStamps { get => m_LeftFootTimeStamps; }

    /// Property to get the right foot time stamp (Read-Only)
    public List<float> RightFootTimeStamps { get => m_RightFootTimeStamps; }


    /// <summary>
    /// Gets called first when script is invoked
    /// Cached the references to Animator, SetNoise and NoiseDataPanelTitle
    /// </summary>
    void Awake() 
    {
        m_Animator = GetComponent<Animator>();       
        m_SetNoise = GameObject.Find("NoiseController").GetComponent<SetNoise>();
        m_NoiseDataPanelTitle = GameObject.FindGameObjectWithTag("NoiseDataPanelTitle").GetComponent<TextMeshPro>();
        m_LeftFootTimeStamps = new List<float>();
        m_RightFootTimeStamps = new List<float>();
    }

    /// <summary>
    /// Cached a reference to ScaledNoisePatterns class.
    /// </summary>
    private void Start()
    {
        m_ScaledNoisePatterns = GameObject.Find("NoiseController").GetComponent<ScaleNoisePatterns>();
        m_OriginalAnimationLength = new Dictionary<string, float>();
        StoreAnimationClipsLength();

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
            m_Animator.speed = m_DefaultAnimationSpeed;
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
            if( m_NoiseIndex < m_ScaledNoisePatterns.ScaledPinkNoise.Count )
            {
                m_NoisePatternLbl = "Pink";

                if( m_IsAnimationLocked == false )
                {
                    StartCoroutine( RunGaitCycle( m_NoisePatternLbl ) );
                }                
            }
            else
            {
                m_NoiseIndex = 0;
            }
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_Iso") )
        {
             if( m_IsAnimationLocked == false )
             {
                m_NoisePatternLbl = "ISO";
                StartCoroutine( RunGaitCycle( m_NoisePatternLbl ) );
            }
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_WGN") )
        {
            if( m_NoiseIndex < m_ScaledNoisePatterns.WhiteNoise.Count )
            {
                if( m_IsAnimationLocked == false )
                {
                    m_NoisePatternLbl = "White";
                    StartCoroutine( RunGaitCycle( m_NoisePatternLbl ) );
                }
            }
            else
            {
                m_NoiseIndex = 0;
            }
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") )
        {            
            m_Animator.speed = m_DefaultAnimationSpeed;
            m_NoiseDataPanelTitle.text = m_NoisePatternLbl + " Noise = " + m_DefaultAnimationSpeed;
        }
        else
        {
            Debug.Log(" Animation is unknown ");
        }
    }

    /// <summary>
    /// Expand and Shrink gait cycle animation
    /// The completion time of animation is the value of pink or white noise.
    /// </summary>
    /// <param name="noise"></param>
    /// <returns></returns>
    private IEnumerator RunGaitCycle( string noise )
    {
        float noiseValue = m_DefaultAnimationSpeed;
        float desiredSpeed = m_DefaultAnimationSpeed;

        if( String.Equals( noise, "Pink" ) )
        {
            noiseValue = Mathf.Abs( m_ScaledNoisePatterns.ScaledPinkNoise[m_NoiseIndex] );
        }
        else if( String.Equals( noise, "White" ) )
        {
            noiseValue = Mathf.Abs( m_ScaledNoisePatterns.WhiteNoise[m_NoiseIndex] );
        }
        else if( String.Equals( noise, "ISO" ) )
        {
            noiseValue = Mathf.Abs( m_ScaledNoisePatterns.PreferredWalkingSpeed );
        }
        else
        {
            m_NoisePatternLbl = "ERROR!!!";
            noiseValue = m_DefaultAnimationSpeed;
        }

        float animationLength = 0.0f;
        bool isValidAnimLength = m_OriginalAnimationLength.TryGetValue( noise, out animationLength );
                
        if( isValidAnimLength )
        {
            desiredSpeed = ( animationLength / noiseValue );
        }

        m_IsAnimationLocked = true;
        m_Animator.speed = desiredSpeed;
        m_NoiseDataPanelTitle.text = m_NoisePatternLbl + " Noise = " + noiseValue;

        var len = m_Animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log("Number: " + noiseValue + " Time: " + Time.realtimeSinceStartup + " Len: " + len);

        yield return new WaitForSeconds( noiseValue );
        m_IsAnimationLocked = false;

        if( !String.Equals( noise, "ISO" ) )
        {
            m_NoiseIndex++;
        }
        
    }

    /// <summary>
    /// Store the original animation clips values in a dictionary.
    /// </summary>
    private void StoreAnimationClipsLength()
    {
        AnimationClip[] clips = m_Animator.runtimeAnimatorController.animationClips;

        foreach( AnimationClip clip in clips )
        {
            switch( clip.name )
            {
                case "Joel_PGN":
                    m_OriginalAnimationLength.Add( "Pink", clip.length );
                    break;

                case "Joel_Iso":
                    m_OriginalAnimationLength.Add( "ISO", clip.length );
                    break;

                case "Joel_WGN":
                    m_OriginalAnimationLength.Add( "White", clip.length );
                    break;

                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Lockdown the animation until it played out.
    /// Animation will unlock after it complete the current loop.
    /// This event is mapped to animation event.
    /// </summary>
    private void LockdownAnimation()
    {
        //m_IsAnimationLocked = true;
        m_RightFootTimeStamps.Add( Time.unscaledTime );
    }

    /// <summary>
    /// Unlock the animation after it played out.
    /// Mapped to animation event.
    /// </summary>
    private void UnlockAnimation()
    {        
        //m_IsAnimationLocked = false;
    }
}
