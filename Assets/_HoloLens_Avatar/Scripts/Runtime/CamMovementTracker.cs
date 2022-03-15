using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A singleton Instance to track Camera Movements
/// This will also calculate distance, rotation and speed (Km/h). 
/// </summary>
public class CamMovementTracker : MonoBehaviour
{
    /// <summary>
    /// Singleton Instance
    /// We have 1 Camera instance and 1 User at all time.
    /// Therefore, I believe Singleton is useful in this case.
    /// </summary>
    public static CamMovementTracker m_CamTrackerInstance { get; private set; }

    /// Just a default value to compare against
    [SerializeField]
    private readonly float m_SampleTime = 1.0f;

    /// Holds last location value    
    private Vector3 m_LastSampleLocation;

    /// Holds last rotation value 
    private Quaternion m_LastSampleRotation;

    /// Holds last timestamp value 
    private float m_LastSampleTime;

    /// Property to get and set Speed
    public float Speed { get; private set; }

    /// Property to get and set RotationDelta
    public float RotationDelta { get; private set; }

    /// Property to get and set Distance
    public float Distance { get; private set; }

    /// Property to get and set IsMoving flag
    public bool IsMoving { get; private set; }

    /// <summary>
    /// If there is an instance, and it is not me, delete myself.
    /// </summary>
    private void Awake()
    {
        if (m_CamTrackerInstance != null && m_CamTrackerInstance != this)
        {
            Destroy(this);
        }
        else
        {
            m_CamTrackerInstance = this;
        }
    }

    /// Start is called before the first frame update
    void Start()
    {
        m_LastSampleTime = Time.time;
        m_LastSampleLocation = CameraCache.Main.transform.position;
        m_LastSampleRotation = CameraCache.Main.transform.rotation;
    }

    /// Update is called once per frame
    void Update()
    {
        if (Time.time - m_LastSampleTime > m_SampleTime)
        {
            Speed = CalculateSpeed();
            RotationDelta = CalculateRotation();
            Distance = CalculateDistanceCovered();
            IsMoving = IsCameraMoving(Speed);

            m_LastSampleTime = Time.time;
            m_LastSampleLocation = CameraCache.Main.transform.position;
            m_LastSampleRotation = CameraCache.Main.transform.rotation;
        }
    }

    /// <summary>
    /// Calculate Distance
    /// </summary>
    private float CalculateDistanceCovered()
    {
        return Vector3.Distance(m_LastSampleLocation, CameraCache.Main.transform.position);
    }

    /// <summary>
    /// Calculate Speed in killo meters per hour.
    /// </summary>
    private float CalculateSpeed()
    {
        return Mathf.Abs(CalculateDistanceCovered() / (Time.time - m_LastSampleTime) * 3.6f);
    }

    /// <summary>
    /// Calculate the rotations.
    /// </summary>
    private float CalculateRotation()
    {
        return Mathf.Abs(Quaternion.Angle(m_LastSampleRotation, CameraCache.Main.transform.rotation));
    }

    /// <summary>
    /// Calculate whether the player is moving or not.
    /// </summary>
    private bool IsCameraMoving(float speed)
    {
        if (speed > 0.6f)
        {
            return true;
        }

        return false;
    }
}
