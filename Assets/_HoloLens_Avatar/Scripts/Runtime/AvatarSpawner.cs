using UnityEngine;
using UnityEngine.AI;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// This script is responsible for spawning the avatar in the scene.
/// </summary>
public class AvatarSpawner : MonoBehaviour 
{

    
    /// Reference to the scrollview   
    [SerializeField] private ScrollingObjectCollection m_Scrollview;

    /// Reference to the avatar prefabs
    [SerializeField] private GameObject[] m_AvatarPrefabs;

    /// Avatar index value
    private int m_AvatarIndex = -1;

    /// Defines the number of avatars
    private const int m_NumberOfAvatars = 7;

    /// Last avatar.
    private GameObject m_LastAvatar;

    /// <summary>
    /// Gets called per each frame
    /// </summary>
    private void Update()
    {
        SetVisibleCellIndex();
    }

    /// <summary>
    /// Set the current cell index belong to the avatar.
    /// </summary>    
    private void SetVisibleCellIndex()
    {
        m_AvatarIndex = m_Scrollview.FirstVisibleCellIndex;       

        // Prevent accessing out of bound cell number
        if (m_AvatarIndex >= m_NumberOfAvatars)
        {            
            m_AvatarIndex -= 1;
        }
    }

    /// <summary>
    /// Spawn the avatar corresponding to the specified index.
    /// </summary>    
    public void SpawnAvatar() 
    {

        int index = m_AvatarIndex;

        if (index < 0 || index >= m_AvatarPrefabs.Length) 
        {
            Debug.LogError($"Can't instantiate prefab, unable to find specified index '{index}'.");
            return;
        }

        var prefab = m_AvatarPrefabs[index];

        if (!prefab) 
        {
            Debug.LogError("Can't load avatar prefab.");
            return;
        }

        if (m_LastAvatar)
        {
            Destroy(m_LastAvatar);
        }           

        var position = Camera.main.transform.position + Camera.main.transform.forward * 3 + new Vector3(0, -0.6f, 0);
        m_LastAvatar = Instantiate(prefab, position, Quaternion.identity);       
    }
}
