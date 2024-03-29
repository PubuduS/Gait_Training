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

    /// Reference to the parent object of Noise Data Panel
    [SerializeField] private GameObject m_NoiseDataPanel;

    /// Toggle the mesh visibility
    private bool m_IsMeshToggled = true;

    /// Toggle the diagnostic system
    private bool m_ToggleDiagnosticFlag = false;

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

    /// Toggle Noise Data Panel
    /// You can toggle this UI via voice command or hand-bound menu
    public void ToggleNoiseDataPanel()
    {
        bool isObjectActive = m_NoiseDataPanel.activeSelf;
        isObjectActive = !isObjectActive;
        m_NoiseDataPanel.SetActive(isObjectActive);
    }

    /// <summary>
    /// Toggle Diagnostic profiler which shows memory and frame rates.
    /// Mapped to the 3rd button in hand bound UI.
    /// </summary>
    public void ToggleDiagnosticProfiler()
    {
        if( m_ToggleDiagnosticFlag )
        {
            Microsoft.MixedReality.Toolkit.CoreServices.DiagnosticsSystem.ShowDiagnostics = false;
            m_ToggleDiagnosticFlag = false;
        }
        else
        {
            Microsoft.MixedReality.Toolkit.CoreServices.DiagnosticsSystem.ShowDiagnostics = true;
            m_ToggleDiagnosticFlag = true;
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
