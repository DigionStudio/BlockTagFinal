using UnityEngine;

public class TargetEffect : MonoBehaviour
{
    public static TargetEffect Instance;
    [SerializeField] private RectTransform[] targetrect;
    [SerializeField] private TargetEffectShow effectObject;

    [SerializeField] private PointShow pointShow;
    [SerializeField] private RectTransform pointrect;
    private float coinsCount;
    private float totalCoinCount;
    private int lifeCount;
    private int totalLifeCount;
    private int wheelCount;
    private int totalWheelCount;
    private Vector3 pos1;
    private Vector3 pos2;
    private float radius;

    [SerializeField] private AbilityShow abilityGiftEffect;
    [SerializeField] private RectTransform finalrect;


    [SerializeField] private RectTransform[] coinrect;
    private Vector3 pos3coin;
    private Vector3 pos4coin;

    private Vector3 pos3life;
    private Vector3 pos4life;

    private Vector3 pos3wheel;
    private Vector3 pos4wheel;

    private int gameTypeCode;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }


    }


    public void SetUpEffect(Vector3 pos, Normal_Block_Type type, BlockType abilityType, int targetCode)
    {
        TargetEffectShow effect = Instantiate(effectObject);
        effect.transform.position = pos;
        Vector3 pos21 = targetrect[targetCode].TransformPoint(targetrect[targetCode].rect.center);
        pos21.z = 0;
        effect.SetUp(pos, pos21, type, abilityType, Gem_Type.none);
    }
    public void SetUpGemTarget(Vector3 pos, Gem_Type gemType, int targetCode)
    {
        TargetEffectShow effect = Instantiate(effectObject);
        effect.transform.position = pos;
        Vector3 pos21 = targetrect[targetCode].TransformPoint(targetrect[targetCode].rect.center);
        pos21.z = 0;
        effect.SetUp(pos, pos21, Normal_Block_Type.none, BlockType.None, gemType);
    }

    public void SetUpPointShow(int totalcount)
    {
        CancelInvoke(nameof(InstaPoint));
        coinsCount = 0;
        totalCoinCount = (float)totalcount * 5;
        if (totalCoinCount > 30)
            totalCoinCount = 30;

        pos2 = pointrect.TransformPoint(pointrect.rect.center);
        pos1 = pos2 + Vector3.up * 10f;
        InvokeRepeating(nameof(InstaPoint), 0, 0.05f);
    }

    

    private void InstaPoint()
    {
        if(coinsCount < totalCoinCount)
        {
            coinsCount++;
            PointShow effect = Instantiate(pointShow);
            effect.transform.position = pos1;
            effect.SetUp(pos2, 0);
        }
        else
        {
            CancelInvoke(nameof(InstaPoint));
        }

    }

    public void SetUpCoinShow(Vector2 pos)
    {
        gameTypeCode = BoardManager.Instance.gameDataManager.GameTypeCode;
        Vector3 pos41 = coinrect[gameTypeCode].TransformPoint(coinrect[gameTypeCode].rect.center);
        pos3coin = pos;
        pos4coin = pos41;
        InstaCoin();
    }

    private void InstaCoin()
    {
        PointShow effect = Instantiate(pointShow);
        effect.transform.position = pos3coin;
        effect.SetUp(pos4coin, 1);
    }



    public void FreeRewardEffectCoins(Vector2 pos, int num, float rad = 0.2f)
    {
        coinsCount = 0;
        totalCoinCount = num;
        Vector3 pos41 = coinrect[1].TransformPoint(coinrect[1].rect.center);
        pos3coin = pos;
        pos4coin = pos41;
        radius = rad;
        InvokeRepeating(nameof(InstaCoinReward), 0, 0.05f);
    }
    private void InstaCoinReward()
    {
        if (coinsCount < totalCoinCount)
        {
            coinsCount++;
            PointShow effect = Instantiate(pointShow);
            Vector3 randomPoint = GenerateRandomPoint(pos3coin, radius);
            effect.transform.position = randomPoint;
            if (totalCoinCount == 1)
                effect.transform.position = pos3coin;
            effect.MoveInParabola(pos4coin, 1);
        }
        else
        {
            CancelInvoke(nameof(InstaCoinReward));
        }

    }
    public void FreeRewardEffectLifes(Vector2 pos, int num)
    {
        lifeCount = 0;
        totalLifeCount = num;
        Vector3 pos41 = coinrect[0].TransformPoint(coinrect[0].rect.center);
        pos3life = pos;
        pos4life = pos41;
        InvokeRepeating(nameof(InstaLifeReward), 0, 0.05f);
    }

    private void InstaLifeReward()
    {
        if (lifeCount < totalLifeCount)
        {
            lifeCount++;
            PointShow effect = Instantiate(pointShow);
            Vector3 randomPoint = GenerateRandomPoint(pos3life, 0.3f);
            effect.transform.position = randomPoint;
            if (totalLifeCount == 1)
                effect.transform.position = pos3life;
            effect.MoveInParabola(pos4life, 0);
        }
        else
        {
            CancelInvoke(nameof(InstaLifeReward));
        }

    }

    public void FreeRewardEffectWheel(Vector2 pos, int num)
    {
        wheelCount = 0;
        totalWheelCount = num;
        Vector3 pos41 = coinrect[2].TransformPoint(coinrect[2].rect.center);
        pos3wheel = pos;
        pos4wheel = pos41;
        InvokeRepeating(nameof(InstaWheelReward), 0, 0.05f);
    }

    private void InstaWheelReward()
    {
        if (wheelCount < totalWheelCount)
        {
            wheelCount++;
            PointShow effect = Instantiate(pointShow);
            Vector3 randomPoint = GenerateRandomPoint(pos3wheel, 0.5f);
            effect.transform.position = randomPoint;
            if (totalWheelCount == 1)
                effect.transform.position = pos3wheel;
            effect.MoveInParabola(pos4wheel, 2);
        }
        else
        {
            CancelInvoke(nameof(InstaWheelReward));
        }

    }

    Vector3 GenerateRandomPoint(Vector3 center, float radius)
    {
        // Generate a random point within a circle (2D)
        Vector2 randomPoint2D = Random.insideUnitCircle * radius;

        // Convert the 2D point to a 3D point
        Vector3 randomPoint3D = new Vector3(randomPoint2D.x, randomPoint2D.y, 0);

        // Offset the random point by the center position
        return center + randomPoint3D;
    }


    public void InstaAbilitygiftShow(Sprite icon,float xPos, float count)
    {
        Vector3 cen = new Vector3(xPos, -1.5f, 0);
        Vector2 random = GenerateRandomPoint(cen, 0.05f * count);
        Vector2 posI = new Vector3(random.x, -2, 0);
        AbilityShow effect = AbilityShowObj(posI);
        Vector3 posF = finalrect.TransformPoint(finalrect.rect.center);
        effect.MoveInAbilityEffect(posF, icon);
    }
    public void WheelAbility(Sprite icon, Vector2 posI, Vector2 posF, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 rand = posI;
            if(count > 1)
            {
                rand = GenerateRandomPoint(posI, 0.5f);
            }
            AbilityShow effect = AbilityShowObj(rand);
            effect.MoveInAbilityEffect(posF, icon);
        }
    }

    private AbilityShow AbilityShowObj(Vector2 pos)
    {
        AbilityShow effect = Instantiate(abilityGiftEffect);
        effect.transform.position = pos;
        return effect;
    }
}
