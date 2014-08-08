using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Door : InteractableObject
{
    public string name { get; private set; }
    bool locked = false;
    public Door(Vector2 pos, string name, bool locked, bool open)
    {
        this.name = name;
        this.SetPosition(pos);
        this.locked = locked;
        toggleable = true;
        interactable = !locked;
        on = open;
        this.X_INTERACT_DIST = 36;
        this.Y_INTERACT_DIST = 20;
        interactSprite = new FAnimatedSprite("Door/door");
        interactSprite.addAnimation(new FAnimation("off", new int[] { 1 }, 100, false));
        interactSprite.addAnimation(new FAnimation("on", new int[] { 2 }, 100, false));
        interactSprite.addAnimation(new FAnimation("offHover", new int[] { 5, 6 }, 100, true));
        interactSprite.addAnimation(new FAnimation("onHover", new int[] { 3, 4 }, 100, true));
        interactSprite.play(on ? "on" : "off");
        this.AddChild(interactSprite);
    }
    float particleXSpeed = 20;
    float particleYSpeed = 20;
    int numParticles = 10;
    public const float DOOR_COLLISION_WIDTH = 10;

    public void setState(bool open, Player p)
    {
        on = !open;
        interact(p);
    }
    public override void interact(Player p)
    {
        on = !on;
        if (p.x > this.x)
            interactSprite.scaleX = -1;
        else
            interactSprite.scaleX = 1;
        this.interactSprite.play(on ? "on" : "off");

        if (this.y - 16 <= p.y &&
                this.y + 16 >= p.y &&
                this.x + DOOR_COLLISION_WIDTH / 2 > p.x - Player.collisionWidth / 2 &&
                this.x - DOOR_COLLISION_WIDTH / 2 < p.x + Player.collisionWidth / 2)
        {
            if (this.x < p.x)
                p.x = this.x + Player.collisionWidth / 2 + DOOR_COLLISION_WIDTH / 2;
            else
                p.x = this.x - Player.collisionWidth / 2 - DOOR_COLLISION_WIDTH / 2;

        }
        for (int x = 0; x < numParticles; x++)
        {
            VanishParticle particle = VanishParticle.getParticle();

            Vector2 pos = this.GetPosition() + new Vector2(0, RXRandom.Float() * 32 - 16);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
            this.container.AddChild(particle);
        }

    }
}

