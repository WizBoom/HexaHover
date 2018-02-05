using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public float MusicVolume = 0.5f;

    //fadeTime
    [SerializeField]
    private float _fadeTime = 2.0f;
    private float _fadeTimer = 0;
    //tracks
    private AudioSource _source;
    [SerializeField]
    public AudioClip[] MenuMusic;
    [SerializeField]
    public AudioClip[] GameMusic;

    private bool _inMainMenu = true;
    private int _currentSong = -1;
    private float _songTimer = 0;

    private void Start()
    {
        _source = GetComponent<AudioSource>();
        _source.volume = MusicVolume;
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlayRandMenu();
        }
        else
        {
            PlayRandGame();
        }

        Debug.Log(_source.clip);
    }

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        DontDestroyOnLoad(this.GetComponent<AudioManager>());

    }

    private void Update()
    {
        AudioFade(true);

        if (SceneManager.GetActiveScene().buildIndex == 0 && !_inMainMenu)
        {
            PlayRandMenu();
            _inMainMenu = true;
        }
        else if (SceneManager.GetActiveScene().buildIndex != 0 && _inMainMenu)
        {
            PlayRandGame();
            _inMainMenu = false;
        }

        //check ending of song
        _songTimer += Time.deltaTime;
        if (_songTimer > _source.clip.length - _fadeTime)
            AudioFade(false);
        if (_songTimer >= _source.clip.length)
        {
            if (_inMainMenu)
                PlayRandMenu();
            else
                PlayRandGame();
        }
    }

    public void Play(AudioClip clip)
    {
        _source.clip = clip;
        _source.Play();
        _currentSong = GetClipIndex(_source.clip);
        _fadeTimer = 0;
    }

    private void PlayRandMenu()
    {
        int songIndex = Random.Range(0, MenuMusic.Length);
        while (songIndex == _currentSong)
        {
            songIndex = Random.Range(0, MenuMusic.Length);
        }
        Play(MenuMusic[songIndex]);
    }
    private void PlayRandGame()
    {
        int songIndex = Random.Range(0, GameMusic.Length);
        while (songIndex == _currentSong)
        {
            songIndex = Random.Range(0, GameMusic.Length);
        }
        Play(GameMusic[songIndex]);
    }
    private void AudioFade(bool fadeIn)
    {
        if (fadeIn && _fadeTimer < _fadeTime)
        {
            _fadeTimer += Time.deltaTime;
        }
        else if (_fadeTimer < _fadeTime)
        {
            _fadeTimer -= Time.deltaTime;
        }
        _source.volume = _fadeTimer / _fadeTime * MusicVolume;
    }

    private int GetClipIndex(AudioClip clip)
    {
        for (int i = 0; i < MenuMusic.Length; i++)
        {
            if (MenuMusic[i]==clip)
                return i;
        }
        for (int i = 0; i < GameMusic.Length; i++)
        {
            if (GameMusic[i] == clip)
                return i;
        }
        return 0;
    }
}