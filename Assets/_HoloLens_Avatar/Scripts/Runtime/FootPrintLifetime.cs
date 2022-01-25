using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootPrintLifetime : MonoBehaviour
{
    float lifetime = 25.0f;

    // Start is called before the first frame update
    public void Start()
    {
        StartCoroutine(WaitThenDie());
    }


    IEnumerator WaitThenDie()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }
}
