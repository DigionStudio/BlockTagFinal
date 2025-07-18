using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core.Easing;

public class LogoBoardManager : MonoBehaviour
{
    public Transform crushTile;
    public Transform crushTileHolder;
    public BlockLogo[] blockList;
    public LogoCrush[] allLogoCrush; 
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        crushTile.DOMove(new Vector3(7.56f, -1.65f, 0), 1.5f).OnComplete(() =>
        {
            StartCoroutine(BlockBlastCO());
        });
    }

    IEnumerator BlockBlastCO()
    {
        for (int i = 0; i < allLogoCrush.Length; i++)
        {
            if (allLogoCrush[i].thisBlock != null)
            {
                allLogoCrush[i].thisBlock.DestroyObject();
            }
        }
        Col_Crush(1);
        Col_Crush(2);
        Row_Crush(2);
        Row_Crush(3);
        Color_Bomb(1);
        Area_Crush(2, 2);
        crushTileHolder.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        crushTile.DOMove(new Vector3(8f, -15f, 0), 1.5f);
    }

    public void Col_Crush(int value)
    {
        List<BlockLogo> colElement = BlockElements(value, 1);
        foreach (var item in colElement)
        {
            if (item != null)
            {
                item.DestroyObject();
            }
        }
    }
    public void Row_Crush(int value)
    {
        List<BlockLogo> rowElements = BlockElements(value, 0);
        foreach (var item in rowElements)
        {
            if (item != null)
            {
                item.DestroyObject();
            }
        }
    }
    public void Color_Bomb(int value)
    {
        List<BlockLogo> elements = BlockElements(value, 2);
        foreach (var item in elements)
        {
            if (item != null)
            {
                item.DestroyObject();
            }
        }
    }
    public void Area_Crush(int row, int col)
    {
        List<BlockLogo> elements = AreaBlock(row, col);
        foreach (var item in elements)
        {
            if (item != null)
            {
                item.DestroyObject();
            }
        }
    }
    private List<BlockLogo> AreaBlock(int row, int column)
    {
        var list = new List<BlockLogo>();
        int colmin = 1;
        int colmax = 4;
        for (int i = colmin; i <= colmax; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                foreach (var item in blockList)
                {

                    if (item != null && item.gameObject.activeInHierarchy)
                    {
                        if (item.transform.position.y < 15)
                        {
                            if (item.rowValue == i && item.colValue == j)
                            {
                                list.Add(item);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
        }
        return list;
    }

    private List<BlockLogo> BlockElements(int value, int code)
    {
        var list = new List<BlockLogo>();
        for (int i = 0; i < blockList.Length; i++)
        {
            var item = blockList[i];
            if (item != null  && item.gameObject.activeInHierarchy)
            {
                if (code == 0 && item.rowValue == value)
                {
                    list.Add(item);
                }
                else if (code == 1 && item.colValue == value)
                {
                    list.Add(item);
                }
                else if (code == 2 && item.colorCode == value)
                {
                    list.Add(item);
                }
            }
        }
        return list;
    }
}
