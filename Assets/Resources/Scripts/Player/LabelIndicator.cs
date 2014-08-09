using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LabelIndicator : ShadowLabel
{
    const float IND_TIME = 1.5f;
    const float IND_UP_DIST = 30.0f;
    const float IND_NEG_DIST = 4;
    FSprite bg;
    bool negative;
    float count = 0;
    public LabelIndicator(string text, string elementName = "textBG", bool negative = false)
        : base(text)
    {
        this.negative = negative;
        bg = new FSprite(elementName);
        Go.to(this, IND_TIME, new TweenConfig().floatProp("y", IND_UP_DIST, true).setEaseType(EaseType.QuadOut).onComplete((a) => { this.RemoveFromContainer(); bg.RemoveFromContainer(); }));
        this.AddChild(bg);
        bg.MoveToBack();
    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    public override void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= Update;
        base.HandleRemovedFromStage();
    }

    float lastXInc = 0;
    private void Update()
    {
        if (negative)
        {
            this.x -= lastXInc;
            lastXInc = (RXRandom.Float() * IND_NEG_DIST * 2 - IND_NEG_DIST / 2) * Math.Max(0,(1 - count / (IND_TIME/2)));
            this.x += lastXInc;

        }
        count += Time.deltaTime;

    }
}

