using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SilhouetteItem : MonoBehaviour
{
    public int id;

    private GameObject originalObj;
    private bool _isTriggerred;

    [SerializeField] private List<MeshRenderer> renderers = new List<MeshRenderer>();

    private RoboticArm roboticArm;

    // Start is called before the first frame update
    void Start()
    {
        roboticArm = RoboticArm.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isTriggerred && roboticArm.pickUpState == RoboticArm.PickUpState.idle)
        {
            _isTriggerred = false;
            ObjectReachedTarget(originalObj.GetComponent<PickableItem>());
        }

        if (roboticArm.pickUpState != RoboticArm.PickUpState.idle)
            EnableSilhouette();
        else
            DisableSilhouette();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            _isTriggerred = true;

            PickableItem item = other.gameObject.GetComponent<PickableItem>();

            if (item.id == id)
            {
                item.correctPlacement = true;
                originalObj = item.gameObject;
            }
        }
    }

    public void ObjectReachedTarget(PickableItem item)
    {
        Debug.Log("Object reached its target position");

        StartCoroutine(CheckForAccuracy(item));
    }

    private IEnumerator CheckForAccuracy(PickableItem item)
    {
        yield return new WaitForSeconds(0f);

        // Calculate the overlapping area.
        Bounds intersection = BoundsIntersection(GetComponent<Collider>().bounds, item.GetComponent<Collider>().bounds);
        float areaCovered = intersection.size.x * intersection.size.z;

        // Calculate the total area of the object that needs to be covered.
        float totalArea = GetComponent<Collider>().bounds.size.x * GetComponent<Collider>().bounds.size.z;

        // Calculate the percentage coverage.
        float percentageCoverage = (areaCovered / totalArea) * 100;

        Debug.Log("Percentage Coverage: " + percentageCoverage + "%");

        LevelManager.instance.currentPlacements++;

        gameObject.SetActive(false);

        TweenPlate();
    }

    // Helper function to calculate the intersection of two bounding boxes.
    private Bounds BoundsIntersection(Bounds bounds1, Bounds bounds2)
    {
        Vector3 min = Vector3.Max(bounds1.min, bounds2.min);
        Vector3 max = Vector3.Min(bounds1.max, bounds2.max);
        return new Bounds((min + max) * 0.5f, max - min);
    }

    private void TweenPlate()
    {
        transform.parent.DOPunchScale(Vector3.one * 0.2f, 0.25f).OnComplete(() =>
        {
            if(LevelManager.instance.targetPlacements == LevelManager.instance.currentPlacements)
            {
                LevelManager.instance.Confetti();
                UIManager.instance.LevelCompleted();
            }
        }
        );
    }

    public void EnableSilhouette()
    {
        //if (roboticArm.grabbableObject == null)oug
        //    return;

        //if (id != roboticArm.grabbableObject.GetComponent<PickableItem>().id)
        //    return;

        foreach (MeshRenderer renderer in renderers)
            renderer.enabled = true;
    }

    public void DisableSilhouette()
    {
        //foreach (MeshRenderer renderer in renderers)
        //    renderer.enabled = false;
    }
}
