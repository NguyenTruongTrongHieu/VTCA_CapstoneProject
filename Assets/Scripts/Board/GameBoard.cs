using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    // define the size of the board
    public int boardWidth = 8; // chiều rộng của bàn cờ
    public int boardHeight = 8; // chiều cao của bàn cờ

    // defien some spacing between the cells
    public float spacingX; // khoảng cách giữa các ô theo chiều ngang
    public float spacingY; // khoảng cách giữa các ô theo chiều dọc

    // get a prafernce  to the cells prefabs
    public GameObject[] foodPrefab; // prefabs của đồ ăn
    public GameObject cellPrefab; // prefab của ô mặc định
    public Transform foodParent; // đối tượng cha chứa các ô thức ăn

    // get a reference to the collection nodes gameBoard + GO
    private string[,] gameBoard; // mảng hai chiều chứa các ô của bàn cờ
    public GameObject gameBoardGO; // GameObject chứa bàn cờ
    public int boardSize; // kích thước của bàn cờ (số lượng ô)
    public Node[,] nodes; // danh sách các ô của bàn cờ
    public List<GameObject> foodList = new List<GameObject>(); // danh sách các ô chứa thức ăn


    //layoutArray
    //public ArrayLayout layoutArray; //  kiểu bố cục của bàn cờ
    // public static of gameBoard
    public static GameBoard Instance; // singleton instance of GameBoard


    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeBoard();
        InitializeFood();
    }

    void InitializeBoard()
    {
        gameBoard = new string[boardSize, boardSize]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        nodes = new Node[boardSize, boardSize]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ


        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                Vector2 position = new Vector2(x , y);

                GameObject cell = Instantiate(cellPrefab, position, Quaternion.identity, transform); // tạo một ô mới từ prefab đã chọn
                cell.GetComponent<Node>().SetIndex(x, y); // thiết lập chỉ số hàng và cột của ô
                gameBoard[x, y] = string.Empty; // tạo một ô mới và thêm trạng thái rỗng vào mảng hai chiều
                nodes[x, y] = cell.GetComponent<Node>(); // lưu ô vào mảng nodes
            }
        }
    }

    void InitializeFood()
    {

        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                Vector2 position = nodes[x, y].transform.position;

                int randomIndex = Random.Range(0, foodPrefab.Length); // chọn ngẫu nhiên prefab của ô

                GameObject food = Instantiate(foodPrefab[randomIndex], position, Quaternion.identity, foodParent); // tạo một ô mới từ prefab đã chọn
                food.GetComponent<Food>().SetIndex(x, y); // thiết lập chỉ số hàng và cột của ô
                gameBoard[x, y] = "Occupied"; // tạo một ô mới và thêm trạng thái rỗng vào mảng hai chiều
                foodList.Add(food); // thêm thức ăn vào danh sách thức ăn
            }
        }
    }
}
