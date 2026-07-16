using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class ClearLocation : BaseLocation, IFusionMaterialHolder
{
    [SerializeField] private Transform holdingPoint;

    private FusionMaterialObject holdingFusionMaterialObject;

    public override void Interact(Player player)
    {
        if (!IsHoldingObject())
        {
            if (!player.IsHoldingObject())
            {
                //Do nothing
            }
            else
            {
                player.GetFusionMaterialObject().SetFusionMaterialObjectParent(this);
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
        return holdingPoint;
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


