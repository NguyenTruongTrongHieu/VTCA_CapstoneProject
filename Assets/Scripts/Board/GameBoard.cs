using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Accessibility;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameBoard : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    // define the size of the board
    [Header("Size Board")]
    public int boardWidth = 6; // chiều rộng của bàn cờ
    public int boardHeight = 8; // chiều cao của bàn cờ

    // defien some spacing between the cells
    public float spacingX; // khoảng cách giữa các ô theo chiều ngang
    public float spacingY; // khoảng cách giữa các ô theo chiều dọc

    [Header("Matching manager")]
    public List<Food> hasMatchedFoods = new List<Food>(); // danh sách các ô thức ăn đã được so khớp

    public string specialFoodType; // loại món ăn đặc biệt, "": không có special, "Multiple": loại nhân dam đòn đánh, "DebuffTakeDam": Loại tăng sát thương nhận vào của enemy
    public int numberOfFoodMatching;
    public int multipleScore;
    public int numberToCheckSpecialFood; //  dùng để kiểm tra món đặt biệt
    public bool isCheckSpecialFood; // biến kiểm tra xem có cần kiểm tra món đặc biệt hay không

    private int[] foodCount = { 4, 7, 10, 13, 16, 19, 21 }; // mảng đếm số lượng món đặc biệt theo loại
    private int[] specialMutiplie = { 3, 5, 7, 9, 11, 13, 15 }; // mảng nhân điểm theo loại món đặc biệt

    public bool onDeleteFood; // biến kiểm tra xem có đang xoá thức ăn hay không

    // get a prafernce  to the cells prefabs
    [Header("Prefabs")]
    public GameObject[] foodPrefab; // prefabs của đồ ăn
    public GameObject specialFoodPrefab;
    public GameObject debuffTakeDamSpecialFoodPrefab;
    public GameObject statePrefab; // prefab của khối gỗ
    public GameObject cellPrefab; // prefab của ô mặc định
    public GameObject lockCellPrefab;

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
    public List<Food> foodList = new List<Food>();
    public State[,] states2DArray; // mảng hai chiều chứa các khối gỗ của bàn cờ
    public GameObject[,] lockedCells2DArray;

    public string[,] gameBoard; //"EmptyCell": ô trống; "HavingFood": ô chứa thức ăn; "LockedCell": ô bị khoá; "HavingState": ô chứa khối gỗ
    public GameObject gameBoardGO; // GameObject chứa bàn cờ
    private bool interleavedCell = false;
    public Color cellColor1;
    public Color cellColor2;
    //public int boardSize; // kích thước của bàn cờ (số lượng ô)
    //public Node[,] nodes; // danh sách các ô của bàn cờ
    //public List<GameObject> foodList = new List<GameObject>(); // danh sách các ô chứa thức ăn

    [Header("Line Settings")]
    //[SerializeField] private RectTransform[] foodPoints; // các điểm thức ăn để vẽ đường nối
    //[SerializeField] private LineController lineController; // đối tượng điều khiển đường nối
    [SerializeField] private UILineRenderer uiLineRenderer; // đối tượng điều khiển đường nối UI

    [Header("Tutorial")]
    [SerializeField] private bool isTutorial; // biến kiểm tra xem có đang trong chế độ hướng dẫn hay không

    public bool GetTutorial()
    { 
        return isTutorial;
    }

    [SerializeField] private int guideStep = 1; // bước hiện tại trong chế độ hướng dẫn

    public void ResetGuideStep()
    {
        guideStep = 1;
    }

    public int GetGuideStep()
    {
        return guideStep;
    }

    private bool isPlayAnimGuideStep = false; // biến kiểm tra xem có đang chơi hoạt ảnh hướng dẫn hay không
    public List<Vector2Int> compulsoryStep1;
    public List<Vector2Int> compulsoryStep2;
    public List<Vector2Int> compulsoryStep3;

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
        InitializeFood(LevelManager.instance.currentLevel.statesInBoard, LevelManager.instance.currentLevel.lockCellInBoard);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            startFalling = true; // Test: bắt đầu rơi thức ăn khi nhấn phím Space
            isDoneOneFallingRound = true;
        }

        if (Input.GetKeyDown(KeyCode.End))
        {
            if (CheckIfNoFoodCanMatch())
            {
                Debug.Log("No food can match, reset board.");
                ShuffleBoard();
            }
            else
            {
                Debug.Log("There are still food can match, continue game.");
            }
            ShuffleBoard();
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

                    if (CheckIfNoFoodCanMatch())
                    {
                        Debug.Log("No food can match, reset board.");
                        ShuffleBoard();
                    }
                    else
                    {
                        Debug.Log("There are still food can match, continue game.");
                    }

                    //Debug.Log("No more food to fall down.");
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

    #region INITIALIZE BOARD AND FOOD
    public void InitializeBoard()
    {
        gameBoard = new string[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        cells = new Node[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        foods = new Food[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các ô thức ăn
        states2DArray = new State[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các khối gỗ của bàn cờ

        //spacingX = 0; // tính toán khoảng cách giữa các ô theo chiều ngang
        //spacingY = 0; // tính toán khoảng cách giữa các ô theo chiều dọc


        for (int y = 0; y < boardHeight; y++)
        {
            for (int x = 0; x < boardWidth; x++)
            {
                Vector2 backgroundPosition = new Vector2(0, 0); // vị trí của ô mặc định


                int randomIndex = UnityEngine.Random.Range(0, foodPrefab.Length); // chọn ngẫu nhiên prefab của ô
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
    public void InitializeFood(List<Vector2Int> states, List<Vector2Int> lockCell)
    {
        for (int y = 0; y < boardWidth; y++)
        {
            for (int x = 0; x < boardHeight; x++)
            {
                var findLockCell = lockCell.FindIndex(xy => xy.x == x && xy.y == y); // tìm trạng thái của ô hiện tại trong danh sách lockCell
                if (findLockCell >= 0)
                {
                    Vector2 pos = cells[x, y].transform.position; // lấy vị trí của ô hiện tại

                    //GameObject lockCellObj = Instantiate(lockCellPrefab, pos, Quaternion.identity, foodParent);
                    GameObject lockCellObj = PoolManager.Instance.GetObject
                        ($"LockCell",
                        pos, Quaternion.identity, foodParent, lockCellPrefab);
                    lockCellObj.GetComponent<RectTransform>().localScale = Vector3.one;

                    // nếu ô hiện tại là ô bị khóa, không tạo thức ăn
                    gameBoard[x, y] = "LockedCell"; // cập nhật trạng thái ô thành bị khóa
                    cells[x, y].cellState = "LockedCell"; // cập nhật trạng thái ô thành bị khóa
                    foods[x, y] = null; // không có thức ăn trong ô bị khóa
                    lockedCells2DArray[x, y] = lockCellObj; // lưu ô bị khóa vào mảng lockedCells
                    continue; // bỏ qua việc tạo thức ăn cho ô này
                }

                var findStateCell = states.FindIndex(xy => xy.x == x && xy.y == y); // tìm trạng thái của ô hiện tại trong danh sách states
                if (findStateCell >= 0)
                {
                    Vector2 pos = new Vector2(0, 0);
                    pos = cells[x, y].transform.position;

                    //GameObject state = Instantiate(statePrefab, pos, Quaternion.identity, foodParent); // tạo một khối gỗ mới từ prefab đã chọn
                    GameObject state = PoolManager.Instance.GetObject
                        ($"State",
                        pos, Quaternion.identity, foodParent, statePrefab);

                    state.GetComponent<State>().SetIndex(x, y); // thiết lập chỉ số hàng và cột của khối gỗ
                    gameBoard[x, y] = "HavingState"; // cập nhật trạng thái ô thành có khối gỗ
                    cells[x, y].cellState = "HavingState"; // cập nhật trạng thái ô thành có khối gỗ
                    foods[x, y] = null; // không có thức ăn trong ô có khối gỗ
                    states2DArray[x, y] = state.GetComponent<State>(); // lưu khối gỗ vào mảng states
                    continue; // bỏ qua việc tạo thức ăn cho ô này
                }

                GameObject food = null;
                Vector2 position = new Vector2(0, 0);

                if (isTutorial)
                {
                    bool needToRandom = true;
                    (food, needToRandom) = InitializzeFoodNotRandom(x, y);
                    if (needToRandom)
                    {
                        int randomIndex = UnityEngine.Random.Range(1, foodPrefab.Length - 1); // chọn ngẫu nhiên prefab của ô
                                                                                          // tính toán vị trí của ô dựa trên chỉ số hàng và cột
                        RectTransform rectTransform = foodPrefab[randomIndex].GetComponent<RectTransform>(); // lấy RectTransform của prefab ô

                        // điều chỉnh vị trí của ô dựa trên kích thước của prefab
                        position = cells[x, y].transform.position; // lấy vị trí của ô hiện tại

                        // tạo một ô mới từ prefab đã chọn
                        //GameObject food = Instantiate(foodPrefab[randomIndex], position, Quaternion.identity, foodParent); // tạo một ô mới từ prefab đã chọn
                        food = PoolManager.Instance.GetObject
                            ($"Food {randomIndex}",
                            position, Quaternion.identity, foodParent, foodPrefab[randomIndex]);
                    }
                }
                else
                {

                    int randomIndex = UnityEngine.Random.Range(0, foodPrefab.Length); // chọn ngẫu nhiên prefab của ô
                                                                                      // tính toán vị trí của ô dựa trên chỉ số hàng và cột
                    RectTransform rectTransform = foodPrefab[randomIndex].GetComponent<RectTransform>(); // lấy RectTransform của prefab ô

                    // điều chỉnh vị trí của ô dựa trên kích thước của prefab
                    position = cells[x, y].transform.position; // lấy vị trí của ô hiện tại

                    // tạo một ô mới từ prefab đã chọn
                    //GameObject food = Instantiate(foodPrefab[randomIndex], position, Quaternion.identity, foodParent); // tạo một ô mới từ prefab đã chọn
                    food = PoolManager.Instance.GetObject
                        ($"Food {randomIndex}",
                        position, Quaternion.identity, foodParent, foodPrefab[randomIndex]);

                }

                food.GetComponent<Food>().SetIndex(x, y); // thiết lập chỉ số hàng và cột của ô
                gameBoard[x, y] = "HavingFood"; // tạo một ô mới và thêm trạng thái rỗng vào mảng hai chiều
                foods[x, y] = food.GetComponent<Food>(); // lưu ô vào mảng nodes
                cells[x, y].food = food; // lưu food vao node
                foodList.Add(food.GetComponent<Food>());
                cells[x, y].cellState = "HavingFood"; // cập nhật trạng thái ô thành có thức ăn
            }
        }

        if (CheckIfNoFoodCanMatch())
        {
            ShuffleBoard();
        }
    }

    public (GameObject, bool) InitializzeFoodNotRandom(int xIndex, int yIndex)
    {
        bool needToRandom = false;

        GameObject food = null;
        Vector2 position = cells[xIndex, yIndex].transform.position; // vị trí của ô mới

        if (yIndex == 3)
        {
            if (xIndex == 1)
            {
                food = PoolManager.Instance.GetObject
                    ($"Food 0",
                    position, Quaternion.identity, foodParent, foodPrefab[0]);
            }
            else if (xIndex == 4)
            {
                food = PoolManager.Instance.GetObject
                    ($"Food 4",
                    position, Quaternion.identity, foodParent, foodPrefab[4]);
            }
            else if (xIndex == 5)
            {
                food = PoolManager.Instance.GetObject
                    ($"Food 4",
                    position, Quaternion.identity, foodParent, foodPrefab[4]);
            }
            else
            {
                needToRandom = true;
            }
        }
        else if (yIndex == 4)
        {
            if (xIndex == 2)
            {
                food = PoolManager.Instance.GetObject
                    ($"Food 0",
                    position, Quaternion.identity, foodParent, foodPrefab[0]);
            }
            else if (xIndex == 4)
            {
                food = PoolManager.Instance.GetObject
                    ($"Food 4",
                    position, Quaternion.identity, foodParent, foodPrefab[4]);
            }
            else if (xIndex == 5)
            {
                food = PoolManager.Instance.GetObject
                    ($"Food 4",
                    position, Quaternion.identity, foodParent, foodPrefab[4]);
            }
            else
            { 
                needToRandom = true;
            }
        }
        else
        {
            needToRandom = true;
        }

        return (food, needToRandom);
    }

    public void InitializeBoard2()
    {
        gameBoard = new string[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        cells = new Node[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các ô của bàn cờ
        foods = new Food[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các ô thức ăn
        states2DArray = new State[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các khối gỗ của bàn cờ
        lockedCells2DArray = new GameObject[boardHeight, boardWidth]; // khởi tạo mảng hai chiều chứa các ô bị khoá của bàn cờ

        //spacingX = 0; // tính toán khoảng cách giữa các ô theo chiều ngang
        //spacingY = 0; // tính toán khoảng cách giữa các ô theo chiều dọc


        for (int y = 0; y < boardWidth; y++)
        {
            for (int x = 0; x < boardHeight; x++)
            {
                // tạo một ô mới từ prefab đã chọn
                Transform cell = cellParent.GetChild(boardWidth * x + y); // tạo một ô mặc định từ prefab đã chọn
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

    public void ResetBoard()
    {
        for (int y = 0; y < boardWidth; y++)
        {
            for (int x = 0; x < boardHeight; x++)
            {
                if (gameBoard[x, y] == "EmptyCell")
                    continue;

                if (gameBoard[x, y] == "LockedCell")
                {
                    PoolManager.Instance.ReturnObject("LockCell", lockedCells2DArray[x, y]); // xóa ô bị khoá khỏi mảng lockedCells

                    gameBoard[x, y] = "EmptyCell";
                    cells[x, y].cellState = "Empty";
                }
                else if (gameBoard[x, y] == "HavingState")
                {
                    PoolManager.Instance.ReturnObject("State", states2DArray[x, y].gameObject); // xóa khối gỗ khỏi mảng states

                    gameBoard[x, y] = "EmptyCell";
                    cells[x, y].cellState = "Empty";
                    //cells[x, y].food = null; // xóa khối gỗ khỏi ô
                }
                else if (gameBoard[x, y] == "HavingFood")
                {
                    Food food = DeleteFoodAtPos(x, y); // xóa thức ăn khỏi ô
                    //Destroy(food.gameObject); // hủy đối tượng thức ăn
                    food.ReturnFoodToPool(food);
                }
            }
        }

        //Delete all child in foodParent
        //foreach (Transform child in foodParent)
        //{
        //    Destroy(child.gameObject); // hủy tất cả các đối tượng con trong foodParent
        //}
    }

    #endregion


    #region FALLING FOOD
    public bool FoodFallDown()//return true nếu có ô trống để rơi thức ăn, return false nếu không còn gì để rơi
    {
        bool result = false; // biến để kiểm tra xem có ô trống để rơi thức ăn hay không

        isFirstFallingFood = true;
        firstFallingFood = null; // reset the first falling food

        for (int y = boardWidth - 1; y >= 0; y--)
        {
            for (int x = boardHeight - 1; x >= 0; x--)
            {
                if (gameBoard[x, y] != "EmptyCell")//Đổi điều kiện dựa trên tình hình game sau này
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
                    Food foodToMove = DeleteFoodAtPos(x - 1, y);

                    if (isFirstFallingFood)
                    {
                        isFirstFallingFood = false; // Đặt isFirstFallingFood về false sau khi đã lấy thức ăn đầu tiên
                        firstFallingFood = foodToMove; // Lưu thức ăn đầu tiên để xử lý sau
                    }

                    StartCoroutine(MoveFoodToNode(cells[x - 1, y].transform.position, x, y, foodToMove)); // Di chuyển thức ăn xuống ô hiện tại
                }
                else if (y < boardWidth - 1 && foods[x - 1, y + 1] != null)
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
        int randomIndex = UnityEngine.Random.Range(0, foodPrefab.Length); // chọn ngẫu nhiên prefab của ô
        Vector2 position = new Vector2(cells[x, y].transform.position.x, cells[x, y].transform.position.y + deltaYBetweenTwoCell); // vị trí của ô mới

        //GameObject food = Instantiate(foodPrefab[randomIndex], position, Quaternion.identity, foodParent); // tạo một ô mới từ prefab đã chọn
        GameObject food = PoolManager.Instance.GetObject
            ($"Food {randomIndex}",
            position, Quaternion.identity, foodParent, foodPrefab[randomIndex]);

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
        foodList.Remove(result); // xóa thức ăn khỏi danh sách thức ăn

        return result;
    }

    public void AddFoodAtPos(int x, int y, Food food)
    {
        food.SetIndex(x, y); // thiết lập chỉ số hàng và cột của thức ăn
        gameBoard[x, y] = "HavingFood"; // cập nhật trạng thái ô thành có thức ăn
        cells[x, y].cellState = "HavingFood"; // cập nhật trạng thái ô thành có thức ăn
        cells[x, y].food = food.gameObject; // lưu thức ăn vào ô
        foods[x, y] = food; // lưu thức ăn vào mảng foods
        foodList.Add(food); // thêm thức ăn vào danh sách thức ăn
    }

    public void AddStateAtPos(int x, int y, State state)
    {
        state.SetIndex(x, y); // thiết lập chỉ số hàng và cột của khối gỗ
        gameBoard[x, y] = "HavingState"; // cập nhật trạng thái ô thành có khối gỗ
        cells[x, y].cellState = "HavingState"; // cập nhật trạng thái ô thành có khối gỗ
        foods[x, y] = null; // không có thức ăn trong ô có khối gỗ
        states2DArray[x, y] = state; // lưu khối gỗ vào mảng states
    }

    public State DeleteStateAtPos(int x, int y)
    {
        gameBoard[x, y] = "EmptyCell";
        cells[x, y].cellState = "Empty";
        cells[x, y].food = null;
        State result = states2DArray[x, y];
        foods[x, y] = null;
        states2DArray[x, y] = null; // xóa khối gỗ khỏi mảng states

        return result;
    }

    #endregion

    #region CREATE THE WAY FOR PLAYER

    public bool CheckIfNoFoodCanMatch()//True: không còn thức ăn nào có thể so khớp, False: còn thức ăn có thể so khớp
    {
        bool result = true;

        for (int y = 0; y < boardWidth; y++)
        {
            for (int x = 0; x < boardHeight; x++)
            {
                if (gameBoard[x, y] != "HavingFood")
                    continue;

                //Nếu ô thức ăn là ô đặc biệt, kiểm tra xem có ô nào có thể sử dụng được không
                //Nếu có ô nào có thể sử dụng được, thì trả về false
                if (gameBoard[x, y] == "HavingFood" && foods[x, y].foodType == FoodType.Special &&
                    CheckEnableUsedFood(new Vector2Int(x, y), new List<Vector2Int>()).Count > 0)
                {
                    result = false;
                    Debug.Log("There is a special food can be used, continue game.");
                    break;
                }

                if (y > 0 && gameBoard[x, y - 1] == "HavingFood" && foods[x, y - 1].foodType == foods[x, y].foodType)
                {
                    result = false;
                    break;
                }

                if (y < boardWidth - 1 && gameBoard[x, y + 1] == "HavingFood" && foods[x, y + 1].foodType == foods[x, y].foodType)
                {
                    result = false;
                    break;
                }

                if (x > 0)
                {
                    if (gameBoard[x - 1, y] == "HavingFood" && foods[x - 1, y].foodType == foods[x, y].foodType)
                    {
                        result = false;
                        break;
                    }
                    else if (y > 0 && gameBoard[x - 1, y - 1] == "HavingFood" && foods[x - 1, y - 1].foodType == foods[x, y].foodType)
                    {
                        result = false;
                        break;
                    }
                    else if (y < boardWidth - 1 && gameBoard[x - 1, y + 1] == "HavingFood" && foods[x - 1, y + 1].foodType == foods[x, y].foodType)
                    {
                        result = false;
                        break;
                    }
                }

                if (x < boardHeight - 1)
                {
                    if (gameBoard[x + 1, y] == "HavingFood" && foods[x + 1, y].foodType == foods[x, y].foodType)
                    {
                        result = false;
                        break;
                    }
                    else if (y > 0 && gameBoard[x + 1, y - 1] == "HavingFood" && foods[x + 1, y - 1].foodType == foods[x, y].foodType)
                    {
                        result = false;
                        break;
                    }
                    else if (y < 5 && gameBoard[x + 1, y + 1] == "HavingFood" && foods[x + 1, y + 1].foodType == foods[x, y].foodType)
                    {
                        result = false;
                        break;
                    }
                }
            }
        }

        return result;
    }

    public List<Vector2Int> CheckEnableUsedFood(Vector2Int center, List<Vector2Int> disableFoods)
    {
        List<Vector2Int> enableFoods = new List<Vector2Int>();

        if (gameBoard[center.x, center.y] != "HavingFood")
            return enableFoods;

        if (center.y > 0 && gameBoard[center.x, center.y - 1] == "HavingFood" && !disableFoods.Contains(new Vector2Int(center.x, center.y - 1)))
        {
            enableFoods.Add(new Vector2Int(center.x, center.y - 1)); // thêm ô thức ăn phía trên vào danh sách có thể sử dụng
        }

        if (center.y < boardWidth - 1 && gameBoard[center.x, center.y + 1] == "HavingFood" && !disableFoods.Contains(new Vector2Int(center.x, center.y + 1)))
        {
            enableFoods.Add(new Vector2Int(center.x, center.y + 1)); // thêm ô thức ăn phía dưới vào danh sách có thể sử dụng
        }

        if (center.x > 0)
        {
            if (gameBoard[center.x - 1, center.y] == "HavingFood" && !disableFoods.Contains(new Vector2Int(center.x - 1, center.y)))
            {
                enableFoods.Add(new Vector2Int(center.x - 1, center.y));
            }
            if (center.y > 0 && gameBoard[center.x - 1, center.y - 1] == "HavingFood" && !disableFoods.Contains(new Vector2Int(center.x - 1, center.y - 1)))
            {
                enableFoods.Add(new Vector2Int(center.x - 1, center.y - 1));
            }
            if (center.y < boardWidth - 1 && gameBoard[center.x - 1, center.y + 1] == "HavingFood" && !disableFoods.Contains(new Vector2Int(center.x - 1, center.y + 1)))
            {
                enableFoods.Add(new Vector2Int(center.x - 1, center.y + 1));
            }
        }

        if (center.x < boardHeight - 1)
        {
            if (gameBoard[center.x + 1, center.y] == "HavingFood" && !disableFoods.Contains(new Vector2Int(center.x + 1, center.y)))
            {
                enableFoods.Add(new Vector2Int(center.x + 1, center.y));
            }
            if (center.y > 0 && gameBoard[center.x + 1, center.y - 1] == "HavingFood" && !disableFoods.Contains(new Vector2Int(center.x + 1, center.y - 1)))
            {
                enableFoods.Add(new Vector2Int(center.x + 1, center.y - 1));
            }
            if (center.y < boardWidth - 1 && gameBoard[center.x + 1, center.y + 1] == "HavingFood" && !disableFoods.Contains(new Vector2Int(center.x + 1, center.y + 1)))
            {
                enableFoods.Add(new Vector2Int(center.x + 1, center.y + 1));
            }
        }

        return enableFoods;
    }

    public void ShuffleBoard()
    {
        Vector2Int center = new Vector2Int(-1, -1);
        List<Vector2Int> disableFoods = new List<Vector2Int>(); // danh sách các ô thức ăn không thể sử dụng
        List<Vector2Int> enableFoods = new List<Vector2Int>(); // danh sách các ô thức ăn có thể sử dụng

        //Tìm 1 ô có thể dùng làm mốc để xáo lần 1
        for (int x = boardHeight - 1; x >= 0; x--)
        {
            int y = (boardWidth - 1) / 2;

            if (gameBoard[x, y] == "HavingFood" && foods[x, y].foodType != FoodType.Special)
            {
                center = new Vector2Int(x, y); // lưu vị trí của ô thức ăn ở giữa

                disableFoods = new List<Vector2Int> {
                            new Vector2Int(x, y)
                        };
                enableFoods = CheckEnableUsedFood(new Vector2Int(x, y), new List<Vector2Int>()); // lấy danh sách các ô thức ăn có thể sử dụng

                if (enableFoods.Count >= 2)
                    break;
            }

            for (int i = y + 1; i < boardWidth; i++)
            {
                if (gameBoard[x, i] == "HavingFood" && foods[x, i].foodType != FoodType.Special)
                {
                    center = new Vector2Int(x, i);

                    disableFoods = new List<Vector2Int> {
                            new Vector2Int(x, i)
                        };
                    enableFoods = CheckEnableUsedFood(new Vector2Int(x, i), new List<Vector2Int>()); // lấy danh sách các ô thức ăn có thể sử dụng

                    if (enableFoods.Count >= 2)
                        break;
                    else
                        continue;
                }
            }

            if (enableFoods.Count < 2)
            {
                for (int j = y - 1; j >= 0; j--)
                {
                    if (gameBoard[x, j] == "HavingFood" && foods[x, j].foodType != FoodType.Special)
                    {
                        center = new Vector2Int(x, j);

                        disableFoods = new List<Vector2Int> {
                            new Vector2Int(x, j)
                        };
                        enableFoods = CheckEnableUsedFood(new Vector2Int(x, j), new List<Vector2Int>()); // lấy danh sách các ô thức ăn có thể sử dụng

                        if (enableFoods.Count >= 2)
                            break;
                        else
                            continue;
                    }
                }
            }

            if (enableFoods.Count >= 2)
                break;
        }


        List<Vector2Int> alreadyShuffleFood = new List<Vector2Int>(); // danh sách các ô thức ăn đã được xáo
        //Kiểm tra nếu có ô có thể sử dụng làm mốc để xáo lần 1, tiến hành xáo lần 1
        if (enableFoods.Count >= 2)
        {
            enableFoods = enableFoods.OrderBy(x => UnityEngine.Random.value).ToList(); // xáo trộn danh sách các ô thức ăn có thể sử dụng
            Vector2Int firstRandomPos = enableFoods[0];
            Vector2Int secondRandomPos = enableFoods[1];
            Debug.Log("Center position: " + center + ", First random position: " + firstRandomPos + ", Second random position: " + secondRandomPos);
            alreadyShuffleFood.Add(center);
            alreadyShuffleFood.Add(firstRandomPos);
            alreadyShuffleFood.Add(secondRandomPos);

            GameObject newFoodPrefab = null;
            foreach (var foodObject in foodPrefab)
            {
                if (foodObject.GetComponent<Food>().foodType == foods[center.x, center.y].foodType)
                {
                    newFoodPrefab = foodObject; // tìm prefab thức ăn có cùng loại với thức ăn ở ô giữa
                    break;
                }
            }

            Food firstOldFood = DeleteFoodAtPos(firstRandomPos.x, firstRandomPos.y); // xóa thức ăn ở ô đầu tiên
            Food secondOldFood = DeleteFoodAtPos(secondRandomPos.x, secondRandomPos.y); // xóa thức ăn ở ô thứ hai
            if (newFoodPrefab != null)
            {
                GameObject firstNewFoodObject = Instantiate(newFoodPrefab, cells[firstRandomPos.x, firstRandomPos.y].transform.position, Quaternion.identity, foodParent);
                AddFoodAtPos(firstRandomPos.x, firstRandomPos.y, firstNewFoodObject.GetComponent<Food>());
                GameObject secondNewFoodObject = Instantiate(newFoodPrefab, cells[secondRandomPos.x, secondRandomPos.y].transform.position, Quaternion.identity, foodParent);
                AddFoodAtPos(secondRandomPos.x, secondRandomPos.y, secondNewFoodObject.GetComponent<Food>());
            }
            else
            {
                Debug.LogError("No food prefab found with the same type as the food at the center position.");
            }

            //Destroy(firstOldFood.gameObject); // hủy đối tượng thức ăn cũ
            //Destroy(secondOldFood.gameObject); // hủy đối tượng thức ăn cũ
            firstOldFood.ReturnFoodToPool(firstOldFood);
            secondOldFood.ReturnFoodToPool(secondOldFood);
        }

        //Tiến hành xáo lần 2
        int lowIndex = 0, highIndex = foodList.Count - 1;
        while (lowIndex < highIndex)
        {
            Vector2Int lowPos = new Vector2Int(foodList[lowIndex].xIndex, foodList[lowIndex].yIndex);
            Vector2Int highPos = new Vector2Int(foodList[highIndex].xIndex, foodList[highIndex].yIndex);

            while (alreadyShuffleFood.Contains(lowPos))
            {
                lowIndex++;
                if (lowIndex >= highIndex)
                    break;
                lowPos = new Vector2Int(foodList[lowIndex].xIndex, foodList[lowIndex].yIndex);
            }

            while (alreadyShuffleFood.Contains(highPos))
            {
                highIndex--;
                if (lowIndex >= highIndex)
                    break;
                highPos = new Vector2Int(foodList[highIndex].xIndex, foodList[highIndex].yIndex);
            }

            if (lowIndex >= highIndex)
                break;

            Food lowFood = DeleteFoodAtPos(lowPos.x, lowPos.y); // xóa thức ăn ở ô thấp
            Food highFood = DeleteFoodAtPos(highPos.x, highPos.y); // xóa thức ăn ở ô cao
            StartCoroutine(MoveFoodToNode(cells[lowPos.x, lowPos.y].transform.position, highPos.x, highPos.y, lowFood)); // di chuyển thức ăn ở ô thấp xuống ô cao
            StartCoroutine(MoveFoodToNode(cells[highPos.x, highPos.y].transform.position, lowPos.x, lowPos.y, highFood)); // di chuyển thức ăn ở ô cao lên ô thấp


            lowIndex++;
            highIndex--;
        }

        //Xáo lại list food
        foodList = foodList.OrderBy(x => UnityEngine.Random.value).ToList();
    }

    #endregion

    #region DRAG
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.instance.currentTurn != "Player")
        {
            return;
        }

        GameObject underPointer = eventData.pointerEnter;

        if (underPointer == null || !underPointer.CompareTag("Food"))
        {
            return;
        }
        else if (underPointer.GetComponentInParent<Food>().isFlying)
        {
            return;
        }

        if (underPointer != null && underPointer.CompareTag("Food") && !hasMatchedFoods.Contains(underPointer.GetComponentInParent<Food>()))
        {
            if (hasMatchedFoods.Count == 0)
            {
                if (isTutorial && guideStep <= 3)
                {
                    StopHighlightCompulsoryStep();
                }

                hasMatchedFoods.Add(underPointer.GetComponentInParent<Food>()); // thêm ô thức ăn đầu tiên vào danh sách đã so khớp
                underPointer.GetComponentInParent<Food>().isMatched = true; // đặt biến isMatched của ô thức ăn đã so khớp về true
                StartCoroutine(underPointer.GetComponentInParent<Food>().ChoosenAnim());
                uiLineRenderer.CreateLine(underPointer.GetComponent<RectTransform>()); // tạo đường nối giữa các ô thức ăn đã so khớp

                if (hasMatchedFoods[0].foodType == FoodType.Special || hasMatchedFoods[0].foodType == FoodType.DebuffSpecial)
                {
                    specialFoodType = hasMatchedFoods[0].specialType;
                    numberToCheckSpecialFood = 1;
                    multipleScore = hasMatchedFoods[0].multipleScore; // lấy số điểm nhân của ô thức ăn đầu tiên
                    isCheckSpecialFood = true; // đặt biến kiểm tra món đặc biệt là true
                    AudioManager.instance.PlaySFX("ChoseSpecialFruit");
                }
                else
                {
                    specialFoodType = "";
                    numberToCheckSpecialFood = 0;
                    multipleScore = 1; // đặt số điểm nhân mặc định là 1
                    isCheckSpecialFood = false;
                    AudioManager.instance.PlaySFX("ChoseFruit");
                }
            }
        }


        //Những ô thức ăn không cùng foodType với ô thức ăn đang được kéo sẽ bị mờ đi và thu nhỏ lại
        if (!isCheckSpecialFood)
        {
            foreach (var food in foodList)
            {
                if (food.foodType != underPointer.GetComponentInParent<Food>().foodType &&
                    (food.foodType != FoodType.Special && food.foodType != FoodType.DebuffSpecial))
                {
                    StartCoroutine(food.FadeOut(0.15f, 0.7f));
                    StartCoroutine(food.ZoomOut(0.15f, 0.8f));
                }
            }
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.instance.currentTurn != "Player")
        {
            return;
        }

        GameObject underPointer = eventData.pointerEnter;

        if (underPointer == null || !underPointer.CompareTag("Food") || hasMatchedFoods.Count < 1)
        {
            return;
        }
        else if (underPointer.GetComponentInParent<Food>().isFlying)
        {
            return;
        }

        // Đi lùi
        if (underPointer != null && hasMatchedFoods.Count - 2 == hasMatchedFoods.FindIndex(x => x == underPointer.GetComponentInParent<Food>()) && hasMatchedFoods.Count >= 2)
        {
            if (hasMatchedFoods[hasMatchedFoods.Count - 1].foodType == FoodType.Special ||
                hasMatchedFoods[hasMatchedFoods.Count - 1].foodType == FoodType.DebuffSpecial)
            {
                specialFoodType = "";
                isCheckSpecialFood = false; // đặt biến kiểm tra món đặc biệt là false
                multipleScore = 1; // đặt số điểm nhân mặc định là 1
            }
            hasMatchedFoods[hasMatchedFoods.Count - 1].isMatched = false; // đặt biến isMatched của ô thức ăn đã so khớp về false
            StartCoroutine(hasMatchedFoods[hasMatchedFoods.Count - 1].ReturnOriginalScale(0.15f));
            hasMatchedFoods.RemoveAt(hasMatchedFoods.Count - 1); // nếu ô thức ăn đã so khớp là ô cuối cùng thì xóa nó khỏi danh sách
            uiLineRenderer.ClearLatestLine(); // xóa đường nối giữa các ô thức ăn đã so khớp

            if (hasMatchedFoods.Count == 1 && (FoodType.Special == hasMatchedFoods[0].foodType ||
                FoodType.DebuffSpecial == hasMatchedFoods[0].foodType))
            {
                foreach (var food in foodList)
                {
                    if (food.foodType != hasMatchedFoods[0].foodType)
                    {
                        StartCoroutine(food.ReturnOriginalColor(0.15f)); // làm ro các ô thức ăn không cùng loại
                        StartCoroutine(food.ReturnOriginalScale(0.15f)); // phong to các ô thức ăn không cùng loại
                    }
                }
            }

            AudioManager.instance.PlaySFX("UndoChoseFruit");
            return;
        }

        if (underPointer != null && underPointer.CompareTag("Food") &&
            !hasMatchedFoods.Contains(underPointer.GetComponentInParent<Food>()) &&
            CheckEnableUsedFood(new Vector2Int(hasMatchedFoods[hasMatchedFoods.Count - 1].xIndex,//Check xem food đang kéo có đang ở gần với food cuối cùng trong list không
            hasMatchedFoods[hasMatchedFoods.Count - 1].yIndex), new List<Vector2Int>()).
            Contains(new Vector2Int(underPointer.GetComponentInParent<Food>().xIndex,
            underPointer.GetComponentInParent<Food>().yIndex)))
        {

            if (hasMatchedFoods.Count > 0)
            {
                if (numberToCheckSpecialFood == 1)
                {
                    if (hasMatchedFoods.Count == 1 && (underPointer.GetComponentInParent<Food>().foodType != FoodType.Special &&
                        underPointer.GetComponentInParent<Food>().foodType != FoodType.DebuffSpecial))
                    {
                        hasMatchedFoods.Add(underPointer.GetComponentInParent<Food>()); // thêm ô thức ăn đầu tiên vào danh sách đã so khớp
                        underPointer.GetComponentInParent<Food>().isMatched = true; // đặt biến isMatched của ô thức ăn đã so khớp về true
                        StartCoroutine(underPointer.GetComponentInParent<Food>().ChoosenAnim());
                        uiLineRenderer.CreateLine(underPointer.GetComponent<RectTransform>()); // thiết lập đường nối giữa các ô thức ăn đã so khớp

                        foreach (var food in foodList)
                        {
                            if (food.foodType != underPointer.GetComponentInParent<Food>().foodType &&
                                (food.foodType != FoodType.Special && food.foodType != FoodType.DebuffSpecial))
                            {
                                StartCoroutine(food.FadeOut(0.15f, 0.7f));
                                StartCoroutine(food.ZoomOut(0.15f, 0.8f));
                            }
                        }
                    }
                    else if (hasMatchedFoods.Count > 1)
                    {
                        if (hasMatchedFoods[1].foodType == underPointer.GetComponentInParent<Food>().foodType)
                        {
                            hasMatchedFoods.Add(underPointer.GetComponentInParent<Food>()); // thêm ô thức ăn đầu tiên vào danh sách đã so khớp
                            underPointer.GetComponentInParent<Food>().isMatched = true; // đặt biến isMatched của ô thức ăn đã so khớp về true
                            StartCoroutine(underPointer.GetComponentInParent<Food>().ChoosenAnim());
                            uiLineRenderer.CreateLine(underPointer.GetComponent<RectTransform>()); // tạo đường nối giữa các ô thức ăn đã so khớp
                        }
                    }

                    AudioManager.instance.PlaySFX("ChoseFruit");
                }
                else
                {
                    if (hasMatchedFoods[0].foodType == underPointer.GetComponentInParent<Food>().foodType)
                    {
                        hasMatchedFoods.Add(underPointer.GetComponentInParent<Food>()); // thêm ô thức ăn đầu tiên vào danh sách đã so khớp
                        underPointer.GetComponentInParent<Food>().isMatched = true; // đặt biến isMatched của ô thức ăn đã so khớp về true
                        StartCoroutine(underPointer.GetComponentInParent<Food>().ChoosenAnim());
                        uiLineRenderer.CreateLine(underPointer.GetComponent<RectTransform>()); // thiết lập đường nối giữa các ô thức ăn đã so khớp
                        AudioManager.instance.PlaySFX("ChoseFruit");
                    }

                    else if ((underPointer.GetComponentInParent<Food>().foodType == FoodType.Special ||
                        underPointer.GetComponentInParent<Food>().foodType == FoodType.DebuffSpecial) && isCheckSpecialFood == false)
                    {
                        hasMatchedFoods.Add(underPointer.GetComponentInParent<Food>()); // thêm ô thức ăn đầu tiên vào danh sách đã so khớp
                        underPointer.GetComponentInParent<Food>().isMatched = true; // đặt biến isMatched của ô thức ăn đã so khớp về true
                        StartCoroutine(underPointer.GetComponentInParent<Food>().ChoosenAnim());

                        specialFoodType = underPointer.GetComponentInParent<Food>().specialType; // lấy loại món đặc biệt của ô thức ăn đầu tiên
                        multipleScore = underPointer.GetComponentInParent<Food>().multipleScore; // lấy số điểm nhân của ô thức ăn đầu tiên
                        isCheckSpecialFood = true; // đặt biến kiểm tra món đặc biệt là true

                        uiLineRenderer.CreateLine(underPointer.GetComponent<RectTransform>());
                        AudioManager.instance.PlaySFX("ChoseSpecialFruit");
                    }
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.instance.currentTurn != "Player")
        {
            return;
        }

        if (hasMatchedFoods.Count < 1)
        {
            return;
        }

        foreach (var food in foodList)
        {
            if (food.foodType != hasMatchedFoods[0].foodType &&
                (food.foodType != FoodType.Special && food.foodType != FoodType.DebuffSpecial))
            {
                StartCoroutine(food.ReturnOriginalColor(0.15f)); // làm ro các ô thức ăn không cùng loại
                StartCoroutine(food.ReturnOriginalScale(0.15f)); // phong to các ô thức ăn không cùng loại
            }
        }

        bool havingSpecialFood = false; // biến để kiểm tra xem có ô thức ăn đặc biệt nào trong danh sách đã so khớp hay không
        bool isFoodMoveToPlayer = true;
        string specialTypeTmp = "";
        for (int i = hasMatchedFoods.Count; i > 0; i--)
        {
            if (hasMatchedFoods[i - 1] != null)
            {
                hasMatchedFoods[i - 1].isMatched = false; // đặt biến isMatched của ô thức ăn đã so khớp về false

                if (hasMatchedFoods[i - 1].foodType == FoodType.Special ||
                    hasMatchedFoods[i - 1].foodType == FoodType.DebuffSpecial)
                {
                    isFoodMoveToPlayer = specialFoodType == "Multiple";
                    specialTypeTmp = specialFoodType;

                    specialFoodType = "";
                    havingSpecialFood = true; // nếu có ô thức ăn đặc biệt thì đặt biến havingSpecialFood về true

                    isCheckSpecialFood = false;
                    numberToCheckSpecialFood = 0; // đặt biến numberToCheckSpecialFood về 0
                    GameManager.instance.multipleScoreForPlayerHit = multipleScore;
                    multipleScore = 1;
                }
                hasMatchedFoods[i - 1].highlightVFX1.Stop();
                hasMatchedFoods[i - 1].highlightVFX2.Stop();
            }
        }

        //Xử lý food bay đến player hoặc enemy hoặc không bay đến đâu cả
        if (hasMatchedFoods.Count >= 2)
        {
            if (isTutorial && guideStep <= 3 && !CheckCompulsoryStepAtTutorial())
            {
                StartCoroutine(OnTurnOffHighlightFood()); // nếu không có thức ăn nào được so khớp thì tắt hiệu ứng highlight
            }
            else
            {

                StartCoroutine(OnDeletedMatchFood(havingSpecialFood, isFoodMoveToPlayer, specialTypeTmp)); // gọi hàm xóa thức ăn đã so khớp sau khi kết thúc kéo

                if (MissionsManager._instance.missions != null)
                {
                    MissionsManager._instance.FruitMatching(hasMatchedFoods.Count); // cập nhật tiến độ nhiệm vụ nếu có
                }

            }
        }
        else
        {
            StartCoroutine(OnTurnOffHighlightFood()); // nếu không có thức ăn nào được so khớp thì tắt hiệu ứng highlight
        }

        uiLineRenderer.ClearLines(); // xóa đường nối giữa các ô thức ăn đã so khớp
    }

    IEnumerator OnDeletedMatchFood(bool haveSpecialFood, bool isFoodMoveToPlayer, string specialType)
    {
        GameManager.instance.currentTurn = "None";
        onDeleteFood = true;
        Vector2 specialPos = new Vector2();
        Vector2Int specialFoodIndex = new Vector2Int();

        if (isFoodMoveToPlayer)
        {
            PlayerUltimate.instance.playerTransform.GetComponent<PlayerAttack>().PlayTakeFruitVFX();
        }

        if (specialType == "DebuffTakeDam")
        {
            LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].GetComponent<EnemyAttack>().debuffVFX.Play();
            LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].GetComponent<EnemyAttack>().enemyDebuffTurn = 1;
        }

        for (int i = 0; i < hasMatchedFoods.Count; i++)// int i = hasMatchedFoods.Count; i > 0; i--
        {
            if (i == hasMatchedFoods.Count - 1)
            {
                specialPos = cells[hasMatchedFoods[i].xIndex, hasMatchedFoods[i].yIndex].transform.position;

                specialFoodIndex = new Vector2Int(hasMatchedFoods[i].xIndex, hasMatchedFoods[i].yIndex);
            }

            if (hasMatchedFoods[i] != null)
            {
                Vector2Int currentPos = new Vector2Int(hasMatchedFoods[i].xIndex, hasMatchedFoods[i].yIndex);

                StartCoroutine(hasMatchedFoods[i].MoveToTarget(0.25f, isFoodMoveToPlayer, specialType)); // di chuyển thức ăn đã so khớp vào thanh máu của người chơi
                yield return new WaitForSeconds(0.25f); // đợi một khoảng thời gian trước khi xóa thức ăn

                //Smoke VFX
                cells[currentPos.x, currentPos.y].TurnOnSmokeVFX();

                //Hit state
                HitState(currentPos); // gọi hàm HitState để xử lý va chạm với các khối gỗ
            }
        }

        //Add special food base on number of matched foods
        for (int i = foodCount.Length - 1; i >= 0; i--)//foodCount.Length - 1; i >= 0
        {
            if (hasMatchedFoods.Count >= foodCount[i] && haveSpecialFood != true)
            {
                //StartCoroutine(OnDeletedMatchFood()); // gọi hàm xóa thức ăn đã so khớp sau khi kết thúc kéo
                //int randomIndex = UnityEngine.Random.Range(0, hasMatchedFoods.Count); // chọn ngẫu nhiên một ô thức ăn trong danh sách đã so khớp

                //GameObject specialFood = Instantiate(specialFoodPrefab, specialPos, Quaternion.identity, foodParent); // tạo một ô thức ăn đặc biệt tại vị trí của ô thức ăn đã so khớp
                GameObject specialFood = PoolManager.Instance.GetObject("Food Multiple", 
                    specialPos, Quaternion.identity, foodParent, specialFoodPrefab);

                specialFood.transform.localScale = Vector3.zero; // đặt kích thước của ô thức ăn đặc biệt về 0
                Food food = specialFood.GetComponent<Food>();
                food.SetMultipleScore(specialMutiplie[i]); // đặt số điểm nhân của ô thức ăn đặc biệt là 3
                food.StartCoroutine(food.ReturnOriginalScale(0.1f));
                AddFoodAtPos(specialFoodIndex.x, specialFoodIndex.y, food); // thêm ô thức ăn đặc biệt vào ô đã so khớp

                break; // thoát khỏi vòng lặp nếu đã tạo ô thức ăn đặc biệt
            }
        }



        //Player thuc hien tan cong
        yield return new WaitForSeconds(0.4f);//Waiting for food to move to player

        startFalling = true; // đặt biến startFalling về true để bắt đầu quá trình rơi thức ăn
        isDoneOneFallingRound = true;

        if (isFoodMoveToPlayer)
        {
            if (haveSpecialFood)
            {
                StartCoroutine(PlayerUltimate.instance.playerTransform.GetComponent<PlayerAttack>().
                    PlayAttackSequence(hasMatchedFoods.Count - 1, true));
            }
            else
            {
                StartCoroutine(PlayerUltimate.instance.playerTransform.GetComponent<PlayerAttack>().
                    PlayAttackSequence(hasMatchedFoods.Count, false));
            }
        }
        else
        {
            GameManager.instance.currentTurn = "Player";
        }

        hasMatchedFoods.Clear(); // xóa danh sách các ô thức ăn đã so khớp
        onDeleteFood = false;
    }

    public bool CheckCompulsoryStepAtTutorial()
    { 
        bool result = true;

        List<Vector2Int> compulsoryStep = null;
        if (guideStep == 1)
        {
            compulsoryStep = compulsoryStep1.ToList();
        }
        else if (guideStep == 2)
        {
            compulsoryStep = compulsoryStep2.ToList();
        }
        else if (guideStep == 3)
        {
            compulsoryStep = compulsoryStep3.ToList();
        }
        else
        {
            return true;
        }

        int countCompulsoryStep = 0;   
        for (int i = 0; i < compulsoryStep.Count; i++)
        {
            if (hasMatchedFoods.Contains(foods[compulsoryStep[i].x, compulsoryStep[i].y]))
            {
                countCompulsoryStep++;
            }
        }

        if (countCompulsoryStep != compulsoryStep.Count)
        {
            result = false;
        }
        else
        { 
            guideStep++;
            result = true;
        }

        return result;
    }

    public void HitState(Vector2Int center)
    {
        if (center.x > 0 && gameBoard[center.x - 1, center.y] == "HavingState")
        {
            State state = states2DArray[center.x - 1, center.y];
            state.StartCoroutine(state.TakeHit(20f));
        }

        if (center.y > 0 && gameBoard[center.x, center.y - 1] == "HavingState")
        {
            State state = states2DArray[center.x, center.y - 1];
            state.StartCoroutine(state.TakeHit(20f));
        }

        if (center.y < 5 && gameBoard[center.x, center.y + 1] == "HavingState")
        {
            State state = states2DArray[center.x, center.y + 1];
            state.StartCoroutine(state.TakeHit(20f));
        }

        if (center.x < 5 && gameBoard[center.x + 1, center.y] == "HavingState")
        {
            State state = states2DArray[center.x + 1, center.y];
            state.StartCoroutine(state.TakeHit(20f));
        }
    }

    IEnumerator OnTurnOffHighlightFood()
    {
        for (int i = hasMatchedFoods.Count; i > 0; i--)
        {
            if (hasMatchedFoods[i - 1] != null)
            {
                StartCoroutine(hasMatchedFoods[i - 1].ReturnOriginalScale(0.15f)); // di chuyển thức ăn đã so khớp vào thanh máu của người chơi
            }
            yield return null;
        }

        hasMatchedFoods.Clear(); // xóa danh sách các ô thức ăn đã so khớp

        if (isTutorial && guideStep <= 3)
        {
            StartCoroutine(HighlightCompulsoryStep());
        }
    }

    #endregion

    #region ANIMATION TUTORIAL

    public IEnumerator HighlightCompulsoryStep()
    { 
        isPlayAnimGuideStep = true;
        List<Vector2Int> compulsoryStep = null;
        if (guideStep == 1)
        {
            compulsoryStep = compulsoryStep1;
        }
        else if (guideStep == 2)
        {
            compulsoryStep = compulsoryStep2;
        }
        else if (guideStep == 3)
        {
            compulsoryStep = compulsoryStep3;
        }

        if (compulsoryStep != null)
        {
            while (isPlayAnimGuideStep)
            {
                float elapsedTime = 0f;
                //Wait for 0.5s
                while (elapsedTime < 0.5f && isPlayAnimGuideStep)
                {
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }


                for (int i = 0; i < compulsoryStep.Count; i++)
                {
                    if (!isPlayAnimGuideStep)
                    {
                        break;
                    }

                    foods[compulsoryStep[i].x, compulsoryStep[i].y].isMatched = true;
                    StartCoroutine(foods[compulsoryStep[i].x, compulsoryStep[i].y].ChoosenAnim());

                    //Wait for 0.5s
                    elapsedTime = 0f;
                    while (elapsedTime < 0.5f && isPlayAnimGuideStep)
                    {
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }
                }

                //Wait for 0.5s
                elapsedTime = 0f;
                while (elapsedTime < 0.75f && isPlayAnimGuideStep)
                {
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                for (int i = 0; i < compulsoryStep.Count; i++)
                {
                    if (!isPlayAnimGuideStep)
                    {
                        break;
                    }
                    foods[compulsoryStep[i].x, compulsoryStep[i].y].isMatched = false;
                    StartCoroutine(foods[compulsoryStep[i].x, compulsoryStep[i].y].ReturnOriginalScale(0.15f));
                }

                //Wait for 0.5s
                elapsedTime = 0f;
                while (elapsedTime < 0.5f && isPlayAnimGuideStep)
                {
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            }
        }
    }

    public void StopHighlightCompulsoryStep()
    {
        isPlayAnimGuideStep = false;
        //StopCoroutine(HighlightCompulsoryStep());

        List<Vector2Int> compulsoryStep = null;
        if (guideStep == 1)
        {
            compulsoryStep = compulsoryStep1;
        }
        else if (guideStep == 2)
        {
            compulsoryStep = compulsoryStep2;
        }
        else if (guideStep == 3)
        {
            compulsoryStep = compulsoryStep3;
        }

        if (compulsoryStep != null)
        {
            for (int i = 0; i < compulsoryStep.Count; i++)
            {
                foods[compulsoryStep[i].x, compulsoryStep[i].y].isMatched = false;
                foods[compulsoryStep[i].x, compulsoryStep[i].y].transform.localScale = 
                    new Vector3(foods[compulsoryStep[i].x, compulsoryStep[i].y].foodScale, 
                    foods[compulsoryStep[i].x, compulsoryStep[i].y].foodScale, 1);
            }
        }
    }

    #endregion
}
