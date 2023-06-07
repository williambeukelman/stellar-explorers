using UnityEngine;
using System.Collections;

public static class ExtensionMethods
{
    public static Vector3 PolarToCartesian(this Transform transform, float x, float y, float r)
    {
        Vector3 cartesian;
        var scale = Mathf.Rad2Deg; //5.7f*10;
        cartesian.x = r * Mathf.Sin(x/scale) * Mathf.Cos(y/scale);
        cartesian.z = r * Mathf.Sin(x/scale) * Mathf.Sin(y/scale);
        cartesian.y = r * Mathf.Cos(x/scale);
        //cartesian *= Mathf.Deg2Rad;
        return cartesian;
    }

    public static Vector2 CartesianToPolar(this Transform transform, float x, float y, float z, float radius)
    {
        Vector2 polar;
        if(z >= 180){
            polar.x = x-180;
        } else {
            polar.x = x;
        }

        if(y >= 90){
            polar.y = 360-(y-90);
        } else {
            if(z >= 180){
                polar.y = 360+(90-y);
            }
            polar.y = 90-y;
        }
        
        //var scale = 5.7f*10;
        /* Vector3 worldCoord = transform.TransformVector(new Vector3(x, y, z));
        x = worldCoord.x;
        y = worldCoord.y;
        z = worldCoord.z;
        Vector2 polar;
        polar.y = Mathf.Atan2(z*Mathf.Rad2Deg, x*Mathf.Rad2Deg);
        polar.x = y*Mathf.Rad2Deg/radius; */
        
         //(Mathf.Sqrt(Mathf.Pow(x,2) + Mathf.Pow(y,2) + Mathf.Pow(z,2)));
        //polar.x = Mathf.Sqrt(Mathf.Pow(x,2) + Mathf.Pow(y,2));
        //polar.y = Mathf.Atan2(y, x);
        //polar *= Mathf.Rad2Deg;
        /* Vector2 polar;
        polar.y = Mathf.Atan2(x,z)/radius;
        var r = new Vector2(x,z).magnitude; 
        polar.x = Mathf.Atan2(-y,r)/radius;
        polar *= Mathf.Rad2Deg; */
    
        return polar;
    }

}
