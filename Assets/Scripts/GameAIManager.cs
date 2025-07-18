using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class GameAIManager : MonoBehaviour
{
    public static GameAIManager Instance;


    private BoardManager boardManager;
    private BlockManager blockManager;
    private GameDataManager gameDataManager;
    private int blockTypeDiff = 3;
    private int abilityTypeDiff = 5;
    private int gemTypeDiff = 3;

    private TargetData[] targetData;
    private int totalMoveCount;
    private int cuurentMoveCount;
    private int[] targetCounts;
    private int percentValue;
    private int prevValue = 3;
    private bool isBombAI;
    private bool hasBombAsisted;
    private bool isBombAiAsists;
    public bool HasAsists { get { return isBombAiAsists; } }
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject wandObject;
    private bool isAIEffectActive;

    [SerializeField] private GameObject wandActiveHolder;
    [SerializeField] private Transform wandIconTrans;
    [SerializeField] private SpriteRenderer wandIcon;
    [SerializeField] private GameObject activeEffect;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        wandObject.SetActive(false);
        boardManager = BoardManager.Instance;
        blockManager = BlockManager.Instance;
        gameDataManager = boardManager.gameDataManager;
        isBombAiAsists = gameDataManager.isBombAiAsist;
        GameManager.OnMoveTaken += CalculateAIAsists;

        wandActiveHolder.SetActive(isBombAiAsists);
        activeEffect.SetActive(false);
        wandIconTrans.gameObject.SetActive(false);
        if (isBombAiAsists)
        {
            wandIconTrans.localScale = Vector2.zero;
            wandIcon.color = new Color(1, 1, 1, 0);
            Invoke(nameof(WandInitialize), 1.5f);
        }
    }
    private void OnDisable()
    {
        GameManager.OnMoveTaken -= CalculateAIAsists;

    }

    private void WandInitialize()
    {
        wandIconTrans.gameObject.SetActive(true);
        wandIconTrans.DOScale(Vector2.one * 3.5f, 0.3f);
        wandIcon.DOFade(1, 0.3f);
        Invoke(nameof(ActiveWand), 1f);
    }

    private void ActiveWand()
    {

        wandIconTrans.DOScale(Vector2.zero, 0.3f);
        wandIcon.DOFade(0, 0.3f);
        ActiveWandEffect();
    }
    private void ActiveWandEffect()
    {
        activeEffect.SetActive(true);
        Invoke(nameof(DisableWandActive), 1f);
    }

    private void DisableWandActive()
    {
        activeEffect.SetActive(false);
        wandActiveHolder.SetActive(false);
        wandIconTrans.gameObject.SetActive(false);
    }

    public void GameStarted(TargetData[] targetdata, int movecount)
    {
        targetData = targetdata;
        totalMoveCount = movecount;
        cuurentMoveCount = 0;
        targetCounts = new int[targetdata.Length];
        for (int i = 0; i < targetdata.Length; i++)
        {
            int num = targetdata[i].count;
            targetCounts[i] = num;
        }
    }



    private void CalculateAIAsists()
    {
        if (gameDataManager.GameTypeCode == 1)
        {
            cuurentMoveCount++;
            percentValue = (cuurentMoveCount * 100) / totalMoveCount;
            if (percentValue >= 20 && percentValue < 60)
            {
                blockTypeDiff = 2;
                abilityTypeDiff = 4;
                gemTypeDiff = 2;

            }
            else if (percentValue >= 60 && percentValue < 80)
            {
                blockTypeDiff = 1;
                abilityTypeDiff = 3;
                gemTypeDiff = 2;

            }
            else if (percentValue >= 85 && percentValue < 100)
            {
                blockTypeDiff = 1;
                abilityTypeDiff = 2;
                gemTypeDiff = 1;

            }
            else if (percentValue > 100)
            {
                blockTypeDiff = 1;
                abilityTypeDiff = 1;
                gemTypeDiff = 1;
            }
            
        }
    }

    private void ChangeTypeDiff()
    {
        int replayCount = gameDataManager.rePlayCount;
        if(blockTypeDiff > replayCount)
        {
            blockTypeDiff -= replayCount;
        }
        else
        {
            if(blockTypeDiff > 1)
            {
                blockTypeDiff-= 1;
            }
        }

        if (abilityTypeDiff > replayCount)
        {
            abilityTypeDiff -= replayCount;
        }
        else
        {
            if (abilityTypeDiff > 1)
            {
                abilityTypeDiff -= 1;
            }
        }
        if (gemTypeDiff > replayCount)
        {
            gemTypeDiff -= replayCount;
        }
        else
        {
            if (gemTypeDiff > 1)
            {
                gemTypeDiff -= 1;
            }
        }


    }
    public void AiAssists(int row, List<BlockTile> blockList, List<GemTile> gemList)
    {
        ChangeTypeDiff();
        if(targetData.Length > 0) {
            foreach (var item in targetData)
            {
                if (item.gemType == Gem_Type.none)
                {
                    int value = (int)item.normalBlockType;
                    if (value < 6)
                    {
                        int num = 0;
                        if (item.blockType == BlockType.Normal_Block)
                        {
                            num = blockTypeDiff;
                        }
                        else if (item.blockType != BlockType.None)
                        {
                            num = abilityTypeDiff;
                        }
                        int max = row;
                        int min = row - num;
                        int loopVal = (row - prevValue) / num;
                        for (int i = loopVal; i > 0; i--)
                        {
                            if (max <= prevValue || min == prevValue)
                            {
                                break;
                            }
                            if (min <= prevValue)
                            {
                                min = prevValue;
                                num = max - min;
                            }
                            List<BlockTile> sampleBlocks = BlockSampleSpace(blockList, max, min);

                            bool isAsist = true;
                            foreach (var block in sampleBlocks)
                            {
                                Normal_Block_Type type = (Normal_Block_Type)block.ColorCode;
                                BlockType abilityType = block.ThisBlockType;
                                if (item.normalBlockType == type && item.blockType == abilityType)
                                {
                                    isAsist = false;
                                    break;
                                }
                                else if (item.normalBlockType == Normal_Block_Type.none && item.blockType == abilityType)
                                {
                                    isAsist = false;
                                    break;
                                }
                            }
                            if (isAsist)
                            {
                                print("Asists");
                                BlockTypeChange(false, num, item, sampleBlocks, 0);
                            }
                            max = min;
                            min -= num;
                        }
                    }
                }
                else
                {
                    int num = gemTypeDiff;
                    int loopVal = (row - prevValue) / num;
                    int max = row;
                    int min = row - num;
                    for (int i = 0; i < loopVal + 1; i++)
                    {
                        if (max <= prevValue)
                        {
                            break;
                        }
                        if (min <= prevValue)
                        {
                            min = prevValue;
                            num = max - min;
                        }
                        List<GemTile> sampleGem = GemSampleSpace(gemList, max, min);
                        bool isAsist = true;
                        foreach (var gem in sampleGem)
                        {

                            if (item.gemType == gem.GemType)
                            {
                                isAsist = false;
                                break;
                            }
                        }
                        int gemIndex = (int)item.gemType;

                        if (isAsist)
                        {
                            if (sampleGem.Count > 0)
                            {
                                int randomGem = Random.Range(0, sampleGem.Count);
                                int maxitter = 50;
                                while (maxitter > 0)
                                {
                                    if (sampleGem.Count > randomGem && sampleGem[randomGem] != null)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        maxitter--;
                                        randomGem = Random.Range(0, sampleGem.Count);
                                    }
                                }
                                int rowVal = sampleGem[randomGem].RowValue;
                                int colVal = sampleGem[randomGem].ColumeValue;
                                if (sampleGem[randomGem].transform.position.y > 16)
                                    blockManager.ChangeGemTile(sampleGem[randomGem], gemIndex, rowVal, colVal);
                            }
                            else
                            {
                                List<BlockTile> sampleBlocks = BlockSampleSpace(blockList, max, min);
                                BlockTypeChange(true, num, item, sampleBlocks, gemIndex - 1);

                            }
                        }
                        max = min;
                        min -= num;

                    }

                }
            }
        }
        prevValue = row;
    }

    private void BlockTypeChange(bool isGem,int num, TargetData item, List<BlockTile> sampleBlocks, int gemindex)
    {
        int randomBlock = (Random.Range(0, num) * 9) + Random.Range(0, 9);
        int maxitter = 50;
        while (maxitter > 0)
        {
            if (sampleBlocks.Count > randomBlock && sampleBlocks[randomBlock] != null && !sampleBlocks[randomBlock].HasAsists)
            {
                break;
            }
            else
            {
                maxitter--;
                randomBlock = (Random.Range(0, num) * 9) + Random.Range(0, 9);
            }
        }
        if (!isGem)
        {
            int blockIndex = (int)item.normalBlockType;
            if (blockIndex >= 5)
            {
                blockIndex = sampleBlocks[randomBlock].ColorCode;
            }
            int abilityIndex = (int)item.blockType;

            if (sampleBlocks[randomBlock].transform.position.y > 17)
                blockManager.ChangeBlockType(sampleBlocks[randomBlock], blockIndex, abilityIndex, true);
        }
        else
        {
            int rowVal = sampleBlocks[randomBlock].RowValue;
            int colVal = sampleBlocks[randomBlock].ColumnValue;
            Vector2 pos = sampleBlocks[randomBlock].transform.position;
            Destroy(sampleBlocks[randomBlock].gameObject);
            boardManager.InstaGem(pos, gemindex, rowVal, colVal);
        }
    }

    private List<BlockTile> BlockSampleSpace(List<BlockTile> blocks, int max, int min)
    {
        List<BlockTile> sample = new List<BlockTile>();
        for (int i = blocks.Count - 1; i > 0; i--)
        {
            int rowval = blocks[i].RowValue;
            if(rowval <= max && rowval > min)
            {
                sample.Add(blocks[i]);
            }
        }
        return sample;
    }

    private List<GemTile> GemSampleSpace(List<GemTile> gemList, int max, int min)
    {
        List<GemTile> sample = new List<GemTile>();
        for (int i = gemList.Count - 1; i > 0; i--)
        {
            int rowval = gemList[i].RowValue;
            if (rowval <= max && rowval > min)
            {
                sample.Add(gemList[i]);
            }
        }
        return sample;
    }

    public void CheckForBombAIAsist(List<BlockTile> sampleBlocks)
    {
        if (isBombAiAsists && !hasBombAsisted)
        {
            for (int i = 0; i < sampleBlocks.Count; i++)
            {
                if (sampleBlocks[i] != null)
                {
                    Vector2 pos = sampleBlocks[i].transform.position;
                    RaycastHit2D[] hits = Physics2D.CircleCastAll(pos, 5, Vector2.zero, 0, layerMask);
                    int count = 0;
                    int abilitycount = 0;
                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.transform.TryGetComponent<BlockTile>(out var block))
                        {
                            if (block.ThisBlockType != BlockType.None)
                            {
                                int blocktypeindex = (int)block.ThisBlockType;
                                if (blocktypeindex == 0)
                                {
                                    count++;
                                }
                                else
                                {
                                    abilitycount += blocktypeindex;
                                }
                            }
                        }
                    }

                    if (abilitycount < 4 && count > 10)
                    {
                        isBombAI = true;
                        break;
                    }
                }
            }
        }
    }


    public bool HasBomb(Vector2 pos)
    {
        bool bomb = false;
        if (isBombAI)
        {
            bomb = true;
            hasBombAsisted = true;
            isBombAI = false;
            AiEffect(pos);
        }
        return bomb;
    }

    public void ReSetModBomb()
    {
        if (hasBombAsisted)
        {
            isBombAI = true;
        }
    }

    public void ChangeBombAsisted()
    {
        hasBombAsisted = false;
    }

    private void AiEffect(Vector2 pos)
    {
        if (!isAIEffectActive)
        {
            isAIEffectActive = true;
            wandObject.transform.position = pos;
            wandObject.SetActive(true);
            Invoke(nameof(DisableEffectObject), 2f);
        }
    }

    public void DisableEffectObject()
    {
        wandObject.SetActive(false);
        isAIEffectActive = false;
    }

}
