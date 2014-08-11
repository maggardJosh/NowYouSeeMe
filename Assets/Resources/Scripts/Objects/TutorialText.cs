using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TutorialText : FContainer
{
    FSprite tutorialBG;
    private enum Type
    {
        SKIP,
        MOVE,
        JUMP,
        VANISH,
        DOWN_JUMP,
        SWITCHES,
        LOOT,
        CHEST,
        RESPAWN
    }
    private Type tutorialType = Type.MOVE;
    private Rect area;
    private FSprite text;
    public TutorialText(XMLNode node)
    {
        tutorialBG = new FSprite("tutorialBG");
        text = new FSprite("tutorialMove");
        area = new Rect(float.Parse(node.attributes["x"]), -float.Parse(node.attributes["y"]) - float.Parse(node.attributes["height"]), float.Parse(node.attributes["width"]), float.Parse(node.attributes["height"]));
        this.AddChild(tutorialBG);
        if (node.children.Count > 0)
            foreach (XMLNode property in ((XMLNode)node.children[0]).children)
            {
                if (property.attributes.ContainsKey("name"))
                    switch (property.attributes["name"].ToLower())
                    {
                        case "tutorial":
                            switch (property.attributes["value"].ToLower())
                            {
                                case "move":
                                    tutorialType = Type.MOVE;
                                    text.SetElementByName("tutorialMove");
                                    break;
                                case "jump":
                                    tutorialType = Type.JUMP;
                                    text.SetElementByName("tutorialJump");
                                    break;
                                case "vanish":
                                    tutorialType = Type.VANISH;
                                    text.SetElementByName("tutorialVanish1");
                                    break;
                                case "skip":
                                    tutorialType = Type.SKIP;
                                    text.SetElementByName("tutorialSkip");
                                    break;
                                case "jumpdown":
                                    tutorialType = Type.DOWN_JUMP;
                                    text.SetElementByName("tutorialDownJump");
                                    break;
                                case "switches":
                                    tutorialType = Type.SWITCHES;
                                    text.SetElementByName("tutorialSwitches");
                                    break;
                                case "loot":
                                    text.SetElementByName("tutorialLoot");
                                    break;
                                case "chests":
                                    text.SetElementByName("tutorialCheckPoints");
                                    break;
                                case "respawn":
                                    text.SetElementByName("tutorialRespawn");
                                    break;
                                case "hatjump":
                                    text.SetElementByName("tutorialHatJump");
                                    break;
                                case "enemy":
                                    text.SetElementByName("tutorialGuards");
                                    break;

                            }
                            break;
                    }
            }
        text.y = tutorialBG.height / 2 - TUTORIAL_BG_HEIGHT / 2;
        this.AddChild(text);
        this.y = -Futile.screen.halfHeight - tutorialBG.height / 2;
    }

    private bool isActive = false;
    private const float TUTORIAL_IN_TIME = .8f;
    private const float TUTORIAL_OUT_TIME = .4f;
    private const float TUTORIAL_BG_HEIGHT = 37;
    private void activate()
    {
        if (!isActive)
        {
            Go.killAllTweensWithTarget(this);
            Go.to(this, TUTORIAL_IN_TIME, new TweenConfig().setDelay(TUTORIAL_OUT_TIME).floatProp("y", -Futile.screen.halfHeight + (tutorialBG.height- TUTORIAL_BG_HEIGHT)/2).setEaseType(EaseType.BackOut));
            isActive = true;
        }
    }

    private void deactivate()
    {
        if (isActive)
        {
            Go.killAllTweensWithTarget(this);
            Go.to(this, TUTORIAL_IN_TIME, new TweenConfig().floatProp("y", -Futile.screen.halfHeight -tutorialBG.height/ 2).setEaseType(EaseType.BackIn));
            isActive = false;
        }
    }

    bool solved = false;
    public void Update(Player p)
    {
        if(area.Contains(p.GetPosition()))
            activate();
        else
            deactivate();

        if (isActive)
            switch (this.tutorialType)
            {
                case Type.VANISH:
                    if (!solved && World.getInstance().player.currentState == Player.State.VANISHING)
                    {
                        World.getInstance().openDoor("VanishTutorialLeft");
                        World.getInstance().openDoor("VanishTutorialRight");
                        solved = true;
                    }
                    if (World.getInstance().player.currentState != Player.State.VANISHING)
                        solved = false;
                    if (!World.getInstance().player.isMarking)
                        text.SetElementByName("tutorialVanish1");
                    else if (!World.getInstance().player.hasLeftMarkPos)
                        text.SetElementByName("tutorialVanish2");
                    else
                        text.SetElementByName("tutorialVanish3");
                    break;
            }
    }
}

