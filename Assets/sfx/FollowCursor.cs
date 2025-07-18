using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    public static FollowCursor Instance;
    public SpriteRenderer spriteRenderer;
    public Vector3 offset;
    private bool isPressed;
    public static Action<bool> OnMousePressed = delegate { };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    
    void Start()
    {
        spriteRenderer.enabled = false;
        OnMousePressed += Mousepressed;
    }
    private void OnDisable()
    {
        OnMousePressed -= Mousepressed;

    }

    private void Mousepressed(bool pressed)
    {
        isPressed = pressed;
    }

    // Update is called once per frame
    void Update()
    {

        //bool isMouseInGameView = (
        //    mousePos.x >= 0 &&
        //    mousePos.y >= 0 &&
        //    mousePos.x <= Screen.width &&
        //    mousePos.y <= Screen.height
        //);


        if (isPressed)
        {
            Vector2 mousePos = Input.mousePosition;
            spriteRenderer.enabled = true;

            // Convert the screen coordinates to world coordinates
            Vector2 cursorWorldPos = Camera.main.ScreenToWorldPoint(mousePos) - offset;

            // Set the position of the GameObject to follow the cursor
            transform.position = new Vector2(cursorWorldPos.x, cursorWorldPos.y);

        }
        else
        {
            spriteRenderer.enabled = false;
        }



    }


}
