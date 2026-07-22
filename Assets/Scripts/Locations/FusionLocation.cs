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

    private bool fusionStarts = false;
    public override void Interact(Player player)
    {
        //if (fusionStarts)
        //{
        //    return;
        //}
        //Current design: Player cant take object away after they drop object
        if (!player.IsHoldingObject())
        {
            return;
        }

        FusionMaterialObject playerHoldingObject = player.GetFusionMaterialObject();

        FusionBlock emptyBlock = GetEmptyBlockForFusionMaterial(playerHoldingObject);

        if (emptyBlock != null)
        {
            playerHoldingObject.SetFusionMaterialObjectParent(emptyBlock);
        }

        if (IsMonsterCanBeSummoned())
        {
            SpawnMonster();
            Debug.Log(1);
            //if (extraBlock.IsHoldingObject())
            //{
            //    return;
            //}
        }
    }

    public override void InteractAlter(Player player)
    {
        //if
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
        //Set monster transform
        Transform monsterTransform = Instantiate(monsterPrefab.transform);
        monsterTransform.SetParent(monsterSpawnPoint);
        monsterTransform.localPosition = Vector3.zero;
        monsterTransform.rotation = Quaternion.identity;

        //Set monster visual
        MonsterVisual monsterVisual = monsterTransform.GetComponent<MonsterVisual>();
        monsterVisual.SetMonsterPartVisual(headBlock.GetBlockType(), headBlock.GetFusionMaterialObject());
        monsterVisual.SetMonsterPartVisual(torsoBlock.GetBlockType(), torsoBlock.GetFusionMaterialObject());
        monsterVisual.SetMonsterPartVisual(legBlock.GetBlockType(), legBlock.GetFusionMaterialObject());
        monsterVisual.SetMonsterPartVisual(extraBlock.GetBlockType(), extraBlock.GetFusionMaterialObject());


    }
}
