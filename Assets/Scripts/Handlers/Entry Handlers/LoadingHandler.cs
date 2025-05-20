using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingHandler : MonoBehaviour
{
    [SerializeField] private Image loadingBarFill;


    private void OnEnable()
    {
        StartCoroutine(FakeLoadingScreen());
    }
    private void OnDisable()
    {
        loadingBarFill.fillAmount = 0;
    }

    private IEnumerator FakeLoadingScreen()
    {
        float fillAmt = 0;
        float loadingSpeed = 0.5f;
        while (fillAmt <= 1f)
        {
            fillAmt += Time.deltaTime * loadingSpeed;
            loadingBarFill.fillAmount = Mathf.Clamp01(fillAmt);
            yield return null;
        }
    }
}
