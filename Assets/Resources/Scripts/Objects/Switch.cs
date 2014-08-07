using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Switch : InteractableObject
{
    private string doorName;
    private Door door;
    private string actionType;
    public Switch(Vector2 pos, string doorName, string actionType)
    {
        this.X_INTERACT_DIST = 24;
        this.SetPosition(pos);
        this.doorName = doorName;
        this.actionType = actionType.ToLower();
        interactSprite = new FAnimatedSprite("Switch/switch");
        interactSprite.addAnimation(new FAnimation("interactable", new int[] { 1 }, 100, true));
        interactSprite.addAnimation(new FAnimation("uninteractable", new int[] { 2 }, 100, true));
        interactSprite.addAnimation(new FAnimation("hover", new int[] { 3 }, 100, true));

        this.AddChild(interactSprite);
    }
    float particleXSpeed = 3;
    float particleYSpeed = 3;
    int numParticles = 2;

    public void findDoor(List<Door> doorList)
    {
        foreach(Door d in doorList)
            if (d.name.CompareTo(doorName) == 0)
            {
                this.door = d;
                return;
            }

        RXDebug.Log("No door " + doorName + " found");
    }

    float particleDist = 2;
    public override void interact(Player p)
    {
        switch(actionType)
        {
            case "toggle":
                if (this.door != null)
                    door.interact(p);
                break;
            case "open":

                break;
            case "close":

                break;
        }
        
        for (int x = 0; x < numParticles; x++)
        {
            VanishParticle particle = VanishParticle.getParticle();
            float angle = (RXRandom.Float() * Mathf.PI * 2);
            Vector2 pos = this.GetPosition() + new Vector2(Mathf.Cos(angle) * particleDist, Mathf.Sin(angle) * particleDist);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
            this.container.AddChild(particle);
        }
        interactSprite.play("interactable");

    }
}

