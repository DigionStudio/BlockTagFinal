using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class MetaFill
{
    public Transform trans;
    public int levelMin;
    public int levelMax;
}

[Serializable]
public class MetaFillContent
{
    public List<MetaItem> metaitems;
    public bool isLoaded = false;
}

public class LevelMetaFill : MonoBehaviour
{
    [SerializeField] private ColorBombEffect fillEffectPrefab;
    [SerializeField] private Transform fillEffect;
    [SerializeField] private MetaFill[] levelMetaHoldes;
    [SerializeField] private List<MetaFillContent> metaFillContents = new List<MetaFillContent>();
    [SerializeField] private List<MetaItem> currentMetaItems = new List<MetaItem>();

    private Transform originTrans;
    private readonly string FillLevelIndex = "FillLevelIndex";
    private int currrentLevel = 0;
    public bool HasMetaFill { get; private set; }
    private int currentSlideIndex;
    private bool isSetUp = false;
    void Start()
    {
        SetUp();
    }
    private void SetUp()
    {
        if (!isSetUp)
        {
            fillEffect.gameObject.SetActive(false);
            currrentLevel = PlayerPrefs.GetInt(FillLevelIndex, 0);
            isSetUp = true;
            for (int i = 0; i < levelMetaHoldes.Length; i++)
            {
                MetaFillContent itemcontent = new MetaFillContent();
                itemcontent.metaitems = new List<MetaItem>();
                metaFillContents.Add(itemcontent);
            }
            LoadAllMetaItems();
        }
    }

    private void LoadAllMetaItems()
    {
        if (currrentLevel >= 0)
        {
            int indexValue = GetFillIndex(currrentLevel);
            MetaFillContent itemcontent = GetMetaContent(indexValue);
            if (!metaFillContents[indexValue].isLoaded)
            {
                metaFillContents[indexValue] = itemcontent;
                metaFillContents[indexValue].isLoaded = true;
            }

            if (indexValue < levelMetaHoldes.Length)
            {
                FillMetaContent(currrentLevel, levelMetaHoldes[indexValue], indexValue);
            }

            int num = indexValue + 1;
            if (num < levelMetaHoldes.Length && !metaFillContents[num].isLoaded)
            {
                MetaFillContent itemcontentfowd = GetMetaContent(num);
                metaFillContents[num] = itemcontentfowd;
                metaFillContents[num].isLoaded = true;
            }
            int num2 = indexValue - 1;
            if (num2 >= 0)
            {
                if (!metaFillContents[num2].isLoaded)
                {
                    MetaFillContent itemcontentprev = GetMetaContent(num2);
                    metaFillContents[num2] = itemcontentprev;
                    metaFillContents[num2].isLoaded = true;
                }
                FillMetaContent(metaFillContents[num2].metaitems);
                currentSlideIndex = num2;
            }
        }
    }

    public void CheckForLevelSlider(int level)
    {
        int indexValue = GetFillIndex(level);

        if (currentSlideIndex == indexValue)
        {
            indexValue -= 1;
            if(indexValue >= 0)
            {
                if (metaFillContents[indexValue].metaitems.Count == 0)
                {
                    MetaFillContent itemcontentprev = GetMetaContent(indexValue);
                    metaFillContents[indexValue] = itemcontentprev;
                }
                FillMetaContent(metaFillContents[indexValue].metaitems);
                currentSlideIndex = indexValue;
            }
        }


    }
    private MetaFillContent GetMetaContent(int index)
    {
        MetaFillContent itemcontent = new MetaFillContent();
        MetaFill item = levelMetaHoldes[index];
        itemcontent.metaitems = new List<MetaItem>();
        if (item != null)
        {
            MetaItem[] content = item.trans.GetComponentsInChildren<MetaItem>();
            foreach (var con in content)
            {
                if (con != null)
                {
                    itemcontent.metaitems.Add(con);
                }
            }
        }
        return itemcontent;
    }

    private void FillMetaContent(List<MetaItem> content)
    {
        foreach (var con in content)
        {
            if (con != null)
            {
                con.Fill(1);
            }
        }
    }


    private void FillMetaContent(int index, MetaFill fillHolder, int value)
    {
        if (index > fillHolder.levelMin && index <= fillHolder.levelMax)
        {
            float percent = PecentInLevel(index, fillHolder.levelMin, fillHolder.levelMax);
            List<MetaItem> content = metaFillContents[value].metaitems;
            float count = percent * (float)content.Count;
            for (int i = 0; i < content.Count; i++)
            {
                if (i < (int)count && content[i] != null)
                {
                    content[i].Fill(1);
                }
                else
                {
                    break;
                }
            }
        }
    }

    public void GetAllMetaFill(int level)
    {
        SetUp();
        int diff = level - currrentLevel;
        if(diff == 0)
        {
            HasMetaFill = false;
        }
        else
        {
            if (diff > 1)
            {
                currrentLevel = level - 1;
                LoadAllMetaItems();
            }
            FillCurrentItems(level);

        }
    }

    public void CheckForMetaFill(int level, Transform origin, bool isGem)
    {
        if(currentMetaItems.Count > 0)
        {
            originTrans = origin;
            StartCoroutine(InstaFillEffect(isGem));
            PlayerPrefs.SetInt(FillLevelIndex, level);
            currrentLevel = level;
        }
        else
        {
            MenuManager.Instance.GiftPanelForMetaFill(false);
        }
    }

    private IEnumerator InstaFillEffect(bool isGem)
    {
        for (int i = 0; i < currentMetaItems.Count; i++)
        {
            int colorCode = 0;
            int maxittter = 10;
            int count = 0;
            if (currentMetaItems[i] != null)
            {
                Transform trans = currentMetaItems[i].transform;
                fillEffect.position = trans.position;
                List<ColorBombEffect> effectlist = new List<ColorBombEffect>();
                while (count < maxittter)
                {
                    count++;
                    Sprite icon = BlockManager.Instance.IconSprite(colorCode);
                    if (isGem)
                    {
                        icon = BlockManager.Instance.GemTypeSprite(colorCode);
                    }
                    ColorBombEffect effect = Instantiate(fillEffectPrefab, transform);
                    effect.transform.position = GenerateRandomPoint(originTrans.position, 0.4f);
                    effect.SetUp(icon, trans);
                    effectlist.Add(effect);
                    yield return new WaitForSeconds(0.05f);
                    if (colorCode >= 4)
                    {
                        colorCode = 0;
                    }
                    else
                    {
                        colorCode++;
                    }
                }
                yield return new WaitForSeconds(0.1f);
                fillEffect.gameObject.SetActive(true);

                foreach (var item in effectlist)
                {
                    item.MetaFillTweenMove(currentMetaItems[i]);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            DisableStarFillEffect();
            yield return new WaitForSeconds(0.5f);
        }
        currentMetaItems.Clear();
        MenuManager.Instance.GiftPanelForMetaFill(false);
    }

    private void DisableStarFillEffect()
    {
        fillEffect.gameObject.SetActive(false);
    }

    

    private float PecentInLevel(int level, int minlevel, int maxlevel)
    {
        int totallevel = maxlevel - minlevel;
        int currentlevel = level - minlevel;
        float value = (float)currentlevel / (float)totallevel;
        return value;
    }

    private int GetFillIndex(int index)
    {
        int value = 0;
        for (int i = 0; i < levelMetaHoldes.Length; i++)
        {
            MetaFill item = levelMetaHoldes[i];
            if (item != null)
            {
                if (index > item.levelMin && index <= item.levelMax)
                {
                    value = i;
                    break;
                }
            }
        }
        return value;
    }


    private void FillCurrentItems(int level)
    {
        HasMetaFill = false;
        int value = GetFillIndex(level);
        MetaFill fillHolder = levelMetaHoldes[value];
        List<MetaItem> content = metaFillContents[value].metaitems;
        float percent = PecentInLevel(level, fillHolder.levelMin, fillHolder.levelMax);
        float count = percent * (float)content.Count;
        MetaItem item = new();
        for (int i = 0; i < content.Count; i++)
        {
            item = content[i];
            if (i < (int)count && item != null)
            {
                if(!item.HasFilled)
                    currentMetaItems.Add(item);
            }
            else
            {
                break;
            }
        }

        if (currentMetaItems.Count > 0)
            HasMetaFill = true;
    }


    Vector2 GenerateRandomPoint(Vector2 center, float radius)
    {
        // Generate a random point within a circle (2D)
        Vector2 randomPoint2D = UnityEngine.Random.insideUnitCircle * radius;

        // Convert the 2D point to a 3D point
        Vector2 randomPoint = new Vector2(randomPoint2D.x, randomPoint2D.y);

        // Offset the random point by the center position
        return center + randomPoint;
    }

}
