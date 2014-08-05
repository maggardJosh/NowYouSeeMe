using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class PlayerGUI : FCamObject
{
    FLabel cashCounter;
    FLabel cashLeftToAddCounter;
    FLabel panacheCounter;
    FLabel panacheLeftToAddCounter;
    FRadialWipeSprite markCounter;
    FSprite markMedallion;
    Player p;

    float guiSideMargin = 5;
    float textVertMargin = 10;

    public PlayerGUI()
    {
        cashCounter = new FLabel(C.smallFontName, "$0");
        cashCounter.anchorX = 1;
        this.AddChild(cashCounter);
        cashCounter.SetPosition(new Vector2(-Futile.screen.halfWidth / 2, Futile.screen.halfHeight - cashCounter.textRect.height/2 - guiSideMargin));
        
        cashLeftToAddCounter = new FLabel(C.smallFontName, "+0");
        cashLeftToAddCounter.anchorX = 1;
        this.AddChild(cashLeftToAddCounter);
        cashLeftToAddCounter.SetPosition(new Vector2(-Futile.screen.halfWidth / 2, Futile.screen.halfHeight - cashCounter.textRect.height / 2 - guiSideMargin - textVertMargin));
        cashLeftToAddCounter.isVisible = false;

        panacheCounter = new FLabel(C.smallFontName, "0");
        panacheCounter.anchorX = 0;
        this.AddChild(panacheCounter);
        panacheCounter.SetPosition(new Vector2(Futile.screen.halfWidth / 2, Futile.screen.halfHeight - cashCounter.textRect.height / 2 - guiSideMargin));

        panacheLeftToAddCounter = new FLabel(C.smallFontName, "+0");
        panacheLeftToAddCounter.anchorX = 0;
        this.AddChild(panacheLeftToAddCounter);
        panacheLeftToAddCounter.SetPosition(new Vector2(Futile.screen.halfWidth / 2, Futile.screen.halfHeight - cashCounter.textRect.height / 2 - guiSideMargin - textVertMargin));
        panacheLeftToAddCounter.isVisible = false;
    }

    public void setPlayer(Player p)
    {
        this.p = p;
    }

    public override void Update()
    {
        base.Update();
        if (p == null)
            return;
        cashCounter.text = "$ " + p.cashCounter.value.ToString();
        cashLeftToAddCounter.text = (p.cashCounter.valueLeftToAdd >= 0 ? "+" : "-") + p.cashCounter.valueLeftToAdd;
        cashLeftToAddCounter.isVisible = p.cashCounter.valueLeftToAdd != 0;

        panacheCounter .text = p.panacheCounter.value.ToString();
        panacheLeftToAddCounter.text = (p.panacheCounter.valueLeftToAdd >= 0 ? "+" : "-") + p.panacheCounter.valueLeftToAdd;
        panacheLeftToAddCounter.isVisible = p.panacheCounter.valueLeftToAdd != 0;
    }
}

