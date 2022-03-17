using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnWaypoints : MonoBehaviour
{
    public GameObject m_Waypoint;

    public List<Transform> m_AllWaypoints;

    private GameObject m_LastWaypoint;        

    [SerializeField]
    private Transform m_WaypointParent;

    //! Start is called before the first frame update
    void Start()
    {
        m_AllWaypoints = new List<Transform>();  
    }

    //! Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnAWaypoint()
    {
        Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 1.5f;
        m_LastWaypoint = Instantiate( m_Waypoint, position, Quaternion.identity );
        m_AllWaypoints.Add(m_LastWaypoint.transform);
        m_LastWaypoint.transform.SetParent(m_WaypointParent);
    }

    public void DrawLines()
    {
         int waypointCount = m_AllWaypoints.Count;

         LineRenderer line = new GameObject("Line").AddComponent<LineRenderer>();
         line.startColor = Color.red;
         line.endColor = Color.red;
         line.startWidth = 0.1f;
         line.endWidth = 0.1f;
         line.positionCount = m_AllWaypoints.Count + 1;
         line.useWorldSpace = true;


         for ( int i = 0; i < waypointCount; i++ )
         {
             line.SetPosition( i, m_AllWaypoints[i].transform.position );

             if ( i != ( waypointCount - 1 ) )
             {
                 line.SetPosition(i + 1, m_AllWaypoints[i + 1].transform.position);
             }
             else
             {
                 line.SetPosition(i + 1, m_AllWaypoints[0].transform.position);
             }

         }

    }

}
