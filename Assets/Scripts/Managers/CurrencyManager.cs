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

    public void AddCoinsDontHaveAnim(int amount)
    {
        coins += amount;
        SaveLoadManager.instance.currentCoin = coins; // Update the current coin count in SaveLoadManager
        UIManager.instance.UpdateCoinText();
    }

    public IEnumerator AddCoins(Vector3 startPos, int amount, bool needChangeTransformFromWordToScreen, float waitTimeBeforeAdd, Transform parent)
    {
        if (waitTimeBeforeAdd > 0f)
        { 
            yield return new WaitForSeconds(waitTimeBeforeAdd);
        }

        //Play anim spawn coins
        yield return UIManager.instance.StartCoroutine(UIManager.instance.SpawnCurrencyPrefabAndMoveToPanel(startPos, "coin", amount, needChangeTransformFromWordToScreen, parent));
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("coin", 0.1f));
        AudioManager.instance.PlaySFX("AddCurrency");

        AddCoinsDontHaveAnim(amount); // Update the coin count without animation
    }

    public void SubtractCoins(int amount)
    {
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("coin", 0.1f));

        // Ensure coins do not go below zero
        coins = Mathf.Max(0, coins - amount);
        SaveLoadManager.instance.currentCoin = coins; // Update the current coin count in SaveLoadManager
        UIManager.instance.UpdateCoinText();
    }

    #endregion

    #region UPDATE CRYSTAL

    public IEnumerator AddCrystals(Vector3 startPos, int amount, bool needChangeTransformFromWordToScreen, float waitTimeBeforeAdd, Transform parent)
    {
        if (waitTimeBeforeAdd > 0f)
        {
            yield return new WaitForSeconds(waitTimeBeforeAdd);
        }

        //Play anim spawn crystals
        yield return UIManager.instance.StartCoroutine(UIManager.instance.SpawnCurrencyPrefabAndMoveToPanel(startPos, "crystal", amount, needChangeTransformFromWordToScreen, parent));
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("crystal", 0.1f));
        AudioManager.instance.PlaySFX("AddCurrency");

        AddCrystalsDontHaveAnim(amount); // Update the crystal count without animation
    }

    public void AddCrystalsDontHaveAnim (int amount)
    {
        crystals += amount;
        SaveLoadManager.instance.currentCrystal = crystals; // Update the current crystal count in SaveLoadManager
        UIManager.instance.UpdateCrystalText();
    }

    public void SubtractCrystal(int amount)
    {
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("crystal", 0.1f));

        crystals = Mathf.Max(0, crystals - amount);
        SaveLoadManager.instance.currentCrystal = crystals; 
        UIManager.instance.UpdateCrystalText();
    }
    #endregion

    #region UPDATE STAR

    public IEnumerator AddStars(Vector3 startPos, int amount, bool needChangeTransformFromWordToScreen, float waitTimeBeforeAdd, Transform parent)
    {
        if (waitTimeBeforeAdd > 0f)
        {
            yield return new WaitForSeconds(waitTimeBeforeAdd);
        }

        //Play anim spawn stars
        yield return UIManager.instance.StartCoroutine(UIManager.instance.SpawnCurrencyPrefabAndMoveToPanel(startPos, "star", amount, needChangeTransformFromWordToScreen, parent));
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("star", 0.1f));
        AudioManager.instance.PlaySFX("AddCurrency");

        AddStarsDontHaveAnim(amount); // Update the star count without animation
    }

    public void AddStarsDontHaveAnim(int amount)
    {
        stars += amount;
        SaveLoadManager.instance.currentStar = stars; // Update the current star count in SaveLoadManager
        UIManager.instance.UpdateStarText();
    }

    public void SubtractStar(int amount)
    {
        StartCoroutine(UIManager.instance.CurrencyPanelZoomInAndZoomOut("star", 0.1f));

        // Ensure stars do not go below zero
        stars = Mathf.Max(0, stars - amount);
        SaveLoadManager.instance.currentStar = stars; // Update the current star count in SaveLoadManager
        UIManager.instance.UpdateStarText();
    }
    #endregion
}
