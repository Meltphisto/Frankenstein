using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class FusionLocation : BaseLocation
{
    [SerializeField] private FusionBlock headBlock;
    [SerializeField] private FusionBlock torsoBlock;
    [SerializeField] private FusionBlock legBlock;
    [SerializeField] private FusionBlock extraBlock;

    [SerializeField] private Transform monsterSpawnPoint;
    [SerializeField] private GameObject monsterPrefab;
    public override void Interact(Player player)
    {
        //Current design: Player cant take object away after they drop object
        if (!player.IsHoldingObject())
        {
            return;
        }

        if (IsMonsterCanBeSummoned())
        {
            if (extraBlock.IsHoldingObject())
            {
                SpawnMonster();
                return;
            }
        }

        FusionMaterialObject playerHoldingObject = player.GetFusionMaterialObject();

        FusionBlock emptyBlock = GetEmptyBlockForFusionMaterial(playerHoldingObject);

        if (emptyBlock != null)
        {
            playerHoldingObject.SetFusionMaterialObjectParent(emptyBlock);
        }
    }

    private FusionBlock GetEmptyBlockForFusionMaterial(FusionMaterialObject fusionMaterialObject)
    {
        MaterialType materialType = fusionMaterialObject.GetMaterialType();

        if (materialType == MaterialType.None)
        {
            return null;
        }

        //Base parts are assigned, try adding extra fusion material
        if (IsMonsterCanBeSummoned())
        {
            return GetFusionBlockIfIsEmpty(extraBlock);
        }

        //Base parts havent assigned, get relative fusion block
        switch (materialType)
        {
            case MaterialType.Head:
                return GetFusionBlockIfIsEmpty(headBlock);
            case MaterialType.Torso:
                return GetFusionBlockIfIsEmpty(torsoBlock);
            case MaterialType.Leg:
                return GetFusionBlockIfIsEmpty(legBlock);
        }

        return null;
    }

    private FusionBlock GetFusionBlockIfIsEmpty(FusionBlock targetBlock)
    {
        if (targetBlock.IsHoldingObject())
        {
            return null;
        }
        else return targetBlock;
    }

    private bool IsMonsterCanBeSummoned()
    {
        return headBlock.IsHoldingObject() && torsoBlock.IsHoldingObject() && legBlock.IsHoldingObject();
    }

    public void SpawnMonster()
    {
        Transform monsterTransform = Instantiate(monsterPrefab.transform);
        monsterTransform.SetParent(monsterSpawnPoint);
        monsterTransform.localPosition = Vector3.zero;
        monsterTransform.rotation = Quaternion.identity;
        Monster monster = monsterTransform.GetComponent<Monster>();

        monster.SetMonsterPartVisual(headBlock.GetBlockType(), headBlock.GetFusionMaterialObject());
        monster.SetMonsterPartVisual(torsoBlock.GetBlockType(), torsoBlock.GetFusionMaterialObject());
        monster.SetMonsterPartVisual(legBlock.GetBlockType(), legBlock.GetFusionMaterialObject());
        monster.SetMonsterPartVisual(extraBlock.GetBlockType(), extraBlock.GetFusionMaterialObject());
    }
}
