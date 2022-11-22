using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is used to set noise pattern levels.
/// When user press a button, it will change the pattern.
/// </summary>
public class SetNoise : MonoBehaviour
{

    /// This will hold the user's selected pattern.
    private ColoredNoise m_PatternType = ColoredNoise.Pink;

    /// This will show the user selection of the pattern
    /// under the avatar in avatar panel.
    [SerializeField] private TextMeshPro m_NoiseLable;

    /// This will show the user selection of the pattern
    /// under the NoiseDataPanel.
    [SerializeField] private TextMeshPro m_NoiseLableInNoiseDataPanel;

    /// This will show the player's current speed in Avatar label.
    [SerializeField] private TextMeshPro m_AvatarLable;

    /// A flag to check wheather an avatar is spawned or not.
    private bool m_Flag = false;

    private GameObject m_NoiseObject = null;
    private RemoveAllComponents m_Remover = null;
    
    /// <summary>
    /// 
    /// </summary>
    private void Start()
    {
        m_NoiseObject = GameObject.FindGameObjectWithTag("NoisePattern");
        m_Remover = m_NoiseObject.GetComponent<RemoveAllComponents>();
        NoiseController.Instance.BaseNoise = m_NoiseObject.AddComponent<PinkNoise>();
    }

    /// <summary>
    /// 
    /// </summary>
    private void Update()
    {
        /// { Debug }
        /// This is only for debug purposes. Remove Later.
        if( GameObject.FindGameObjectWithTag("Avatar") == null )
        {
            m_Flag = false;
        }
        else
        {
            m_Flag = true;            
        }

        if( m_Flag )
        {
            m_AvatarLable.text = "Speed " + CamMovementTracker.m_CamTrackerInstance.Speed;
        }
        /// { Debug }

    }

    /// <summary>
    /// Gets called from the button's OnClick event.
    /// 0 is for deafult pattern
    /// 1 is for Pink noise pattern.
    /// 2 is for White noise(Random) pattern.
    /// 3 is for ISO (Constant) pattern.
    /// </summary>
    public void SetNoisePattern( int pattern )
    {
        m_PatternType = (ColoredNoise)pattern;        
        SetPatternLable();
    }

    /// <summary>
    /// Sets the label according to the user selection.
    /// User can see what they selected in Avatar panel
    /// under the avatar.
    /// </summary>
    private void SetPatternLable()
    {
        switch( m_PatternType )
        {
            case ColoredNoise.Pink:
                SetColoredObject("Noise: Pink");
                NoiseController.Instance.BaseNoise = m_NoiseObject.AddComponent<PinkNoise>();                
                break;

            case ColoredNoise.ISO:
                SetColoredObject("Noise: ISO");
                NoiseController.Instance.BaseNoise = m_NoiseObject.AddComponent<ISONoise>();                
                break;

            case ColoredNoise.White:               
                SetColoredObject("Noise: Random");
                NoiseController.Instance.BaseNoise = m_NoiseObject.AddComponent<WhiteNoise>();                
                break;

            default:
                Debug.Log("Incorect Input. Can't Change Labele");
                m_NoiseLable.text = "!!! Noise: Error !!!";
                m_NoiseLableInNoiseDataPanel.text = m_NoiseLable.text;
                break;
        }
    }

    /// <summary>
    /// Set the lable and removed previously attached noise game objects.
    /// For example, when we transit from pink to white, we don't need pink
    /// gameobject. We can destroy that object.
    /// </summary>
    /// <param name="noise"></param>
    private void SetColoredObject( string noise )
    {
        m_NoiseLable.text = noise;
        m_Remover.RemoveAllNoiseComponents();
        m_NoiseLableInNoiseDataPanel.text = m_NoiseLable.text;
    }

    /// <summary>
    /// Gettr to get the user's selection.
    /// </summary>
    public ColoredNoise GetNoisePattern()
    {
        return m_PatternType;
    }
}
