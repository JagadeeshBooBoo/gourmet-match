using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class TweenScaleElement : MonoBehaviour
{
    private Vector3 _originalScale;
    [SerializeField] private float _scaleSpeed;
    [SerializeField] private float _delay;

    [SerializeField] private Ease easeType;

    [Space(5)]
    public UnityEvent OnTweenCompleteEvent;

    // Start is called before the first frame update
    void Start()
    {
        TweenScale();
    }

    private void TweenScale()
    {
        _originalScale = transform.localScale;

        transform.localScale = Vector3.zero;
        transform.DOScale(_originalScale, _scaleSpeed).SetDelay(_delay).SetEase(easeType).OnComplete(
            () => OnTweenCompleteEvent.Invoke());
    }
}
