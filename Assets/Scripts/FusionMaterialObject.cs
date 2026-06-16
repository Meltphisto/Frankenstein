using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionMaterialObject : MonoBehaviour
{
    private IFusionMaterialHolder holder;

    public void SetFusionMaterialObjectParent(IFusionMaterialHolder newHolder)
    {

        if (holder != null)
        {
            holder.ClearHoldingObject();
        }

        if (newHolder.IsHoldingObject())
        {
            Debug.LogError("This one has a kitchenObject");
        }

        holder = newHolder;
        newHolder.SetHoldingObject(this);
        transform.SetParent(newHolder.GetHoldPointTransform());
        transform.localPosition = Vector3.zero;
    }
}
