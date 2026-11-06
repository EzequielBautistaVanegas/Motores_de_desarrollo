using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _gameScene = "Game";
    [SerializeField] private GameObject _introPanel;
    [SerializeField] private GameObject _continueHint;
    [SerializeField] private float _minReadTime = 1f;

    private IDisposable _anyKeyListener;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        _introPanel.SetActive(false);
    }

    private void OnDisable()
    {
        _anyKeyListener?.Dispose();
    }

    public void Play()
    {
        _introPanel.SetActive(true);
        StartCoroutine(WaitForAnyKey());
    }

    private IEnumerator WaitForAnyKey()
    {
        if (_continueHint != null) _continueHint.SetActive(false);

        // Minimum time to read the intro text
        yield return new WaitForSecondsRealtime(_minReadTime);

        if (_continueHint != null) _continueHint.SetActive(true);

        // Any button press listener
        _anyKeyListener = InputSystem.onAnyButtonPress.CallOnce(_ => LoadGame());
    }

    private void LoadGame()
    {
        SceneManager.LoadScene(_gameScene);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}