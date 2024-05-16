using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoTool : PhotoTool_Base
{
    public override void Init()
    {
        (width, height) = (Screen.width, Screen.height);
        base.Init();
    }
}
