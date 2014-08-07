using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class C
{
    public const string versionNumber = "v0.1.21";

    public static bool isSpawning = false;

    private static PlayerGUI cameraInstance;
    public static PlayerGUI getCameraInstance()
    {
        if (cameraInstance == null)
        {
            cameraInstance = new PlayerGUI();
            Futile.stage.AddChild(cameraInstance);
        }
        return cameraInstance;
    }

    public const string smallFontName = "smallFont";
    public const string smallDarkFontName = "smallFontDark";

    public const KeyCode LEFT_KEY = KeyCode.LeftArrow;
    public const KeyCode RIGHT_KEY = KeyCode.RightArrow;
    public const KeyCode UP_KEY = KeyCode.UpArrow;
    public const KeyCode DOWN_KEY = KeyCode.DownArrow;
    public const KeyCode JUMP_KEY = KeyCode.A;
    public const KeyCode ACTION_KEY = KeyCode.S;
}
