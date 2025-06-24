using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
   public bool isOccupied; // biến kiểm tra xem ô có bị chiếm hay không

    //public GameObject cell; // biến chứa prefab của ô

    public int xIndex; // biến chứa chỉ số hàng của ô
    public int yIndex; // biến chứa chỉ số cột của ô

    public Node(bool _isOccupied)
    {
        isOccupied = _isOccupied;
    }

    public void SetIndex(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }
}
