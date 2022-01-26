using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script sets a lifetime for a footsteps.
/// After the lifetime expired, footstep will be destroyed.
/// This is attached to each footstep prefabs.
/// </summary>
public class FootPrintLifetime : MonoBehaviour
{
    float lifetime = 25.0f;

    /// Start the coroutine.
    public void Start()
    {
        StartCoroutine(WaitThenDie());
    }


    /// <summary>
    /// After the lifetime expired, footstep will be destroyed.
    /// </summary>
    IEnumerator WaitThenDie()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
}
