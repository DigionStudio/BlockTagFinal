using UnityEngine;
using UnityEngine.UI;

public class DailyGoalShow : MonoBehaviour
{
    [SerializeField] private GameObject goalHolder;
    [SerializeField] private Image goalIconImage;
    [SerializeField] private Image goalAbilityIconImage;
    [SerializeField] private Text goalCountText;


    [SerializeField] private Text coinText;
    [SerializeField] private Button goalachiveButton;
    [SerializeField] private Image fillArea;
    [SerializeField] private Text goalRemaining;
    [SerializeField] private GameObject goalAchived;

    private int coinCount;
    private BlockManager blockManager;
    private GameDataManager gameDataManager;
    private int goalIndex;
    void Start()
    {
        goalachiveButton.onClick.AddListener(GoalAchive);
        if (blockManager == null)
        {
            blockManager = BlockManager.Instance;
            gameDataManager = blockManager.gameDataManager;
        }
    }

    public void DisableGoal()
    {
        if(goalHolder != null)
            goalHolder.SetActive(false);
    }

    public void SetUpGoalShow(DailyTargetData goalData, int goalindex)
    {
        goalIndex = goalindex;
        goalAchived.SetActive(false);
        goalachiveButton.interactable = false;
        if (blockManager == null)
        {
            blockManager = BlockManager.Instance;
            gameDataManager = blockManager.gameDataManager;
        }
        coinCount = goalData.coins;
        Sprite icon = blockManager.IconSprite(5);
        int iconIndex = (int)goalData.normalBlockType;
        if (iconIndex >= 0 && iconIndex < 5)
        {
            icon = blockManager.IconSprite(iconIndex);
        }
        else
        {
            if (iconIndex >= 7 && iconIndex < 12)
            {
                int val = iconIndex - 7;
                icon = blockManager.ObstacleSprite(val + 1);
            }
        }
        goalIconImage.sprite = icon;

        if (goalData.abilityType == BlockType.None)
        {
            goalAbilityIconImage.enabled = false;
        }
        else
        {
            int abilityIndex = (int)goalData.abilityType;
            Sprite abilityicon = blockManager.ColorAbilitySprite();
            if (abilityIndex > 0 && abilityIndex < 5)
            {
                abilityicon = blockManager.AbilitySprite(abilityIndex - 1);
            }
            goalAbilityIconImage.sprite = abilityicon;
        }
        goalCountText.text = "x" + (goalData.totalCount.ToString());


        coinText.text = "x" + (coinCount.ToString());
        if (!goalData.isActive)
        {
            if (!goalData.isClaimed)
                goalachiveButton.interactable = true;
            else
                goalAchived.SetActive(true);
        }

        float num = (float)goalData.currentCount/(float)goalData.totalCount;
        fillArea.fillAmount = num;
        goalRemaining.text = goalData.currentCount.ToString() + "/" + goalData.totalCount.ToString();
    }

    private void GoalAchive()
    {
        goalAchived.SetActive(true);
        DailyGoalsManager.Instance.GoalCoinClaimed(goalIndex);
        gameDataManager.CoinValueChange(coinCount, true);
        RectTransform panel = goalachiveButton.GetComponent<RectTransform>();
        Vector2 pos = panel.TransformPoint(panel.rect.center);
        int showcoin = coinCount / 5;
        if(showcoin > 30)
        {
            showcoin = 30;
        }
        MenuManager.Instance.GoalsClaimButton(pos, showcoin);
    }
}
