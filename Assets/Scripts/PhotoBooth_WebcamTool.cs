using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoBooth_WebcamTool : WebcamTool
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
        
        webcamTexture = new WebCamTexture(deviceName, width, height);
        webcamTexture.Play();
        if (targetMaterial != null)
            SetToMaterial(targetMaterial);
    }
}
