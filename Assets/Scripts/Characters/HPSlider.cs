using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HPSlider : MonoBehaviour
{
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider yellowSlider;
    [SerializeField] private Text objectName;
    [SerializeField] private GameObject healingText;

    [SerializeField] private float maxValue;
    [SerializeField] private float currentValue;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        { 
            SetSLiderAtStart("player", 100f); // Example to set slider at start with max HP of 100
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            MinusValue(currentValue - 15f); // Example to decrease HP to 80
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlusValue(currentValue + 15f, 15); // Example to increase HP to 90
        }
    }

    public void SetSLiderAtStart(string name, float maxHP)
    {
        this.StopAllCoroutines();
        objectName.text = name;

        redSlider.maxValue = maxHP;
        yellowSlider.maxValue = maxHP;
        redSlider.value = maxHP;
        yellowSlider.value = maxHP;

        maxValue = maxHP;
        currentValue = maxHP;
    }

    public void MinusValue(float currentHP)
    {
        if (currentValue <= 0)
        {
            return;
        }

        float changeValue = currentValue - currentHP;
        if (changeValue <= 0)
        {
            Debug.LogError("Minus value error");
            return;
        }

        currentValue = currentHP;
        redSlider.value = currentValue;
        StartCoroutine(SetSlider(yellowSlider, currentValue, 0.3f, 0.3f)); // Smoothly update the yellow slider over 0.5 seconds
    }

    public void PlusValue(float currentHP, float heal)
    {
        if (currentValue >= maxValue)
        {
            return;
        }

        float changeValue = currentHP - currentValue;
        if (changeValue <= 0)
        {
            Debug.LogError("Plus value error");
            return;
        }

        currentValue = currentHP;
        StartCoroutine(SetSlider(redSlider, currentValue, 0.3f, 0f)); // Smoothly update the yellow slider over 0.5 seconds
        StartCoroutine(DisplayHealingText(heal));
    }

    public IEnumerator SetSlider(Slider slider, float targetValue, float duration, float waitTimeBeforeSetSlider)
    {
        if (waitTimeBeforeSetSlider > 0f)
        {
            yield return new WaitForSeconds(waitTimeBeforeSetSlider); // Wait before starting the transition
        }

        float startValue = slider.value;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            slider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null; // Wait for the next frame
        }
        slider.value = targetValue; // Ensure the final value is set
        if (slider == redSlider)
        {
            yellowSlider.value = targetValue; // Update yellow slider to match red slider
        }
    }

    public IEnumerator DisplayHealingText(float heal)
    {
        string text = NumberFomatter.FormatFloatToString(heal, 2);

        GameObject healingTextObject = Instantiate(healingText, transform.position, Quaternion.identity, transform);
        Text healText = healingTextObject.GetComponent<Text>();
        RectTransform rectTransformHealText = healingTextObject.GetComponent<RectTransform>();

        healText.text = "+" + text; // Set the healing text

        //Animation Text Move Up
        //float elaspedTime = 0f;
        //Vector3 startScale = rectTransformHealText.localScale;
        //Vector3 targetScale = Vector3.one;
        //while (elaspedTime < 0.1f)//Chạy anim này trong vòng 0.1s
        //{
        //    elaspedTime += Time.deltaTime;
        //    rectTransformHealText.localScale = Vector3.Lerp(startScale, targetScale, elaspedTime / 0.1f);
        //    yield return null;
        //}
        //rectTransformHealText.localScale = targetScale; // Đặt tỉ lệ cuối cùng

        float elaspedTime = 0f;
        Vector3 startPos = rectTransformHealText.anchoredPosition;
        Vector3 targetPos = startPos + new Vector3(0, 100f, 0); // Di chuyển lên trên 1 đơn vị
        while (elaspedTime < 0.2f)//Chạy anim này trong vòng 0.2s
        {
            elaspedTime += Time.deltaTime;
            rectTransformHealText.anchoredPosition = Vector3.Lerp(startPos, targetPos, elaspedTime / 0.2f);
            yield return null;
        }
        rectTransformHealText.anchoredPosition = targetPos; // Đặt vị trí cuối cùng

        yield return new WaitForSeconds(0.5f); // Đợi một chút trước khi xóa text
        Destroy(rectTransformHealText.gameObject); // Xóa text sau khi hoàn thành
    }
}
