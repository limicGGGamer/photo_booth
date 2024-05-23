using System;
using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PhotoBoothController : MonoBehaviour
{
    public PhotoBooth_WebcamTool webcamTool;
    public PhotoBooth_PhotoTool photoTool;
    public PhotoAnimController photoAnimController;
    public SimpleScrollSnap scrollSnap;
    
    public string savePath;
    private string fileName;
    private int photoCount;
    
    [Header("UI")]
    public Button btn_takePhoto;
    public RawImage rImg_takenPhoto;

    // public RectTransform[] posMarkers;
    public Button[] filterBtns;
    public RectTransform[] filterPanel;
    private RectTransform lastPanel;
    
    public RectTransform camView;
    //public int width = 4032;
    //public int height = 3024;
     public int width = 2160;
    public int height = 1620;

    private ScreenOrientation _curOrientation;
    
    void Start ()
    {
        width = Screen.width;
        height = Screen.height;

        Vector3 iconSize = Vector3.one;
        Vector3 rawImgSize = Vector3.one;
        if (Screen.orientation == ScreenOrientation.LandscapeLeft)
        {
            camView.transform.rotation = Quaternion.Euler(Vector3.zero);
            //iconSize = new Vector3(1.5f, 1.5f, 1);
            //rawImgSize = new Vector3(1.5f, 1.5f, 1);
        }
        else if (Screen.orientation == ScreenOrientation.Portrait)
        {
            camView.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
            //iconSize = Vector3.one;
            //rawImgSize = Vector3.one;
        }

        //foreach (RectTransform panel in filterPanel)
        //{
        //    panel.Find("Image").transform.localScale = iconSize;
        //    panel.Find("RawImage").transform.localScale = rawImgSize;
        //}

        webcamTool.Setup(width, height);
        photoTool.Setup(width, height);
        
        
        photoAnimController.Init();

        rImg_takenPhoto.gameObject.SetActive(false);
        for (int i = 0; i < filterBtns.Length; i++)
        {
            
            int index = i;
            scrollSnap.Add(filterBtns[index].gameObject, index);
            scrollSnap.Panels[index].gameObject.SetActive(true);
            scrollSnap.Panels[index].GetComponent<Button>().onClick.AddListener(() =>
            {
                ChangePanel(index);
            });
        }
        ChangePanel(0);
        scrollSnap.OnPanelCentered.AddListener((next, last) =>
        {
            if (lastPanel != null)
                lastPanel.gameObject.SetActive(false);
            filterPanel[next].gameObject.SetActive(true);
            lastPanel = filterPanel[next];
        });

        _curOrientation = Screen.orientation;
    }

    private void ChangePanel(int i)
    {
        scrollSnap.GoToPanel(i);
        if (lastPanel != null)
            lastPanel.gameObject.SetActive(false);
        filterPanel[i].gameObject.SetActive(true);
        lastPanel = filterPanel[i];
    }

    void OnEnable()
    {
        btn_takePhoto.onClick.AddListener(HandleBtnTakePhotoClicked);
        PhotoAnimController.OnCountDownFinish += HandleCountDownFinish;
        PhotoTool.OnPhotoTaken += HandlePhotoTaken;
    }

    void OnDisable()
    {
        btn_takePhoto.onClick.RemoveAllListeners();
        PhotoAnimController.OnCountDownFinish -= HandleCountDownFinish;
        PhotoTool.OnPhotoTaken -= HandlePhotoTaken;
    }

    void HandleBtnTakePhotoClicked()
    {
        // Hide the button
        btn_takePhoto.transform.parent.gameObject.SetActive(false);
        // Play count down animation
        photoAnimController.PlayCountDownAnim();
    }

    void HandleCountDownFinish()
    {
        Debug.Log("Count Down Finish!");
        photoTool.TakePhoto();
    }

    void HandleShotFinish()
    {
        Debug.Log("Shot Finish!");
    }

    void HandlePhotoTaken(Texture2D _takenPhoto)
    {
        photoAnimController.PlayShotAnim();

        photoCount += 1;

        // Show rImg_takenPhoto
        rImg_takenPhoto.texture = _takenPhoto;
        rImg_takenPhoto.gameObject.SetActive(true);
        print(_takenPhoto.width);

#if UNITY_EDITOR

        // Save taken photo as a png file
        string fullName = FileIOUtility.GenerateFileName(fileName, photoCount, FileIOUtility.FileExtension.PNG);
        FileIOUtility.SaveImage(_takenPhoto, savePath, fullName, FileIOUtility.FileExtension.PNG);
#else
        fileName = DateTime.Now.ToString(@"yyyy\-MM\-dd\-hh\-mm\-ss");
        NativeGallery.Permission permission =
            NativeGallery.SaveImageToGallery(_takenPhoto, "GGGamer_PhotoBooth", fileName);
        Debug.Log("Permission result: "+permission);
        //SharePhoto(_takenPhoto);
#endif
        // Wait for some time and go back to a playable status
        StartCoroutine(E_WaitAndReset());
    }

    IEnumerator E_WaitAndReset()
    {
        yield return new WaitForSeconds(2f);

        // Show the button again
        btn_takePhoto.transform.parent.gameObject.SetActive(true);

        // Hide rImg_takenPhoto
        rImg_takenPhoto.gameObject.SetActive(false);
    }

    void SharePhoto(Texture2D _takenPhoto)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            new NativeShare().AddFile(path).SetSubject("Print Image").SetText("Printing Image...").Share();
        });
    }

    private void Update()
    {
        if (Screen.orientation != _curOrientation)
        {
            _curOrientation = Screen.orientation;
            webcamTool.ChangeOrientation();
            photoTool.ChangeOrientation();
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
                camView.transform.rotation = Quaternion.Euler(Vector3.zero);
            else if (Screen.orientation == ScreenOrientation.Portrait)
                camView.transform.rotation = Quaternion.Euler(new Vector3(0,0,-90));

            //foreach(RectTransform panel in filterPanel)
            //{
            //    if(_curOrientation == ScreenOrientation.LandscapeLeft)
            //    {
            //        panel.Find("Image").transform.localScale = new Vector3(1.5f, 1.5f, 1);
            //        panel.Find("RawImage").transform.localScale = new Vector3(2, 2, 1);
            //    }
            //    else
            //    {
            //        panel.Find("Image").transform.localScale = Vector3.one;
            //        panel.Find("RawImage").transform.localScale = Vector3.one;
            //    }
            //}
        }
    }
}
