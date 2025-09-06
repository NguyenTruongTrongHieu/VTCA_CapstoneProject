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
        GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f, 1); // Reset vị trí khi tái sử dụng từ pool
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

    public IEnumerator TakeHit(float magnitude)
    {
        //State thisState = this;
        //durability--;
        //if (durability <= 0)
        //{
        //    thisState = GameBoard.Instance.DeleteStateAtPos(xIndex, yIndex);
        //}

        //yield return new WaitForSeconds(0.25f); // Đợi một chút trước khi xử lý va chạm

        //if (durability > 0)
        //{
        //    image.sprite = stateSprite[durability - 1]; // Cập nhật hình ảnh của khối gỗ theo độ 
        //    AudioManager.instance.PlaySFX("HitState");
        //}
        //else
        //{
        //    AudioManager.instance.PlaySFX("BreakState");
        //}
        yield return new WaitForSeconds(0.25f); // Đợi một chút trước khi xử lý va chạm
        durability--;


        State thisState = this;
        if (durability > 0)
        {
            image.sprite = stateSprite[durability - 1]; // Cập nhật hình ảnh của khối gỗ theo độ 
            AudioManager.instance.PlaySFX("HitState");
        }
        else
        {
            thisState = GameBoard.Instance.DeleteStateAtPos(xIndex, yIndex);
            AudioManager.instance.PlaySFX("BreakState");
        }
        hitStateVFX1.Play(); // Phát hiệu ứng VFX khi khối gỗ bị đánh
        hitStateVFX2.Play();
        hitStateVFX3.Play();
        hitStateVFX4.Play();

        //shake state when take hit
        float elapsed = 0f;
        RectTransform rt = image.rectTransform;
        Vector3 originalPos = Vector3.zero; // Lưu vị trí ban đầu của khối gỗ

        // Tạo điểm đích bị lệch
        Vector3 offset = new Vector3(1,1, 0) * magnitude;
        Vector3 targetPos1 = originalPos + offset;
        Vector3 targetPos2 = originalPos - offset;

        while (elapsed < 0.05f)
        {
            // Di chuyển dần đến vị trí lắc
            rt.anchoredPosition = Vector3.Lerp(originalPos, targetPos1, elapsed / 0.1f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = targetPos1; // Đặt vị trí lắc

        elapsed = 0f; // Reset thời gian đã trôi qua
        while (elapsed < 0.1f)
        {
            // Di chuyển dần đến vị trí lắc ngược lại
            rt.anchoredPosition = Vector3.Lerp(targetPos1, targetPos2, elapsed / 0.2f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = targetPos2; // Đặt vị trí lắc ngược lại

        elapsed = 0f; // Reset thời gian đã trôi qua
        while (elapsed < 0.05f)
        {
            // Di chuyển dần về vị trí ban đầu
            rt.anchoredPosition = Vector3.Lerp(targetPos2, originalPos, elapsed / 0.1f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = originalPos; // Reset về vị trí cũ

        if (durability <= 0)
        {
            //Destroy(thisState.gameObject); // Destroy the state object when durability reaches 0
            PoolManager.Instance.ReturnObject("State", thisState.gameObject);
        }
    }
}
