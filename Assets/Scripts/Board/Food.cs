using UnityEngine;

public class Food : MonoBehaviour
{
    public FoodType foodType; // loại thức ăn

    public int xIndex; // chỉ số hàng của ô chứa thức ăn
    public int yIndex; // chỉ số cột của ô chứa thức ăn

    public bool isMatched; // biến kiểm tra xem thức ăn có được ăn hay không
    private Vector2 currentPos;
    private Vector2 targetPos;

    public Food(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public void SetIndex(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

}

public enum FoodType
{
    Apple_Pie,
    Apple_Strudel,
    Avocado_Salad,
    Bacon,
    Bagel
}
