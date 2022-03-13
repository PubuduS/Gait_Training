using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockdownPoint : MonoBehaviour
{

    [SerializeField]
    private GameObject m_Message;
    private TMP_Text m_Text;

    private Rigidbody m_Rigidbody;
    private BoxCollider m_BoxColider;

    private bool m_SetFlag = false;
    private bool m_StopFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_BoxColider = GetComponent<BoxCollider>();
        m_Text = m_Message.GetComponent<TMP_Text>();
        StartCoroutine(LockdownWaypoint());
        m_StopFlag = false;
    }

    // Update is called once per frame
    void Update()
    {  
        if( m_SetFlag == true && m_StopFlag == false )
        {
            Debug.Log(" Co Routine Stopped ");
            StopCoroutine(LockdownWaypoint());
            m_StopFlag = true;
        }
    }

    private IEnumerator LockdownWaypoint()
    {
        yield return new WaitForSeconds(2);
        m_Rigidbody.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        m_Rigidbody.useGravity = false;
        m_BoxColider.enabled = false;
        m_Text.SetText("X = " + transform.position.x + " Y = " + transform.position.y + " Z =  " + transform.position.z);
        m_SetFlag = true;
    }    
}
