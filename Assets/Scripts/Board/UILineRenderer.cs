using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : MonoBehaviour
{
    [SerializeField] private List<RectTransform> points = new List<RectTransform>();
    [SerializeField] private GameObject line_image;

    public void CreateLine(RectTransform Rect_Transform)
    {
        if (points.Contains(Rect_Transform))
        {
            return; // Avoid adding the same point multiple times
        }

        
        points.Add(Rect_Transform);

        if (points.Count < 2)
        {
            return;
        }

        int i = points.Count - 2; // Get the last two points

        Vector3 positionOne = points[i].position;
        Vector3 positionTwo = points[i + 1].position;
        Vector2 point1 = new Vector2(positionTwo.x, positionTwo.y);
        Vector2 point2 = new Vector2(positionOne.x, positionOne.y);
        Vector2 midpoint = (point1 + point2) / 2f;

        GameObject line = Instantiate(line_image, transform);
        line.GetComponent<RectTransform>().position = midpoint;

        Vector2 dir = point1 - point2;
        line.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        line.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 0.18f, 1f);
    }

    public void ClearLines()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        points.Clear();
    }

    public void ClearLatestLine()
    {
        if (points.Count > 0)
        {
            points.RemoveAt(points.Count - 1);
        }
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(transform.childCount - 1).gameObject);
        }
    }
}