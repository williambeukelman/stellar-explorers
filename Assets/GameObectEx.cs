using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectEx
{
    public static void DrawCircle(this GameObject container, float radius, float lineWidth, Color[] colors = null)
    {
        Color c1;
        if(colors != null){
            c1 = colors[0];
        } else{
            c1 = new Color(255, 255, 255, 0.05f);
        }
        //Color c1 = new Color(255, 255, 255, 0.05f);
        var segments = 360;
        var line = container.GetComponent<LineRenderer>();
        if(line == null){
            line = container.AddComponent<LineRenderer>();
        }
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = c1;
        line.endColor = c1;

        var pointCount = segments + 1;
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);
    }
}
