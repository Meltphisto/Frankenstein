using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLocation : BaseLocation,IFusionMaterialHolder
{
    [SerializeField] private GameObject output;
    [SerializeField] private Transform spawnPoint;

    private FusionMaterialObject holdingFusionMaterialObject;

    public override void Interact(Player player)
    {
        if (!IsHoldingObject())
        {
            if (player.IsHoldingObject())
            {
                //Do nothing
            }
            else
            {
                GameObject item = Instantiate(output);

                item.GetComponent<FusionMaterialObject>().SetFusionMaterialObjectParent(this);
            }
        }
        else
        {
            if (player.IsHoldingObject())
            {
                //Do nothing
            }
            else
            {
                GetFusionMaterialObject().SetFusionMaterialObjectParent(player);
            }
        }
    }

    public void ClearHoldingObject()
    {
        holdingFusionMaterialObject = null;
    }

    public FusionMaterialObject GetFusionMaterialObject()
    {
        return holdingFusionMaterialObject;
    }

    public Transform GetHoldPointTransform()
    {
        return spawnPoint;
    }

    public bool IsHoldingObject()
    {
        return holdingFusionMaterialObject != null;
    }

    public void SetHoldingObject(FusionMaterialObject fusionMaterialObject)
    {
        holdingFusionMaterialObject = fusionMaterialObject;
    }
}
