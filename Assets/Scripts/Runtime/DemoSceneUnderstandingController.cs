// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Examples.Demos;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Microsoft.MixedReality.Toolkit.Experimental.SceneUnderstanding
{
    /// <summary>
    /// Controll the scene understanding system
    /// </summary>
    public class DemoSceneUnderstandingController : DemoSpatialMeshHandler, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        #region Private Fields

        #region Serialized Fields


        /// This name is use as a prefix when saving the scene.
        /// Format will be $"{prefix}_{timestamp}.bytes"
        [SerializeField]
        private string m_SavedSceneNamePrefix = "SceneUnderstandingData";

        [Header("UI")]

        /// Reference to Toggle Auto Update Button
        /// Automatically update the scene at each 5 seconds.
        /// If you need to change the time inteval do this
        /// MixedRealityToolkit -> Spartial Awareness -> WindowsSceneUndersandingObserver -> Update Interval
        [SerializeField]
        private Interactable m_AutoUpdateToggle = null;


        /// Reference to Toggle Quads Button
        /// Request Quads from SU
        /// NavMesh is baked into Quads.
        /// Without Quads Avatar can't navigate.
        [SerializeField]
        private Interactable m_QuadsToggle = null;

        /// Reference to Toggle Infer Regions Button
        /// Fill out the holes.
        [SerializeField]
        private Interactable m_InferRegionsToggle = null;

        /// Reference to Toggle Mesh Button
        /// Request World Mesh from SU
        [SerializeField]
        private Interactable m_MeshesToggle = null;

        /// Reference to Toggle Mask Button
        /// Request Occlusion from SU
        /// Occlusion for more realistic experience.
        [SerializeField]
        private Interactable m_MaskToggle = null;

        /// Reference to Toggle Platform Button
        [SerializeField]
        private Interactable m_PlatformToggle = null;

        /// Toggle Wall Button
        [SerializeField]
        private Interactable m_WallToggle = null;

        /// Reference to Toggle Floor Button
        [SerializeField]
        private Interactable m_FloorToggle = null;

        /// Reference to Toggle Ceiling Button
        [SerializeField]
        private Interactable m_CeilingToggle = null;

        /// Reference to Toggle World Button
        [SerializeField]
        private Interactable m_WorldToggle = null;

        /// Reference to Toggle Inferred Button
        [SerializeField]
        private Interactable m_CompletelyInferred = null;

        /// Reference to Toggle Background Objects Button
        [SerializeField]
        private Interactable m_BackgroundToggle = null;

        #endregion Serialized Fields


        /// Scene Understanding Observer
        private IMixedRealitySceneUnderstandingObserver m_Observer;

        /// Store the observed scene objects
        private Dictionary<SpatialAwarenessSurfaceTypes, Dictionary<int, SpatialAwarenessSceneObject>> m_ObservedSceneObjects;

        /// Reference to NavMesh Surface
        [SerializeField]
        private NavMeshSurface m_NavMeshSurf;

        /// Toggle Colors.
        /// Each scene objects are assigned a color
        /// When enables display the scene objects with assigned colors.
        private bool m_SceneObjectColorToggle = true;

        
        /// Defines Walkable and NotWalkable enumarations      
        private enum AreaType
        {
            Walkable,
            NotWalkable
        }

        #endregion Private Fields

        #region MonoBehaviour Functions

        /// <summary>
        /// Get called at the start.
        /// Initialize observer, buttons and observedSceneObjects
        /// </summary>
        protected override void Start()
        {
            m_Observer = CoreServices.GetSpatialAwarenessSystemDataProvider<IMixedRealitySceneUnderstandingObserver>();

            if (m_Observer == null)
            {
                Debug.LogError("Couldn't access Scene Understanding Observer! Please make sure the current build target is set to Universal Windows Platform. "
                    + "Visit https://docs.microsoft.com/windows/mixed-reality/mrtk-unity/features/spatial-awareness/scene-understanding for more information.");
                return;
            }
            InitToggleButtonState();            
            m_ObservedSceneObjects = new Dictionary<SpatialAwarenessSurfaceTypes, Dictionary<int, SpatialAwarenessSceneObject>>();
        }

        /// <summary>
        /// Register the event handler
        /// </summary>
        protected override void OnEnable()
        {
            RegisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        /// <summary>
        /// Unregister the event handler on disable
        /// </summary>
        protected override void OnDisable()
        {
            UnregisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        /// <summary>
        /// Unregister the event handler on destroy
        /// </summary>
        protected override void OnDestroy()
        {
            UnregisterEventHandlers<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>, SpatialAwarenessSceneObject>();
        }

        #endregion MonoBehaviour Functions

        #region IMixedRealitySpatialAwarenessObservationHandler Implementations

        /// <summary>
        /// This method called everytime a SceneObject created by the SU observer
        ///  The eventData contains everything you need do something useful
        ///  For each quads, we bake NavMesh so our AI can find paths.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {

            AddToData(eventData.Id);

            if (m_ObservedSceneObjects.TryGetValue(eventData.SpatialObject.SurfaceType, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjectDict))
            {
                sceneObjectDict.Add(eventData.Id, eventData.SpatialObject);
            }
            else
            {
                m_ObservedSceneObjects.Add(eventData.SpatialObject.SurfaceType, new Dictionary<int, SpatialAwarenessSceneObject> { { eventData.Id, eventData.SpatialObject } });
            }            


            foreach (var quad in eventData.SpatialObject.Quads)
            {
                if(m_SceneObjectColorToggle)
                {
                    quad.GameObject.GetComponent<Renderer>().material.color = ColorForSurfaceType(eventData.SpatialObject.SurfaceType);
                }
                else
                {
                    quad.GameObject.GetComponent<Renderer>().material.color = Color.clear;
                }                    

                var nvmQuads = eventData.SpatialObject.Quads[0].GameObject.AddComponent<NavMeshModifier>();
                nvmQuads.overrideArea = true;

                if (eventData.SpatialObject.SurfaceType == SpatialAwarenessSurfaceTypes.Floor)
                {
                    nvmQuads.area = (int)AreaType.Walkable;
                }
                else
                {
                    nvmQuads.area = (int)AreaType.NotWalkable;
                }
            }
                // When bake navmesh surface into meshes, the avatar will spawn on a weird location.
                // Currently, I have no idea why this happens.
                // Need further investigation.
                /*
                foreach (var mesh in eventData.SpatialObject.Meshes)
                {
                    var nvmMesh = eventData.SpatialObject.Meshes[0].GameObject.AddComponent<NavMeshModifier>();
                    nvmMesh.overrideArea = true;


                    if (eventData.SpatialObject.SurfaceType == SpatialAwarenessSurfaceTypes.Floor)
                    {
                        nvmMesh.area = (int)AreaType.Walkable;
                    }
                    else
                    {
                        nvmMesh.area = (int)AreaType.NotWalkable;
                    }
                }*/

                m_NavMeshSurf.BuildNavMesh();
            
        }

        /// <summary>
        /// Update the observed objects when we have new data
        /// </summary>
        /// <param name="eventData"></param>
        public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            UpdateData(eventData.Id);
            
            if (m_ObservedSceneObjects.TryGetValue(eventData.SpatialObject.SurfaceType, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjectDict))
            {
                m_ObservedSceneObjects[eventData.SpatialObject.SurfaceType][eventData.Id] = eventData.SpatialObject;
            }
            else
            {
                m_ObservedSceneObjects.Add(eventData.SpatialObject.SurfaceType, new Dictionary<int, SpatialAwarenessSceneObject> { { eventData.Id, eventData.SpatialObject } });
            }
        }

        /// <summary>
        /// Remove the observed objects
        /// </summary>
        /// <param name="eventData"></param>
        public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            RemoveFromData(eventData.Id);

            foreach (var sceneObjectDict in m_ObservedSceneObjects.Values)
            {
                sceneObjectDict?.Remove(eventData.Id);
            }
        }

        #endregion IMixedRealitySpatialAwarenessObservationHandler Implementations

        #region Public Functions

        /// <summary>
        /// Get all currently observed SceneObjects of a certain type.
        /// </summary>
        /// <remarks>
        /// Before calling this function, the observer should be configured to observe the specified type by including that type in the SurfaceTypes property.
        /// </remarks>
        /// <returns>A dictionary with the scene objects of the requested type being the values and their ids being the keys.</returns>
        public IReadOnlyDictionary<int, SpatialAwarenessSceneObject> GetSceneObjectsOfType(SpatialAwarenessSurfaceTypes type)
        {
            if (!m_Observer.SurfaceTypes.HasFlag(type))
            {
                Debug.LogErrorFormat("The Scene Objects of type {0} are not being observed. You should add {0} to the SurfaceTypes property of the observer in advance.", type);
            }

            if (m_ObservedSceneObjects.TryGetValue(type, out Dictionary<int, SpatialAwarenessSceneObject> sceneObjects))
            {
                return sceneObjects;
            }
            else
            {
                m_ObservedSceneObjects.Add(type, new Dictionary<int, SpatialAwarenessSceneObject>());
                return m_ObservedSceneObjects[type];
            }
        }

        #region UI Functions

        /// <summary>
        /// Request the observer to update the scene
        /// </summary>
        public void UpdateScene()
        {
            m_Observer.UpdateOnDemand();
        }

        /// <summary>
        /// Request the observer to save the scene
        /// </summary>
        public void SaveScene()
        {
            m_Observer.SaveScene(m_SavedSceneNamePrefix);
        }

        /// <summary>
        /// Request the observer to clear the observations in the scene
        /// </summary>
        public void ClearScene()
        {
            m_Observer.ClearObservations();
        }

        /// <summary>
        /// Change the auto update state of the observer
        /// </summary>
        public void ToggleAutoUpdate()
        {
            m_Observer.AutoUpdate = !m_Observer.AutoUpdate;
        }

        /// <summary>
        /// Change whether to request occlusion mask from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleOcclusionMask()
        {
            var observerMask = m_Observer.RequestOcclusionMask;
            m_Observer.RequestOcclusionMask = !observerMask;
            if (m_Observer.RequestOcclusionMask)
            {
                if (!(m_Observer.RequestPlaneData || m_Observer.RequestMeshData))
                {
                    m_Observer.RequestPlaneData = true;
                    m_QuadsToggle.IsToggled = true;
                }
            }
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request plane data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleGeneratePlanes()
        {
            m_Observer.RequestPlaneData = !m_Observer.RequestPlaneData;
            if (m_Observer.RequestPlaneData)
            {
                m_Observer.RequestMeshData = false;
                m_MeshesToggle.IsToggled = false;
            }
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request mesh data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleGenerateMeshes()
        {
            m_Observer.RequestMeshData = !m_Observer.RequestMeshData;
            if (m_Observer.RequestMeshData)
            {
                m_Observer.RequestPlaneData = false;
                m_QuadsToggle.IsToggled = false;
            }
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request floor data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleFloors()
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Floor);
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request wall data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleWalls()
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Wall);
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request ceiling data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleCeilings()
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Ceiling);
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request platform data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void TogglePlatforms()
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Platform);
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request inferred region data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleInferRegions()
        {
            m_Observer.InferRegions = !m_Observer.InferRegions;
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request world mesh data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleWorld()
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.World);

            if (m_Observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World))
            {
                // Ensure we requesting meshes
                m_Observer.RequestMeshData = true;
                m_MeshesToggle.GetComponent<Interactable>().IsToggled = true;
            }
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request background data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleBackground()
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Background);
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change whether to request completely inferred data from the observer followed by
        /// clearing existing observations and requesting an update
        /// </summary>
        public void ToggleCompletelyInferred()
        {
            ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes.Inferred);
            ClearAndUpdateObserver();
        }

        /// <summary>
        /// Change the colors of the scene objects.
        /// When we want to see scene objects clearly we can turn on the color.
        /// </summary>
        public void ToggleSceneObjectColor()
        {
            m_SceneObjectColorToggle = !m_SceneObjectColorToggle;            
            ClearAndUpdateObserver();
        }

        #endregion UI Functions

        #endregion Public Functions

        #region Helper Functions

        /// <summary>
        /// Handle initial button states
        /// </summary>
        private void InitToggleButtonState()
        {
            // Configure observer
            m_AutoUpdateToggle.IsToggled = m_Observer.AutoUpdate;
            m_QuadsToggle.IsToggled = m_Observer.RequestPlaneData;
            m_MeshesToggle.IsToggled = m_Observer.RequestMeshData;
            m_MaskToggle.IsToggled = m_Observer.RequestOcclusionMask;
            m_InferRegionsToggle.IsToggled = m_Observer.InferRegions;

            // Filter display
            m_PlatformToggle.IsToggled = m_Observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Platform);
            m_WallToggle.IsToggled = m_Observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Wall);
            m_FloorToggle.IsToggled = m_Observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Floor);
            m_CeilingToggle.IsToggled = m_Observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Ceiling);
            m_WorldToggle.IsToggled = m_Observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.World);
            m_CompletelyInferred.IsToggled = m_Observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Inferred);
            m_BackgroundToggle.IsToggled = m_Observer.SurfaceTypes.HasFlag(SpatialAwarenessSurfaceTypes.Background);
        }

        /// <summary>
        /// Gets the color of the given surface type
        /// </summary>
        /// <param name="surfaceType">The surface type to get color for</param>
        /// <returns>The color of the type</returns>
        private Color ColorForSurfaceType(SpatialAwarenessSurfaceTypes surfaceType)
        {
            // shout-out to solarized!

            switch (surfaceType)
            {
                case SpatialAwarenessSurfaceTypes.Unknown:
                    return new Color32(220, 50, 47, 255); // red
                case SpatialAwarenessSurfaceTypes.Floor:
                    return new Color32(38, 139, 210, 255); // blue
                case SpatialAwarenessSurfaceTypes.Ceiling:
                    return new Color32(108, 113, 196, 255); // violet
                case SpatialAwarenessSurfaceTypes.Wall:
                    return new Color32(181, 137, 0, 255); // yellow
                case SpatialAwarenessSurfaceTypes.Platform:
                    return new Color32(133, 153, 0, 255); // green
                case SpatialAwarenessSurfaceTypes.Background:
                    return new Color32(203, 75, 22, 255); // orange
                case SpatialAwarenessSurfaceTypes.World:
                    return new Color32(211, 54, 130, 255); // magenta
                case SpatialAwarenessSurfaceTypes.Inferred:
                    return new Color32(42, 161, 152, 255); // cyan
                default:
                    return new Color32(220, 50, 47, 255); // red
            }
        }

        /// <summary>
        /// Clear the observer from old data
        /// Update the observer with new data
        /// </summary>
        private void ClearAndUpdateObserver()
        {
            ClearScene();
            m_Observer.UpdateOnDemand();
        }

        /// <summary>
        /// Toggle Observed SurfaceType
        /// </summary>
        /// <param name="surfaceType"></param>
        private void ToggleObservedSurfaceType(SpatialAwarenessSurfaceTypes surfaceType)
        {
            if (m_Observer.SurfaceTypes.HasFlag(surfaceType))
            {
                m_Observer.SurfaceTypes &= ~surfaceType;
            }
            else
            {
                m_Observer.SurfaceTypes |= surfaceType;
            }
        }

        #endregion Helper Functions
    }
}
