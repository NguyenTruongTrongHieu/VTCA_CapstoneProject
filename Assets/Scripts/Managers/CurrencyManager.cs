using System.Collections;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    [Header("Coin")]
    public int coins;

    [Header("Crystal")]
    public int crystals;

    [Header("Star")]
    public int stars;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        //else
        //{
        //    Destroy(gameObject);
        //}
    }

    #region UPDATE COIN
    public IEnumerator AddCoins(Transform startPos, int amount)
    {
        //Play anim spawn coins
        yield return UIManager.instance.StartCoroutine(UIManager.instance.SpawnCoinPrefabAndMoveToCoinPanel(startPos, amount));
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("coin"));

        coins += amount;
        SaveLoadManager.instance.currentCoin = coins; // Update the current coin count in SaveLoadManager
        UIManager.instance.UpdateCoinText();
    }

    public void SubtractCoins(int amount)
    {
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("coin"));

        // Ensure coins do not go below zero
        coins = Mathf.Max(0, coins - amount);
        SaveLoadManager.instance.currentCoin = coins; // Update the current coin count in SaveLoadManager
        UIManager.instance.UpdateCoinText();
    }

    #endregion

    #region UPDATE CRYSTAL
    public void SubtractCrystal(int amount)
    {
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("crystal"));

        crystals = Mathf.Max(0, crystals - amount);
        SaveLoadManager.instance.currentCrystal = crystals; 
        UIManager.instance.UpdateCrystalText();
    }
    #endregion

    #region UPDATE STAR
    public void SubtractStar(int amount)
    {
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("star"));

        // Ensure stars do not go below zero
        stars = Mathf.Max(0, stars - amount);
        SaveLoadManager.instance.currentStar = stars; // Update the current star count in SaveLoadManager
        UIManager.instance.UpdateStarText();
    }
    #endregion
}
