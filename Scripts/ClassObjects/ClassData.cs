using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ClassData", menuName = "ClassData/ClassData", order = 1)]
[Serializable]
public class ClassData : ScriptableObject
{   
    [Header("ClassInfo")]
    public string className="";
    public string classInfo="";
    public Sprite classImg;
    public int classMaxHp;
    public float classSpeed;
    public int classAtk;
    public int classDef;
}

