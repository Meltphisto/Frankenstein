using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BlockType
{
    Head,
    Torso,
    Leg,
    Extra
}
public class FusionBlock : MonoBehaviour, IFusionMaterialHolder
{
    public BlockType blockType;

    [SerializeField] private Transform holdingPoint;
    private FusionMaterialObject fusionMaterial;

    public void ClearHoldingObject()
    {
        fusionMaterial = null;
    }

    public FusionMaterialObject GetFusionMaterialObject()
    {
        return fusionMaterial;
    }

    public Transform GetHoldPointTransform()
    {
        return holdingPoint;
    }

    public bool IsHoldingObject()
    {
        return fusionMaterial != null;
    }

    public void SetHoldingObject(FusionMaterialObject fusionMaterialObject)
    {
        fusionMaterial = fusionMaterialObject;
    }

    public BlockType GetBlockType()
    {
        return blockType;
    }
}
