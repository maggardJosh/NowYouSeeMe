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
    FSprite scoreBG;
    FSprite panacheBG;
    FSprite cashBG;
    FSprite enterBG;
    ShadowLabel pressEnter;
    int cash;
    int panache;
    public int cashCount { get; set; }
    public int panacheCount { get; set; }
    string nextLevel = "";
    bool onScreen = false;
    float textBGWidth = 130;
    public LevelEnd(string nextLevel, int cash, int panache)
    {
        this.nextLevel = nextLevel;
        bg = new FSprite("levelEnd");
        this.x = bg.width;
        this.cash = cash;
        this.panache = panache;
        Go.killAllTweensWithTarget(FSoundManager.musicSource);
        Go.to(FSoundManager.musicSource, C.sceneTransitionTime, new TweenConfig().floatProp("volume", .1f));
        Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", 0).setEaseType(EaseType.BackOut).onComplete((b) =>
        {
            onScreen = true;
            Go.to(this, 2.0f, new TweenConfig().intProp("cashCount", this.cash).intProp("panacheCount", this.panache).onComplete((a) => { countersDone = true; }));
        }));
        this.AddChild(bg);

        pressEnter = new ShadowLabel("PRESS ENTER");
        pressEnter.y = -Futile.screen.halfHeight + 20;
        pressEnter.isVisible = false;

        enterBG = new FSprite("textBG");
        enterBG.SetPosition(pressEnter.GetPosition());
        enterBG.width = pressEnter.textRect.width * 1.4f;
        enterBG.isVisible = false;
      //  this.AddChild(enterBG);
        this.AddChild(pressEnter);

        cashBG = new FSprite("textBG");
        cashLabel = new ShadowLabel("Cash: $0");
        cashLabel.y = 20;
        cashBG.width = textBGWidth;
        cashBG.SetPosition(cashLabel.GetPosition());

        panacheBG = new FSprite("textBG");
        panacheLabel = new ShadowLabel("Panache: 0");
        panacheBG.width = textBGWidth;
        panacheBG.SetPosition(panacheLabel.GetPosition());

        scoreBG = new FSprite("textBG");
        scoreLabel = new ShadowLabel("Score: 0");
        scoreLabel.y = -20;
        scoreBG.width = textBGWidth;
        scoreBG.SetPosition(scoreLabel.GetPosition());

        this.AddChild(cashLabel);
        this.AddChild(scoreLabel);
        this.AddChild(panacheLabel);


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
        enterBG.isVisible = pressEnter.isVisible;
        cashLabel.text = "Cash: $" + cashCount;
        panacheLabel.text = "Panache: " + panacheCount;
        scoreLabel.text = "Score: " + cashCount + " X " + ((panacheCount + 1000) / 1000) + " = " + cashCount * ((panacheCount + 1000) / 1000);

        if (C.getStartPressed() && !isTransOff)
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
                playBlip();
                isDone = true; 
                isTransOff = true;
                World.getInstance().LoadMap(nextLevel);
                Go.to(this, C.sceneTransitionTime, new TweenConfig().floatProp("x", -Futile.screen.width).setDelay(C.sceneTransitionTime).setEaseType(EaseType.BackOut).onComplete((a) => { World.getInstance().player.spawn(); this.RemoveFromContainer(); }));
            }
        }
        base.Update();
    }
}

