using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class WebcamTool : MonoBehaviour
{

    public string deviceName = "USB Camera";

    public int width = 1920;
    public int height = 1080;

    public Material targetMaterial;

    public bool IsCamReady { get { return isCamReady; } }
    bool isCamReady;

    protected WebCamTexture webcamTexture;
    WebCamDevice[] devices;

    bool isValidCamFound;

    public void Init()
    {
        // List all webcam devices
        ListAllDevices();

        // Open webcam
        WebCamDevice device = devices.ToList().Find(x => x.name == deviceName);

        if (device.name != null)
        {
            webcamTexture = new WebCamTexture(deviceName, width, height);
            webcamTexture.Play();
            isValidCamFound = true;
        }
        else
        {
            device = devices.ToList().Find(x => x.isFrontFacing);
            webcamTexture = new WebCamTexture(device.name, width, height);
            webcamTexture.Play();
            isValidCamFound = true;
            deviceName = device.name;
        }

        if (!isValidCamFound)
        {
            Debug.LogError("Please supply a valid device name");
        }

        // Check if cam is opened
        if (webcamTexture && webcamTexture.isPlaying)
        {
            isCamReady = true;
            Debug.LogWarning("Opened " + deviceName + " with " + webcamTexture.width + ", " + webcamTexture.height);
        }
        else
        {
            isCamReady = false;
            Debug.LogError("Webcam Not Opened: " + deviceName);
        }

        // If targetMaterial is not null, automatically replace its texture with webcamTexture
        if (targetMaterial != null)
            SetToMaterial(targetMaterial);

    }

    void ListAllDevices()
    {
        devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.LogWarning("Device detected: " + devices[i].name);
        }
    }

    public Texture GetWebcamTex()
    {
        if (isCamReady)
            return webcamTexture;
        else
        {
            Debug.LogError("WebcamTexture is not ready");
            return null;
        }
    }

    public void SetToMaterial(Material _material)
    {
        _material.SetTexture("_MainTex", GetWebcamTex());
    }

    public void PauseWebcam()
    {
        if (webcamTexture.isPlaying)
            webcamTexture.Pause();
    }

    public void PlayWebcam()
    {
        if (webcamTexture.isPlaying == false)
            webcamTexture.Play();
    }
}
