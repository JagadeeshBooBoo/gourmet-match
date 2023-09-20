using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private GameObject _levelCompleted;
    [SerializeField] private GameObject _levelFailed;

    private int _currentScene;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Next()
    {
        if (PlayerPrefs.GetInt("LevelIndex") == 3)
            PlayerPrefs.SetInt("LevelIndex", 0);

        PlayerPrefs.SetInt("LevelIndex", PlayerPrefs.GetInt("LevelIndex") + 1);

        SceneManager.LoadScene(_currentScene);
    }

    public void Fail()
    {
        SceneManager.LoadScene(_currentScene);
    }

    public void LevelCompleted()
    {
        _levelCompleted.SetActive(true);
    }

    public void LevelFailed()
    {
        _levelFailed.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(_currentScene);
    }
}
