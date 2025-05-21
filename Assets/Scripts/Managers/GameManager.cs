using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : UIManager
{
    [Header("CoinSpwan Properties")]
    [SerializeField] private int poolSize = 20;
    [SerializeField] private Queue<GameObject> coinPool = new Queue<GameObject>();

    [SerializeField] private float gameTimer = 30f;

    [SerializeField] private float spawnIntervalMin = 1f;
    [SerializeField] private float spawnIntervalMax = 1.5f;

    [SerializeField] private int maxActiveCoins = 5;

    public int Score { get; private set; }
    public System.Action<int> OnScoreUpdated;

    [Header("GameState")]
    [SerializeField] private GameState currentState;


    [Header("Intance Classes")]
    [SerializeField] private AudioManager audioManager;
    public static GameManager instance;

    [Header("Timers Properties")]
    private float remainingGameTime;
    private float nextCoinSpawnTime;

    private float pauseStartTime;
    private float totalPausedTime;

    private Coroutine gameTimerCoroutine;
    private Coroutine coinSpawnerCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
 
        }
    }
    private void OnDestroy()
    {
        foreach (GameObject coin in coinPool)
        {
            if (coin != null)
            {
                Coin coinComponent = coin.GetComponent<Coin>();
                if (coinComponent != null)
                {
                    coinComponent.coinTap -= OnCoinTapped;
                }
            }
        }

    }
    private void Start()
    {
        restartBtn.onClick.AddListener(GameRestart);
        pauseBtn.onClick.AddListener(PauseGame);
        backToMainMenuBtn.onClick.AddListener(BackToMainMenu);
        resumeBtn.onClick.AddListener(ResumeGame);
        SoundToggle.onValueChanged.AddListener(ToggleSound);

        UpdateCurrentState(GameState.Init);
    }

    private void UpdateCurrentState(GameState newState)
    {
        GameState previousState = currentState;
        currentState = newState;

        switch (currentState)
        {
            case GameState.Init:
                GameInitalize();
                break;

            case GameState.Playing:
                if (previousState == GameState.Init)
                {

                    remainingGameTime = gameTimer;
                    nextCoinSpawnTime = 0f;

                    gameTimerCoroutine = StartCoroutine(GameTimer());
                    coinSpawnerCoroutine = StartCoroutine(CoinSpawner());
                }
                break;

            case GameState.GameOver:
                FinalResult();


                if (gameTimerCoroutine != null)
                    StopCoroutine(gameTimerCoroutine);

                if (coinSpawnerCoroutine != null)
                    StopCoroutine(coinSpawnerCoroutine);
                break;
        }
    }

    private void GameInitalize()
    {
        InitializePool();
        audioManager.StopAudio(Audioenum.GameBgm);
        audioManager.PlayAudio(Audioenum.GameBgm, true);
        UpdateCurrentState(GameState.Playing);
    }

    private void InitializePool()
    {
        coinPool.Clear();
        totalPausedTime = 0f;
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(CoinPrefab, coinSpawnner);
            coin.SetActive(false);
            Coin tap = coin.GetComponent<Coin>();
            tap.coinTap += OnCoinTapped;
            coinPool.Enqueue(coin);
        }

        Score = 0;
        scoreText.text = "0";
    }

    public void AddScore()
    {
        Score++;
        OnScoreUpdated?.Invoke(Score);
        scoreText.text = Score.ToString();
    }
    private void OnCoinTapped(Coin tapped)
    {
        tapped.gameObject.SetActive(false);
        coinPool.Enqueue(tapped.gameObject);
        audioManager.StopAudio(Audioenum.CoinTap);
        audioManager.PlayAudio(Audioenum.CoinTap);
        AddScore();
    }

    private IEnumerator GameTimer()
    {
        while (remainingGameTime > 0)
        {
            if (currentState == GameState.Playing)
            {
                remainingGameTime -= Time.deltaTime;
                timerTxt.text = Mathf.CeilToInt(remainingGameTime).ToString();
            }
            yield return null;
        }
        GameOver();
    }
    private IEnumerator CoinSpawner()
    {
        while (currentState != GameState.GameOver)
        {
            if (currentState == GameState.Playing)
            {
                float adjustedTime = Time.time - totalPausedTime;

                if (adjustedTime >= nextCoinSpawnTime && CountActiveCoins() < maxActiveCoins)
                {
                    float spawnCoininterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
                    nextCoinSpawnTime = Time.time + spawnCoininterval;
                    SpawnCoin();
                }
            }

            yield return null;
        }
    }
    private int CountActiveCoins()
    {
        int count = 0;
        foreach (GameObject coin in coinPool)
        {
            if (coin.activeInHierarchy)
                count++;
        }
        return count;
    }

    private void SpawnCoin()
    {
        if (coinPool.Count > 0 && currentState == GameState.Playing)
        {
            float rangeX = Random.Range(-2.5f, 2.5f);
            float rangeY = Random.Range(-5f, 5f);

            GameObject coin = coinPool.Dequeue();
            coin.SetActive(true);
            coin.transform.position = new Vector3(rangeX, rangeY, 0);
        }
    }
    public void ToggleSound(bool value)
    {
        SoundToggle.isOn = value;
        Debug.Log("Audio Toggle Values ! " + value);
        if (SoundToggle.isOn)
            audioManager.StopAudio(Audioenum.ToggleSound);
        audioManager.PlayAudio(Audioenum.ToggleSound);

        if (SoundToggle.isOn)
        {
            Debug.Log("Audio Un muted !");

            audioManager.SetVolumeUnMute();
        }
        else
        {
            Debug.Log("Audio muted !");
            audioManager.SetVolumeMute();
        }

    }

    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            pauseStartTime = Time.time;
            UpdateCurrentState(GameState.Paused);
            pauseMenu.SetActive(true);
            audioManager.PlayAudio(Audioenum.ButtonTap);

        }
    }

    public void ResumeGame()
    {

        if (currentState == GameState.Paused)
        {
            float pauseDuration = Time.time - pauseStartTime;
            totalPausedTime += pauseDuration;
            nextCoinSpawnTime += pauseDuration;
            UpdateCurrentState(GameState.Playing);
            pauseMenu.SetActive(false);

            audioManager.StopAudio(Audioenum.GameBgm);
            audioManager.PlayAudio(Audioenum.GameBgm, true);
        }
    }
    public void BackToMainMenu()
    {
        audioManager.StopAudio(Audioenum.ButtonTap);
        audioManager.PlayAudio(Audioenum.ButtonTap);
        audioManager.StopAudio(Audioenum.GameBgm);
        
        SceneLoader.Instance.LoadSceneWithLoading("MainMenu", 1f);
    }
    private void GameOver()
    {
        currentState = GameState.GameOver;
        Debug.Log("Game Over! Final Score: " + Score);
        finalScoreTxt.text = Score.ToString();
        UpdateCurrentState(GameState.GameOver);
        Score = 0;
        scoreText.text = "0";
    }
    private void GameRestart()
    {
        audioManager.PlayAudio(Audioenum.ButtonTap);

        GameObject[] activeCoins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in activeCoins)
        {
            if (coin.activeInHierarchy)
            {
                coin.SetActive(false);
                coinPool.Enqueue(coin);
            }
        }
        UpdateCurrentState(GameState.Init);
    }

}
public enum GameState
{
    None,
    Init,
    Playing,
    Paused,
    GameOver
}