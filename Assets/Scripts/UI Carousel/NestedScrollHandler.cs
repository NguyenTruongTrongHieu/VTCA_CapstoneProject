using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NestedScrollHandler : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private ScrollRect parentScroll;  // Scroll View cha (carousel ngang)
    [SerializeField] private ScrollRect[] childScroll;  // Scroll View con (này chính là object đang gắn script)
    [SerializeField] private Carousel carousel; // Tham chiếu đến Carousel để có thể gọi hàm ActivateCurrentIndicatorByPlayerClass
    [SerializeField] private int thisChildScroll;
    private bool isHorizontalDrag;   // Cờ xác định đang vuốt ngang


    public void OnBeginDrag(PointerEventData eventData)
    {
        // Xác định hướng kéo
        float x = Mathf.Abs(eventData.delta.x);
        float y = Mathf.Abs(eventData.delta.y);

        isHorizontalDrag = x > y;

        if (isHorizontalDrag)
        {
            // Tạm khóa scroll con, để cha xử lý
            foreach (var scroll in childScroll)
            {
                scroll.enabled = false;
            }
            parentScroll.OnBeginDrag(eventData);
        }
        else
        {
            // Tạm khóa scroll cha, để con xử lý
            parentScroll.enabled = false;
            childScroll[thisChildScroll].enabled = true;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isHorizontalDrag)
        {
            parentScroll.OnDrag(eventData);
        }
        else
        {
            childScroll[thisChildScroll].OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isHorizontalDrag)
        {
            parentScroll.OnEndDrag(eventData);
            carousel.OnEndDrag(eventData); // Gọi hàm OnEndDrag của Carousel để cập nhật chỉ báo
        }
        else
        {
            childScroll[thisChildScroll].OnEndDrag(eventData);
        }

        // Bật lại cả 2 sau khi kết thúc drag
        foreach (var scroll in childScroll)
        {
            scroll.enabled = true;
        }
        parentScroll.enabled = true;
    }
}
