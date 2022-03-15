using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the functionality of the hand-bound UI.
/// </summary>
public class ToggleUIVisibility : MonoBehaviour
{
    /// Reference to the parent object of Avatar UI
    [SerializeField] private GameObject m_ParentAvatarUI;

    /// Reference to the parent object of Noise UI
    [SerializeField] private GameObject m_NoiseUI;

    /// Toggle Avatar UI
    /// You can toggle this UI via voice command or hand-bound menu
    public void ToggleAvatarUI() 
    {
        bool isObjectActive = m_ParentAvatarUI.activeSelf;
        isObjectActive = !isObjectActive;
        m_ParentAvatarUI.SetActive(isObjectActive);
    }

    /// Toggle Noise UI
    /// You can toggle this UI via voice command or hand-bound menu
    public void ToggleNoiseUI()
    {
        bool isObjectActive = m_NoiseUI.activeSelf;
        isObjectActive = !isObjectActive;
        m_NoiseUI.SetActive(isObjectActive);
    }

}
