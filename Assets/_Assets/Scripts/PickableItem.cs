using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    private Renderer rend;

    public int id;
    public bool isDropped;
    public bool correctPlacement;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void Update()
    {
        
    }

    public float GetXBoundsSixe()
    {
        return rend.bounds.size.x;
    }

    public void AddRigidbody()
    {
        gameObject.AddComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (isDropped && !correctPlacement)
            {
                UIManager.instance.LevelFailed();
            }
        }
    }
} 
