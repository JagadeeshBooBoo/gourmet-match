using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnTriggerAction : MonoBehaviour
{
    public UnityEvent OnTriggerEnterEvent;
    public int targetLayer;

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (other.gameObject.layer != targetLayer) return;
        OnTriggerEnterEvent.Invoke();
    }
}
