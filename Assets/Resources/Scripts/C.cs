using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
