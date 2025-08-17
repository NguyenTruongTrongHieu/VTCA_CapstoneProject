using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    public FoodType foodType; // loại thức ăn

    public int xIndex; // chỉ số hàng của ô chứa thức ăn
    public int yIndex; // chỉ số cột của ô chứa thức ăn

    public bool isFalling;
    public bool isMatched; // biến kiểm tra xem thức ăn có được ăn hay không
    private Vector2 currentPos;
    private Vector2 targetPos;

    [SerializeField] private Image foodHighLightImage;
    [SerializeField] private Image foodImage;
    public ParticleSystem highlightVFX1;
    public ParticleSystem highlightVFX2;
    public float foodScale = 0.8f; // Tỷ lệ kích thước của thức ăn

    [Header("Specical food")]
    public string specialType;//"": không có special; "Multiple": thức ăn nhân thêm dam, tạo ra đòn đặc biệt cho player;
                              //"DebuffTakeDam": thức ăn làm enemy tăng dam phải nhận
    public int multipleScore;
    public Text multipleText;
    public ParticleSystem auraSpecialVFX;


    public Food(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    public Food(int _xIndex, int _yIndex, int multipleScore)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
        this.multipleScore = multipleScore;
    }

    public void SetIndex(int _xIndex, int _yIndex)
    {
        xIndex = _xIndex;
        yIndex = _yIndex;
    }

    //private void OnDestroy()
    //{
    //    GameBoard.Instance.DeleteFoodAtPos(xIndex, yIndex);
    //}

    private void Start()
    {
        if (foodType == FoodType.Special)
        {
            multipleText.text = $"X{multipleScore}"; // Hiển thị số điểm nhân
        }
        else if (foodType == FoodType.DebuffSpecial)
        {
            if (specialType == "DebuffTakeDam")
            {
                multipleText.text = "TD";
            }
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.D))
    //    {
    //        StartCoroutine(FallAnim());
    //    }
    //    if (Input.GetKeyDown(KeyCode.A))
    //    {
    //        StartCoroutine(ChoosenAnim());
    //    }
    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        StartCoroutine(MoveToTarget(0.5f, true, "Multiple"));
    //    }
    //}

    public void SetMultipleScore(int multipleScore)
    {
        this.multipleScore = multipleScore;
        if (multipleText != null)
        {
            multipleText.text = $"X{multipleScore}"; // Cập nhật số điểm nhân
        }
    }

    public IEnumerator ChoosenAnim()
    {
        highlightVFX1.Play();
        highlightVFX2.Play();
        StartCoroutine(FoodHighLight(0.5f));
        StartCoroutine(ZoomIn(0.15f, 1.1f)); // Tăng kích thước lên 10%
        while (isMatched)
        {
            yield return StartCoroutine(RotateTheFood(0.15f, 20f));
            yield return new WaitForSeconds(0.1f);
            yield return StartCoroutine(ReturnOriginalRotation(0.15f));
            yield return new WaitForSeconds(0.1f);

        }
        highlightVFX1.Stop();
        highlightVFX2.Stop();
        //StartCoroutine(ReturnOriginalScale(0.15f)); // Trả về góc ban đầu
    }

    public IEnumerator RotateTheFood(float duration, float rotation)
    {
        Vector3 originalRotation = transform.localEulerAngles;
        Vector3 targetRotation = originalRotation + new Vector3(0, 0, rotation); // Xoay 180 độ
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localEulerAngles = Vector3.Lerp(originalRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = targetRotation; // Đảm bảo góc cuối cùng chính xác
        yield return null;
    }

    public IEnumerator ReturnOriginalRotation(float duration)
    {
        Vector3 startRotation = transform.localEulerAngles;
        Vector3 targetRotation = new Vector3(0, 0, 0); // Xoay 180 độ
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localEulerAngles = Vector3.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = targetRotation; // Đảm bảo góc cuối cùng chính xác
    }

    public IEnumerator FallAnim()
    {
        yield return StartCoroutine(ReduceScaleY(0.05f, 0.5f)); // Giảm kích thước Y xuống 50%
        yield return new WaitForSeconds(0.02f);
        yield return StartCoroutine(ReturnOriginalScale(0.05f));
    }

    private IEnumerator ReduceScaleY(float duration, float targetScaleY)
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(originalScale.x, targetScaleY, originalScale.z);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Đảm bảo kích thước cuối cùng chính xác
    }

    public IEnumerator ZoomIn(float duration, float increasePercent)//Lý tưởng nhất: 1.2f
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * increasePercent; // Tăng kích thước lên 20%

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Đảm bảo kích thước cuối cùng chính xác
    }

    public IEnumerator ReturnOriginalScale(float duration)
    {
        Vector3 originalScale = new Vector3(foodScale, foodScale, 1f);
        Vector3 startScale = transform.localScale;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, originalScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale; // Đảm bảo kích thước cuối cùng chính xác
    }

    public IEnumerator ZoomOut(float duration, float decreasePercent)//lý tưởng nhất: 0.8f
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * decreasePercent; // Giảm kích thước xuống 80%

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale; // Đảm bảo kích thước cuối cùng chính xác
    }

    public IEnumerator TurnOverAndScaleIn(float duration)
    {
        Vector3 originalRotation = transform.localEulerAngles;
        Vector3 targetRotation = originalRotation + new Vector3(0, 0, -180); // Xoay 180 độ
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 3f; // Tăng kích thước lên 20%

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localEulerAngles = Vector3.Lerp(originalRotation, targetRotation, elapsedTime / duration);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale; // Đảm bảo kích thước cuối cùng chính xác
        transform.localEulerAngles = targetRotation; // Đảm bảo góc cuối cùng chính xác
    }

    public IEnumerator ScaleInAndScaleOut()
    {
        yield return StartCoroutine(ZoomIn(0.2f, 5f)); // Tăng kích thước lên 20%
        //yield return new WaitForSeconds(0.1f);
        StartCoroutine(ReturnOriginalScale(0.15f)); // Giảm kích thước xuống 80%
    }

    public IEnumerator TurnOver(float duration)
    {
        Vector3 originalRotation = transform.localEulerAngles;
        Vector3 targetRotation = originalRotation + new Vector3(0, 0, -180); // Xoay 180 độ
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.localEulerAngles = Vector3.Lerp(originalRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localEulerAngles = targetRotation; // Đảm bảo góc cuối cùng chính xác
    }

    public IEnumerator MoveTo(Vector3 targetPosition, float duration)
    {
        currentPos = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(currentPos, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // Đảm bảo vị trí cuối cùng chính xác
    }

    public IEnumerator FadeOut(float duration, float targetColorA)
    {
        Color originalColor = foodImage.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, targetColorA); // Màu sắc mờ

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            foodImage.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foodImage.color = targetColor; // Đảm bảo màu sắc cuối cùng chính xác
    }

    //Trở lại màu gốc
    public IEnumerator ReturnOriginalColor(float duration)
    {
        Color originalColor = foodImage.color;
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f); // Màu sắc mờ

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            foodImage.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        foodImage.color = targetColor; // Đảm bảo màu sắc cuối cùng chính xác
    }

    public IEnumerator MoveToTarget(float duration, bool targetIsPlayer, string specialFoodType)//SpecialFoodTyoe: dựa theo Food và GameBoard
    {
        Vector3 targetPos = targetIsPlayer ? Camera.main.WorldToScreenPoint(PlayerUltimate.instance.playerTransform.position) 
            : Camera.main.WorldToScreenPoint(LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform.position);

        this.transform.SetParent(UIManager.instance.inGamePanel.transform);
        isMatched = false;
        Food deletedFood = GameBoard.Instance.DeleteFoodAtPos(xIndex, yIndex);

        //this.StopCoroutine(FoodHighLight(0.5f)); // Dừng hiệu ứng highlight
        //foodHighLightImage.color = new Color(foodHighLightImage.color.r, foodHighLightImage.color.g, foodHighLightImage.color.b, 0.0f);

        //StartCoroutine(ZoomIn(0.2f, 3f));
        //StartCoroutine(ZoomOut(0.15f, 0.5f));
        StartCoroutine(ScaleInAndScaleOut());
        yield return StartCoroutine(TurnOver(0.3f));
        //StartCoroutine(ReturnOriginalScale(0.2f));

        yield return StartCoroutine(MoveTo(targetPos, duration));

        if (!targetIsPlayer)
        {
            LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].GetComponent<EnemyAttack>().GetHitAnim();

            // Kiểm tra loại thức ăn bay đến enemy và thực hiện công dụng của food đó
            if (specialFoodType == "DebuffTakeDam")
            {
                LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].GetComponent<EnemyStat>().defense += 0.1f;
            }
        }

        deletedFood.gameObject.SetActive(false); // ẩn đối tượng Food
    }

    public IEnumerator FoodHighLight(float duration)
    {
        foodHighLightImage.gameObject.SetActive(true);
        Color originalColor = foodHighLightImage.color; // Lưu màu sắc gốc
        Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1.0f); // Màu sắc mờ


        while (isMatched)
        {
            // Làm rõ dần màu
            float elapsedTime = 0f;
            while (elapsedTime < duration && isMatched)
            {
                foodHighLightImage.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            foodHighLightImage.color = targetColor; // Đảm bảo màu sắc cuối cùng chính xác

            elapsedTime = 0f;
            // Chờ một khoảng thời gian trước khi làm mờ
            while (elapsedTime < duration && isMatched)
            {
                elapsedTime += Time.deltaTime;
                yield return null; // Chờ trong khi vẫn là thức ăn được ăn
            }

            // Làm mờ dần màu sắc
            elapsedTime = 0f;
            while (elapsedTime < duration && isMatched)
            {
                foodHighLightImage.color = Color.Lerp(targetColor, originalColor, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            foodHighLightImage.color = originalColor; // Đảm bảo màu sắc cuối cùng chính xác
        }

        Color currentHighLightColor = foodHighLightImage.color;
        //Làm mờ màu sắc về 0 bằng Lerp
        //float fadeTimer = 0f; // Thời gian làm mờ
        //while (fadeTimer < 0.1f)
        //{
        //    foodHighLightImage.color = Color.Lerp(currentHighLightColor, originalColor, fadeTimer / 0.1f);
        //    fadeTimer += Time.deltaTime;
        //    yield return null;
        //}

        foodHighLightImage.color = originalColor; // Đảm bảo màu sắc cuối cùng chính xác
    }

}
    public enum FoodType
{
    Special,
    DebuffSpecial,
    Apple,
    Banana,
    Orange,
    Blueberry,
    Grape
}
