using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// This class is used to count cash and panache
public class GUICounter
{
    public int value = 0;
    public int valueLeftToAdd = 0;
    private float count = 0;
    private const float countMax = .005f;
    public GUICounter()
    {
        Futile.instance.SignalUpdate += Update;
    }
    public void addAmount(int valueToAdd)
    {
        valueLeftToAdd += valueToAdd;
    }
    private void Update()
    {
        if (valueLeftToAdd > 0)
        {
            while (count > countMax)
            {
                count -= countMax;
                if (valueLeftToAdd != 0)
                {
                    if (valueLeftToAdd > 0)
                    {
                        value++;
                        valueLeftToAdd--;
                    }
                    else
                    {
                        //Subtract cash
                    }
                }
            }
            count += Time.deltaTime;
        }
        else
            count = 0;
    }
}

