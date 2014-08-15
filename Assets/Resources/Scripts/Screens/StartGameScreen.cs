using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StartGameScreen : BaseScreen
{
    private ShadowLabel pressEnter;
    private FSprite enterBG;
    public StartGameScreen()
    {
        FSprite splashScreen = new FSprite("splashScreen");
        this.AddChild(splashScreen);

        pressEnter = new ShadowLabel("PRESS ENTER");
        pressEnter.y = -Futile.screen.halfHeight + 30;
        pressEnter.isVisible = false;

        enterBG = new FSprite("textBG");
        enterBG.SetPosition(pressEnter.GetPosition());
        enterBG.width = pressEnter.textRect.width * 1.4f;
        this.AddChild(enterBG);
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
        pressEnter.isVisible = count > 2.0f && ((int)(count* 2)) % 4 != 0;
        enterBG.isVisible = pressEnter.isVisible;
        if (C.getStartPressed())
        {
            playBlip();
            World world = World.getInstance();
            world.LoadMap("tutorial");
            Futile.stage.AddChild(world);
            transitioningOff = true;
            C.getCameraInstance().MoveToFront();
            Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", -Futile.screen.width).setEaseType(EaseType.BackIn).onComplete((a) => { C.getCameraInstance().MoveToFront(); isDone = true; World.getInstance().player.spawn(); this.RemoveFromContainer(); }));
            
        }
        base.Update();
    }
}

