using UnityEngine;
using UnityEngine.UI;

public class LevelShow : MonoBehaviour
{
    [SerializeField] private Image bgImage;
    [SerializeField] private Text levelNumText;
    [SerializeField] private GameObject starGameObject;
    [SerializeField] private GameObject colObject;
    private Color bgDisableColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    private Color textDisableColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    [SerializeField] private Button levelButton;
    public StarObject[] starObject;
    private LevelData levelData;
    private int levelIndex;
    public int Get_Level_Index { get { return levelIndex; }}

    void Start()
    {
        levelButton.onClick.AddListener(Playlevel);
    }

    public void Set_Level_Index(int levelindex)
    {
        levelIndex = levelindex;
        levelNumText.color = textDisableColor;
        levelNumText.text = (levelindex + 1).ToString();
        bgImage.color = bgDisableColor;
        levelButton.interactable = false;
        starGameObject.SetActive(false);
        for (int i = 0; i < starObject.Length; i++)
        {
            starObject[i].Star.SetActive(false);
        }
    }

    public void Playlevel()
    {
        if(levelData != null)
            MenuManager.Instance.LevelShowPanelSetUp(levelData);
    }
    public void TrailerAnim(bool isUnLock, int starCount)
    {
        if (isUnLock)
        {
            bgImage.color = Color.white;
        }
        starGameObject.SetActive(isUnLock);
        for (int i = 0; i < starCount; i++)
        {
            if(i < starObject.Length)
                starObject[i].Star.SetActive(true);
        }
    }

    public void SetUp(bool isUnLock, int starCount, int index, LevelData lvData)
    {
        if (isUnLock)
        {
            bgImage.color = Color.white;
            levelNumText.color = Color.white;
        }
        levelButton.interactable = isUnLock;
        if (lvData != null)
        {
            levelData = lvData;
            starGameObject.SetActive(isUnLock);
            if (isUnLock)
            {
                colObject.tag = "Finish";
            }
            else
            {
                colObject.tag = "LevelSlide";
            }
            for (int i = 0; i < starCount; i++)
            {
                if (i < starObject.Length)
                    starObject[i].Star.SetActive(true);
            }

        }
    }

    


}
