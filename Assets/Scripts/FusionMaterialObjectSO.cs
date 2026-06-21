using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaterialType
{
    None,
    Head,
    Torso,
    Leg
}

[CreateAssetMenu()]
public class FusionMaterialObjectSO : ScriptableObject
{
    public int id;
    public string itemName;
    public Transform prefab;
    public MaterialType materialType;
    public Sprite sprite;
}
