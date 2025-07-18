using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoCrush : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public BlockLogo thisBlock;
    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            ChangeTileColor(collision.gameObject, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            ChangeTileColor(collision.gameObject,1);
        }
    }

    private void ChangeTileColor(GameObject tile, int code)
    {
        BlockLogo block = tile.GetComponent<BlockLogo>();
        if (block != null)
        {
            thisBlock = block;
            if (code == 0)
            {
                block.SelectedColor();

            }
            else
            {
                block.NormalColor();
                thisBlock = null;
            }
            ChangeColor(code);
        }
    }
    public void ChangeColor(int code)
    {
        Color col = Color.red;
        if (code == 0)
        {
            col = Color.white;
        }
        spriteRenderer.color = col;
    }
}
