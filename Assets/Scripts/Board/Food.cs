using System.Collections;
using UnityEngine;

public class Food : MonoBehaviour
{
    public FoodType foodType; // loại thức ăn

    public int xIndex; // chỉ số hàng của ô chứa thức ăn
    public int yIndex; // chỉ số cột của ô chứa thức ăn

    public bool isFalling;
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

    //private void OnDestroy()
    //{
    //    GameBoard.Instance.DeleteFoodAtPos(xIndex, yIndex);
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(ZoomOut(0.1f, 0.8f));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(ZoomIn(0.1f, 1.2f));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(MoveToPlayerHpSlider(0.5f));
        }
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
        Vector3 originalScale = new Vector3(0.9f, 0.9f, 1f);
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

    public IEnumerator MoveTo(Transform targetPosition, float duration)
    {
        currentPos = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(currentPos, targetPosition.position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition.position; // Đảm bảo vị trí cuối cùng chính xác
    }

    public IEnumerator MoveToPlayerHpSlider(float duration)
    {
        this.transform.parent = UIManager.instance.targetPos.transform;

        StartCoroutine(ZoomIn(0.2f, 3f));
        yield return StartCoroutine(TurnOver(0.3f));
        StartCoroutine(ReturnOriginalScale(0.1f));

        yield return StartCoroutine(MoveTo(UIManager.instance.targetPos.transform, duration));
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
