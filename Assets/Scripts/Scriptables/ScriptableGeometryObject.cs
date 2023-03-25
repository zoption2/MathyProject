using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Create a geometry object list
/// </summary>
[CreateAssetMenu(fileName = "New Geometry Objects List", menuName = "ScriptableObjects/Geometry Objects List")]
public class ScriptableGeometryObject : ScriptableObject
{
    public List<GeometryObject> objects;
}

[Serializable] public struct GeometryObject
{
    public string Name;
    public Sprite Image;
}