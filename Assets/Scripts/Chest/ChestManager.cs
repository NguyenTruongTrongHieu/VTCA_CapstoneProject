using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum ChestType
{
    Common,
    Rare,
    Epic,
    Legendary
}


public class ChestManager : MonoBehaviour
{
    public ChestType chestType;
    [Header("Rewards in chest box after open")]
    public int amountOfItemRewards;
    public List<Item> itemRewardsInChestBox = new List<Item>();//used as item class
    public int crystals;
    public int coins;
    public int stars;

    [Header("All rewards in chest box")]
    [Header("Currency rewards")]

    public int minCrystal;
    public int maxCrystal;
    public int minCoin, maxCoin;
    public int minStar, maxStar;

    [Header("Item rewards")]
    public ItemLevel[] itemLevelsInChest;
    public int[] dropItemRate;
    public ItemType[] itemTypesInChest;

    private void Start()
    {
        //Initialize the chest box with rewards
        SetRewardForChestBox();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt)) // For testing purposes
        {
            Debug.Log("Opening Chest Box");
            OpenChest();
        }
    }

    public void OpenChest()
    {
        if (crystals > 0)
        {
            CurrencyManager.instance.AddCrystalsDontHaveAnim(crystals);
        }
        if (coins > 0)
        {
            CurrencyManager.instance.AddCoinsDontHaveAnim(coins);
        }
        if (stars > 0)
        {
            CurrencyManager.instance.AddStarsDontHaveAnim(stars);
        }

        foreach (var itemReward in itemRewardsInChestBox)
        {
            Debug.Log($"Reward Item: {itemReward.itemLevel} - {itemReward.itemType}");
        }

        SetRewardForChestBox();
    }
    public void SetRewardForChestBox()
    { 
        //Set currency rewards
        if(minCrystal == maxCrystal)
        {
            if (minCrystal != 0)
            {
                crystals = minCrystal;
            }
        }
        else
        {
            crystals = Random.Range(minCrystal, maxCrystal + 1);
        }
        if (minCoin == maxCoin)
        {
            if (minCoin != 0)
            {
                coins = minCoin;
            }
        }
        else
        {
            coins = Random.Range(minCoin, maxCoin + 1);
        }
        if (minStar == maxStar)
        {
            if (minStar != 0)
            {
                stars = minStar;
            }
        }
        else
        {
            stars = Random.Range(minStar, maxStar + 1);
        }


        if (amountOfItemRewards > 0)
        { 
            itemRewardsInChestBox.Clear(); // Clear previous rewards if any
            for(int i = 0; i < amountOfItemRewards; i++)
            {
                RandomItemForChestBox();
            }
        }
    }

    public void RandomItemForChestBox()
    {
        int rate = Random.Range(0, 101);
        for (int i = 0; i < itemLevelsInChest.Length; i++)
        { 
            if(rate <= dropItemRate[i])
            {
                ItemLevel itemLevel = itemLevelsInChest[i];
                int randomItemTypeIndex = Random.Range(0, itemTypesInChest.Length);
                ItemType itemType = itemTypesInChest[randomItemTypeIndex];

                //Add itemLEvel and ItemType to itemRewardsInChestBox
                Item item = new Item(itemLevel, itemType);
                itemRewardsInChestBox.Add(item);
                break;
            }
        }
    }
}
