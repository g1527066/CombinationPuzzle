using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointCollision : MonoBehaviour {

    public POINT point;

    public void SettingCollistion(POINT p,float Size)
    {
        point = p;
        GetComponent<RectTransform>().sizeDelta = new Vector2(Size, Size);
        GetComponent<BoxCollider2D>().size = new Vector2(Size, Size);
    }
}
