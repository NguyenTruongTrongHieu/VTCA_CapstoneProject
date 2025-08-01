using System.Collections;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    [Header("Coin")]
    public int coins;

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
        StartCoroutine(UIManager.instance.CoinPanelZoomInAndZoomOut());

        coins += amount;
        SaveLoadManager.instance.currentCoin = coins; // Update the current coin count in SaveLoadManager
        UIManager.instance.UpdateCoinText();
    }

    public void SubtractCoins(int amount)
    {
        StartCoroutine(UIManager.instance.CoinPanelZoomInAndZoomOut());

        // Ensure coins do not go below zero
        coins = Mathf.Max(0, coins - amount);
        SaveLoadManager.instance.currentCoin = coins; // Update the current coin count in SaveLoadManager
        UIManager.instance.UpdateCoinText();
    }

    #endregion
}
