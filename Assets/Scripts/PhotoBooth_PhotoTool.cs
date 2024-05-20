using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoBooth_PhotoTool : PhotoTool
{

    public void Setup(int w, int y)
    {
        width = w;
        height = y;
        Init();
    }
    public void ChangeOrientation()
    {
        (width, height) = (height, width);
        
        Init();
    }
}
