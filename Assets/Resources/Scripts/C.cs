﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class C
{
    private static FCamObject cameraInstance;
    public static FCamObject getCameraInstance()
    {
        if (cameraInstance == null)
        {
            cameraInstance = new FCamObject();
            Futile.stage.AddChild(cameraInstance);
        }
        return cameraInstance;
    }

    public const KeyCode LEFT_KEY = KeyCode.A;
    public const KeyCode RIGHT_KEY = KeyCode.D;
    public const KeyCode UP_KEY = KeyCode.W;
    public const KeyCode DOWN_KEY = KeyCode.S;
    public const KeyCode JUMP_KEY = KeyCode.Space;
    public const KeyCode ACTION_KEY = KeyCode.Return;
}