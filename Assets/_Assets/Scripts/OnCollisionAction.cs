using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnCollisionAction : MonoBehaviour
{
    public UnityEvent OnCollisionEnterEvent;
    public int targetLayer;

    private void OnCollisionEnter(Collision other)
    {
        if (!enabled) return;
        if (other.gameObject.layer != targetLayer) return;
        Debug.Log("Collided");
        OnCollisionEnterEvent.Invoke();
    }
}
