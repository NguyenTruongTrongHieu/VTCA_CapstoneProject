using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour
{
    // define the size of the board
    [Header("Size Board")]
    public int boardWidth = 6; // chiều rộng của bàn cờ
    public int boardHeight = 8; // chiều cao của bàn cờ

    // defien some spacing between the cells
    public float spacingX; // khoảng cách giữa các ô theo chiều ngang
    public float spacingY; // khoảng cách giữa các ô theo chiều dọc

    [Header("Matching manager")]
    public int numberOfFoodMatching;
    public int multipleScore;

    // get a prafernce  to the cells prefabs
    [Header("Prefabs")]
    public GameObject[] foodPrefab; // prefabs của đồ ăn

    public GameObject cellPrefab; // prefab của ô mặc định

    [Header("Transform")]
    public Transform foodParent; // đối tượng cha chứa các ô thức ăn
    public Transform cellParent;

    // get a reference to the collection nodes gameBoard + GO
    [Header("Game Board")]
    public Node[,] cells; // mảng hai chiều chứa các ô của bàn cờ
    public Food[,] foods;
    public string [,] gameBoard; //"EmptyCell": ô trống; "HavingFood": ô chứa thức ăn; 
    public GameObject gameBoardGO; // GameObject chứa bàn cờ
    private bool interleavedCell = false;
    public Color cellColor1;
    public Color cellColor2;
    //public int boardSize; // kích thước của bàn cờ (số lượng ô)
    //public Node[,] nodes; // danh sách các ô của bàn cờ
    //public List<GameObject> foodList = new List<GameObject>(); // danh sách các ô chứa thức ăn


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
        InitializeBoard2();
        InitializeFood();
    }

    public void InitializeBoard()
    {
        gameBoard = new string[boardWidth, boardHeight]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        cells = new Node[boardWidth, boardHeight]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        foods = new Food[boardWidth, boardHeight]; // khởi tạo mảng hai chiều chứa các ô thức ăn

        //spacingX = 0; // tính toán khoảng cách giữa các ô theo chiều ngang
        //spacingY = 0; // tính toán khoảng cách giữa các ô theo chiều dọc


        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector2 backgroundPosition = new Vector2(0, 0); // vị trí của ô mặc định


                int randomIndex = Random.Range(0, foodPrefab.Length); // chọn ngẫu nhiên prefab của ô
                // tính toán vị trí của ô dựa trên chỉ số hàng và cột
                RectTransform backgroundRectTransform = cellPrefab.GetComponent<RectTransform>(); // lấy RectTransform của prefab ô mặc định\
                
                // điều chỉnh vị trí của ô dựa trên kích thước của prefab
                backgroundPosition.x += (float)((float)backgroundRectTransform.rect.x * 1.15f * (x - spacingX)); // điều chỉnh vị trí theo chiều ngang
                backgroundPosition.y += (float)((float)backgroundRectTransform.rect.y * 1.15f * (y - spacingY)); // điều chỉnh vị trí theo chiều dọc

                // tạo một ô mới từ prefab đã chọn
                GameObject cell = Instantiate(cellPrefab, backgroundPosition, Quaternion.identity, cellParent); // tạo một ô mặc định từ prefab đã chọn
                cell.GetComponent<Node>().SetIndex(x, y); // thiết lập chỉ số hàng và cột của ô
                gameBoard[x, y] = "EmptyCell"; // tạo một ô mới và thêm trạng thái rỗng vào mảng hai chiều
                cells[x, y] = cell.GetComponent<Node>(); // lưu ô vào mảng nodes

                if (y % 2 != 0)
                {
                    if (interleavedCell)
                    {
                        cell.GetComponent<Image>().color = cellColor1;
                        interleavedCell = false; // đặt lại biến interleavedCell về false
                    }
                    else
                    {
                        cell.GetComponent<Image>().color = cellColor2;
                        interleavedCell = true;
                    }
                }
                else
                {
                    if (interleavedCell)
                    {
                        cell.GetComponent<Image>().color = cellColor2;
                        interleavedCell = false; // đặt lại biến interleavedCell về false
                    }
                    else
                    {
                        cell.GetComponent<Image>().color = cellColor1;
                        interleavedCell = true;
                    }
                }
            }
        }
    }

    public void InitializeFood()
    {
        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector2 position = new Vector2(0, 0);


                int randomIndex = Random.Range(0, foodPrefab.Length); // chọn ngẫu nhiên prefab của ô
                // tính toán vị trí của ô dựa trên chỉ số hàng và cột
                RectTransform rectTransform = foodPrefab[randomIndex].GetComponent<RectTransform>(); // lấy RectTransform của prefab ô

                // điều chỉnh vị trí của ô dựa trên kích thước của prefab
                position = cells[x,y].transform.position; // lấy vị trí của ô hiện tại

                // tạo một ô mới từ prefab đã chọn
                GameObject food = Instantiate(foodPrefab[randomIndex], position, Quaternion.identity, foodParent); // tạo một ô mới từ prefab đã chọn
                
                food.GetComponent<Food>().SetIndex(x, y); // thiết lập chỉ số hàng và cột của ô
                gameBoard[x, y] = "HavingFood"; // tạo một ô mới và thêm trạng thái rỗng vào mảng hai chiều
                foods[x, y] = food.GetComponent<Food>(); // lưu ô vào mảng nodes
                cells[x, y].food = food; // lưu food vao node
            }
        }
    }

    public void InitializeBoard2()
    {
        gameBoard = new string[boardWidth, boardHeight]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        cells = new Node[boardWidth, boardHeight]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        foods = new Food[boardWidth, boardHeight]; // khởi tạo mảng hai chiều chứa các ô thức ăn

        //spacingX = 0; // tính toán khoảng cách giữa các ô theo chiều ngang
        //spacingY = 0; // tính toán khoảng cách giữa các ô theo chiều dọc


        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                // tạo một ô mới từ prefab đã chọn
                Transform cell = cellParent.GetChild(6 * x + y); // tạo một ô mặc định từ prefab đã chọn
                cell.GetComponent<Node>().SetIndex(x, y); // thiết lập chỉ số hàng và cột của ô
                gameBoard[x, y] = "EmptyCell"; // tạo một ô mới và thêm trạng thái rỗng vào mảng hai chiều
                cells[x, y] = cell.GetComponent<Node>(); // lưu ô vào mảng nodes

                if (y % 2 != 0)
                {
                    if (interleavedCell)
                    {
                        cell.GetComponent<Image>().color = cellColor1;
                        interleavedCell = false; // đặt lại biến interleavedCell về false
                    }
                    else
                    {
                        cell.GetComponent<Image>().color = cellColor2;
                        interleavedCell = true;
                    }
                }
                else
                {
                    if (interleavedCell)
                    {
                        cell.GetComponent<Image>().color = cellColor2;
                        interleavedCell = false; // đặt lại biến interleavedCell về false
                    }
                    else
                    {
                        cell.GetComponent<Image>().color = cellColor1;
                        interleavedCell = true;
                    }
                }
            }
        }
    }
}
