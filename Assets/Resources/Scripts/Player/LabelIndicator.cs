using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LabelIndicator : ShadowLabel
{
    const float IND_TIME = 1.5f;
    const float IND_UP_DIST = 30.0f;
    public LabelIndicator(string text) : base(text)
    {
        Go.to(this, IND_TIME, new TweenConfig().floatProp("y", IND_UP_DIST, true).setEaseType(EaseType.QuadOut).onComplete((a) => { this.RemoveFromContainer(); }));
    }
}

