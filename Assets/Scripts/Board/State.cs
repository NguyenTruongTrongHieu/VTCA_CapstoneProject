using System.Collections;
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
    public ParticleSystem hitStateVFX1;
    public ParticleSystem hitStateVFX2;
    public ParticleSystem hitStateVFX3;
    public ParticleSystem hitStateVFX4;

    private void OnEnable()
    {
        durability = 3; // Reset độ bền khi khối gỗ được kích hoạt
        image.sprite = stateSprite[durability - 1]; // Cập nhật hình ảnh của khối gỗ theo độ bền
    }

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

    public IEnumerator TakeHit(float duration, float magnitude)
    {
        durability--;
        if (durability > 0)
        {
            image.sprite = stateSprite[durability - 1]; // Cập nhật hình ảnh của khối gỗ theo độ bền
        }
        hitStateVFX1.Play(); // Phát hiệu ứng VFX khi khối gỗ bị đánh
        hitStateVFX2.Play();
        hitStateVFX3.Play();
        hitStateVFX4.Play();

        //shake state when take hit
        float elapsed = 0f;
        RectTransform rt = image.rectTransform;
        Vector3 originalPos = Vector3.zero; // Lưu vị trí ban đầu của khối gỗ

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            rt.anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = originalPos; // Reset về vị trí cũ

        if (durability <= 0)
        {
            State thisState = GameBoard.Instance.DeleteStateAtPos(xIndex, yIndex);
            Destroy(thisState.gameObject); // Destroy the state object when durability reaches 0
        }
    }
}
