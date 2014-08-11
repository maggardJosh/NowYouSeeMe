using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Credits : BaseScreen
{
    FSprite bg;
    ShadowLabel pressEnter;
    bool onScreen = false;
    public Credits()
    {
        bg = new FSprite("creditsScreen");
        this.x = bg.width;
        Go.killAllTweensWithTarget(FSoundManager.musicSource);
        Go.to(FSoundManager.musicSource, C.sceneTransitionTime, new TweenConfig().floatProp("volume", .1f));
        Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete((b) =>
        {
            onScreen = true;
        }));
        this.AddChild(bg);

        pressEnter = new ShadowLabel("PRESS ENTER");
        pressEnter.y = -Futile.screen.halfHeight + 20;
        pressEnter.isVisible = false;

        this.AddChild(pressEnter);


    }

    bool isTransOff = false;
    bool countersDone = false;
    float count = 0;
    protected override void Update()
    {
        if (!onScreen)
            return;
        count += Time.deltaTime;
        pressEnter.isVisible = count > 2.0f && ((int)(count * 2)) % 4 != 0;

        if (C.getStartPressed() && !isTransOff)
        {
            playBlip();
            isTransOff = true;
            Go.killAllTweensWithTarget(FSoundManager.musicSource);
            Go.to(FSoundManager.musicSource, C.sceneTransitionTime, new TweenConfig().floatProp("volume", 1));
            Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", -Futile.screen.width).setDelay(C.sceneTransitionTime).setEaseType(EaseType.BackOut).onComplete((a) => { isDone = true; this.RemoveFromContainer(); }));
        }
        base.Update();
    }
}

