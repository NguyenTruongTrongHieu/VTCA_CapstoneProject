using UnityEngine;
using UnityEngine.UI;

public class UIRewardInChestBox : MonoBehaviour
{
    [Header("Item info UI")]
    public Image itemIcon;
    public Image levelItemImage;
    public Text itemTypeText;

    [Header("Item level sprites")]
    public Sprite greenLevelSprite;
    public Sprite blueLevelSprite;
    public Sprite purpleLevelSprite;
    public Sprite orangeLevelSprite;

    [Header("ItemIcon")]
    public Sprite weaponSprite;
    public Sprite helmetSprite;
    public Sprite armorSprite;
    public Sprite bootsSprite;

    public void SetUI(ItemLevel itemLevel, ItemType itemType)
    { 
        if (itemLevel == ItemLevel.green)
        {
            levelItemImage.sprite = greenLevelSprite;
        }
        else if(itemLevel == ItemLevel.blue)
        {
            levelItemImage.sprite = blueLevelSprite;
        }
        else if (itemLevel == ItemLevel.purple)
        {
            levelItemImage.sprite = purpleLevelSprite;
        }
        else if( itemLevel == ItemLevel.orange)
        {
            levelItemImage.sprite = orangeLevelSprite;
        }    

        if(itemType == ItemType.weapon)
        {
            itemIcon.sprite = weaponSprite;
            itemTypeText.text = "Weapon";
        }
        else if (itemType == ItemType.helmet)
        {
            itemIcon.sprite = helmetSprite;
            itemTypeText.text = "Helmet";
        }
        else if (itemType == ItemType.armor)
        {
            itemIcon.sprite = armorSprite;
            itemTypeText.text = "Armor";
        }
        else if (itemType == ItemType.boots)
        {
            itemIcon.sprite = bootsSprite;
            itemTypeText.text = "Boots";
        }
    }
}
