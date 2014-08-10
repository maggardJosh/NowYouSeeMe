using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PreLevel : BaseScreen
{
    FSprite bg;
    ShadowLabel levelNameLabel;


    bool onScreen = false;
    public PreLevel()
    {
        bg = new FSprite("preLevel");
        this.x = bg.width;
        Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete((b) =>
        {
            onScreen = true;
        }));


        this.AddChild(bg);
        levelNameLabel = new ShadowLabel("Next Level: " + World.getInstance().map.mapName);
        this.AddChild(levelNameLabel);


    }

    bool isTransOff = false;
    protected override void Update()
    {
        if (!onScreen)
            return;
        if (C.getStartPressed() && !isTransOff)
        {
            isTransOff = true;
            Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", -Futile.screen.width).setEaseType(EaseType.BackIn).onComplete((a) => { isDone = true; this.RemoveFromContainer(); }));

        }
        base.Update();
    }
}

