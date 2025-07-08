using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class State : MonoBehaviour
{
    public int xIndex; // chỉ số hàng của ô chứa khối gỗ
    public int yIndex; // chỉ số cột của ô chứa khối gỗ

    //Độ bền khối gỗ
    public int durability = 3;

    public Image image;
    public List<Sprite> stateSprite = new List<Sprite>(); // Danh sách các hình ảnh biểu thị trạng thái của khối gỗ

    public State(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public void SetIndex(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public void DecreaseDurability()
    { 
        durability--;
        image.sprite = stateSprite[durability - 1]; // Cập nhật hình ảnh của khối gỗ theo độ bền
    }
}
