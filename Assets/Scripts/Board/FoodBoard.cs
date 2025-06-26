using UnityEngine;

public class FoodBoard : MonoBehaviour
{
    public int boardWidth = 6; // chiều rộng của bàn cờ
    public int boardHeight = 8; // chiều cao của bàn cờ
    public GameObject tilePrefab; // prefab của ô mặc định
    private BackgroundTile[,] allTiles;

    void Start()
    {
        allTiles = new BackgroundTile[boardWidth, boardHeight];
        SetUp();
    }


    private void SetUp()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Vector2 tempPosition = new Vector2(x, y);
                RectTransform rectTransform = tilePrefab.GetComponent<RectTransform>();
                tempPosition.x += rectTransform.rect.width * (x + 1);
                tempPosition.y += (float)(rectTransform.rect.height * (y + 0.5));
                Instantiate(tilePrefab, tempPosition, Quaternion.identity,transform.GetChild(0));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
