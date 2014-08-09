using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Stairwell : FContainer
{
    public IndividualStairwell[] individualStairwells = new IndividualStairwell[2];
    
    public Stairwell(Vector2 v, Vector2 v2)
    {
        individualStairwells[0] = new IndividualStairwell(v, this);
        individualStairwells[1] = new IndividualStairwell(v2, this);

        for (int x = 0; x < 2; x++)
            this.AddChild(individualStairwells[x]);
    }

    public void interact(Player p, IndividualStairwell s)
    {

    }
}

public class IndividualStairwell : InteractableObject
{
    Stairwell owner;
    public IndividualStairwell(Vector2 pos, Stairwell owner)
    {
        this.owner = owner;
        this.SetPosition(pos);
        this.X_INTERACT_DIST = 24;
        this.Y_INTERACT_DIST = 36;
        interactSprite = new FAnimatedSprite("StairWell/stairWell");
        interactSprite.addAnimation(new FAnimation("interactable", new int[] { 1 }, 200, true));
        interactSprite.addAnimation(new FAnimation("hover", new int[] { 1 }, 200, true));
        interactSprite.play("interactable", true);
        this.AddChild(interactSprite);

    }

    public override void interact(Player p)
    {
        owner.interact(p, this);
    }
}
