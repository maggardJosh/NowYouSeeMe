using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Particle : FAnimatedSprite
{  
    Vector2 vel;
    float rot;
    Vector2 accel;
    public bool isActive = false;
    int animRandom = 180;
    int animBaseSpeed = 100;
    private Particle(string elementName)
        : base(elementName)
    {
        this.isVisible = false;
    }

    public void activate(Vector2 pos, Vector2 vel, Vector2 accel, float rot)
    {
        this.isVisible = true;
        this.isActive = true;
        this.vel = vel;
        this.SetPosition(pos);
        this.rot = rot;
        this.accel = accel;
        this.play("active", true);
        this.currentAnim.delay = animBaseSpeed + (int)(RXRandom.Float() * animRandom);
    }

    public override void Update()
    {
        this.x += vel.x * Time.deltaTime;
        this.y += vel.y * Time.deltaTime;
        vel += accel * Time.deltaTime;

        this.rotation += rot * Time.deltaTime;

        if (this.IsStopped)
        {
            this.RemoveFromContainer();
            this.isActive = false;
        }

        base.Update();
    }
    public class VanishParticle : Particle
    {
        
        private static VanishParticle[] particleList;
        const int MAX_PARTICLES = 100;
        public static VanishParticle getParticle()
        {

            if (particleList == null)
                particleList = new VanishParticle[MAX_PARTICLES];
            for (int x = 0; x < particleList.Length; x++)
            {
                if (particleList[x] == null)
                {
                    VanishParticle p = new VanishParticle();
                    particleList[x] = p;
                    return p;
                }
                else if (!particleList[x].isActive)
                {
                    return particleList[x];
                }
            }
            return particleList[RXRandom.Int(MAX_PARTICLES)];

        }

        private VanishParticle() : base("vanishParticle0" + (RXRandom.Int(3) + 1))
        {

            this.animRandom = 180;
            this.animBaseSpeed = 100;
            this.addAnimation(new FAnimation("active", new int[] { 1, 2, 3, 4 }, animBaseSpeed + (int)(RXRandom.Float() * animRandom), false)); 
        }
    }


    public class SparkleParticle : Particle
    {
        private static SparkleParticle[] particleList;
        const int MAX_PARTICLES = 100;
        public static SparkleParticle getParticle()
        {
            if (particleList == null)
                particleList = new SparkleParticle[MAX_PARTICLES];
            for (int x = 0; x < particleList.Length; x++)
            {
                if (particleList[x] == null)
                {
                    SparkleParticle p = new SparkleParticle();
                    particleList[x] = p;
                    return p;
                }
                else if (!particleList[x].isActive)
                {
                    return particleList[x];
                }
            }
            return particleList[RXRandom.Int(MAX_PARTICLES)];

        }

        private SparkleParticle()
            : base("sparkleParticle")
        {
            this.animBaseSpeed = 100;
            this.animRandom = 10;
            this.addAnimation(new FAnimation("active", new int[] { 1, 2, 3, 4, 5}, animBaseSpeed + (int)(RXRandom.Float() * animRandom), false));
        }
    }

}

