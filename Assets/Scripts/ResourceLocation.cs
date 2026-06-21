using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLocation : BaseLocation
{
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private FusionMaterialObjectSO outputMaterial;

    public override void Interact(Player player)
    {
        //Spawn resource and give it to player
        if (player.IsHoldingObject())
        {
            //Do nothing
        }
        else
        {
            FusionMaterialObject.SpawnFusionMaterialObject(outputMaterial, player);
        }
    }


}
