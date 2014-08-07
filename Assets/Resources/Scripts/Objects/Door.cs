using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class Door : InteractableObject
{
    public Door(Vector2 pos)
    {
        this.SetPosition(pos);
        toggleable = true;
        on = false;
        interactSprite = new FAnimatedSprite("Door/door");
        interactSprite.addAnimation(new FAnimation("off", new int[] { 1 }, 100, false));
        interactSprite.addAnimation(new FAnimation("on", new int[] { 2 },100, false));
        interactSprite.addAnimation(new FAnimation("offHover", new int[] { 5,6 }, 100, true));
        interactSprite.addAnimation(new FAnimation("onHover", new int[] { 3,4 }, 100, true));
        this.AddChild(interactSprite);
    }
    float particleXSpeed = 20;
    float particleYSpeed = 20;
    int numParticles = 10;

    public override void interact(Player p)
    {
        on = !on;
        if (p.x > this.x)
            interactSprite.scaleX = -1;
        else
            interactSprite.scaleX = 1;
        this.interactSprite.play(on ? "on" : "off");

        for (int x = 0; x < numParticles; x++)
        {
            VanishParticle particle = VanishParticle.getParticle();
            
            Vector2 pos = this.GetPosition() + new Vector2(0, RXRandom.Float() * 32 - 16);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
            this.container.AddChild(particle);
        }

    }
}

