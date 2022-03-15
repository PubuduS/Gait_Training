using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// The main purpose of this is to mirror the user movements.
/// </summary>
public class MirrorUserMovements : MonoBehaviour
{

    /// Approximately the time it will take to reach the target. 
    /// A smaller value will reach the target faster.
    private float m_TurnSmoothTime = 0.1f;

    /// The current velocity, this value is modified by the function every time you call it.
    private float m_TurnSmoothVelocity;       
   
    /// Update is called once per frame
    void FixedUpdate()
    {
        MatchUserMovements();        
    }

    /// <summary>
    /// Match the user movements and rotation.
    /// </summary>
    private void MatchUserMovements()
    {
        float speed = CamMovementTracker.m_CamTrackerInstance.Speed;

        if ( CamMovementTracker.m_CamTrackerInstance.IsMoving == true )
        {
            /*
            Vector3 direction = Camera.main.transform.position.normalized;
            float targetAngle = Mathf.Atan2( direction.x, direction.z ) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle( transform.eulerAngles.y, targetAngle, ref m_TurnSmoothVelocity, m_TurnSmoothTime );
            transform.rotation = Quaternion.Euler( 0f, targetAngle, 0f );
            transform.Translate( direction * speed * Time.fixedDeltaTime );
            */

            transform.position = new Vector3( 0, -0.6f, 2 ) + Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }

        return;               
    }

}
