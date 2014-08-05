using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LabelIndicator : ShadowLabel
{
    float count = 0;
    const float IND_TIME = 2.0f;
    const float IND_UP_SPEED = 10.0f;
    public LabelIndicator(string text) : base(text)
    {

    }
    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    private void Update()
    {
        if (count < IND_TIME)
        {
            count += Time.deltaTime;
            y += IND_UP_SPEED * Time.deltaTime;
        }
        else
        {
            this.RemoveFromContainer();
            Futile.instance.SignalUpdate -= Update;
        }
    }
}

