using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FusionMaterialObject : MonoBehaviour
{
    [SerializeField] private FusionMaterialObjectSO fusionMaterialObjectSO;

    private IFusionMaterialHolder holder;

    public void SetFusionMaterialObjectParent(IFusionMaterialHolder newHolder)
    {

        if (holder != null)
        {
            holder.ClearHoldingObject();
        }

        if (newHolder.IsHoldingObject())
        {
            Debug.LogError("This one is holding an object");
        }

        holder = newHolder;
        newHolder.SetHoldingObject(this);
        transform.SetParent(newHolder.GetHoldPointTransform());
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    //Spawn fusion materail objet and set its holder
    public static void SpawnFusionMaterialObject(FusionMaterialObjectSO objectToSpawn, IFusionMaterialHolder holder)
    {
        Transform spawnedTransform = Instantiate(objectToSpawn.prefab);
        FusionMaterialObject spawnedObject = spawnedTransform.GetComponent<FusionMaterialObject>();

        spawnedObject.SetFusionMaterialObjectParent(holder);
    }

    public FusionMaterialObjectSO GetFusionMaterialObjectSO()
    {
        return fusionMaterialObjectSO;
    }

    public IFusionMaterialHolder GetHolder()
    {
        return holder;
    }

    public MaterialType GetMaterialType()
    {
        return fusionMaterialObjectSO.materialType;
    }
}
