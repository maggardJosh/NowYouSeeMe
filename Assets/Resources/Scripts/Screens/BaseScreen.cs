using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class BaseScreen : FContainer
{
    public bool isDone = false;
    FSprite bg;
    public BaseScreen()
    {
        bg = new FSprite("levelEnd");
        this.x = bg.width;
        Go.to(this, 2.0f, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BounceOut));
        this.AddChild(bg);
    }

    public override void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += Update;
        base.HandleAddedToStage();
    }

    protected virtual void Update()
    {

    }
}

