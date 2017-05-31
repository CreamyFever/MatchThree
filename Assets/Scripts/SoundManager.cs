using SingletonPattern;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum SoundState
{
    Main,
    Lobby,
    Room,
    End
}

public class SoundManager : SingletonPattern<SoundManager>
{
    [HideInInspector]
    public GameObject settingBtn;       // 設定ボタン
    [HideInInspector]
    public GameObject settingBase;      // 設定ボタンを押すと出るボタンのベース
    [HideInInspector]
    public GameObject bgmBtn;           // 背景音ボタン
    [HideInInspector]
    public GameObject efxBtn;           // 効果音ボタン
    [HideInInspector]
    public GameObject[] banImage;       // 禁止イメージ

    [HideInInspector]
    public AudioSource bgmSource;
    [HideInInspector]
    public AudioSource efxSource;
    public float lowPitchRange = 0.95f;
    public float highPitchRange = 1.05f;

    public AudioListener globalAudioListener;

    public SoundState soundState;

    public bool isPlaying = false;

    public AudioClip[] bgmClip;
    public AudioClip[] efxClip;

    void Awake()
    {
        instance = this;

        SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(globalAudioListener);
    }

    /// <summary>
    /// シーンのロードが終わるたびに呼び出される。（Unityバージョン5.4から）
    /// </summary>
    /// <param name="loadedScene"></param>
    /// <param name="mode"></param>
    void OnSceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        switch (loadedScene.name)
        {
            case "Title":
                settingBtn = GameObject.Find("SettingBtn");
                settingBase = GameObject.Find("SettingBase");
                bgmBtn = GameObject.Find("BGMBtn");
                efxBtn = GameObject.Find("EFXBtn");
                banImage[0] = GameObject.Find("BanIcon0");
                banImage[1] = GameObject.Find("BanIcon1");

                settingBase.SetActive(false);
                banImage[0].SetActive(false);
                banImage[1].SetActive(false);
                break;
            case "Game":


                break;
        }
    }

    void Update()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            switch (soundState)
            {
                case SoundState.Main:
                    ChangeSceneMusic(bgmClip[0]);
                    break;
                case SoundState.Lobby:
                    ChangeSceneMusic(bgmClip[1]);
                    break;
                case SoundState.Room:
                    ChangeSceneMusic(bgmClip[2]);
                    break;
                case SoundState.End:
                    TurnOffMusic();
                    break;
            }
        }
    }

    /// <summary>
    /// 効果音を再生する。
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySoundEffect(AudioClip clip)
    {
        if (!efxSource.isPlaying)
        {
            efxSource.clip = clip;
            efxSource.Play();
        }
    }


    public void RandomSoundEffect(params AudioClip[] clips)
    {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        efxSource.pitch = randomPitch;

        if (!efxSource.isPlaying)
        {
            efxSource.clip = clips[randomIndex];
            efxSource.Play();
        }
    }

    /// <summary>
    /// シーンで流れるBGMを変える。
    /// </summary>
    /// <param name="clip"></param>
    public void ChangeSceneMusic(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void TurnOnMusic()
    {
        bgmSource.Play();
    }

    public void TurnOffMusic()
    {
        bgmSource.Stop();
    }

    public void ControlVolume(float volume)
    {
        bgmSource.volume = volume;
    }
}
