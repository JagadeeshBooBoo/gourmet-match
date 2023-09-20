using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RoboticArm : MonoBehaviour
{
    public static RoboticArm instance;

    public ControllableUnClampedRotationalJoint baseJoint;
    public ControllableRotationalJoint bendingJoint;
    public ControllablePositionalJoint reachJoint;
    public ControllableUnClampedRotationalJoint clawJoint;
    [SerializeField] ControllablePositionalJoint descentJoint;
    [SerializeField] ControllableScalableModel descentScalableModel;

    public bool pickableDetected;
    public bool canDrop;
    public GameObject grabbableObject;
    public GameObject claw;
    [SerializeField] private Transform _clawEnd1;
    [SerializeField] private Transform _clawEnd2;
    private Vector3 _clawEnd1Pivot;
    private Vector3 _clawEnd2Pivot;
    [SerializeField] private float _rotationSpeed;
    private bool _canCloseClaw;
    private bool _canOpenClaw;
    private float _rotationTImer = 0f;
    private float _closeTargetValue = 0f;
    private bool _clawRetractMode;
    private int _clawProgressionCount = 0;

    public int layer1;
    public int layer2;

    private bool collisionsEnabled = true;

    public enum PickUpState { idle ,isPicking, isHolding}
    public PickUpState pickUpState;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CanvasManager canvasInstance = CanvasManager.instance;

        canvasInstance.sliderjoystick1.OnValueChanged += reachJoint.HandleSlidervalue;
        canvasInstance.sliderjoystick2.OnValueChanged += bendingJoint.HandleSlidervalue;
        canvasInstance.sliderjoystick3.OnValueChanged += clawJoint.HandleSlidervalue;
        canvasInstance.sliderjoystick4.OnValueChanged += baseJoint.HandleSlidervalue;

        //canvasInstance.LinkRotatingWheel(baseJoint.HandleSlidervalue);

        // Getting pivot points for claws
        _clawEnd1Pivot = _clawEnd1.transform.InverseTransformPoint(_clawEnd1.transform.position);
        _clawEnd2Pivot = _clawEnd2.transform.InverseTransformPoint(_clawEnd2.transform.position);

        pickUpState = PickUpState.idle;
    }

    // Update is called once per frame
    void Update()
    {
        HandleSingleGrabDropButtonClicked();

        if(canDrop)
        {
            CanvasManager.instance.ChangePickUpButtonState();

            canDrop = false;

            if (_clawRetractMode)
                _canOpenClaw = true;
            else
                _canCloseClaw = true;

            collisionsEnabled = !collisionsEnabled;
            UpdateCollisionState();
        }

        if (_canCloseClaw)
        {
            ClosingClaw(1);
        }

        if (_canOpenClaw)
        {
            ClosingClaw(0);
        }
    }

    private void ChangePhysicPorperstiesOfGrabbedObject(Rigidbody grabObjRb)
    {
        //grabObjRb.mass = 5f;
    }

    private void UpdateCollisionState()
    {
        if (collisionsEnabled)
        {
            // Enable collisions between layer1 and layer2
            Debug.Log("Collisions between layers enabled");
            Physics.IgnoreLayerCollision(layer1, layer2, false);
        }
        else
        {
            // Disable collisions between layer1 and layer2
            Debug.Log("Collisions between layers disabled");
            Physics.IgnoreLayerCollision(layer1, layer2, true);
        }
    }

    private void ClosingClaw(int close)
    {
        float targetTimer = Mathf.Abs(4f - _closeTargetValue);
        targetTimer = targetTimer / 1.5f;

        Debug.Log("Target Timer: " + targetTimer);

        if (_rotationTImer < targetTimer)
        {
            if(close == 1)
            {
                //if (!_clawRetractMode && _closeTargetValue > 4f)
                //{
                //    DropObject();
                //}

                // Rotate the GameObject continuously around its pivot
                _clawEnd1.transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);
                _clawEnd2.transform.Rotate(Vector3.up * -1f * _rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Rotate the GameObject continuously around its pivot
                _clawEnd2.transform.Rotate(Vector3.up * _rotationSpeed * Time.deltaTime);
                _clawEnd1.transform.Rotate(Vector3.up * -1f * _rotationSpeed * Time.deltaTime);
            }
            _rotationTImer += Time.deltaTime;
        }
        else
        {
            _rotationTImer = 0f;
            targetTimer = -1f;

            if(_canOpenClaw)
            {
                _clawRetractMode = false;
                _canOpenClaw = false;
                _clawProgressionCount++;
            }

            if (_canCloseClaw)
            {
                _clawRetractMode = true;
                _canCloseClaw = false;
                _clawProgressionCount++;
            }

            if (_clawProgressionCount == 2)
            {
                DropObject();
            }

            StartCoroutine(ChangePickUpStateBackToIdle(_clawProgressionCount));
        }
    }

    private void DropObject()
    {
        collisionsEnabled = !collisionsEnabled;
        UpdateCollisionState();

        grabbableObject.transform.SetParent(DataContainer.instance.mainPlate.transform);
        Rigidbody grabObjRb = grabbableObject.AddComponent<Rigidbody>();
        //ChangePhysicPorperstiesOfGrabbedObject(grabObjRb);
        grabbableObject.GetComponent<PickableItem>().isDropped = true;

        PlatesManager.instace.TweenMovePlate();
    }

    private IEnumerator ChangePickUpStateBackToIdle(int close)
    {
        yield return new WaitForSeconds(2f);

        if (close == 1)
        {
            pickUpState = PickUpState.isHolding;
            PlatesManager.instace.RotatePlate();
        }
        else if (close == 2)
        {
            pickUpState = PickUpState.idle;
            _closeTargetValue = 0f;
            _clawProgressionCount = 0;

            claw.GetComponent<Claw>().grabbedObj = false;

            grabbableObject = null;
        }
    }

    void HandleSingleGrabDropButtonClicked()
    {
        PickUp();   
    }

    public void TurnOffGroundDetected()
    {
        Debug.Log("Collided Ground");
    }

    public void PickableDetected(GameObject obj)
    {
        Debug.Log("Pick up");
        pickableDetected = false;

        if(grabbableObject == null)
            grabbableObject = obj;
    }

    Coroutine pickUpCoroutine;

    void PickUp()
    {
        if (pickUpCoroutine == null)
        {
            Debug.Log("Pick up called");
            pickUpCoroutine = StartCoroutine(PickUpCoroutine(grabbableObject));
        }
    }

    IEnumerator PickUpCoroutine(GameObject objToPick)
    {
        while (!pickableDetected)
        {
            descentJoint.HandleSlidervalue(-1);
            //descentScalableModel.HandleSlidervalue(-1);

            yield return null;
        }

        if (!pickableDetected)
        {
            bool waiting = true;
            //AudioManager.i?.truckPickupAudio.Play();
            //CloseClaw(() => { waiting = false; });
            while (waiting) yield return null;
        }
        //HapticFeedback.Vibrate(UIFeedbackType.ImpactHeavy);

        pickUpState = PickUpState.isPicking;

        CanvasManager.instance.ChangePickUpButtonState();

        grabbableObject.transform.SetParent(claw.transform);

        while (descentJoint.normalizedValue < 1)
        {
            if (_closeTargetValue == 0f)
            {
                _closeTargetValue = grabbableObject.GetComponent<PickableItem>().GetXBoundsSixe();

                if (grabbableObject.name == "Cake Slice")
                    _closeTargetValue = 3.5f;

                Debug.Log("Close Target value: " + _closeTargetValue);

                if (_closeTargetValue > 4f)
                    _canOpenClaw = true;
            }

            if (!pickableDetected)
            {
                if (_closeTargetValue < 4f)
                    _canCloseClaw = true;

                descentJoint.HandleSlidervalue(descentJoint.normalizedValue);
                break;
            }

            descentJoint.HandleSlidervalue(1);
            //descentScalableModel.HandleSlidervalue(1);
            yield return null;
        }

        Destroy(grabbableObject.GetComponent<Rigidbody>());

        descentJoint.normalizedValue = Mathf.Clamp01(descentJoint.normalizedValue);
        descentJoint.HandleSlidervalue(0);
        //descentScalableModel.HandleSlidervalue(0);

        //grabbableObject.transform.localPosition = new Vector3(0f, -0.1f, -0.25f);
        grabbableObject.transform.DOLocalMove(new Vector3(0f, -0.1f, -0.25f), 0.5f);

        LeanTween.delayedCall(0f, () => { pickUpCoroutine = null; });
    }

    public void StopClosingClaw()
    {
        _canOpenClaw = false;
        _canCloseClaw = false;
    }
}

[System.Serializable]
public class ControllableJoint
{
    public ConfigurableJoint joint;
    public float changeSpeed = 1;

    float vibrationsInterval = 0.3f;
    float vibrationsTimer = 0f;

    public virtual void HandleSlidervalue(float newVal)
    {
        if (newVal != 0)
        {
            //handle vibrations
            vibrationsTimer += Time.deltaTime;
            if (vibrationsTimer > vibrationsInterval)
            {
                //HapticFeedback.Vibrate(UIFeedbackType.ImpactMedium);
                vibrationsTimer = Mathf.Repeat(vibrationsTimer, vibrationsInterval);
            }
            //handle audio
        }
        //HandleAudio(newVal);
    }

    //AudioSource currAudioSource;

    //void HandleAudio(float input)
    //{
    //    if (currAudioSource == null) currAudioSource = AudioManager.i.GetJointAudioSource();
    //    if (currAudioSource == null) return;
    //    input = Mathf.Abs(input);
    //    float threshold = 0;//.01f;
    //                        //if movement speed is greater than some threshold value start haptics
    //                        // if(input>threshold)
    //                        // {
    //    currAudioSource.volume = ExtensionMethods.Map(input, threshold, 1, 0f, 1f);
    //    if (!currAudioSource.isPlaying) currAudioSource.Play();
    //    // }
    //}
}

[System.Serializable]
public class ControllableRotationalJoint : ControllableJoint
{
    public Vector3 minRotation;
    public Vector3 maxRotation;
    [Range(0f, 1f)]
    public float normalizedValue;

    public override void HandleSlidervalue(float sliderValue)
    {
        base.HandleSlidervalue(sliderValue);
        normalizedValue += sliderValue * changeSpeed * Time.deltaTime;
        normalizedValue = Mathf.Clamp01(normalizedValue);
        joint.targetRotation = Quaternion.Slerp(Quaternion.Euler(minRotation), Quaternion.Euler(maxRotation), normalizedValue);
    }
}

[System.Serializable]
public class ControllableUnClampedRotationalJoint : ControllableJoint
{
    public override void HandleSlidervalue(float sliderValue)
    {
        base.HandleSlidervalue(sliderValue);
        joint.targetRotation = Quaternion.Euler(0f, sliderValue * changeSpeed * Time.deltaTime, 0f) * joint.targetRotation;
    }
}

[System.Serializable]
public class ControllablePositionalJoint : ControllableJoint
{
    public Vector3 minPosition;
    public Vector3 maxPosition;
    public bool clampNormalizedValue = true;
    [Range(0f, 1f)]
    public float normalizedValue;

    // Scaling rod properties
    public GameObject scalableRod; // Object to be scaled
    public List<bool> scaleAxis = new List<bool>(); // Axis in which the object to be scaled
    public float scaleMinLimit; // Min limit of object's scale
    public float scaleMaxLimit; // Max limit of object's scale
    public bool inverseScaling;

    public override void HandleSlidervalue(float sliderValue)
    {
        base.HandleSlidervalue(sliderValue);
        normalizedValue += sliderValue * changeSpeed * Time.deltaTime;
        if (clampNormalizedValue)
            normalizedValue = Mathf.Clamp01(normalizedValue);
        joint.targetPosition = Vector3.LerpUnclamped(minPosition, maxPosition, normalizedValue);

        // Scaling the rod in desired axis when performing action
        if (scalableRod == null) return;

        float scaleFactor = (inverseScaling?0.8f:-1) * sliderValue * Time.deltaTime;
        scalableRod.transform.localScale += new Vector3(scaleAxis[0] ? scaleFactor : 0, scaleAxis[1] ? scaleFactor : 0, scaleAxis[2] ? scaleFactor : 0);

        // Limiting scale within given bounds
        int trueIndex = scaleAxis.FindIndex(x => x);
        if (trueIndex == 0)
            scalableRod.transform.localScale = new Vector3(Mathf.Clamp(scalableRod.transform.localScale.x, scaleMinLimit, scaleMaxLimit), 1f, 1f);
        else if (trueIndex == 1)
            scalableRod.transform.localScale = new Vector3(1f, Mathf.Clamp(scalableRod.transform.localScale.y, scaleMinLimit, scaleMaxLimit), 1f);
        else if (trueIndex == 2)
            scalableRod.transform.localScale = new Vector3(1f, 1f, Mathf.Clamp(scalableRod.transform.localScale.z, scaleMaxLimit, scaleMinLimit));
    }
}

[System.Serializable]
public abstract class ScalableJoint
{
    public Transform model;
    public float changeSpeed = 1;

    public abstract void HandleSlidervalue(float newVal);
}

[System.Serializable]
public class ControllableScalableModel : ScalableJoint
{
    public Vector3 minScale;
    public Vector3 maxScale;
    public bool clampNormalizedValue = true;
    [Range(0f, 1f)]
    public float normalizedValue;

    public override void HandleSlidervalue(float sliderValue)
    {
        normalizedValue += sliderValue * changeSpeed * Time.deltaTime;
        if (clampNormalizedValue)
            normalizedValue = Mathf.Clamp01(normalizedValue);
        model.localScale = Vector3.LerpUnclamped(minScale, maxScale, normalizedValue);
    }
}
