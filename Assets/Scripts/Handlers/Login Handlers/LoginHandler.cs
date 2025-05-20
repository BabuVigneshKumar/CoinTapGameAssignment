using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class LoginHandler : MonoBehaviour
{
    [Header("Login Details")]
    [SerializeField] private TMP_InputField passwordInput, phoneNumberInput;
    [SerializeField] private Toggle passwordToggle;
    [SerializeField] private Button LoginBtn;
    [SerializeField] private GameObject LoginPanelPrefab;
    [SerializeField] private CanvasGroup loginfadeGroup;
    [Header("Handle Validate")]
    [SerializeField] private TMP_Text validateErrorText;
    private bool isPasswordHidden = true;

    private IUserValidator validator;

    private void Awake()
    {
        validator = new LoginUserValidate();
    }

    private void Start()
    {
        passwordToggle.onValueChanged.AddListener(HandlePassWordToggle);
        LoginBtn.onClick.AddListener(UserValidateDetails);
    }

    private void HandlePassWordToggle(bool isShown)
    {
        isPasswordHidden = !isShown;
        passwordInput.contentType = isPasswordHidden ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
        passwordInput.ForceLabelUpdate();
    }

    private void UserValidateDetails()
    {
        StartCoroutine(ValidateErrorCheck());
    }

    private IEnumerator ValidateErrorCheck()
    {
        validateErrorText.text = "";
        string error = validator.Validate(phoneNumberInput.text, passwordInput.text);

        if (!string.IsNullOrEmpty(error))
        {
            validateErrorText.color = Color.red;
            validateErrorText.text = error;
            yield return new WaitForSeconds(1.5f);
            validateErrorText.text = "";
        }
        else
        {
            validateErrorText.color = Color.green;
            validateErrorText.text = "Login Success";
            yield return new WaitForSeconds(0.5f);
            validateErrorText.text = "";
            LoginPanelPrefab.SetActive(false);
            SceneLoader.Instance.LoadSceneWithLoading("MainMenu", 2f);
        }
    }
}
