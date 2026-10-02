using System.Collections;
using System.Collections.Generic;
using HarvestShore.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;
    public GameObject menuPrefab;
    public string titleSceneName = "StartGame";

    public Button settingsBtn;
    public GameObject pausePanel;
    public Slider volumeSlider;


    private void Awake()
    {
        if (settingsBtn != null)
            settingsBtn.onClick.AddListener(TogglePausePanel);
        if (volumeSlider != null && AudioManager.Instance != null)
            volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneLoadedEvent;
    }


    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
    }
    private void OnAfterSceneLoadedEvent()
    {
        if (menuCanvas != null && menuCanvas.transform.childCount > 0)
            Destroy(menuCanvas.transform.GetChild(0).gameObject);
    }

    public void TogglePausePanel()
    {
        if (pausePanel == null)
            return;

        bool isOpen = pausePanel.activeInHierarchy;

        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            System.GC.Collect();
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }


    public void ReturnMenuCanvas()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(titleSceneName);
    }
}
