using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevelEnd : BaseScreen
{
    FSprite bg;
    ShadowLabel scoreLabel;
    ShadowLabel cashLabel;
    ShadowLabel panacheLabel;
    int cash;
    int panache;
    public int cashCount { get; set; }
    public int panacheCount { get; set; }
    string nextLevel = "";
    bool onScreen = false;
    public LevelEnd(string nextLevel, int cash, int panache)
    {
        this.nextLevel = nextLevel;
        bg = new FSprite("levelEnd");
        this.x = bg.width;
        this.cash = cash;
        this.panache = panache;
        Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete((b) =>
        {
            onScreen = true;
            Go.to(this, 2.0f, new TweenConfig().intProp("cashCount", this.cash).intProp("panacheCount", this.panache).onComplete((a) => { countersDone = true; }));
        }));
        this.AddChild(bg);
        panache = 2000;
        cashLabel = new ShadowLabel("Cash: $0");
        panacheLabel = new ShadowLabel("Panache: 0");
        scoreLabel = new ShadowLabel("Score: 0");
        cashLabel.y = 20;
        scoreLabel.y = -20;
        this.AddChild(cashLabel);
        this.AddChild(scoreLabel);
        this.AddChild(panacheLabel);


    }

    bool isTransOff = false;
    bool countersDone = false;
    protected override void Update()
    {
        if (!onScreen)
            return;
        cashLabel.text = "Cash: $" + cashCount;
        panacheLabel.text = "Panache: " + panacheCount;
        scoreLabel.text = "Score: " + cashCount + " X " +((panacheCount + 1000) / 1000) + " = " + cashCount * ((panacheCount + 1000) /1000);
        if (C.getActionPressed() && !isTransOff)
        {
            
            if (!countersDone)
            {
                countersDone = true;
                Go.killAllTweensWithTarget(this);
                cashCount = cash;
                panacheCount = panache;
            }
            else
            {

                isTransOff = true;
                World.getInstance().LoadMap(nextLevel);
                Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", -Futile.screen.width).setEaseType(EaseType.BackIn).onComplete((a) => { isDone = true; }));
            }
        }
        base.Update();
    }
}

