using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class HandleKeyboard : MonoBehaviour
{

    private bool m_KeyboardToggle = true;

    private TouchScreenKeyboard m_Keyboard;

    private string m_Text = "";

    [SerializeField] private TextMeshPro m_MeanPeriod;

    [SerializeField] private TextMeshPro m_SDPeriod;

    [SerializeField] private TextMeshPro m_SD;

    // Start is called before the first frame update
    void Start()
    {
        m_Keyboard = TouchScreenKeyboard.Open( m_Text, TouchScreenKeyboardType.NumberPad, false, true, false, false );
    }

    // Update is called once per frame
    void Update()
    {
        if( ( m_Keyboard != null ) && ( m_Keyboard.status == TouchScreenKeyboard.Status.Done ) && ( m_KeyboardToggle == true ) )
        {
            m_Text = m_Keyboard.text;
            
            if( m_Text != "" )
            {
                string[] line = m_Text.Split(char.Parse(" "));
                m_MeanPeriod.text = line[0];
                m_SDPeriod.text = line[1];
                m_SD.text = line[2];
            }

        }          

    }

    public void GetKeyboard()
    {
        ToggleKeyboard();

        if (m_KeyboardToggle)
        {
            m_Keyboard.active = true;
        }
        else
        {
            m_Keyboard.active = false;
        }

    }

    public void ToggleKeyboard()
    {
        m_KeyboardToggle = !m_KeyboardToggle;
    }
}
