using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeShop : MonoBehaviour
{
    public static LifeShop Instance;

    public bool WarningText
    {
        get { return warningText.activeSelf; }
        set
        {
            warningText.SetActive(value);
            StartCoroutine(DeactivateWarningText());
        }
    }

    [Header("Life")]
    [SerializeField] TMP_Text livesText;
    [SerializeField] GameObject imageInfiniteLife;

    [Header("Timer")]
    [SerializeField] TMP_Text timerLivesText;

    [Header("Coins")]
    [SerializeField] TMP_Text coinsText;
    [Space]
    [SerializeField] GameObject warningText;

    void OnEnable()
    {
        LifeController.OnInfiniteLife += ChangeImageInfiniteLife;
        imageInfiniteLife.SetActive(LifeController.Instance.IsInfinite ? true : false);
        livesText.enabled = LifeController.Instance.IsInfinite ? false : true;
    }

    void OnDisable()
    {
        LifeController.OnInfiniteLife -= ChangeImageInfiniteLife;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetLifeTimerCoins();
    }

    void Update()
    {
        if ((livesText.text != LifeController.Instance.Lives.ToString())
        || (timerLivesText.text != LifeController.Instance.WaitTime.text)
        || coinsText.text != CoinController.Instance.Coins.ToString())
        {
            SetLifeTimerCoins();
        }
    }

    /// <summary>
    /// Navigates to the coin shop by activating the CoinShop object and disabling the current object.
    /// </summary>
    public void SendCoinShop()
    {
        LevelMenuController levelMenuController = FindObjectOfType<LevelMenuController>(true);
        levelMenuController.OnScreenAdvance(FindObjectOfType<CoinShop>(true).gameObject);
        gameObject.SetActive(false);
        levelMenuController.OffScreen(gameObject);
    }

    // It gives us a life in exchange for seeing an ad
    public void PlusLifeAds()
    {
        AdsManager.Instance.ShowRewardedAd(AdsManager.Instance.RewardedIdLife);
        AdsManager.Instance.LoadRewardedAd(AdsManager.Instance.RewardedIdLife);
    }

    // Method in charge of showing the UI to ask your friends to send us lives
    public void AksFriend()
    {
        Debug.LogWarning("TODO: ASK FRIENDS");
    }

    void SetLifeTimerCoins()
    {
        livesText.text = LifeController.Instance.Lives.ToString();
        timerLivesText.text = LifeController.Instance.WaitTime.text;
        coinsText.text = CoinController.Instance.Coins.ToString();
    }

    // Method in charge of changing the UI when it changes to infinity or not
    void ChangeImageInfiniteLife()
    {
        livesText.enabled = !livesText.isActiveAndEnabled;
        imageInfiniteLife.SetActive(!imageInfiniteLife.activeSelf);
    }

    public IEnumerator DeactivateWarningText()
    {
        yield return new WaitForSeconds(2);
        warningText.SetActive(false);
    }
}