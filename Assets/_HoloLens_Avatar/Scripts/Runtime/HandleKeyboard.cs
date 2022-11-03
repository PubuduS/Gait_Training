using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Allow touch screen keyboard to enter noise related data.
/// </summary>
public class HandleKeyboard : MonoBehaviour
{

    /// Flag to toggle keyboard.
    /// When set to true, keyboard is not visible
    private bool m_KeyboardToggle = false;

    /// Instance of the touch screen keyboard.    
    private TouchScreenKeyboard m_Keyboard;

    /// This string will hold the keyboard entries.
    private string m_Text = "";

    /// Reference to Mean Period label in noise data panel.
    [SerializeField] private TextMeshPro m_MeanPeriod;

    /// Reference to SD Period label in noise data panel.
    [SerializeField] private TextMeshPro m_SDPeriod;

    /// Reference to sample size label in noise data panel.
    [SerializeField] private TextMeshPro m_SampleSize;

    /// Reference to the inputstring label in the data panel.
    [SerializeField] private TextMeshPro m_InputString;

    /// Reference to the noise label in the data panel.
    [SerializeField] private TextMeshPro m_NoiseLbl;

    /// Reference to the user's preferred walking speed.
    [SerializeField]  private TextMeshPro m_PreferredWalkingSpeed;

    /// <summary>
    /// We instantiate keyboard instance. 
    /// </summary>
    void Start()
    {
        m_Keyboard = TouchScreenKeyboard.Open( m_Text, TouchScreenKeyboardType.NumberPad, false, true, false, false );
    }

    /// <summary>
    /// Whenever use enters space separated noise values it will be set in the noise data panel.
    /// </summary>
    void Update()
    {
        if( m_KeyboardToggle == false )
        {
            m_Keyboard.active = false;
            return;
        }

        if( ( m_Keyboard != null ) && ( m_Keyboard.status == TouchScreenKeyboard.Status.Visible ) && ( m_KeyboardToggle == true ) )
        {
            m_Text = m_Keyboard.text;
            m_InputString.text = "Input = " + m_Text;

            if( m_Text != "" && m_NoiseLbl.text.Equals("Noise: Pink") )
            {
                string[] line = m_Text.Split( char.Parse(" ") );
                m_MeanPeriod.text = "Mean Period = " + line[0];
                m_SDPeriod.text = "SD Period = " + line[1];
                m_SampleSize.text = "Sample Size = " + line[2];
            }
            else if( m_Text != "" && m_NoiseLbl.text.Equals("Noise: ISO") )
            {
                string[] line = m_Text.Split(char.Parse(" "));
                m_PreferredWalkingSpeed.text = "Preferred Speed = " + line[0];
                m_SampleSize.text = "Sample Size = " + line[1];
            }
            else if( m_Text != "" && m_NoiseLbl.text.Equals("Noise: Random") )
            {
                string[] line = m_Text.Split(char.Parse(" "));
                m_MeanPeriod.text = "Mean Period = " + line[0];
                m_SDPeriod.text = "SD Period = " + line[1];                
                m_SampleSize.text = "Sample Size = " + line[2];
            }

        }          

    }

    /// <summary>
    /// Toggle keyboard and set it active or inactive.
    /// </summary>
    public void GetKeyboard()
    {
        m_KeyboardToggle = !m_KeyboardToggle;

        if ( m_KeyboardToggle )
        {            
            m_Keyboard.active = true;
        }
        else
        {
            m_Keyboard.active = false;            
        }
    }

}
