using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    private Camera mainCamera;
    private float[] sizeNum = new float[2];
    //void Start()
    //{
    //    Vector2 res = new Vector2(Screen.width, Screen.height);
    //    print(res);
    //}
    private void Awake()
    {
        mainCamera = Camera.main;
        SetUpCameraSize();
    }

    private void CheckScreenRes()
    {
        Vector2 screenVec = new Vector2(Screen.width, Screen.height);
        Vector2 safearea = new Vector2(Screen.safeArea.width, Screen.safeArea.height);
        float val = 24.5f;
        if (screenVec.y > safearea.y)
        {
            val = 27.5f;
        }
        else
        {
            float num = Screen.height / Screen.width;
            if(num == 2)
            {
                val = 26.5f;
            }
        }
        mainCamera.orthographicSize = val;
    }

    private void SetUpCameraSize()
    {
        float num = CameraResolutionRatio();
        float cameraSize = 24.5f;
        if (num == 2f)
        {
            cameraSize = 25.5f;
        }
        else if(num > 2 && num < 2.1)
        {
            cameraSize = 26f;

        }else if(num >= 2.1 && num < 2.3f)
        {
            cameraSize = 26.5f;
        }
        mainCamera.orthographicSize = cameraSize;
    }

    private float CameraResolutionRatio()
    {
        float ratio = (float)Screen.height / (float)Screen.width;
        float roundedNumber = Mathf.Round(ratio * 100f) / 100f;
        return roundedNumber;
    }
}
