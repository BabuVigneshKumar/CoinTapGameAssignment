using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryHandler : MonoBehaviour
{
    [SerializeField] private Button LoginBtn;
    [SerializeField] SceneLoader sceneLoader;
    void Start()
    {
        LoginBtn.onClick.AddListener(OpenLoginPanel);
    }

    private void OpenLoginPanel()
    {
        if (sceneLoader != null)
            LoginBtn.gameObject.SetActive(false);
        sceneLoader.LoadSceneWithLoading("Login", 2f);

    }
}
