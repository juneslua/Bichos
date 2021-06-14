using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Letterbox : MonoBehaviour
{

    private RectTransform _top, _bottom;

    void Awake()
    {
        _top = transform.Find("Top").GetComponent<RectTransform>();
        _bottom = transform.Find("Bottom").GetComponent<RectTransform>();
    }

    void Update() => ActivarLetterbox();

    void ActivarLetterbox()
    {
        Rect _topR = _top.rect, _bottomR = _bottom.rect;
        
        Vector2 medPnt = new Vector2(Screen.width, Screen.height);
        float altoCaja = (medPnt.y  - 9f * medPnt.x / 21.5f) / 2f;
        
        _top.SetSizeWithCurrentAnchors(0, medPnt.x);
        _top.SetSizeWithCurrentAnchors((RectTransform.Axis) 1, altoCaja);
        _bottom.SetSizeWithCurrentAnchors(0, medPnt.x);
        _bottom.SetSizeWithCurrentAnchors((RectTransform.Axis) 1, altoCaja);
    }
}
