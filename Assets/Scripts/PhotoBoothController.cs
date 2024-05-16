using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PhotoBoothController : MonoBehaviour
{
    public PhotoBooth_WebcamTool webcamTool;
    public PhotoBooth_PhotoTool photoTool;
    public PhotoAnimController photoAnimController;
    
    public string savePath;
    private string fileName;
    private int photoCount;
    
    [Header("UI")]
    public Button btn_takePhoto;
    public RawImage rImg_takenPhoto;

    public RectTransform[] posMarkers;
    public Button[] filterBtns;
    public RectTransform[] filterPanel;
    
    void Start () {

        webcamTool.Init();
        photoTool.Init();
        photoAnimController.Init();

        rImg_takenPhoto.gameObject.SetActive(false);

        Movement(filterBtns[0], posMarkers[1].position);
        Movement(filterBtns[1], posMarkers[2].position);
        filterPanel[0].gameObject.SetActive(true);
        filterPanel[1].gameObject.SetActive(false);
    }

    void OnEnable()
    {
        filterBtns[0].onClick.AddListener(() =>
        {
            Movement(filterBtns[0], posMarkers[1].position);
            Movement(filterBtns[1], posMarkers[2].position);
            filterPanel[0].gameObject.SetActive(true);
            filterPanel[1].gameObject.SetActive(false);
        });
        filterBtns[1].onClick.AddListener(() =>
        {
            Movement(filterBtns[0], posMarkers[0].position);
            Movement(filterBtns[1], posMarkers[1].position);
            filterPanel[0].gameObject.SetActive(false);
            filterPanel[1].gameObject.SetActive(true);
        });
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

    private void Movement(Button btn, Vector2 targetPos)
    {
        btn.transform.DOMove(targetPos, 0.5f);
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

#if UNITY_EDITOR

        // Save taken photo as a png file
        string fullName = FileIOUtility.GenerateFileName(fileName, photoCount, FileIOUtility.FileExtension.PNG);
        FileIOUtility.SaveImage(_takenPhoto, savePath, fullName, FileIOUtility.FileExtension.PNG);
#else
        fileName = DateTime.Now.ToString(@"yyyy\-MM\-dd\-hh\-mm\-ss");
        NativeGallery.Permission permission =
            NativeGallery.SaveImageToGallery(_takenPhoto, "GGGamer_PhotoBooth", fileName);
        Debug.Log("Permission result: "+permission);
        SharePhoto(_takenPhoto);
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
        new NativeShare().AddFile(_takenPhoto).SetSubject("Print Image").SetText("Printing Image...").Share();
    }
}
