using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    [SerializeField]
    private Scrollbar _musicVolume;
    [SerializeField]
    private Scrollbar _effectsVolume;
    private bool _active= true;

    void Start () {
        _musicVolume.value = GameObject.FindObjectOfType<AudioManager>().MusicVolume;
        _effectsVolume.value = GameObject.FindObjectOfType<PlayerAudioManager>().GetVolume();

        _musicVolume.onValueChanged.AddListener(delegate { MusicVolumeChange(); });
        _effectsVolume.onValueChanged.AddListener(delegate { EffectsVolumeChange(); });

        TogglePauseMenu();
    }

    void Update() {
        if (Input.GetButtonUp("Pause"))
            TogglePauseMenu();
    }

    void MusicVolumeChange()
    {
        GameObject.FindObjectOfType<AudioManager>().MusicVolume = _musicVolume.value;
    }
    void EffectsVolumeChange()
    {
        GameObject.FindObjectOfType<PlayerAudioManager>().SetVolume(_effectsVolume.value);
    }

    public void LoadScene(int index)
    {
        if (index == 0)
        {
            GameObject.FindObjectOfType<GameManager>().StaticReset();
        }
        SceneManager.LoadScene(index);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void TogglePauseMenu()
    {
        _active = !_active;
        GetComponent<Canvas>().enabled = _active;
        Time.timeScale = (_active ? 0.0f: 1.0f);
    }
}
