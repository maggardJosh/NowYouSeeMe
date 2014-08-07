using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Container : InteractableObject
{
    private int money;

    public Container(Vector2 pos, int type, int money)
    {
        this.X_INTERACT_DIST = 24;
        this.SetPosition(pos);
        this.money = money;
        interactSprite = new FAnimatedSprite("Containers/container" + type);
        interactSprite.addAnimation(new FAnimation("interactable", new int[] { 1 }, 100, true));
        interactSprite.addAnimation(new FAnimation("uninteractable", new int[] { 2 }, 100, true));
        interactSprite.addAnimation(new FAnimation("hover", new int[] { 3 }, 100, true));

        this.AddChild(interactSprite);
    }
    float particleXSpeed = 30;
    float particleYSpeed = 30;
    int numParticles = 20;

    float particleDist = 15;
    public override void interact(Player p)
    {
        p.addCash(money);
        interactable = false;
        for (int x = 0; x < numParticles; x++)
        {
            VanishParticle particle = VanishParticle.getParticle();
            float angle = (RXRandom.Float() * Mathf.PI * 2);
            Vector2 pos = this.GetPosition() + new Vector2(Mathf.Cos(angle) * particleDist, Mathf.Sin(angle) * particleDist);
            particle.activate(pos, new Vector2(RXRandom.Float() * particleXSpeed * 2 - particleXSpeed, RXRandom.Float() * particleYSpeed * 2 - particleYSpeed), Vector2.zero, 360);
            this.container.AddChild(particle);
        }
        interactSprite.play("uninteractable");

    }
}

