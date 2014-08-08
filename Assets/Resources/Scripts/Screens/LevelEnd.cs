using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevelEnd : BaseScreen
{
    FSprite bg;
    FLabel score;
    string nextLevel = "";
    public LevelEnd(string nextLevel)
    {
        this.nextLevel = nextLevel;
        bg = new FSprite("levelEnd");
        this.x = bg.width;
        Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut));
        this.AddChild(bg);
    }

    bool isTransOff = false;
    protected override void Update()
    {
        if (Input.GetKeyDown(C.ACTION_KEY) && !isTransOff)
        {
            isTransOff = true;
            World.getInstance().LoadMap(nextLevel);
            Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", -Futile.screen.width).setEaseType(EaseType.BackIn).onComplete((a) => { isDone = true; }));
        }
        base.Update();
    }
}

