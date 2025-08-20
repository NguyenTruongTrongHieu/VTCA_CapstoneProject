using UnityEngine;


public enum ItemLevel
{
    white,
    green,
    blue,
    purple,
    orange,
}

public enum ItemType
{
    weapon,
    helmet,
    armor,
    boots
}

public class AllItemManager : MonoBehaviour
{
    //Dùng để lưu toàn bộ item có trong game, phân theo cấp bậc (ItemLevel) và loại (ItemType)
}
