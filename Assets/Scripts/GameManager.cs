using System.Collections;
using SingletonPattern;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : SingletonPattern<GameManager>
{
    private Text scoreText = null;
    private Text comboText = null;
    private Text highScoreText = null;
    private GameObject highScoreUpdatedImage = null;
    private Text timeText = null;
    private Image timeMaskImg = null;
    public GameObject pausePopup = null;
    public GameObject resultPopup = null;

    private const float TIME_MAX = 60.0f;

    public bool canControl = false;

    private float m_Time;
    public float GameTime
    {
        get { return m_Time; }
        set { m_Time = value; }
    }
    private int m_Score = 0;
    public int Score
    {
        get { return m_Score; }
        set { m_Score = value; }
    }

    private int m_Combo = 0;
    public int Combo
    {
        get { return m_Combo; }
        set { m_Combo = value; }
    }

    public float comboTime = 0.0f;
    public float comboRate = 1.0f;
    private const float COMBO_TIME = 2.0f;

    public int highScore;

    void Awake()
    {
        //PlayerPrefs.SetInt("HighScore", 0);
        instance = this;
        DontDestroyOnLoad(instance);
        highScore = PlayerPrefs.GetInt("HighScore", 0);

        SceneManager.sceneLoaded += OnSceneLoaded;
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
                StopAllCoroutines();
                highScoreText = GameObject.Find("HighScoreNumText").GetComponent<Text>();
                UpdateHighScore(highScore);
                break;
            case "Game":
                m_Time = TIME_MAX;
                pausePopup = GameObject.Find("PausePopup");
                resultPopup = GameObject.Find("ResultPopup");

                scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
                comboText = GameObject.Find("ComboText").GetComponent<Text>();
                highScoreText = resultPopup.transform.FindChild("HighScoreNumText").GetComponent<Text>();
                highScoreUpdatedImage = GameObject.Find("HighScoreUpdated");
                timeText = GameObject.Find("TimeText").GetComponent<Text>();
                timeMaskImg = GameObject.Find("TimeBarForeground").GetComponent<Image>();

                highScoreUpdatedImage.SetActive(false);
                pausePopup.SetActive(false);
                resultPopup.SetActive(false);

                Invoke("ReadyToDecrease", 2.0f);
                StartCoroutine(ElapseComboTime());
                break;
        }
    }

    void ReadyToDecrease()
    {
        canControl = true;
        StartCoroutine(DecreaseTimeBar());
    }
    
    /// <summary>
    /// タイムバーを毎フレーム削らせる。
    /// </summary>
    /// <returns></returns>
    IEnumerator DecreaseTimeBar()
    {
        while (m_Time >= 0)
        {
            m_Time -= Time.deltaTime;
            timeText.text = ((int)m_Time).ToString();

            timeMaskImg.fillAmount = m_Time / TIME_MAX;

            if (m_Time < 15.0f)
                timeMaskImg.color = Color.red;

            if (m_Time < 0.0f)
            {
                canControl = false;
                UpdateHighScore(m_Score);
                resultPopup.SetActive(true);
                SoundManager.Instance.PlaySoundEffect(SoundManager.Instance.efxClip[1]);
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// コンボタイムを過ぎたらコンボが途切れる。
    /// </summary>
    /// <returns></returns>
    IEnumerator ElapseComboTime()
    {
        while(comboTime < COMBO_TIME)
        {
            comboTime += Time.deltaTime;

            if (comboTime >= COMBO_TIME)
            {
                m_Combo = 0;
                comboTime = 0.0f;
                comboRate = 1.0f;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// コンボを更新する。
    /// </summary>
    /// <param name="score"></param>
    public void UpdateCombo()
    {
        comboTime = 0.0f;
        m_Combo++;

        //Debug.Log("Combo " + m_Combo);

        //5、10、15、20...コンボなら
        if (m_Combo % 5 == 0)
        {
            comboRate += 0.1f; // レートを10％くらい上げる。
            m_Time += 3.0f;  // 制限時間を3秒くらい延ばす。
        }

        comboText.text = string.Format(m_Combo.ToString() + "コンボ");
    }

    /// <summary>
    /// スコアを更新する。
    /// </summary>
    /// <param name="score"></param>
    public void UpdateScore(int score)
    {
        m_Score += score;
        scoreText.text = m_Score.ToString("D8");
    }

    /// <summary>
    /// ゲームが終わるたびに最高記録を更新。
    /// </summary>
    /// <param name="score"></param>
    public void UpdateHighScore(int score)
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            highScoreUpdatedImage.SetActive(true);
        }

        highScoreText.text = highScore.ToString("D8");
        m_Score = 0;
    }
}
