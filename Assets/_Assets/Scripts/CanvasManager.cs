using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager instance;

    [SerializeField] Canvas uiCanvas;
    public Camera uiCamera;
    public Button fpsButton;
    public TextMeshProUGUI targetFrameRateText;

    //[Header("LevelWin Panel")]
    //public UIPanel levelWinPanel;
    //public TextMeshProUGUI rewardCoinsText;
    //public Button nextLevelButton;
    //[SerializeField] Button retryButton;

    //[Header("Tutorial Panel")]
    //public UIPanel tutorialPanel;
    //public TextMeshProUGUI tutorialText;
    //public RectTransform infinityTutorial;

    [Space(5)]
    [SerializeField] Vector3 baseJointTutorialTargetRotation;
    [SerializeField] Vector3 bendJointTutorialTargetRotation;
    [SerializeField] Vector3 reachJointTutorialTargetPosition;

    [Header("TutorialSpotLight Panel")]
    public RectTransform tutorialSpotLightRt;
    public Image[] tutorialSpotLightImages;
    public float tutorialSpotLightmaxAlpha = 0.95f;

    //[Header("LevelLose Panel")]
    //public UIPanel levelLosePanel;
    //[SerializeField] Button loseRetryButton;


    [Header("Ingame Panel")]
    //[SerializeField] UIPanel ingamePanel;
    [SerializeField] RectTransform slidersContainer;
    [SerializeField] TextMeshProUGUI currentLevelText;
    public CustomSlider baseJointSlider;
    public CustomSlider bendJointSlider;
    public CustomSlider reachJointSlider;
    public CustomSlider descentJointSlider;
    public CustomSlider grappleJointSlider;
    public CustomSlider sliderjoystick1;
    public CustomSlider sliderjoystick2;
    public CustomSlider sliderjoystick3;
    public CustomSlider sliderjoystick4;
    public GameObject pickUpButtonObj;
    public GameObject dropButtonObj;
    public GameObject pickUpSwitch;
    public List<Material> switchMats = new List<Material>();
    private MeshRenderer pickUpSwitchMesh;
    public Animator switchAnim;
    public TMP_Text switchText;
    [SerializeField] GameObject slidersPrefab;
    [SerializeField] GameObject toggleButtonPrefab;
    //public CustomJoystick leftJoystick;
    //public CustomJoystick rightJoystick;
    // public Button grabButton;
    // public Button dropButton;
    public Button singleGrabDropButton;
    public LeanTweenType pumpAnimationType;
    TextMeshProUGUI grabButtonText;
    TextMeshProUGUI dropButtonText;
    TextMeshProUGUI singleGrabDropButtonText;
    public Color enabledButtonColor;
    public Color disabledButtonColor;
    public Button ingameRestartButton;
    // [SerializeField]CustomSlider accelSlider;
    [SerializeField] SteeringWheel rotatingWheel;
    [SerializeField] Joystick joystick;
    [SerializeField] GameObject controlsContainer;

    [Header("BlackFade Panel")]
    public Image blackFadeImage;

    // [Header("Coins Panel")]
    // [SerializeField]UIPanel coinsPanel;
    // [SerializeField]TextMeshProUGUI coinsText;
    // [SerializeField]GameObject coinsImageObject;

    public static Vector2 screen;

    bool Vibration
    {
        get
        {
            return PlayerPrefs.GetInt("Vibration", 1) == 1;
        }
        set
        {
            // HapticFeedback.SetVibrationOn(value);
            // vibrationToggleButton.image.sprite = value?GameAssets.i.vibrationOnSprite:GameAssets.i.vibrationOffSprite;
            PlayerPrefs.SetInt("Vibration", value == true ? 1 : 0);
        }
    }

    void Awake()
    {
        instance = this;
    }

   

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Destroy(gameObject);
        //    LevelManager.Instance.LoadLevel(0);
        //    Destroy(LevelManager.Instance.gameObject);
        //}

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    // GameManager.Instance?.OnLevelComplete?.Invoke(false);
        //    GameManager.Instance?.GameOver(true);
        //}
    }

    void Start()
    {
        //LevelManager.Instance.OnLevelLoaded += SubscribeToGameManagerEvents;
        //LevelManager.Instance.OnLevelLoaded += HandleTutorials;
        // GAEventManager.Init();
        Initialise();

        pickUpSwitchMesh = pickUpSwitch.GetComponent<MeshRenderer>();
    }

    //void SubscribeToGameManagerEvents()
    //{
    //    if (GameManager.Instance == null) return;
    //    GameManager.Instance.OnLevelComplete += OnLevelCompleteHandler;
    //}

    //void HandleTutorials()
    //{
    //    controlsContainer.gameObject.SetActive(true);
    //    joystick.gameObject.SetActive(true);
    //    infinityTutorial.gameObject.SetActive(false);

    //    if (LevelManager.Instance.CurrentLevel == 1)
    //        ShowLevel1Tutorial();
    //    else if (LevelManager.Instance.CurrentLevel == 2)
    //        ShowLevel2Tutorial();
    //}

    void Initialise()
    {
        //Application.targetFrameRate = Screen.currentResolution.refreshRate;
        //targetFrameRateText.text = Application.targetFrameRate.ToString();
    }

    int spotLightTween;

    public void ShowSpotlightTutorial(RectTransform focusRt)
    {
        tutorialSpotLightRt.position = focusRt.position;
        Color c = Color.black;
        ChangeTutorialImagesAlpha(0f, tutorialSpotLightmaxAlpha);
    }

    public void ShowSpotlightTutorial(Vector3 worldPos)
    {
        //tutorialSpotLightRt.position = RectTransformUtility.WorldToScreenPoint(CameraController.Instance?.mainCam, worldPos);
        //Color c = Color.black;
        //ChangeTutorialImagesAlpha(0f, tutorialSpotLightmaxAlpha);
    }

    public void HideSpotLightTutorial(bool instant = false, bool keepActiveAtEnd = false)
    {
        ChangeTutorialImagesAlpha(tutorialSpotLightmaxAlpha, 0f, instant ? 0f : 0.5f, keepActiveAtEnd);
    }

    public void ChangePickUpButtonState()
    {
        if(pickUpButtonObj.activeInHierarchy)
        {
            pickUpButtonObj.SetActive(false);
            dropButtonObj.SetActive(true);

            switchText.text = "Drop";

            pickUpSwitch.transform.DOLocalRotate(new Vector3(20f, 0f, 0f), 0.25f);

            Material[] materials = pickUpSwitchMesh.materials;
            materials[1] = switchMats[0];

            pickUpSwitch.GetComponent<MeshRenderer>().materials = materials;
        }
        else
        {
            pickUpButtonObj.SetActive(true);
            dropButtonObj.SetActive(false);

            switchText.text = "Pick";

            pickUpSwitch.transform.DOLocalRotate(Vector3.zero, 0.25f);

            Material[] materials = pickUpSwitchMesh.materials;
            materials[1] = switchMats[1];

            pickUpSwitch.GetComponent<MeshRenderer>().materials = materials;
        }
    }

    public void EnablePickUpInteractibility()
    {
        Button pickUpbutton = pickUpButtonObj.GetComponent<Button>();

        pickUpbutton.interactable = true;
    }

    public void DisablePickUpInteractibility()
    {
        Button pickUpbutton = pickUpButtonObj.GetComponent<Button>();

        pickUpbutton.interactable = false;
    }

    public void UpdateSwitchAnimation(bool state)
    {
        switchAnim.SetBool("scale", state);
    }

    void ChangeTutorialImagesAlpha(float from, float to, float transitionTime = 0.5f, bool keepActiveAtEnd = false)
    {
        LeanTween.cancel(spotLightTween, true);

        if (from == 0)
        {
            foreach (Image i in tutorialSpotLightImages)
                i.gameObject.SetActive(true);
        }

        Color c = Color.black;
        spotLightTween = LeanTween.value(gameObject, from, to, transitionTime).setOnUpdate((float val) =>
        {
            c.a = val;
            foreach (Image i in tutorialSpotLightImages)
                i.color = c;
        })
        .setOnComplete(() =>
        {
            c.a = to;
            foreach (Image i in tutorialSpotLightImages)
                i.color = c;

            if (to == 0 && !keepActiveAtEnd)
            {
                foreach (Image i in tutorialSpotLightImages)
                    i.gameObject.SetActive(false);
            }
        }).id;
    }

    public void OnLevelCompleteHandler(bool win)
    {
        //ingamePanel.TogglePanel(false);
        //HideSpotLightTutorial(true);
        //LeanTween.delayedCall(1f, () =>
        //{
        //    if (win)
        //        levelWinPanel.TogglePanel(true, null);
        //    else
        //        levelLosePanel.TogglePanel(true, null);
        //});
    }

    void InGameHomeButtonHandler()
    {
        //LevelManager.Instance.OnLevelLoaded -= InGameHomeButtonHandler;
    }

    void WinPanelHomeButtonHandler()
    {
        //LevelManager.Instance.OnLevelLoaded -= WinPanelHomeButtonHandler;
    }

    public LTDescr moveTo(RectTransform rt, Vector2 targetPos, float transitionTime)
    {
        return LeanTween.move(rt, targetPos, transitionTime);
    }

    // LTDescr CoinsAnimHandler(int coinsToAdd,GameObject coinsImageObject)
    // {   
    //     float maxWaitTime = float.MinValue;
    //     // Time.timeScale = 0.05f;
    //     for (int i = 0; i < 100; i++)
    //     {
    //         float randWaitTime = UnityEngine.Random.Range(0f, 0.3f);
    //         maxWaitTime = Mathf.Max(maxWaitTime, randWaitTime);

    //         LeanTween.delayedCall(randWaitTime, () =>
    //         {
    //             RectTransform newCoinRt = Instantiate(coinsImageObject,coinsImageObject.transform.parent).GetComponent<RectTransform>();
    //             newCoinRt.transform.SetParent(coinsText.rectTransform);
    //             newCoinRt.gameObject.SetActive(true);
    //             float xPos = UnityEngine.Random.Range(newCoinRt.anchoredPosition.x - Screen.width/3, newCoinRt.anchoredPosition.x + Screen.width/3);
    //             float yPos = UnityEngine.Random.Range(newCoinRt.anchoredPosition.y - Screen.width/3, newCoinRt.anchoredPosition.y + Screen.width/3);
    //             moveTo(newCoinRt,new Vector2(xPos,yPos),0.3f).setEaseOutBack(,()=>{
    //                 LeanTween.scale(newCoinRt,Vector3.one*0.3f,0.5f);
    //                 moveTo(newCoinRt,coinsText.rectTransform.anchoredPosition,0.5f).setEaseInBack(,()=>{
    //                     Destroy(newCoinRt.gameObject);
    //                 });
    //             });
    //         });
    //     }

    //     return LeanTween.value(coinsText.gameObject, Coins, Coins + coinsToAdd, 1f + maxWaitTime).setOnUpdate((float val) =>
    //     {
    //         Coins = (int)val;
    //     });
    // }

    public void ShowFadePanel(float transitionTime = 0.3f, System.Action callback = null)
    {
        blackFadeImage.gameObject.SetActive(true);
        if (transitionTime == 0)
        {
            SetFadeImageOpacity(1f);
            if (callback != null) callback();
        }
        else
        {
            LeanTween.value(gameObject, 0f, 1f, transitionTime).setOnUpdate(SetFadeImageOpacity).setOnComplete(() =>
            {
                if (callback != null) callback();
            });
        }
    }

    public void HideFadePanel(float transitionTime = 0.3f, System.Action callback = null)
    {
        if (transitionTime == 0)
        {
            SetFadeImageOpacity(0f);
            if (callback != null) callback();
        }
        else
        {
            LeanTween.value(gameObject, 1f, 0f, transitionTime).setOnUpdate(SetFadeImageOpacity).setOnComplete(() =>
            {
                SetFadeImageOpacity(0f);
                if (callback != null) callback();
            });
        }
    }

    void SetFadeImageOpacity(float val)
    {
        Color c = blackFadeImage.color;
        c.a = val;
        blackFadeImage.color = c;
        blackFadeImage.gameObject.SetActive(val != 0);
    }

    public CustomSlider GetSlider()
    {
        return Instantiate(slidersPrefab, slidersContainer).GetComponent<CustomSlider>();
    }

    public Button GetToggleButton()
    {
        return Instantiate(toggleButtonPrefab, slidersContainer).GetComponent<Button>();
    }

    public void GetAndLinkSlider(System.Action<float> linkMethod)
    {
        CustomSlider slider = GetSlider();
        slider.OnValueChanged += linkMethod;
    }

    public void GetAndLinkAccelSlider(System.Action<float> linkMethod)
    {
        // accelSlider.OnValueChanged += linkMethod;
    }

    public void GetAndLinkJoystick(System.Action<Vector2> linkMethod)
    {
        //joystick.OnInputChanged += linkMethod;
    }

    public void LinkRotatingWheel(System.Action<float> linkMethod)
    {
        rotatingWheel.OnValueChanged += linkMethod;
    }

    public void PickUpObject()
    {
        RoboticArm.instance.pickableDetected = true;
    }

    public void DropObject()
    {
        RoboticArm.instance.canDrop = true;
    }

    // public void SetGrabButtonStatus(bool _enabled)
    // {
    //     grabButton.interactable = _enabled;
    //     grabButtonText.color = _enabled?enabledButtonColor:disabledButtonColor;
    //     grabButton.GetComponent<ButtonAnimator>().enabled = _enabled;
    // }

    // public void SetDropButtonStatus(bool _enabled)
    // {
    //     dropButton.interactable = _enabled;
    //     dropButtonText.color = _enabled?enabledButtonColor:disabledButtonColor;
    //     dropButton.GetComponent<ButtonAnimator>().enabled = _enabled;
    // }

    const string pickupText = "PICK UP";
    const string dropText = "DROP";

    void UnlinkAllControls()
    {
        baseJointSlider.ResetSlider();
        bendJointSlider.ResetSlider();
        reachJointSlider.ResetSlider();
        descentJointSlider.ResetSlider();
        grappleJointSlider.ResetSlider();
        sliderjoystick1.ResetSlider();
        sliderjoystick2.ResetSlider();
    }

    void ShowLevel1Tutorial()
    {
        //controlsContainer.gameObject.SetActive(false);
        //LeanTween.delayedCall(1f, () =>
        //{
        //    //joystick.background.anchoredPosition = new Vector2(screen.x/2,screen.y/2);
        //    //ShowSpotlightTutorial(joystick.background);
        //    //joystick.background.gameObject.SetActive(true);
        //    infinityTutorial.gameObject.SetActive(true);
        //    StartCoroutine(Level1Coroutine());
        //});
    }

    

    void ShowLevel2Tutorial()
    {
        //rotatingWheel.enabled = false;
        //joystick.gameObject.SetActive(false);
        //sliderjoystick1.enabled = false;
        //sliderjoystick2.enabled = false;
        //TruckController.Instance.GetComponent<Rigidbody>().isKinematic = true;
        //LeanTween.delayedCall(1f, () =>
        //{
        //    StartCoroutine(Level2Coroutine());
        //});
    }

    IEnumerator Level2Coroutine()
    {
        // highlight rotating wheel.
        //ShowSpotlightTutorial(rotatingWheel.GetComponent<RectTransform>());
        //rotatingWheel.enabled = true;
        //yield return StartCoroutine(WaitForRotatingWheelMouseDown(rotatingWheel));
        HideSpotLightTutorial(false, true);
        //wait until the rotating wheel reaches a certain rotation
        //ConfigurableJoint joint = TruckController.Instance.baseJoint.joint;
        Quaternion targetRot = Quaternion.Euler(baseJointTutorialTargetRotation);
        //yield return StartCoroutine(WaitForJointToRotateTo(joint, targetRot));
        // and then snap to that rotation
        //joint.targetRotation = targetRot;
        //rotatingWheel.enabled = false;
        //now highlight the vertical crane control
        sliderjoystick2.enabled = true;
        ShowSpotlightTutorial(sliderjoystick2.GetComponent<RectTransform>());
        yield return StartCoroutine(WaitForSliderMouseDown(sliderjoystick2));
        HideSpotLightTutorial(false, true);
        //now highlight the reach crane control and repeat until it snaps within a range and also fadeout spotlight on touch
        //joint = TruckController.Instance.bendingJoint.joint;
        targetRot = Quaternion.Euler(bendJointTutorialTargetRotation);
        //yield return StartCoroutine(WaitForJointToRotateTo(joint, targetRot));
        //joint.targetRotation = targetRot;
        sliderjoystick2.enabled = false;
        sliderjoystick1.enabled = true;

        //highlight pickup button when ready to press for atleast 0.5 sec.

        ShowSpotlightTutorial(sliderjoystick1.GetComponent<RectTransform>());
        // and repeat until it snaps within a range and also fadeout spotlight on touch
        yield return StartCoroutine(WaitForSliderMouseDown(sliderjoystick1));
        HideSpotLightTutorial(false, true);
        //now highlight the reach crane control and repeat until it snaps within a range and also fadeout spotlight on touch
        //joint = TruckController.Instance.reachJoint.joint;
        Vector3 targetPos = reachJointTutorialTargetPosition;
        //yield return StartCoroutine(WaitForJointToMoveTo(joint, targetPos));
        //joint.targetPosition = targetPos;
        sliderjoystick1.enabled = false;
        //wait until the pickup button is pressed and after picking up show the object show the joystick to let player know that they can move.
        ShowSpotlightTutorial(singleGrabDropButton.GetComponent<RectTransform>());
        singleGrabDropButton.onClick.AddListener(GrabButtonClickHandler);
        bool grabButtonPressed = false;
        while (!grabButtonPressed) yield return null;

        HideSpotLightTutorial(false);
        //TruckController.Instance.OnGrab.AddListener(OnGrabbed);

        bool grabbed = false;
        while (!grabbed) yield return null;

        //TruckController.Instance.GetComponent<Rigidbody>().isKinematic = false;
        //again enable drop button only when above the cart. highlight it and done.
        joystick.gameObject.SetActive(true);
        //joystick.background.anchoredPosition = new Vector2(screen.x/2,screen.y/2 * 0.55f);
        //joystick.background.gameObject.SetActive(true);
        //infinityTutorial.gameObject.SetActive(true);
        //yield return ShowInfinityTutorialCoroutine();

        sliderjoystick1.enabled = sliderjoystick2.enabled = true;

        float timer = 0;
        //var truck = TruckController.Instance;

        //while (timer < 0.5f)
        //{
        //    timer += Time.deltaTime;
        //    if (!truck.canDrop) timer = 0;
        //    yield return null;
        //}

        //truck.GetComponent<Rigidbody>().isKinematic = true;
        ShowSpotlightTutorial(singleGrabDropButton.GetComponent<RectTransform>());
        singleGrabDropButton.onClick.AddListener(DropButtonClickHandler);
        bool dropButtonPressed = false;
        while (!dropButtonPressed) yield return null;
        HideSpotLightTutorial(false);

        //helper functions
        void GrabButtonClickHandler()
        {
            grabButtonPressed = true;
            singleGrabDropButton.onClick.RemoveListener(GrabButtonClickHandler);
        }

        void DropButtonClickHandler()
        {
            dropButtonPressed = true;
            singleGrabDropButton.onClick.RemoveListener(DropButtonClickHandler);
        }

        void OnGrabbed()
        {
            grabbed = true;
        }
    }

    IEnumerator WaitForJointToMoveTo(ConfigurableJoint joint, Vector3 targetPos)
    {
        while (Vector3.Distance(joint.targetPosition, targetPos) > 0.05f)
            yield return null;
    }

    IEnumerator WaitForJointToRotateTo(ConfigurableJoint joint, Quaternion targetRot)
    {
        while (Quaternion.Angle(joint.targetRotation, targetRot) > 5)
            yield return null;
    }

    IEnumerator WaitForSliderMouseDown(CustomSlider slider)
    {
        bool touched = false;
        slider.OnMouseDownTouched += () => touched = true;
        while (!touched) yield return null;
        slider.OnMouseDownTouched = null;
    }

    IEnumerator WaitForMouseDown()
    {
        while (!Input.GetMouseButtonDown(0)) yield return null;
    }

    IEnumerator WaitForMouseUp()
    {
        while (!Input.GetMouseButtonUp(0)) yield return null;
    }

    IEnumerator WaitForTap()
    {
        yield return StartCoroutine(WaitForMouseDown());
        yield return StartCoroutine(WaitForMouseUp());
    }
}
