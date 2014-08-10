using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerGUI : FCamObject
{
    FSprite cashBG;
    FSprite panacheBG;
    ShadowLabel cashCounter;
    ShadowLabel panacheCounter;
    FSprite vanishBarBG;
    FWipeSprite markCounter;
    FSprite markMedallion;
    Player p;
    FAnimatedSprite panacheAnim;
    float guiSideMargin = 1;
    float textVertMargin = 10;

    public PlayerGUI()
    {
        cashBG = new FSprite("moneyBar_01");
        cashBG.x = -Futile.screen.halfWidth + cashBG.width / 2 + guiSideMargin;
        cashBG.y = Futile.screen.halfHeight - cashBG.height / 2 - guiSideMargin;
        this.AddChild(cashBG);


        cashCounter = new ShadowLabel("$0");
        cashCounter.anchorX = 1;
        this.AddChild(cashCounter);
        cashCounter.SetPosition(cashBG.GetPosition() + new Vector2(22, -5));

        vanishBarBG = new FSprite("vanishBar_01");
        markCounter = new FWipeSprite("vanishBar_02");
        markCounter.SetPosition(new Vector2(0, Futile.screen.halfHeight - markCounter.height / 2 - guiSideMargin));
        vanishBarBG.SetPosition(markCounter.GetPosition());
        this.AddChild(vanishBarBG);
        this.AddChild(markCounter);

        panacheBG = new FSprite("panacheBar_01");
        panacheBG.x = Futile.screen.halfWidth - panacheBG.width / 2 - guiSideMargin;
        panacheBG.y = Futile.screen.halfHeight - panacheBG.height / 2 - guiSideMargin;
        this.AddChild(panacheBG);

        panacheAnim = new FAnimatedSprite("panacheBar");
        panacheAnim.addAnimation(new FAnimation("idle", new int[] { 1, 2, 3, 4, 5, 6, 7, 1 }, 100, false));
        panacheAnim.play("idle");
        panacheAnim.SetPosition(panacheBG.GetPosition());
        this.AddChild(panacheAnim);

        panacheCounter = new ShadowLabel("0");
        panacheCounter.anchorX = 0;
        panacheCounter.SetPosition(panacheBG.GetPosition() + new Vector2(-22, -5));
        this.AddChild(panacheCounter);

    }

    BaseScreen currentScreen;
    public void endLevel(string nextLevel)
    {
        currentScreen = new LevelEnd(nextLevel, p.cashCounter.actualValue, p.panacheCounter.actualValue);
        this.AddChild(currentScreen);
        C.isTransitioning = true;

    }

    public void showPreLevel()
    {
        currentScreen = new PreLevel();
        this.AddChild(currentScreen);
        C.isTransitioning = true;
    }

    public void setPlayer(Player p)
    {
        this.p = p;
    }

    int lastPanache = 0;
    public override void Update()
    {
        base.Update();
        if (C.isTransitioning)
        {
            if (currentScreen != null && currentScreen.isDone)
            {
                if (currentScreen is LevelEnd)
                {
                    showPreLevel();
                }
                else
                {

                    C.isTransitioning = false;
                }
            }
            return;
        }
        if (p == null)
            return;
        cashCounter.text = "$" + p.cashCounter.value.ToString();
        panacheCounter.text = p.panacheCounter.value.ToString();

        if (lastPanache < p.panacheCounter.actualValue)
        {
            lastPanache = p.panacheCounter.actualValue;
            panacheAnim.play("idle", true);
        }


        markCounter.wipeLeftAmount = p.GetVanishPercent();
    }

    internal void startGame()
    {
        currentScreen = new StartGameScreen();
        this.AddChild(currentScreen);
        C.isTransitioning = true;
    }
}

