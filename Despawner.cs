using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    private void OnTriggerEnter(Collider otherObj)
    {
        if (otherObj.gameObject.tag == "Destroyer")
        {
            Invoke(nameof(DestroyObject), 0.2f);
        }
    }


}
