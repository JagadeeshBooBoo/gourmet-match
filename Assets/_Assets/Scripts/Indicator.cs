using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Indicator : MonoBehaviour
{
    public LayerMask layerMask; // Layer mask to filter which objects the ray can hit

    [SerializeField] private GameObject _indicatorObject;
    private GameObject _indicator;

    private MeshRenderer _indicatorRenderer;
    [SerializeField] private List<Material> _indicatorMats = new List<Material>();
    private Animator _indicatorAnim;

    // Start is called before the first frame update
    void Start()
    {
        //_indicator = Instantiate(_indicatorObject);
        //_indicator.transform.localScale = Vector3.one * 50f;
        //_indicatorRenderer = _indicator.GetComponent<MeshRenderer>();
        //_indicatorAnim = _indicator.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckRaycastForDisplay();
    }

    private void CheckRaycastForDisplay()
    {
        // Define the ray's starting point (from the GameObject's position)
        Vector3 rayStart = transform.position;

        // Define the ray direction (downward)
        Vector3 rayDirection = Vector3.down;

        // Create a raycast hit variable to store information about the hit
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(rayStart, rayDirection, out hit, 100, layerMask))
        {
            //Debug.Log("Hit object: " + hit.collider.gameObject.name);

            if(RoboticArm.instance.pickUpState == RoboticArm.PickUpState.idle)
            {
                // Place object at raycast hit position
                //_indicator.SetActive(true);
                //_indicator.transform.position = hit.point;

                // Set material based on what it hits
                if (hit.collider.gameObject.layer == 6)
                {
                    PickableItem item = hit.collider.GetComponent<PickableItem>();

                    if (item.isDropped)
                        return;

                    if (RoboticArm.instance.grabbableObject == null)
                        RoboticArm.instance.grabbableObject = item.gameObject;

                   //_indicatorRenderer.material = _indicatorMats[0];
                   //_indicatorAnim.SetBool("scale", true);

                    CanvasManager.instance.EnablePickUpInteractibility();
                    CanvasManager.instance.UpdateSwitchAnimation(true);
                }
                else
                {
                    //_indicatorRenderer.material = _indicatorMats[1];
                    //_indicatorAnim.SetBool("scale", false);

                    RoboticArm.instance.grabbableObject = null;

                    CanvasManager.instance.DisablePickUpInteractibility();
                    CanvasManager.instance.UpdateSwitchAnimation(false);
                }
            }
            else
            {
                //_indicator.SetActive(false);
                CanvasManager.instance.UpdateSwitchAnimation(false);
            }
        }
    }

    // This method is called in the Unity Editor to draw Gizmos
    private void OnDrawGizmos()
    {
        // Define the ray's starting point (from the GameObject's position)
        Vector3 rayStart = transform.position;

        // Define the ray direction (downward)
        Vector3 rayDirection = Vector3.down;

        // Draw the ray as a line in the scene view
        Gizmos.color = Color.red; // Set the color of the Gizmo
        Gizmos.DrawLine(rayStart, rayStart + rayDirection * 100);
    }
}
