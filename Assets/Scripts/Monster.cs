using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private MonsterVisual headVisual;
    [SerializeField] private MonsterVisual torsoVisual;
    [SerializeField] private MonsterVisual legVisual;
    [SerializeField] private MonsterVisual extraVisual;
    public void SetMonsterPartVisual(BlockType blockType, FusionMaterialObject fusionMaterialObject)
    {
        MonsterVisual targetVisual = GetVisualObjectFromBlockType(blockType);
        Debug.Log(targetVisual.transform.name);
        if (fusionMaterialObject != null && targetVisual != null)
        {
            fusionMaterialObject.SetFusionMaterialObjectParent(targetVisual);
        }
    }

    private MonsterVisual GetVisualObjectFromBlockType(BlockType blockType)
    {
        switch(blockType)
        {
            case BlockType.Head:
                return headVisual;
            case BlockType.Torso:
                return torsoVisual;
            case BlockType.Leg:
                return legVisual;
            case BlockType.Extra:
                return extraVisual;
        }
        return null;
    }
}
