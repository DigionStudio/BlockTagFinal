using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    public Transform iconTrans;
    private bool ispressed;

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        // Convert the screen coordinates to world coordinates
        Vector2 cursorWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Set the position of the GameObject to follow the cursor
        transform.position = new Vector2(cursorWorldPos.x, cursorWorldPos.y);

        if (Input.GetMouseButtonDown(0))
        {
            ispressed = true;
            RotateIcon();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ispressed = false;
            RotateIcon();
        }
    }


    private void RotateIcon()
    {
        if (ispressed)
        {
            iconTrans.Rotate(Vector3.forward, 25f);
        }
        else
        {
            iconTrans.Rotate(Vector3.forward, -25f);
        }
    }
}
