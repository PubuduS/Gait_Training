using UnityEngine;
using UnityEngine.AI;
using Microsoft.MixedReality.Toolkit.UI;

/// <summary>
/// This script is responsive of spawn avatar in scene.
/// </summary>
public class AvatarSpawner : MonoBehaviour 
{

    //! Parent Object of the avatar buttons
    [SerializeField] private Transform m_Collection;

    //! This array contains the avatar prefabs.
    [SerializeField] private GameObject[] m_AvatarPrefabs;

    //! Store the reference to the last spawn avatar
    private GameObject m_LastAvatar;

    //! Get called when the script instance is being loaded
    //! Initialize button listeners in order to spawn avatars
    private void Awake() 
    {
        // Initialize button listeners in order to spawn avatars
        for (var i = 0; i < m_Collection.childCount; i++) 
        {
            var child = m_Collection.GetChild(i);

            if (!child)
                continue;

            var buttonTransform = child.Find("Button");

            if (!buttonTransform)
                continue;

            var button = buttonTransform.GetComponent<Interactable>();

            if (!button)
                continue;

            var index = i;
            button.OnClick.AddListener(() => SpawnAvatar(index));
        }
    }


    //! Spawn the avatar corresponding to the specified index.
    //! Also destroy the previous avatar.
    void SpawnAvatar(int index) 
    {

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
            Destroy(m_LastAvatar);

        var position = Camera.main.transform.position + Camera.main.transform.forward * 3 + new Vector3(0, -0.6f, 0);
        m_LastAvatar = Instantiate(prefab, position, Quaternion.identity);

        if (NavMesh.SamplePosition(m_LastAvatar.transform.position, out NavMeshHit closestHit, 500, NavMesh.AllAreas))
            m_LastAvatar.transform.position = closestHit.position;
    }
}
