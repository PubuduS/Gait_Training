using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAllComponents : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public void RemoveAllNoiseComponents()
    {
        foreach (var comp in this.gameObject.GetComponents<Component>())
        {
            if( ( comp is Transform ) != true && ( comp is RemoveAllComponents ) != true )
            {
                Destroy(comp);
            }
        }
    }
}
