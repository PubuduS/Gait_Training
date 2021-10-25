using UnityEngine;
using UnityEngine.AI;
using Microsoft.MixedReality.SceneUnderstanding;
using Microsoft.MixedReality.SceneUnderstanding.Samples.Unity;

/// <summary>
/// This Script will generate a NavMesh in a Scene Understanding Scene already generated inside unity
/// using the unity built in NavMesh engine
/// </summary>
public class NavMeshBaker : MonoBehaviour 
{
    //! Defines the Walkable and NotWalkable DataTypes
    private enum AreaType 
    {
        Walkable,
        NotWalkable
    }

    //! Nav Mesh Surface is a Unity Built in Type, from 
    //! the unity NavMesh Assets
    public NavMeshSurface navMeshSurf;

    //! This is where all the scene understanding objects will be added at the runtime.
    public GameObject sceneRoot;

    //! This function runs as a callback for the OnLoadFinished event
    //! In the SceneUnderstandingManager Component
    //! Everytime SceneUnderstanding finish adding a scene, this will add navmesh
    //! to the floor area marking it as a walkable area.
    public void BakeMesh() 
    {
        UpdateNavMeshSettingsForObjsUnderRoot();
        navMeshSurf.BuildNavMesh();

        var agents = FindObjectsOfType<NavMeshAgent>();

        // Re-sample existing agent positions
        foreach(var agent in agents)
            if (NavMesh.SamplePosition(agent.transform.position, out NavMeshHit closestHit, 500, NavMesh.AllAreas))
                agent.transform.position = closestHit.position;
    }

    //! This will iterate through all the scene understanding object under the scene root.
    //! It will mark floor area as walkable and other areas not walkable
    //! NavMesh Agent will use these labels to calculate best possible path.
    void UpdateNavMeshSettingsForObjsUnderRoot() 
    {
        // Iterate all the Scene Objects
        foreach (Transform sceneObjContainer in sceneRoot.transform)
        {
            foreach (Transform sceneObj in sceneObjContainer.transform)
            {
                var nvm = sceneObj.gameObject.AddComponent<NavMeshModifier>();
                nvm.overrideArea = true;

                var properties = sceneObj.GetComponent<SceneUnderstandingProperties>();

                if (properties != null)
                {
                    // Walkable = 0, Not Walkable = 1
                    // This area types are unity predefined, in the unity inspector in the navigation tab go to areas
                    // to see them
                    nvm.area = properties.suObjectKind == SceneObjectKind.Floor ? (int)AreaType.Walkable : (int)AreaType.NotWalkable;
                }
                else
                {
                    // Walkable = 0, Not Walkable = 1
                    // This area types are unity predefined, in the unity inspector in the navigation tab go to areas
                    // to see them
                    nvm.area = sceneObj.parent.name == "Floor" ? (int)AreaType.Walkable : (int)AreaType.NotWalkable;
                }
            }
        }

    }
}
