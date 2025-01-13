using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    [SerializeField] new BoxCollider collider;
    
    private void OnTriggerEnter(Collider other)
    {
        if(!(other.TryGetComponent<BasketBallController>(out var script)))
        {
            return;
        }
        
        script.IsNear(true);
    }

    void OnTriggerExit(Collider other)
    {
        if(!(other.TryGetComponent<BasketBallController>(out var script)))
        {
            return;
        }

        script.IsNear(false);
    }
}
