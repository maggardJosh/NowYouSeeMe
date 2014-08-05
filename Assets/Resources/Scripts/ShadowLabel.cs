using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ShadowLabel : FContainer
{
    FLabel label;
    FLabel shadow;
    public ShadowLabel(string text)
    {
        label = new FLabel(C.smallFontName, text);
        shadow = new FLabel(C.smallDarkFontName, text);
        shadow.SetPosition(1, 0);
        this.AddChild(shadow);
        this.AddChild(label);
    }

    public float anchorX { set { label.anchorX = value; shadow.anchorX = value; } get { return label.anchorX; } }
    public Rect textRect { set { } get { return label.textRect; } }
    public string text { set { label.text = value; shadow.text = value; } get { return label.text; } }
}

