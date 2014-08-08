using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Switch : InteractableObject
{
    private string[] doorNames;
    private List<Door> doors;
    private string actionType;
    private float time;
    protected float timeCount = 0;
    FRadialWipeSprite timer;
    public Switch(Vector2 pos, string doorName, string actionType, float time)
    {
        this.X_INTERACT_DIST = 24;
        this.SetPosition(pos);
        this.doorNames = doorName.Split(',');
        this.actionType = actionType.ToLower();
        this.time = time;
        interactSprite = new FAnimatedSprite("Switch/switch");
        interactSprite.addAnimation(new FAnimation("interactable", new int[] { 1 }, 100, true));
        interactSprite.addAnimation(new FAnimation("uninteractable", new int[] { 2 }, 100, true));
        interactSprite.addAnimation(new FAnimation("hover", new int[] { 3 }, 100, true));

        if (time > 0)
        {
            timer = new FRadialWipeSprite("collision_01", true, 0, 1);
          //  this.AddChild(timer);
            timer.isVisible = false;
        }
        this.AddChild(interactSprite);
    }
    float particleXSpeed = 3;
    float particleYSpeed = 3;
    int numParticles = 2;

    public void findDoor(List<Door> doorList)
    {
        doors = new List<Door>();
        foreach (Door d in doorList)
            foreach (string doorName in doorNames)
                if (d.name.CompareTo(doorName) == 0)
                    this.doors.Add(d);
    }

    float particleDist = 2;
    protected bool triggered = false;
    public override void interact(Player p)
    {
        switch (actionType)
        {
            case "toggle":
                foreach(Door door in doors)
                    door.interact(p);
                break;
            case "open":
                foreach (Door door in doors)
                    door.setState(true, p);
                break;
            case "close":
                foreach (Door door in doors)
                    door.setState(false, p);
                break;
        }

        timeCount = 0;
        if (time > 0)
        {
            timer.isVisible = true;
            if (!triggered)
            {
                Futile.instance.SignalUpdate += UpdateTimer;
                this.p = p;
            }
            triggered = true;
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

    Player p;
    private void UpdateTimer()
    {
        timer.percentage = 1 - (timeCount / time);
        timeCount += Time.deltaTime;
        if (timeCount > time)
        {
            Futile.instance.SignalUpdate -= UpdateTimer;
            triggered = false;

            switch (actionType)
            {
                case "toggle":
                    foreach (Door door in doors)
                        door.interact(p);
                    break;
                case "open":
                    foreach (Door door in doors)
                        door.setState(false, p);
                    break;
                case "close":
                    foreach (Door door in doors)
                        door.setState(true, p);
                    break;
            }
        }
    }


}

