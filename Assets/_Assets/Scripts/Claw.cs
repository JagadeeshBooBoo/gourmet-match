using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Claw : MonoBehaviour
{
    public bool grabbedObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!grabbedObj && other.gameObject.layer == 6)
        {
            grabbedObj = true;
            RoboticArm.instance.PickableDetected(other.gameObject);
            //RoboticArm.instance.PickUp(other.gameObject);
        }
    }
}
