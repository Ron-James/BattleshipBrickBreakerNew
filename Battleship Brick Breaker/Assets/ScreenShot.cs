using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScreenShot : MonoBehaviour
{
    [SerializeField]
    private string path;
    private string fileName;
    [SerializeField]
    [Range(1, 5)]
    private int size = 1;

    private void Start()
    {
        //TakeScreenShot();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("Screenshot Taken");
            fileName = "screenshot ";
            fileName += System.Guid.NewGuid().ToString() + ".png";

            ScreenCapture.CaptureScreenshot(path + fileName, size);
        }
    }

    public void TakeScreenShot()
    {
        Debug.Log("Screenshot Taken");
        fileName = "screenshot ";
        fileName += System.Guid.NewGuid().ToString() + ".png";

        ScreenCapture.CaptureScreenshot(path + fileName, size);
    }
}
