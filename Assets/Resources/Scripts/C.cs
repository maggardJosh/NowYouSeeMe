using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class C
{
    public const string versionNumber = "v0.5";

    public static float sceneTransitionTime = 1.5f;
    public static bool isSpawning = false;
    public static bool isTransitioning = false;

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

    public static readonly KeyCode[] LEFT_KEY = new KeyCode[] { KeyCode.LeftArrow, KeyCode.A };
    public static readonly KeyCode[] RIGHT_KEY = new KeyCode[] { KeyCode.RightArrow, KeyCode.D };
    public static readonly KeyCode[] UP_KEY = new KeyCode[] { KeyCode.UpArrow, KeyCode.W };
    public static readonly KeyCode[] DOWN_KEY = new KeyCode[] { KeyCode.DownArrow, KeyCode.S };
    public static readonly KeyCode[] JUMP_KEY = new KeyCode[] { KeyCode.X, KeyCode.L };
    public static readonly KeyCode[] ACTION_KEY = new KeyCode[] { KeyCode.Z, KeyCode.K };

    public static bool getLeftPressed() { return getKey(LEFT_KEY); }
    public static bool getRightPressed() { return getKey(RIGHT_KEY); }
    public static bool getUpPressed() { return getKey(UP_KEY); }
    public static bool getDownPressed() { return getKey(DOWN_KEY); }
    public static bool getJumpPressed() { return getKey(JUMP_KEY); }
    public static bool getActionPressed() { return getKey(ACTION_KEY); }

    public static bool getUpPress() { return getKeyDown(UP_KEY); }
    public static bool getJumpPress() { return getKeyDown(JUMP_KEY); }

    private static bool getKey(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKey(key))
                return true;
        return false;
    }

    private static bool getKeyDown(KeyCode[] keys)
    {
        foreach (KeyCode key in keys)
            if (Input.GetKeyDown(key))
                return true;
        return false;
    }

    internal static bool getStartPressed()
    {
        return Input.GetKeyDown(KeyCode.Return);
    }
}
