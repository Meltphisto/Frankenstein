using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFusionMaterialHolder
{
    public Transform GetHoldPointTransform();

    public void SetHoldingObject(FusionMaterialObject fusionMaterialObject);

    public FusionMaterialObject GetFusionMaterialObject();

    public void ClearHoldingObject();

    public bool IsHoldingObject();
}
