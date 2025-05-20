using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private CanvasGroup fadeGroup;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        if (loadingScreen != null)
            DontDestroyOnLoad(loadingScreen);

    }
    public void LoadSceneWithLoading(string sceneName, float sceneDelay)
    {
        StartCoroutine(LoadSceneWithLoadingDelay(sceneName, sceneDelay));
    }

    private IEnumerator LoadSceneWithLoadingDelay(string sceneName, float sceneDelay)
    {
        yield return StartCoroutine(FadeIn());
        loadingScreen.SetActive(true);

        yield return new WaitForSeconds(sceneDelay);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        while (!async.isDone)
            yield return null;

        yield return StartCoroutine(FadeOut());
            loadingScreen.SetActive(false);
    }

    private IEnumerator FadeIn()
    {
        if (fadeGroup == null) yield break;

        fadeGroup.gameObject.SetActive(true);
        fadeGroup.alpha = 0;
        fadeGroup.DOFade(1, fadeDuration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(fadeDuration);
    }

    private IEnumerator FadeOut()
    {
        if (fadeGroup == null) yield break;

        fadeGroup.DOFade(0, fadeDuration).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(fadeDuration);
        fadeGroup.gameObject.SetActive(false);
    }

}
