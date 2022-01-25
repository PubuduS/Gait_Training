using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField]
    private GameObject m_LeftFootPrint;

    [SerializeField]
    private GameObject m_RightFootPrint;

    private Transform m_LeftFootLocation;
    private Transform m_RightFootLocation;
    private Transform m_ParentForFootSteps;

    private float m_FootPrintOffset = 0.05f;

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

    public void LeftFoot()
    {
        PlaceFootPrint(m_LeftFootLocation, m_LeftFootPrint);
    }

    public void RightFoot()
    {
        PlaceFootPrint(m_RightFootLocation, m_RightFootPrint);
    }

    private void PlaceFootPrint(Transform footLocation, GameObject footprint)
    {
        RaycastHit hit;

        if (Physics.Raycast(footLocation.position, footLocation.forward, out hit))
        {
            Instantiate(footprint, hit.point + hit.normal * m_FootPrintOffset, Quaternion.LookRotation(hit.normal, footLocation.up), m_ParentForFootSteps);
        }

    }

}
