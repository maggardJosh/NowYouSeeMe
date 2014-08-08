using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StartGameScreen : BaseScreen
{
    private ShadowLabel pressEnter;
    public StartGameScreen()
    {
        FSprite splashScreen = new FSprite("splashScreen");
        this.AddChild(splashScreen);

        pressEnter = new ShadowLabel("PRESS ENTER");
        pressEnter.y = -Futile.screen.halfHeight + 25;
        pressEnter.isVisible = false;
        this.AddChild(pressEnter);

        ShadowLabel versionNumber = new ShadowLabel(C.versionNumber);
        versionNumber.y = -Futile.screen.halfHeight + versionNumber.textRect.height / 2 + 2;
        this.AddChild(versionNumber);
    }

    private float count = 0;
    bool transitioningOff = false;
    protected override void Update()
    {
        if (transitioningOff)
            return;
        count += Time.deltaTime;
        pressEnter.isVisible = count > 1.0f && ((int)count) % 3 != 0;
        if (Input.GetKeyUp(C.ACTION_KEY))
        {
            World world = World.getInstance();
            world.LoadMap("testMap");
            Futile.stage.AddChild(world);
            transitioningOff = true;
            C.getCameraInstance().MoveToFront();
            Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", -Futile.screen.width).setEaseType(EaseType.BackIn).onComplete((a) => { C.getCameraInstance().MoveToFront(); isDone = true; }));
        }
        base.Update();
    }
}

