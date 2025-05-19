using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject CoinPrefab;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private TMP_Text timerTxt;
    [SerializeField] private Transform coinSpawnner;
    private Queue<GameObject> coinPool = new Queue<GameObject>();

    [SerializeField] private TMP_Text scoreText;  
    private int score = 0;
    void Start()
    {
        InitializePool();
        StartCoroutine(StartGameTimer());
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(touchPosition, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Coin"))
                {
                    Debug.Log("Hit object Destroyed " + hit.collider.gameObject.name);
                    hit.collider.gameObject.SetActive(false);
                    coinPool.Enqueue(hit.collider.gameObject);

                    score++;
                    scoreText.text = "Score: " + score;
                }
            }
            else
            {
                Debug.Log("Raycast did not hit anything!");
            }
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject coin = Instantiate(CoinPrefab, coinSpawnner);
            coin.SetActive(false);
            coinPool.Enqueue(coin);
        }
        scoreText.text = "Score: 0";
    }

    private IEnumerator StartGameTimer()
    {
        float timeCount = 0f;
        float interValTime = 2f;

        while (timeCount <= 30)
        {
            timeCount += Time.deltaTime;
            timerTxt.text = timeCount.ToString("F2");

            if (timeCount % interValTime < Time.deltaTime)
            {
                float randTime = Random.Range(1f, 2f);
                StartCoroutine(SpawnCoin(randTime));
            }
            yield return null;
        }
    }

    private IEnumerator SpawnCoin(float delay)
    {
        yield return new WaitForSeconds(delay);

        float rangeX = Random.Range(-2.5f, 2.5f);   
        float rangeY = Random.Range(-4.5f, 4.5f);  

        if (coinPool.Count > 0)
        {
            GameObject coin = coinPool.Dequeue();
            coin.SetActive(true);
            coin.transform.position = new Vector3(rangeX, rangeY, 0);
        }
    }
}
