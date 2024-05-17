using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projecting : MonoBehaviour
{
    public Camera modelCamera;

    public RenderTexture renderTexture;

    public RawImage rawImage;
    
    // Start is called before the first frame update
    void Start()
    {
        modelCamera.targetTexture = renderTexture;

        rawImage.texture = renderTexture;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
