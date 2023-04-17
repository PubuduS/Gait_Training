using System;
using Microsoft.MixedReality.Toolkit.UI;
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

    /// Reference to NoiseController class.
    private NoiseController m_NoiseController;

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

    /// UI confirmation pop-up panel.
    [SerializeField] private GameObject m_DialogPrefabMedium = null;

    /// When Avatar goes to Idle, m_NoiseIndex will
    /// increment by 1. This will omit the next noise
    /// value from applying. We use this flag to prevent
    /// values being ommitted without applying.
    private bool m_IdleElementFlag = false;

    /// Use to apply previous noise after changing idle to moving.
    private int m_Counter = 1;

    /// This list stores timestamps of the animation length of the gait cycle.
    private List<float> m_AnimationLength = null;

    /// This stores the length of the original gait cycle animations.    
    private Dictionary<string, float> m_OriginalAnimationLength;

    /// Property to get the Animation length (Read-Only)
    public List<float> AnimationLength { get => m_AnimationLength; }

    /// <summary>
    /// Gets called first when script is invoked
    /// Cached the references to Animator, SetNoise and NoiseDataPanelTitle
    /// </summary>
    void Awake() 
    {
        m_Animator = GetComponent<Animator>();       
        m_SetNoise = GameObject.Find("NoiseController").GetComponent<SetNoise>();
        m_NoiseDataPanelTitle = GameObject.FindGameObjectWithTag("NoiseDataPanelTitle").GetComponent<TextMeshPro>();
        m_NoiseController = GameObject.Find("NoiseController").GetComponent<NoiseController>();
        m_AnimationLength = new List<float>();
    }

    /// <summary>
    /// Cached a reference to ScaledNoisePatterns class.
    /// </summary>
    private void Start()
    {        
        m_OriginalAnimationLength = new Dictionary<string, float>();
        StoreAnimationClipsLength();
        m_NoiseIndex = 0;
    }

    /// <summary>
    /// Gets called per each frame
    /// The CamMovementTracker singleton instance has speed and IsMoving Flags.
    /// When we are moving, we checked the noise pattern. By default it use pink noise.
    /// If user didn't add SD or mean, it use 1 as the default time to complete the animation( one gait cycle).
    /// Otherwise, it shrinks or expands the time to complete the gait animation cycle.
    /// </summary>
    private void Update() 
    {
        
        // Check the player is moving or not.
        var playerMoving = CamMovementTracker.m_CamTrackerInstance.IsMoving;       
        m_Animator.SetBool("IsWalking", playerMoving);
        m_Animator.SetInteger("NoiseLvl", (int)m_SetNoise.GetNoisePattern());

        if( m_NoiseController.BaseNoise.NoiseAppliedFlag == true )
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
            ResetNoiseAfterEnd( m_NoiseController.BaseNoise.NoiseValueList.Count, "Pink" );
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_Iso") )
        {            
            ResetNoiseAfterEnd( m_NoiseController.BaseNoise.NoiseValueList.Count, "ISO" );
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Joel_WGN") )
        {
            ResetNoiseAfterEnd( m_NoiseController.BaseNoise.NoiseValueList.Count, "Random" );
        }
        else if( m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") )
        {
            m_NoisePatternLbl = "Idle";            
            m_NoiseDataPanelTitle.text = m_NoisePatternLbl + " Noise = " + m_DefaultAnimationSpeed;
        }
        else
        {
            Debug.Log(" Animation is unknown ");
        }
    }

    /// <summary>
    /// When we reached to the end of the colred noise list,
    /// we looped back to the beginning.
    /// </summary>
    private void ResetNoiseAfterEnd( int noiseLength, string lbl )
    {
        m_NoisePatternLbl = lbl;

        if( m_NoiseIndex >= noiseLength )
        {
            OpenConfirmationDialogMedium();
            m_NoiseIndex = 0;
        }
    }

    /// <summary>
    /// Opens confirmation dialog example
    /// </summary>
    private void OpenConfirmationDialogMedium()
    {
        Dialog.Open( m_DialogPrefabMedium, DialogButtonType.OK, "Good Job!", "Times Up!" +
                     " You covered required distance.", true);
    }

    /// <summary>
    /// Expand and Shrink gait cycle animation
    /// The completion time of animation is the value of pink or white noise.
    /// </summary>
    /// <returns></returns>
    private void RunGaitCycle()
    {
        float noiseValue = m_DefaultAnimationSpeed;
        float desiredSpeed = m_DefaultAnimationSpeed;

        if( String.Equals( m_NoisePatternLbl, "Pink" ) || ( String.Equals( m_NoisePatternLbl, "Random" ) ) ||
            String.Equals( m_NoisePatternLbl, "ISO" ) )
        {
            noiseValue = Mathf.Abs( m_NoiseController.BaseNoise.NoiseValueList[m_NoiseIndex] );
        }
        else if( String.Equals( m_NoisePatternLbl, "Idle" ) )
        {
            m_IdleElementFlag = true;
            return;
        }
        else
        {
            m_NoisePatternLbl = "ERROR!!!";
            noiseValue = m_DefaultAnimationSpeed;
        }
        
        float animationLength = 0.0f;
        bool isValidAnimLength = m_OriginalAnimationLength.TryGetValue( m_NoisePatternLbl, out animationLength );

        if( isValidAnimLength && noiseValue != 0 )
        {
            desiredSpeed = ( animationLength / noiseValue );            
        }

        m_IsAnimationLocked = true;
        m_Animator.speed = desiredSpeed;
        m_NoiseDataPanelTitle.text = m_NoisePatternLbl + " Noise = " + noiseValue;

        float len = m_Animator.GetCurrentAnimatorStateInfo(0).length;
        m_AnimationLength.Add( len );

        // Only for debug.
        // Debug.Log( "Number: " + noiseValue + " Time: " + Time.realtimeSinceStartup + " Len: " + len );        
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
                    m_OriginalAnimationLength.Add( "Random", clip.length );
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
        if( m_IsAnimationLocked == false )
        {
            RunGaitCycle();            
        }
    }

    /// <summary>
    /// Unlock the animation after it played out.
    /// Mapped to animation event.
    /// </summary>
    private void UnlockAnimation()
    {
        m_IsAnimationLocked = false;

        if ( ( m_IsAnimationLocked == false ) && ( m_IdleElementFlag == false ) )
        {            
            m_NoiseIndex++;            
        }
        else
        {
            m_Counter++;

            // Prevent first element from being skipped.
            if( m_NoiseIndex == 0 && m_Counter == 2 )
            {
                m_Counter = 1;
                m_IdleElementFlag = false;
            }

            // Prevent element from being skipped after transtion from idle to moving.
            if( m_Counter == 3 )
            {
                m_Counter = 1;
                m_IdleElementFlag = false;
            }
        }
    }

    /// <summary>
    /// Called after user click cancel pattern button in Noise UI.
    /// </summary>
    public void ResetAnimationLengthList()
    {
        m_AnimationLength.Clear();
        m_NoiseIndex = 0;
        m_Counter = 1;
    }
}
