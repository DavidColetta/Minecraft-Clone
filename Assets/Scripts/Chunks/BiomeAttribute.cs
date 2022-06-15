using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeAttribute", menuName = "Minecraft/BiomeAttribute", order = 0)]
public class BiomeAttribute : ScriptableObject {
    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;
    public Lode[] lodes;
}
[System.Serializable]
public class Lode{
    public string name;
    public byte blockID;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
}
