using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// This class is used to count cash and panache
public class GUICounter
{
    public int actualValue = 0;
    public int value { get; set; }
    private float count = 0;
    private const float addTime = 1.0f;
    public GUICounter()
    {
        
    }
    public void addAmount(int valueToAdd)
    {
        actualValue += valueToAdd;
        Go.killAllTweensWithTarget(this);
        Go.to(this, addTime, new TweenConfig().intProp("value", actualValue));
    }

    internal void reset()
    {
        value = 0;
        actualValue = 0;
        Go.killAllTweensWithTarget(this);
    }
}

