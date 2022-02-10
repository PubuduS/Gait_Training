using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This sets a footprint tail.
/// Each avatars under their legs, there is a gameobject
/// named LeftFootLocation and RightFootLocation.
/// It cast ray down and the location and position will be used
/// to instantiate footstep.
/// </summary>
public class Footsteps : MonoBehaviour
{
    /// Reference to the m_LeftFootPrint prefab 
    [SerializeField] private GameObject m_LeftFootPrint;

    /// Reference to the m_RightFootPrint prefab 
    [SerializeField] private GameObject m_RightFootPrint;

    /// Reference to the m_LeftFootLocation
    /// Used to get the foot position.
    private Transform m_LeftFootLocation;

    /// Reference to the m_RightFootLocation 
    /// Used to get the foot position.
    private Transform m_RightFootLocation;

    /// Reference to the m_ParentForFootSteps
    /// All the footstep object will spawn under this
    /// as child of this object.
    private Transform m_ParentForFootSteps;

    /// Reference to the m_FootPrintOffset
    /// Need further adjustments to fine tune.
    private float m_FootPrintOffset = 0.05f;

    /// <summary>
    /// Initialize variables and show errors if something is not properly initialized.
    /// </summary>
    private void Start()
    {

        m_LeftFootLocation = GameObject.FindGameObjectWithTag("LeftFootLocation").transform;
        m_RightFootLocation = GameObject.FindGameObjectWithTag("RightFootLocation").transform;

        m_ParentForFootSteps = GameObject.Find("FootStepParent").transform;

        if ((m_LeftFootLocation == null) || (m_RightFootLocation == null))
        {
            Debug.LogError("Can't find foot location to place footprint.");
        }

        if (m_ParentForFootSteps == null)
        {
            Debug.Log("FootStepParent is null");
        }

    }

    /// <summary>
    /// Calls the function to instantiate left foot
    /// </summary>
    public void LeftFoot()
    {
        if(m_LeftFootLocation != null)
        {
            PlaceFootPrint(m_LeftFootLocation, m_LeftFootPrint);
        }
        
    }

    /// <summary>
    /// Calls the function to instantiate right foot
    /// </summary>
    public void RightFoot()
    {
        if(m_RightFootLocation != null)
        {
            PlaceFootPrint(m_RightFootLocation, m_RightFootPrint);
        }
        
    }

    /// <summary>
    /// Cast a ray down and instantiate left or right foot at the hit point.
    /// </summary>
    private void PlaceFootPrint(Transform footLocation, GameObject footprint)
    {
        RaycastHit hit;

        if (Physics.Raycast(footLocation.position, footLocation.forward, out hit))
        {
            Instantiate(footprint, hit.point + hit.normal * m_FootPrintOffset, Quaternion.LookRotation(hit.normal, footLocation.up), m_ParentForFootSteps);
        }

    }

}
