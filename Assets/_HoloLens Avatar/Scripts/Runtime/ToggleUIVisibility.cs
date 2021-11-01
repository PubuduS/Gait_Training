using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUIVisibility : MonoBehaviour
{
    //! Reference to the parent object of Avatar UI
    [SerializeField] private GameObject m_ParentAvatarUI;

    //! Reference to the parent object of Calibator UI
    [SerializeField] private GameObject m_ParentSceneUnderstandingUI;

    //! Reference to the Avatar
    private GameObject m_Avatar;

    //! Toggle Avatar UI
    //! You can toggle this UI via voice command or hand-bound menu
    public void ToggleAvatarUI() 
    {
        bool isObjectActive = m_ParentAvatarUI.activeSelf;
        isObjectActive = !isObjectActive;
        m_ParentAvatarUI.SetActive(isObjectActive);
    }

    //! Toggle Calibator UI
    //! You can toggle this UI via voice command or hand-bound menu
    public void ToggleCalibratorUI()
    {        
        bool isObjectActive = m_ParentSceneUnderstandingUI.activeSelf;
        isObjectActive = !isObjectActive;
        m_ParentSceneUnderstandingUI.SetActive(isObjectActive);
    }

    //! Toggle GazeWalking
    //! When this is activated, avatar will walk to the point where user is currently looking.
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
}
