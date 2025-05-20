using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private Button playBtn, exitBtn;

    private void Start()
    {
        playBtn.onClick.AddListener(PlayGame);
        exitBtn.onClick.AddListener(ExitGame);
    }

    private void PlayGame()
    {
        SceneLoader.Instance.LoadSceneWithLoading("CoinTapGame", 1f);
    }
    private void ExitGame()
    {
        Application.Quit();
    }
}
