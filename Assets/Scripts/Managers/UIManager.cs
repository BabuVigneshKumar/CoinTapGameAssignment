
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIManager : MonoBehaviour
{
    [SerializeField] protected TMP_Text timerTxt, scoreText, finalScoreTxt;
    [SerializeField] protected GameObject CoinPrefab, FinalResultPanel, IngameHud, pauseMenu;
    [SerializeField] protected Transform coinSpawnner;
    [SerializeField] protected Button restartBtn, backToMainMenuBtn, pauseBtn, resumeBtn;
    [SerializeField] protected Toggle SoundToggle;
    protected void FinalResult()
    {
        IngameHud.SetActive(false);

        FinalResultPanel.SetActive(true);
    }

 

}
