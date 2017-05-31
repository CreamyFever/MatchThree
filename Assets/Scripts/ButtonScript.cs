using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void OnSettingButton()
    {
        SoundManager.Instance.settingBase.SetActive(!SoundManager.Instance.settingBase.activeSelf);
    }

    public void OnPauseButton()
    {
        Time.timeScale = 0.0f;
        GameManager.Instance.pausePopup.SetActive(true);
    }

    public void OnResumeButton()
    {
        Time.timeScale = 1.0f;
        GameManager.Instance.pausePopup.SetActive(false);
    }

    public void OnBGMButton()
    {
        SoundManager.Instance.bgmSource.mute = !SoundManager.Instance.banImage[0].activeSelf;
        SoundManager.Instance.banImage[0].SetActive(SoundManager.Instance.bgmSource.mute);
    }

    public void OnEFXButton()
    {
        SoundManager.Instance.efxSource.mute = !SoundManager.Instance.banImage[1].activeSelf;
        SoundManager.Instance.banImage[1].SetActive(SoundManager.Instance.efxSource.mute);
    }

    public void OnChangeSceneBtn(string sceneName)
    {
        Time.timeScale = 1.0f;
        SoundManager.Instance.isPlaying = false;

        if(sceneName == "Title")
            SoundManager.Instance.soundState = SoundState.Main;
        else if(sceneName == "Game")
            SoundManager.Instance.soundState = SoundState.Room;

        SceneManager.LoadScene(sceneName);
    }
}
