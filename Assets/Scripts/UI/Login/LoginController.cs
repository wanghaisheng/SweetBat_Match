using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Assets.SimpleSpinner;
using System.Threading.Tasks;
using System.Collections.Generic;

public enum GenderUser { Male, Female, Unknown }

public class LoginController : MonoBehaviour
{
    // Gets or sets the user's photo sprite.
    public Sprite PhotoUser { get { return photoUser; } set { photoUser = value; } }
    public GenderUser CurrentGenderUser { set { currentGenderUser = value; } }

    [Header("Displays")]
    [SerializeField] GameObject displaySingIn;
    [SerializeField] GameObject displaySingOut;
    [SerializeField] GameObject displayConfirmSingOut;
    [SerializeField] GameObject displayGlobalLoading;

    [Header("Gender")]
    //=================Gender==========================//
    [SerializeField] GameObject displayGetGender;

    [Header("Sing In")]
    //=================Sing In=========================//
    [SerializeField] TMP_Text welcomeText;
    [SerializeField] AvatarController avatar;
    [SerializeField] TMP_Text nameUserText;
    [SerializeField] GameObject warningMessage;

    [Header("Sing Out")]
    //=================Sing Out=========================//
    [SerializeField] GameObject loadingLogOut;
    [SerializeField] Image checkLogOut;
    [SerializeField] TMP_Text textButtonLogOut;
    [SerializeField] SimpleSpinner simpleSpinner;

    [Header("Authentications")]
    [SerializeField] FirebaseApp firebaseApp;
    [SerializeField] CloudFirestore cloudFirestore;
    [SerializeField] GoogleAuth googleAuth;
    [SerializeField] FacebookAuth facebookAuth;

    GenderUser currentGenderUser;
    Sprite photoUser = null;
    Animator animator;

    Dictionary<string, object> userData;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Initiates the Google login process.
    /// </summary>
    public void LoginGoogle()
    {
        googleAuth.LoginGoogle();
    }

    /// <summary>
    /// Initiates the Facebook login process.
    /// </summary>
    public void LoginFacebook()
    {
        warningMessage.SetActive(true);
        Invoke("DisableWarningMessage", 10);
        // facebookAuth.LoginFacebook();
    }

    /// <summary>
    /// Handles the successful login by displaying the appropriate UI.
    /// </summary>
    /// <param name="userData">User data for the logged-in user.</param>
    public void LoginSuccess(Dictionary<string, object> userData)
    {
        this.userData = userData;
        StartCoroutine(LoginSuccessRutiner());
    }

    /// <summary>
    /// Coroutine to handle the UI transition after a successful login.
    /// </summary>
    IEnumerator LoginSuccessRutiner()
    {
        ActivateGlobalLoading();
        Task<bool> checkUserTask = cloudFirestore.CheckUserExists(userData["id"].ToString());

        yield return new WaitUntil(() => checkUserTask.IsCompleted);

        bool userExists = checkUserTask.Result;

        displaySingIn.SetActive(false);

        DisableGlobalLoading();

        if (userExists)
        {
            SetInformationUser();
            StartCoroutine(UpdateAvatar());
            yield break;
        }

        displayGetGender.SetActive(true);
    }

    /// <summary>
    /// Toggles the selection of the male gender.
    /// </summary>
    /// <param name="toggle">Toggle component for male gender.</param>
    public void ToggleMale(Toggle toggle)
    {
        if (toggle.isOn) currentGenderUser = GenderUser.Male;
    }

    /// <summary>
    /// Toggles the selection of the female gender.
    /// </summary>
    /// <param name="toggle">Toggle component for female gender.</param>
    public void ToggleFemale(Toggle toggle)
    {
        if (toggle.isOn) currentGenderUser = GenderUser.Female;
    }

    /// <summary>
    /// Toggles the selection of the unknown gender.
    /// </summary>
    /// <param name="toggle">Toggle component for unknown gender.</param>
    public void ToggleUnknown(Toggle toggle)
    {
        if (toggle.isOn) currentGenderUser = GenderUser.Unknown;
    }

    /// <summary>
    /// Continues the process after selecting the gender and sets the user information.
    /// </summary>
    public void ContinueGenderUser()
    {
        StartCoroutine(SetInformationNewUser());
    }

    /// <summary>
    /// Sets the user information and updates the UI accordingly.
    /// </summary>
    IEnumerator SetInformationNewUser()
    {
        yield return GlobalLoading();

        displayGetGender.SetActive(false);

        userData.Add("gender", currentGenderUser.ToString());

        if (currentGenderUser == GenderUser.Male) avatar.UserMan(photoUser);
        else if (currentGenderUser == GenderUser.Female) avatar.UserWomen(photoUser);
        else if (currentGenderUser == GenderUser.Unknown) avatar.UserUnknown(photoUser);

        SetInformationUser();

        CreateNewUserDataBase();
    }

    /// <summary>
    /// Updates the user avatar based on the gender and photo provided.
    /// </summary>
    IEnumerator UpdateAvatar()
    {
        yield return GlobalLoading();
        avatar.UpdateAvatar(currentGenderUser, photoUser);
    }

    /// <summary>
    /// Updates the UI with the user information.
    /// </summary>
    void SetInformationUser()
    {
        displaySingOut.SetActive(true);
        animator.enabled = true;

        string nameUser = (string)userData["name"];

        welcomeText.text = string.Format($"Hello, {nameUser.Split(' ')[0]}!");
        nameUserText.text = nameUser;
    }

    /// <summary>
    /// Creates a new user in the database.
    /// </summary>
    void CreateNewUserDataBase()
    {
        cloudFirestore.CreateNewUser(userData);
    }

    /// <summary>
    /// Shows the global loading display until the user's photo is loaded.
    /// </summary>
    /// <returns>An IEnumerator used for coroutine execution.</returns>
    IEnumerator GlobalLoading()
    {
        while (photoUser == null)
        {
            if (!displayGlobalLoading.activeInHierarchy)
                ActivateGlobalLoading();

            yield return null;
        }

        if (displayGlobalLoading.activeInHierarchy)
            DisableGlobalLoading();
    }

    void ActivateGlobalLoading() => displayGlobalLoading.SetActive(true);

    void DisableGlobalLoading() => displayGlobalLoading.SetActive(false);

    /// <summary>
    /// Initiates the sign out process.
    /// </summary>
    public void SignOut()
    {
        animator.Play("EnterLogOut");
    }

    /// <summary>
    /// Shows the global loading display until the user's photo is loaded.
    /// </summary>
    /// <returns>An IEnumerator used for coroutine execution.</returns>
    public void ReturnLogOut()
    {
        animator.Play("ExitLogOut");
    }

    /// <summary>
    /// Shows the global loading display until the user's photo is loaded.
    /// </summary>
    /// <returns>An IEnumerator used for coroutine execution.</returns>
    public void ConfirmSignOut()
    {
        StartCoroutine(ConfirmSignOutRutiner());
    }

    /// <summary>
    /// Coroutine for the confirm sign out process.
    /// </summary>
    /// <returns>An IEnumerator used for coroutine execution.</returns>
    IEnumerator ConfirmSignOutRutiner()
    {
        firebaseApp.SignOut();
        googleAuth.SignOut();

        textButtonLogOut.DOFade(0, 0.5f);
        loadingLogOut.SetActive(true);
        loadingLogOut.GetComponent<Image>().DOFade(1, 0.5f);

        yield return new WaitForSeconds(1);

        while (!firebaseApp.IsSignedOut() && !googleAuth.IsSignedOut())
        {
            yield return new WaitForSeconds(0.1f);
        }

        simpleSpinner.RotationSpeed = -1;
        checkLogOut.enabled = true;
        checkLogOut.transform.DOScale(Vector3.one * 1.3f, 0.3f).OnComplete(() =>
        {
            checkLogOut.transform.DOScale(Vector3.one, 0.1f).OnComplete(() =>
            {
                StartCoroutine(HideDisplaySingOut());
            });
        });
    }

    /// <summary>
    /// Coroutine to hide the sign out display.
    /// </summary>
    /// <returns>An IEnumerator used for coroutine execution.</returns>
    IEnumerator HideDisplaySingOut()
    {
        yield return new WaitForSeconds(.8f);

        displaySingIn.SetActive(true);
        displaySingOut.SetActive(false);
        displayConfirmSingOut.SetActive(false);
        animator.enabled = false;

        MainMenuController mainMenuController = FindObjectOfType<MainMenuController>();
        mainMenuController.CloseUILogin(this.gameObject);

        ResetConfirmLogOut();
    }

    /// <summary>
    /// Resets the confirm sign out state.
    /// </summary>
    void ResetConfirmLogOut()
    {
        textButtonLogOut.alpha = 1;
        loadingLogOut.SetActive(false);
        Image imageLoading = loadingLogOut.GetComponent<Image>();
        imageLoading.color = new Color(imageLoading.color.r, imageLoading.color.g, imageLoading.color.b, 0);

        simpleSpinner.RotationSpeed = 1;
        checkLogOut.enabled = false;
        checkLogOut.transform.localScale = Vector3.zero;
    }

    /// <summary>
    /// Disables the warning message game object.
    /// </summary>
    void DisableWarningMessage()
    {
        warningMessage.SetActive(false);
    }
}