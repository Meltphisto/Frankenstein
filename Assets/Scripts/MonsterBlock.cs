using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBlock : MonoBehaviour, IFusionMaterialHolder
{
    [SerializeField] private BlockType partType;

    private FusionMaterialObject holdingFusionMaterialObject;
   
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
        return transform;
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
