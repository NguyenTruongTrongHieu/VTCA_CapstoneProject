using System.Collections;
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
    public List<Food> ChoosenFoods = new List<Food>(); // danh sách các ô thức ăn đã chọn để so khớp
    public int numberOfFoodMatching;
    public int multipleScore;

    // get a prafernce  to the cells prefabs
    [Header("Prefabs")]
    public GameObject[] foodPrefab; // prefabs của đồ ăn

    public GameObject cellPrefab; // prefab của ô mặc định

    [Header("Transform")]
    public Transform foodParent; // đối tượng cha chứa các ô thức ăn
    public Transform cellParent;

    [Header("Falling Food")]
    public float duration = 100f; // tốc độ rơi của thức ăn
    private float countDurationTime = 0;
    public float deltaYBetweenTwoCell = 182f;
    public Food firstFallingFood;
    public bool isFirstFallingFood;
    public bool isDoneOneFallingRound;

    public bool startFalling = false;//Test

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            startFalling = true; // Test: bắt đầu rơi thức ăn khi nhấn phím Space
            isDoneOneFallingRound = true;
        }

        if (startFalling)
        {
            if (isDoneOneFallingRound)
            {
                isDoneOneFallingRound = false; // reset isDoneOneFallingRound 
                bool checkIfDoneFallingAllFood = FoodFallDown();// Kiểm tra xem có ô trống để rơi thức ăn hay không

                if (!checkIfDoneFallingAllFood) // nếu không còn ô trống để rơi thức ăn
                {
                    startFalling = false; // dừng rơi thức ăn
                    isDoneOneFallingRound = false;
                    Debug.Log("No more food to fall down.");
                }
            }
            //else
            //{ 
            //    countDurationTime += Time.deltaTime; // tăng thời gian đã trôi qua
            //    if (countDurationTime >= duration)
            //    {
            //        countDurationTime = 0; // reset thời gian đã trôi qua
            //        isDoneOneFallingRound = true; // đặt isDoneOneFallingRound về true để tiếp tục rơi thức ăn
            //    }
            //}
        }
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
                cells[x, y].cellState = "Empty";

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

    //List<Vector2Int>
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
                cells[x, y].cellState = "HavingFood"; // cập nhật trạng thái ô thành có thức ăn
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

    public bool FoodFallDown()//return true nếu có ô trống để rơi thức ăn, return false nếu không còn gì để rơi
    {
        bool result = false; // biến để kiểm tra xem có ô trống để rơi thức ăn hay không

        isFirstFallingFood = true;
        firstFallingFood = null; // reset the first falling food

        for (int x = boardWidth - 1; x >= 0; x--)
        {
            for (int y = boardHeight - 1; y >= 0; y--)
            {
                if (foods[x, y] != null)//Đổi điều kiện dựa trên tình hình game sau này
                    continue;

                // Nếu ô hiện tại không có thức ăn, tìm ô phía trên để rơi xuống
                if (x == 0)
                { 
                    Debug.Log("Spawn food to fall down above position: " + x + ", " + y);
                    result = true; // có ô trống để rơi thức ăn
                    Food foodToMove = AddNewFoodAbove(x, y); // Thêm thức ăn mới vào ô trên cùng
                    Vector2 currentFoodPos = foodToMove.transform.position;

                    if (isFirstFallingFood)
                    {
                        isFirstFallingFood = false; // Đặt isFirstFallingFood về false sau khi đã lấy thức ăn đầu tiên
                        firstFallingFood = foodToMove; // Lưu thức ăn đầu tiên để xử lý sau
                    }

                    StartCoroutine(MoveFoodToNode(currentFoodPos, x, y, foodToMove)); // Di chuyển thức ăn xuống ô hiện tại

                    continue;
                }

                if (foods[x - 1, y] != null)
                {
                    result = true; // có ô trống để rơi thức ăn
                    Food foodToMove = DeleteFoodAtPos(x-1, y);

                    if (isFirstFallingFood)
                    { 
                        isFirstFallingFood = false; // Đặt isFirstFallingFood về false sau khi đã lấy thức ăn đầu tiên
                        firstFallingFood = foodToMove; // Lưu thức ăn đầu tiên để xử lý sau
                    }

                    StartCoroutine(MoveFoodToNode(cells[x - 1, y].transform.position, x, y, foodToMove)); // Di chuyển thức ăn xuống ô hiện tại
                }
                else if (y < 5 && foods[x - 1, y + 1] != null)
                {
                    result = true; // có ô trống để rơi thức ăn
                    Food foodToMove = DeleteFoodAtPos(x - 1, y + 1);

                    if (isFirstFallingFood)
                    {
                        isFirstFallingFood = false; // Đặt isFirstFallingFood về false sau khi đã lấy thức ăn đầu tiên
                        firstFallingFood = foodToMove; // Lưu thức ăn đầu tiên để xử lý sau
                    }

                    StartCoroutine(MoveFoodToNode(cells[x - 1, y + 1].transform.position, x, y, foodToMove)); // Di chuyển thức ăn xuống ô hiện tại
                }
                else if (y > 0 && foods[x - 1, y - 1] != null)
                {
                    result = true; // có ô trống để rơi thức ăn
                    Food foodToMove = DeleteFoodAtPos(x - 1, y - 1);

                    if (isFirstFallingFood)
                    {
                        isFirstFallingFood = false; // Đặt isFirstFallingFood về false sau khi đã lấy thức ăn đầu tiên
                        firstFallingFood = foodToMove; // Lưu thức ăn đầu tiên để xử lý sau
                    }

                    StartCoroutine(MoveFoodToNode(cells[x - 1, y - 1].transform.position, x, y, foodToMove)); // Di chuyển thức ăn xuống ô hiện tại
                }
                else
                    continue;
            }
        }

        return result;
    }

    public Food AddNewFoodAbove(int x, int y)
    { 
        int randomIndex = Random.Range(0, foodPrefab.Length); // chọn ngẫu nhiên prefab của ô
        Vector2 position = new Vector2(cells[x, y].transform.position.x, cells[x, y].transform.position.y + deltaYBetweenTwoCell); // vị trí của ô mới

        GameObject food = Instantiate(foodPrefab[randomIndex], position, Quaternion.identity, foodParent); // tạo một ô mới từ prefab đã chọn
        return food.GetComponent<Food>();
    }

    public IEnumerator MoveFoodToNode(Vector2 startPos, int targetX, int targetY, Food foodToMove)
    {
        foodToMove.isFalling = true;
        AddFoodAtPos(targetX, targetY, foodToMove); // thêm thức ăn vào ô đích

        Transform targetCell = cells[targetX, targetY].transform; // lấy ô đích
        float elapsedTime = 0f; // thời gian đã trôi qua
        //Di chuyển food đến targetCell
        while (elapsedTime < duration)
        {
            //Dùng Vector2.Lerp để di chuyển food đến ô đích
            float t = elapsedTime / duration;
            foodToMove.transform.position = Vector2.Lerp(startPos, targetCell.position, t);
            elapsedTime += Time.deltaTime; // tăng thời gian đã trôi qua

            yield return null;
        }
        foodToMove.transform.position = targetCell.position; // đảm bảo thức ăn dừng đúng vị trí ô đích

        //Xử lý anim cho food khi ô dưới có food khác
        if (targetX == boardHeight - 1 || cells[targetX + 1, targetY].cellState == "HavingFood")
        {
            foodToMove.StartCoroutine(foodToMove.FallAnim());
        }

        if (firstFallingFood == foodToMove)
        { 
            Debug.Log("First falling food: " + foodToMove.gameObject.name);
            isDoneOneFallingRound = true; // Đặt isDoneOneFallingRound về true khi đã xử lý thức ăn đầu tiên
        }
        foodToMove.isFalling = false;
    }

    public Food DeleteFoodAtPos(int x, int y)
    {
        gameBoard[x, y] = "EmptyCell"; // cập nhật trạng thái ô thành rỗng
        cells[x, y].cellState = "Empty"; // cập nhật trạng thái ô thành rỗng
        cells[x, y].food = null; // xóa thức ăn khỏi ô
        Food result = foods[x, y]; // lưu thức ăn đã xóa vào biến result
        foods[x, y] = null; // xóa thức ăn khỏi mảng foods

        return result;
    }

    public void AddFoodAtPos(int x, int y, Food food)
    { 
        food.SetIndex(x, y); // thiết lập chỉ số hàng và cột của thức ăn
        gameBoard[x, y] = "HavingFood"; // cập nhật trạng thái ô thành có thức ăn
        cells[x, y].cellState = "HavingFood"; // cập nhật trạng thái ô thành có thức ăn
        cells[x, y].food = food.gameObject; // lưu thức ăn vào ô
        foods[x, y] = food; // lưu thức ăn vào mảng foods
    }
}
