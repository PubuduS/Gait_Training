using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.SceneUnderstanding.Samples.Unity;
using MRTK_RenderMode = Microsoft.MixedReality.SceneUnderstanding.Samples.Unity.RenderMode;

/// <summary>
/// Script used to change the scene render mode.
/// </summary>
[RequireComponent(typeof(SceneUnderstandingManager))]
public class SceneRenderMode : MonoBehaviour 
{

    //! This button is used to change the render mode (Wireframe or Mesh)
    [SerializeField] private Interactable m_Button;

    //! Get called when the script instance is being loaded
    //! This is used to change the scene render mode.
    //! This required SceneUnderstandingManager component.
    private void Awake() 
    {
        var manager = GetComponent<SceneUnderstandingManager>();

        // Registe the listener event
        m_Button.OnClick.AddListener(() => 
        {
            // Change the render mode
            manager.SceneObjectRequestMode = m_Button.IsToggled ? MRTK_RenderMode.Wireframe : MRTK_RenderMode.Mesh;
            // Bake scene
            manager.BakeScene();
        });
    }
}
