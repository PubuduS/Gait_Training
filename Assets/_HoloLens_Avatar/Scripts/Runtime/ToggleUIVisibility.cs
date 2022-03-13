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

    /// Reference to the parent object of Calibator UI
    [SerializeField] private GameObject m_ParentSceneUnderstandingUI;

    /// Reference to the parent object of Noise UI
    [SerializeField] private GameObject m_NoiseUI;

    /// Reference to the Avatar
    private GameObject m_Avatar;

    /// Toggle the mesh visibility
    private bool m_IsMeshToggled = false;

    /// Toggle Avatar UI
    /// You can toggle this UI via voice command or hand-bound menu
    public void ToggleAvatarUI() 
    {
        bool isObjectActive = m_ParentAvatarUI.activeSelf;
        isObjectActive = !isObjectActive;
        m_ParentAvatarUI.SetActive(isObjectActive);
    }

    /// Toggle Calibator UI
    /// You can toggle this UI via voice command or hand-bound menu
    public void ToggleCalibratorUI()
    {        
        bool isObjectActive = m_ParentSceneUnderstandingUI.activeSelf;
        isObjectActive = !isObjectActive;
        m_ParentSceneUnderstandingUI.SetActive(isObjectActive);
    }

    /// Toggle Noise UI
    /// You can toggle this UI via voice command or hand-bound menu
    public void ToggleNoiseUI()
    {
        bool isObjectActive = m_NoiseUI.activeSelf;
        isObjectActive = !isObjectActive;
        m_NoiseUI.SetActive(isObjectActive);
    }

    /// Toggle GazeWalking
    /// When this is activated, avatar will walk to the point where user is currently looking.
    public void ToggleGazeWalking()
    {
        m_Avatar = GameObject.FindGameObjectWithTag("Avatar");

        if(m_Avatar != null)
        {
            PathFindingController path = m_Avatar.GetComponent<PathFindingController>();
            path.ToggleGazeWalk();
        }
        else
        {
            return;
        }              
    }

    /// Toggle SpartialMesh
    /// Activate by wireframe button located above the avatar panel.
    public void ToggleMesh()
    {        
        var observer = Microsoft.MixedReality.Toolkit.CoreServices.GetSpatialAwarenessSystemDataProvider<Microsoft.MixedReality.Toolkit.XRSDK.OpenXR.OpenXRSpatialAwarenessMeshObserver>();
        
        m_IsMeshToggled = !m_IsMeshToggled;

        if( m_IsMeshToggled == true )
        {
            observer.DisplayOption = Microsoft.MixedReality.Toolkit.SpatialAwareness.SpatialAwarenessMeshDisplayOptions.Visible;
        }
        else
        {
            observer.DisplayOption = Microsoft.MixedReality.Toolkit.SpatialAwareness.SpatialAwarenessMeshDisplayOptions.Occlusion;
        }
    }
}
