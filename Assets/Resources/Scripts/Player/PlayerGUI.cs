using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerGUI : FCamObject
{
    FSprite cashBG;
    ShadowLabel cashCounter;
    ShadowLabel cashLeftToAddCounter;
    ShadowLabel panacheCounter;
    ShadowLabel panacheLeftToAddCounter;
    FSprite vanishBarBG;
    FWipeSprite markCounter;
    FSprite markMedallion;
    Player p;

    float guiSideMargin = 5;
    float textVertMargin = 10;

    public PlayerGUI()
    {
        cashBG = new FSprite("tilemap_02");
        cashBG.width = Futile.screen.width;
        cashBG.height = cashBG.height * 2;
        
        this.AddChild(cashBG);


        cashCounter = new ShadowLabel("$0");
        cashCounter.anchorX = 1;
        this.AddChild(cashCounter);
        cashCounter.SetPosition(new Vector2(-Futile.screen.halfWidth / 2, Futile.screen.halfHeight - cashCounter.textRect.height/2 - guiSideMargin));

        cashLeftToAddCounter = new ShadowLabel("+0");
        cashLeftToAddCounter.anchorX = 1;
        this.AddChild(cashLeftToAddCounter);
        cashLeftToAddCounter.SetPosition(new Vector2(-Futile.screen.halfWidth / 2, Futile.screen.halfHeight - cashCounter.textRect.height / 2 - guiSideMargin - textVertMargin));
        cashLeftToAddCounter.isVisible = false;
        cashBG.y = (cashCounter.y + cashLeftToAddCounter.y)/2;

        panacheCounter = new ShadowLabel("0");
        panacheCounter.anchorX = 0;
        this.AddChild(panacheCounter);
        panacheCounter.SetPosition(new Vector2(Futile.screen.halfWidth / 2, Futile.screen.halfHeight - cashCounter.textRect.height / 2 - guiSideMargin));

        panacheLeftToAddCounter = new ShadowLabel("+0");
        panacheLeftToAddCounter.anchorX = 0;
        this.AddChild(panacheLeftToAddCounter);
        panacheLeftToAddCounter.SetPosition(new Vector2(Futile.screen.halfWidth / 2, Futile.screen.halfHeight - cashCounter.textRect.height / 2 - guiSideMargin - textVertMargin));
        panacheLeftToAddCounter.isVisible = false;

        vanishBarBG = new FSprite("vanishBar_01");
        markCounter = new FWipeSprite("vanishBar_02");
        markCounter.SetPosition(new Vector2(0, Futile.screen.halfHeight - markCounter.height / 2 - guiSideMargin));
        vanishBarBG.SetPosition(markCounter.GetPosition());
        this.AddChild(vanishBarBG);
        this.AddChild(markCounter);

    }

    BaseScreen currentScreen;
    public void endLevel()
    {
        currentScreen = new LevelEnd();
        this.AddChild(currentScreen);
        C.isTransitioning = true;
        
    }

    public void setPlayer(Player p)
    {
        this.p = p;
    }

    public override void Update()
    {
        base.Update();
        if (C.isTransitioning)
        {
            if (currentScreen.isDone)
                C.isTransitioning = false;
            return;
        }
        if (p == null)
            return;
        cashCounter.text = "$ " + p.cashCounter.value.ToString();
        cashLeftToAddCounter.text = (p.cashCounter.valueLeftToAdd >= 0 ? "+" : "-") + p.cashCounter.valueLeftToAdd;
        cashLeftToAddCounter.isVisible = p.cashCounter.valueLeftToAdd != 0;

        panacheCounter .text = p.panacheCounter.value.ToString();
        panacheLeftToAddCounter.text = (p.panacheCounter.valueLeftToAdd >= 0 ? "+" : "-") + p.panacheCounter.valueLeftToAdd;
        panacheLeftToAddCounter.isVisible = p.panacheCounter.valueLeftToAdd != 0;

        markCounter.SetElementByName(p.currentState == Player.State.COOLDOWN ? "vanishBar_03" : "vanishBar_02");
        markCounter.wipeLeftAmount = p.GetVanishPercent();
    }
}

