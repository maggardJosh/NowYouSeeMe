using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PreLevel : BaseScreen
{
    FSprite bg;
    ShadowLabel levelNameLabel;
    ShadowLabel descriptionLabel;
    ShadowLabel cashInLevelLabel;

    ShadowLabel pressEnter;
    bool onScreen = false;
    int cash = 0;
    public int cashCount { get; set; }
    public PreLevel()
    {
        bg = new FSprite("preLevel");
        this.x = bg.width;
        Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete((b) =>
        {
            onScreen = true;
             Go.to(this, 2.0f, new TweenConfig().intProp("cashCount", this.cash).onComplete((a) => { countersDone = true; }));
        }));
        pressEnter = new ShadowLabel("PRESS ENTER");
        pressEnter.y = -Futile.screen.halfHeight + 20;
        pressEnter.isVisible = false;
        this.AddChild(pressEnter);

        this.AddChild(bg);
        levelNameLabel = new ShadowLabel(World.getInstance().map.mapName);
        this.AddChild(levelNameLabel);
        levelNameLabel.y = Futile.screen.halfHeight / 2;

        descriptionLabel = new ShadowLabel(World.getInstance().map.mapDescription.Replace("\\n", "\n"));
        this.AddChild(descriptionLabel);

        cashInLevelLabel = new ShadowLabel("Available Cash: $0");
        cashInLevelLabel.y = -Futile.screen.halfHeight / 2;
        this.AddChild(cashInLevelLabel);

        cash = World.getInstance().availableCash;

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
        cashInLevelLabel.text = "Available Cash: $" + cashCount;

        if (C.getStartPressed() && !isTransOff)
        {
            if (!countersDone)
            {
                countersDone = true;
                Go.killAllTweensWithTarget(this);
                cashCount = cash;
            }
            else
            {

                isTransOff = true;
                Go.killAllTweensWithTarget(FSoundManager.musicSource);
                Go.to(FSoundManager.musicSource, C.sceneTransitionTime, new TweenConfig().floatProp("volume", 1));
                Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", -Futile.screen.width).setEaseType(EaseType.BackIn).onComplete((a) => { isDone = true; this.RemoveFromContainer(); }));
                playBlip();
            }

        }
        base.Update();
    }
}

