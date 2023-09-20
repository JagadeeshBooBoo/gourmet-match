using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public int levelIndex;

    [SerializeField] private GameObject _confetti;

    public GameObject mainPlate;
    public int targetPlacements;
    public int currentPlacements = 0;

    public bool incorrectPlacement;

    private void Awake()
    {
        instance = this;

        if (PlayerPrefs.GetInt("LevelIndex") == 0)
            PlayerPrefs.SetInt("LevelIndex", 1);

        levelIndex = PlayerPrefs.GetInt("LevelIndex");
    }

    // Start is called before the first frame update
    void Start()
    {
        TweenMainPlate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Confetti()
    {
        GameObject effect = Instantiate(_confetti);
    }

    private void TweenMainPlate()
    {
        mainPlate.transform.DOLocalMoveX(0f, 0.5f).SetEase(Ease.OutBack).SetDelay(1f);
    }
}
