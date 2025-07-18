using UnityEngine;
using UnityEngine.UI;

public class TagSelection : MonoBehaviour
{
    [SerializeField] private GameObject shapeHolder;
    [SerializeField] private RectTransform[] shapeItem;


    [SerializeField] private GameObject effectHolder;
    [SerializeField] private Sprite[] abilitySprites;
    [SerializeField] private Sprite[] bombSprites;
    [SerializeField] private Image[] showImages;

    [SerializeField] private GameObject statusObject;
    [SerializeField] private Image statusImages;
    [SerializeField] private Sprite[] statusSprites;

    [SerializeField] private Image tagSelectImage;
    [SerializeField] private Animator animator;

    [SerializeField] private Button selectButton;
    private bool isProhibited;
    private bool isShowSelect;
    private int tagDiff;
    private SelectTagShape selectTagShape;
    private int tagIndex = -1;
    private int tagValue = -1;
    private void Start()
    {
        selectButton.onClick.AddListener(SelectTag);
    }

    public void SetUp(int index, int value, bool isSelecable, bool isShow, SelectTagShape selecttagShape)
    {
        tagIndex = index;
        tagValue = value;
        statusImages.sprite = statusSprites[0];
        selectTagShape = selecttagShape;
        shapeHolder.SetActive(false);
        effectHolder.SetActive(false);

        if (isSelecable)
        {
            statusImages.sprite = statusSprites[1];
        }
        isProhibited = !isSelecable;
        isShowSelect = !isShow;
        if (isShow)
        {
            tagDiff = 12;
        }
        else
        {
            tagDiff = 23;
            statusImages.gameObject.SetActive(false);
        }
        if(value == 12)
        {
            SetUpAbilityImnages(bombSprites);

        }
        else if(value == 13)
        {
            SetUpAbilityImnages(abilitySprites);

        }
        else
        {
            foreach (var item in shapeItem) 
            {
                item.gameObject.SetActive(false);
            }
            shapeHolder.SetActive(true);
            ShowTagShape(value);
        }
    }

    public void SetUpTagAbilityShow(bool islock)
    {
        statusObject.SetActive(!islock);
    }

    private void SetUpAbilityImnages(Sprite[] sprites)
    {
        effectHolder.SetActive(true);
        for (int i = 0; i < showImages.Length; i++)
        {
            showImages[i].sprite = sprites[i];
        }
    }

    private void SelectTag()
    {
        if (isProhibited)
        {
            tagSelectImage.color = Color.red;
            animator.SetBool("ISPress", true);
            Invoke(nameof(PlayProhibatedAnim), 1f);
        }
        else
        {
            if (isShowSelect)
            {
                selectTagShape.SelectTagToUse(tagValue);
            }
            else
            {
                selectTagShape.ShowTags(tagIndex);
            }
        }
    }

    private void PlayProhibatedAnim()
    {
        tagSelectImage.color = Color.white;
        animator.SetBool("ISPress", false);
    }

    private void ShowTagShape(int shapeindex)
    {
        if(shapeindex == 0)
        {
            Shape1();
        }else if (shapeindex == 1)
        {
            Shape2();
        }else if (shapeindex == 2)
        {
            Shape3();
        }else if (shapeindex == 3)
        {
            Shape4();
        }else if (shapeindex == 4)
        {
            Shape5();
        }else if (shapeindex == 5)
        {
            Shape6();
        }else if (shapeindex == 6)
        {
            Shape7();
        }else if(shapeindex == 7)
        {
            Shape8();
        }else if (shapeindex == 8)
        {
            Shape9();
        }else if(shapeindex == 9)
        {
            Shape10();
        }else if (shapeindex == 10)
        {
            Shape11();
        }else 
        {
            Shape12();
        }
    }

    private void Shape1()
    {
        int index = 0;
        for (int i = 1; i <= 3; i+=2)
        {
            int mul = 1;
            for (int j = 0; j < 2; j++)
            {
                if(j == 1)
                {
                    mul = -1;
                }
                int ypos = (tagDiff * i * mul);
                shapeItem[index].anchoredPosition = new Vector2(0, ypos);
                shapeItem[index].gameObject.SetActive(true);
                index++;

            }
        }
    }
    private void Shape2()
    {
        int index = 0;
        for (int i = 1; i < 3; i++)
        {
            int mul = 1;
            int mul2 = 1;
            if (i == 1)
                mul2 = -1;
            for (int j = 0; j < 2; j++)
            {
                if (j == 1)
                {
                    mul = -1;
                }
                int xpos = (tagDiff  * mul2);
                int ypos = (tagDiff  * mul);
                shapeItem[index].anchoredPosition = new Vector2(xpos, ypos);
                shapeItem[index].gameObject.SetActive(true);
                index++;

            }
        }

    }

    private void Shape3()
    {
        int index = 0;
        int ypos = tagDiff * 2;
        for (int i = 0; i < 3; i++)
        {
            shapeItem[index].anchoredPosition = new Vector2(0, ypos);
            shapeItem[index].gameObject.SetActive(true);
            index++;
            if(i == 2)
            {
                shapeItem[index].anchoredPosition = new Vector2(-tagDiff * 2, ypos);
                shapeItem[index].gameObject.SetActive(true);
            }
            ypos -= tagDiff * 2;
        }
    }

    private void Shape4()
    {
        int index = 0;
        int ypos = tagDiff * 2;
        for (int i = 0; i < 3; i++)
        {
            shapeItem[index].anchoredPosition = new Vector2(0, ypos);
            shapeItem[index].gameObject.SetActive(true);
            index++;
            if (i == 2)
            {
                shapeItem[index].anchoredPosition = new Vector2(tagDiff * 2, ypos);
                shapeItem[index].gameObject.SetActive(true);
            }
            ypos -= tagDiff * 2;
        }
    }

    private void Shape5()
    {
        int index = 0;
        int ypos = tagDiff * 2;
        for (int i = 0; i < 3; i++)
        {
            shapeItem[index].anchoredPosition = new Vector2(0, ypos);
            shapeItem[index].gameObject.SetActive(true);
            index++;
            if (i == 1)
            {
                shapeItem[index].anchoredPosition = new Vector2(-tagDiff * 2, ypos);
                shapeItem[index].gameObject.SetActive(true);
                index++;
            }
            ypos -= tagDiff * 2;
        }
    }

    private void Shape6()
    {
        int index = 0;
        int mul = 1;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                mul = -1;
            }
            int xpos = tagDiff * mul;
            shapeItem[index].anchoredPosition = new Vector2(xpos, 0);
            shapeItem[index].gameObject.SetActive(true);
            index++;
        }
    }
    private void Shape7()
    {
        int index = 0;
        int mul = 1;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                mul = -1;
            }
            int xpos = tagDiff * mul;
            shapeItem[index].anchoredPosition = new Vector2(xpos, 0);
            shapeItem[index].gameObject.SetActive(true);
            index++;
            int ypos = tagDiff * 2 * mul * -1;
            shapeItem[index].anchoredPosition = new Vector2(xpos, ypos);
            shapeItem[index].gameObject.SetActive(true);
            index++;
        }
    }
    private void Shape8()
    {
        int index = 0;
        int mul = 1;
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                mul = -1;
            }
            int xpos = tagDiff * mul;
            shapeItem[index].anchoredPosition = new Vector2(xpos, 0);
            shapeItem[index].gameObject.SetActive(true);
            index++;
            int ypos = tagDiff * 2 * mul;
            shapeItem[index].anchoredPosition = new Vector2(xpos, ypos);
            shapeItem[index].gameObject.SetActive(true);
            index++;
        }
    }


    private void Shape9()
    {
        int index = 0;
        int mul = 1;
        int mul2 = 1;
        for (int j = 0; j < 2; j++)
        {
            if (j == 1)
            {
                mul = -1;
            }
            int xpos = (tagDiff * mul2);
            int ypos = (tagDiff * mul);
            shapeItem[index].anchoredPosition = new Vector2(xpos, ypos);
            shapeItem[index].gameObject.SetActive(true);
            index++;

        }
        shapeItem[index].anchoredPosition = new Vector2(tagDiff * -1, tagDiff * 1);
        shapeItem[index].gameObject.SetActive(true);

    }

    private void Shape10()
    {
        int index = 0;
        int mul = 1;
        for (int j = 0; j < 2; j++)
        {
            if (j == 1)
            {
                mul = -1;
            }
            int xpos = (tagDiff * -1 * mul);
            int ypos = (tagDiff * mul);
            shapeItem[index].anchoredPosition = new Vector2(xpos, ypos);
            shapeItem[index].gameObject.SetActive(true);
            index++;

        }
    }

    private void Shape11()
    {
        int index = 0;
        int ypos = tagDiff * 2;
        for (int i = 0; i < 3; i++)
        {
            shapeItem[index].anchoredPosition = new Vector2(0, ypos);
            shapeItem[index].gameObject.SetActive(true);
            index++;
            ypos -= tagDiff * 2;
        }
    }
    private void Shape12()
    {
        shapeItem[0].anchoredPosition = new Vector2(0, 0);
        shapeItem[0].gameObject.SetActive(true);
    }
}
