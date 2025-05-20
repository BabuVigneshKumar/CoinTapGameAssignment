using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : UIManager
{
    [Header("CoinSpwan Properties")]
    [SerializeField] private int poolSize = 20;
    [SerializeField] private Queue<GameObject> coinPool = new Queue<GameObject>();
    [SerializeField] private float gameTime = 30f;
    [SerializeField] private float spawnIntervalMin = 1f;
    [SerializeField] private float spawnIntervalMax = 2f;
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
    private Coroutine gameTimerCoroutine;
    private Coroutine coinSpawnerCoroutine;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
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
        UpdateCurrentState(GameState.Init);
    }
    public void AddScore()
    {
        Score++;
        OnScoreUpdated?.Invoke(Score);
        scoreText.text = Score.ToString();
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
              
                    remainingGameTime = gameTime;
                    nextCoinSpawnTime = 0f;

              
                    gameTimerCoroutine = StartCoroutine(GameTimer());
                    //coinSpawnerCoroutine = StartCoroutine(CoinSpawner());
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
        UpdateCurrentState(GameState.Playing);
    }

    private void InitializePool()
    {
        coinPool.Clear();

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
    private void OnCoinTapped(Coin tapped)
    {
        tapped.gameObject.SetActive(false);
        coinPool.Enqueue(tapped.gameObject);
        AddScore();
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
    private void BackToMainMenu()
    {
        SceneLoader.Instance.LoadSceneWithLoading("MainMenu", 1f);
    }


    private IEnumerator GameTimer()
    {
        float timeCount = 0f;
        float spawnTimer = 0f;

        float spawnInterval = 1f;

        while (timeCount <= 30)
        {

            while (currentState == GameState.Paused)
            {
                Debug.Log("Game paused !! ");

                yield return null;
            }

            float delta = Time.deltaTime;
            timeCount += delta;
            spawnTimer += delta;


            timerTxt.text = Mathf.RoundToInt(timeCount).ToString();

            if (spawnTimer >= spawnInterval)
            {
                spawnTimer = 0f;
                float randDelay = Random.Range(1f, 2f);

                if (CountActiveCoins() < 5)
                    StartCoroutine(SpawnCoin(randDelay));
            }
            if (timeCount >= 30f)
            {
                GameOver();
                yield break;
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
    private IEnumerator SpawnCoin(float delay)
    {
        float elapsed = 0f;

        while (elapsed < delay)
        {
            if (currentState != GameState.Paused)
            {
                elapsed += Time.deltaTime;
            }

            yield return null;
        }
        if (coinPool.Count > 0 && currentState == GameState.Playing)
        {
            float rangeX = Random.Range(-2.5f, 2.5f);
            float rangeY = Random.Range(-5f, 5f);


            GameObject coin = coinPool.Dequeue();
            coin.SetActive(true);
            coin.transform.position = new Vector3(rangeX, rangeY, 0);
        }
    }

    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            UpdateCurrentState(GameState.Paused);
            pauseMenu.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            UpdateCurrentState(GameState.Playing);
            pauseMenu.SetActive(false);
        }
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