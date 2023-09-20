using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class SteeringWheel : MonoBehaviour, IDragHandler,IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] RectTransform touchArea;// Reference to the touchArea
    [SerializeField] RectTransform visualImageRT;// Reference to the touchArea
    public float minValue = 0f;        // Minimum value of the touchArea
    public float maxValue = 1f;        // Maximum value of the touchArea
    public float currentValue = 0.5f;  // Current value of the touchArea (initialize as desired)
    public float rightRotationAngle;  // Current value of the touchArea (initialize as desired)
    public float leftRotationAngle;  // Current value of the touchArea (initialize as desired)
    Quaternion startRotation;  // Current value of the touchArea (initialize as desired)

    private float touchAreaWidth;
    Vector2 prevLocalPoint;

    public Action<float> OnValueChanged;

    public Action OnMouseDownTouched;
    
    private void Awake()
    {
        startRotation = visualImageRT.transform.localRotation;    
    }

    void Start()
    {
        touchAreaWidth = touchArea.rect.height;
        UpdateSteeringWheelUI();
    }

    private void OnDisable()
    {
        ResetUI();   
        OnValueChanged?.Invoke(currentValue);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(touchArea, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Calculate the normalized value directly from the mouse X position
            float normalizedDifference = (localPoint.x - prevLocalPoint.x)/touchAreaWidth;
            float normalizedValue = Mathf.InverseLerp(minValue, maxValue, currentValue);
            normalizedValue += normalizedDifference;
            normalizedValue = Mathf.Clamp01(normalizedValue);
            currentValue = Mathf.Lerp(minValue, maxValue, normalizedValue);
            // Update the position of the handle and fill
            UpdateSteeringWheelUI();
            prevLocalPoint = localPoint;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(touchArea, eventData.position, eventData.pressEventCamera, out prevLocalPoint))
        {
            //AudioManager.i.rotationWheelTapAudio.Play();
            UpdateSteeringWheelUI();
            OnMouseDownTouched?.Invoke();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetUI();
    }

    void UpdateSteeringWheelUI()
    {
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, currentValue);
        if(normalizedValue<0.5f)
        {
            visualImageRT.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(Vector3.forward*leftRotationAngle),startRotation,normalizedValue*2);
        }
        else
        {
            visualImageRT.transform.localRotation = Quaternion.Slerp(startRotation,Quaternion.Euler(Vector3.forward*rightRotationAngle),normalizedValue*2 - 1);
        }
    }

    private void FixedUpdate()
    {
        OnValueChanged?.Invoke(currentValue);
    }

    public void ResetSteeringWheel()
    {
        OnValueChanged = null;
        ResetUI();
        this.enabled = true;
    }

    void ResetUI()
    {
        currentValue = (minValue + maxValue) * 0.5f;
        UpdateSteeringWheelUI();
    }
}