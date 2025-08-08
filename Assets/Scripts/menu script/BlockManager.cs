using JetBrains.Annotations;
using System;
using UnityEngine;


[Serializable]
public class Special_Object
{
    public Special_Object_Type Special_Object_Type;
    public string discription;
    public string uiShowDis;
    public Sprite iconSprite;
    public int hitPoint;
    public float endTime;
}

[Serializable]
public class ObsDesAnimSprite
{
    public Sprite[] obsDesAnimSprites;

}

[Serializable]
public class BlockTypeData
{
    public Sprite crushTileSprite;
    public Sprite[] blockTileSprite;
    public Sprite[] abilityTileSprite;
}

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance;
    public GameDataManager gameDataManager;
    [SerializeField] private BlockTile blockprefab;
    [SerializeField] private GameObject coinObject;
    [SerializeField] private SpecialObject specialObjTile;


    [SerializeField] private BlockTileData[] blockTileDatas;
    [SerializeField] private SetUpPrefabs[] abilityShowPrefabs;
    [SerializeField] private GameObject[] abilityEffect;
    [SerializeField] private Sprite colorBombSprite;

    public BlockTypeData[] AllTileData;
    private BlockTypeData currentTileData;

    public AbilityData[] tileDetailsData;
    [SerializeField] private Sprite[] ObsTargetSprite;
    [SerializeField] private ObsDesAnimSprite[] obsDesAnimSprites;

    [SerializeField] private Special_Object[] specialObjTypeData;
    [SerializeField] private Sprite[] gemTypeTargetSprite;


    [SerializeField] private Sprite[] giftTypeSprite;


    public Sprite CurrentCrushTile()
    {
        Sprite sprite = currentTileData.crushTileSprite;
        return sprite;
    }


    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        
    }
    private void Start()
    {
        if (MenuManager.Instance == null)
        {
            SelectBlockIcon(0);
        }
    }


    public void SelectBlockIcon(int num)
    {

        gameDataManager.SetSaveValues(4, num);
        currentTileData = AllTileData[num];
        SetUpBlockPrefabs();
        TileShow.OnBlockTypeUpdate.Invoke(num);
    }


    private void SetUpBlockPrefabs()
    {
        if (CurrentCrushTile() == null) return;
        for (int i = 0; i < currentTileData.abilityTileSprite.Length; i++)
        {
            abilityShowPrefabs[i].SetUpSprite(currentTileData.abilityTileSprite[i]);
        }

        for (int i = 0; i < currentTileData.blockTileSprite.Length; i++)
        {
            blockTileDatas[i].blockIconSprite = currentTileData.blockTileSprite[i];
        }
    }







    public BlockTile InstatiateBlockObject(Vector2 pos,Transform parent, int blockindex,int abilityIndex, int row, int col)
    {
        BlockTile tile = Instantiate(blockprefab, parent);
        BlockTileData data = blockTileDatas[blockindex];
        GameObject effect = Instantiate(data.blockEffect, tile.transform);

        GameObject abilityeffect = AbilityEffectSetUp(abilityIndex, tile.transform);
        GameObject abilityshow = AbilityShowtSetUp(abilityIndex, tile.transform);
        GameObject coin = null;
        if (blockindex == 5)
        {
            coin = Instantiate(coinObject, tile.transform);
        }
        BlockType type = (BlockType)abilityIndex;
        tile.BlockTileSetUp(data, effect, type, abilityshow, abilityeffect, coin, row, col, blockindex);
        tile.transform.position = pos;

        return tile;
    }

    private GameObject AbilityEffectSetUp(int abilityIndex, Transform tile)
    {
        GameObject abilityeffect = null;
        if (abilityIndex > 0 && abilityIndex < 5)
        {
            abilityeffect = Instantiate(abilityEffect[abilityIndex - 1], tile);
        }
        return abilityeffect;
    }

    private GameObject AbilityShowtSetUp(int abilityIndex, Transform tile)
    {
        GameObject abilityshow = null;
        if (abilityIndex > 0 && abilityIndex <= 5)
        {
            abilityshow = Instantiate(abilityShowPrefabs[abilityIndex - 1].gameObject, tile.transform);
        }
        return abilityshow;
    }

    public void ColorBombMod(int abilityIndex, BlockTile tile)
    {
        GameObject abilityeffect = AbilityEffectSetUp(abilityIndex, tile.transform);
        GameObject abilityshow = AbilityShowtSetUp(abilityIndex, tile.transform);
        BlockType type = (BlockType)abilityIndex;
        tile.AbilityEffectsSetUp(type, abilityshow, abilityeffect);
    }

    public void ChangeBlockType(BlockTile tile, int blockindex, int abilityIndex, bool isasists)
    {
        BlockTileData data = blockTileDatas[blockindex];
        GameObject effect = Instantiate(data.blockEffect, tile.transform);
        GameObject abilityeffect = AbilityEffectSetUp(abilityIndex, tile.transform);
        GameObject abilityshow = AbilityShowtSetUp(abilityIndex, tile.transform);
        BlockType type = (BlockType)abilityIndex;
        tile.ChangeBlockTypeSetUp(isasists, data, effect, type, abilityshow, abilityeffect, blockindex);
    }

    public BlockTileData GetBlockTileDatas(int index)
    {
        return blockTileDatas[index];
    }

    public Sprite IconSprite(int num)
    {
        Sprite sprite = blockTileDatas[num].blockIconSprite;
        return sprite;
    }
    public Sprite AbilitySprite(int num)
    {
        Sprite sprite = currentTileData.abilityTileSprite[num];
        return sprite;
    }

    public Sprite ColorAbilitySprite()
    {
        return colorBombSprite;
    }

    public Sprite ObstacleSprite(int index)
    {
        Sprite sprite = null;
        if(index >= 0 && index < tileDetailsData.Length)
            sprite = tileDetailsData[index].iconSprite;

        return sprite;
    }
    public Sprite[] ObsDesAnimSprites(int index)
    {
        Sprite[] sprite = null;
        if (index >= 0 && index < obsDesAnimSprites.Length)
            sprite = obsDesAnimSprites[index].obsDesAnimSprites;

        return sprite;
    }

    public Sprite ObstacleTargetEffectShowSprite(int index)
    {
        Sprite sprite = null;
        if (index >= 0 && index < ObsTargetSprite.Length)
            sprite = ObsTargetSprite[index];

        return sprite;
    }

    public SpecialObject InstantiateSpObjectTile(Vector2 pos, Transform parent, int specialobjindex)
    {
        Special_Object_Type specialobjtype = (Special_Object_Type)(specialobjindex + 1);

        SpecialObject tile = Instantiate(specialObjTile, parent);
        tile.SetUp(specialObjTypeData[specialobjindex].iconSprite, specialobjtype, specialObjTypeData[specialobjindex].hitPoint);
        tile.transform.position = pos;
        return tile;
    }
    public void ChangeGemTile(SpecialObject specialObj, int specialobjindex, int rowValue, int colValue)
    {
        Special_Object_Type specialobjtype = (Special_Object_Type)(specialobjindex);
        specialObj.SetUp(specialObjTypeData[specialobjindex - 1].iconSprite, specialobjtype, specialObjTypeData[specialobjindex - 1].hitPoint);
        print(specialobjindex);
    }

    public Special_Object GetSpecialObject(int index)
    {
        if(specialObjTypeData.Length > index)
        {
            return specialObjTypeData[index];
        }
        else
        {
            return null;
        }
    }


    public Sprite GemTypeSprite(int gemindex)
    {
        Sprite sprite = gemTypeTargetSprite[gemindex];
        return sprite;
    }


}
