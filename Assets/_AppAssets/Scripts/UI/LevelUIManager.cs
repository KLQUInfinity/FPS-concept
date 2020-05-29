using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUIManager : MonoBehaviour
{
    #region Singleton
    public static LevelUIManager Instance { private set; get; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region MainUIMenu
    [Header("MainUIMenu")]

    [SerializeField] private Slider playerHealthSlider;
    [SerializeField] private TextMeshProUGUI playerHealthPercentageTxt;

    public void InitPlayerHealth(int maxValue, int value)
    {
        playerHealthSlider.maxValue = maxValue;
        SetPlayerHealthValue(value);
    }

    public void SetPlayerHealthValue(int value)
    {
        playerHealthSlider.value = value;
        playerHealthPercentageTxt.text = (int)((playerHealthSlider.value / playerHealthSlider.maxValue) * 100) + "%";
    }
    #endregion
}
