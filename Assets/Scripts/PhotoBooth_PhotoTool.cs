using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoBooth_PhotoTool : PhotoTool
{
    public override void Init()
    {
        (width, height) = (Screen.width, Screen.height);
        base.Init();
    }
}
