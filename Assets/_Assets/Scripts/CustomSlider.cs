using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class CustomSlider : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    public RectTransform handle;       // Reference to the handle of the slider
    public Transform visualHandle;       // Reference to the handle of the slider
    [SerializeField] Image sliderImage;
    [SerializeField] Image buttonImage;
    [SerializeField] RectTransform slider;       // Reference to the slider
    [SerializeField] Sprite buttonSelectedSprite;       // Reference to the slider
    [SerializeField] Sprite normalButtonSprite;       // Reference to the slider
    [SerializeField] Sprite sliderSelectedSprite;       // Reference to the slider
    [SerializeField] Sprite normalSliderSprite;       // Reference to the slider
    public float minValue = 0f;        // Minimum value of the slider
    public float maxValue = 1f;        // Maximum value of the slider
    public float currentValue = 0.5f;  // Current value of the slider (initialize as desired)

    private float sliderHeight;
    Vector2 prevLocalPoint;

    public Action<float> OnValueChanged;
    public Action OnMouseDownTouched;

    private void Start()
    {
        sliderHeight = slider.rect.height;
        ReserSliderUI();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(slider, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            // Calculate the normalized value directly from the mouse X position
            float normalizedDifference = (localPoint.y - prevLocalPoint.y) / sliderHeight;
            float normalizedValue = Mathf.InverseLerp(minValue, maxValue, currentValue);
            normalizedValue += normalizedDifference;
            normalizedValue = Mathf.Clamp01(normalizedValue);
            currentValue = Mathf.Lerp(minValue, maxValue, normalizedValue);
            // Update the position of the handle and fill
            UpdateSliderUI();
            prevLocalPoint = localPoint;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ReserSliderUI();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(slider, eventData.position, eventData.pressEventCamera, out prevLocalPoint))
        {
            sliderImage.sprite = sliderSelectedSprite;
            buttonImage.sprite = buttonSelectedSprite;
            UpdateSliderUI();
            //AudioManager.i.joystickTapAudio.Play();
            OnMouseDownTouched?.Invoke();
        }
    }

    private void UpdateSliderUI()
    {
        // Calculate the normalized value between 0 and 1 based on the current value
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, currentValue);
        // Update the position of the handle and fill
        handle.anchorMin = new Vector2(0, normalizedValue);
        handle.anchorMax = new Vector2(1, normalizedValue);

        handle.anchoredPosition = new Vector2(handle.anchoredPosition.x, normalizedValue);

        //visualHandle.localRotation = Quaternion.Slerp(Quaternion.Euler(Vector3.right * -60), Quaternion.Euler(Vector3.right * 60), normalizedValue);
        Vector3 pos = visualHandle.transform.localPosition;
        visualHandle.localPosition = new Vector3(pos.x, pos.y, Mathf.Lerp(0.1f, -0.1f, normalizedValue));
    }

    private void OnDisable()
    {
        ReserSliderUI();
        OnValueChanged?.Invoke(currentValue);
    }

    private void Update()
    {
        OnValueChanged?.Invoke(currentValue);
    }

    void ReserSliderUI()
    {
        currentValue = Mathf.Lerp(minValue, maxValue, 0.5f);
        sliderImage.sprite = normalSliderSprite;
        buttonImage.sprite = normalButtonSprite;
        UpdateSliderUI();
    }

    public void ResetSlider()
    {
        OnValueChanged = null;
        print("Removing Listeners");
        ReserSliderUI();
        this.enabled = true;
    }
}
