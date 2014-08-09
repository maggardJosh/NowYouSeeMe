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
        individualStairwells[0] = new IndividualStairwell(v, v2, this);
        individualStairwells[1] = new IndividualStairwell(v2,v, this);

        for (int x = 0; x < 2; x++)
            this.AddChild(individualStairwells[x]);
    }

    public void interact(Player p, IndividualStairwell s)
    {
        IndividualStairwell outStair = s == individualStairwells[0] ? individualStairwells[1] : individualStairwells[0];
        p.enterStairwell(s, outStair);
    }
}

public class IndividualStairwell : InteractableObject
{
    Stairwell owner;
    public bool isFacingLeft = false;
    public IndividualStairwell(Vector2 pos, Vector2 otherPos, Stairwell owner)
    {
        this.owner = owner;
        this.SetPosition(pos);
        this.X_INTERACT_DIST = 24;
        this.Y_INTERACT_DIST = 36;
        isFacingLeft = pos.x > otherPos.x;
        interactSprite = new FAnimatedSprite("StairWell/stairWell");
        int frameNum = pos.y > otherPos.y ? 2 : 1;
        if (pos.x == otherPos.x)
            isFacingLeft = pos.y > otherPos.y ? true : false;
        scaleX = isFacingLeft ? -1 : 1;
        interactSprite.addAnimation(new FAnimation("interactable", new int[] { frameNum }, 200, true));
        interactSprite.addAnimation(new FAnimation("hover", new int[] { frameNum }, 200, true));
        interactSprite.play("interactable", true);
        this.AddChild(interactSprite);

    }

    public override void interact(Player p)
    {
        owner.interact(p, this);
    }
}
