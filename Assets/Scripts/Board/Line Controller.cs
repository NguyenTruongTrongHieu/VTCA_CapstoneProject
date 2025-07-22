using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private List<RectTransform> points = new List<RectTransform>();

 

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
    }

    public void SetUpLine(RectTransform foodTransform)
    {
        this.points.Add(foodTransform);
        lineRenderer.positionCount = points.Count;
        

        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i].position);
        }
    }

    //private void Update()
    //{
    //    for (int i = 0; i < points.Count; i++)
    //    {
    //        lineRenderer.SetPosition(i, points[i].position);
    //    }
    //}
}
