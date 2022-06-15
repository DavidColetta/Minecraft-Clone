using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Block", menuName = "Minecraft/Block", order = 0)]
public class Block : ScriptableObject, IComparable<Block> {
    [SerializeField] protected string _name = "New Block";
    [SerializeField] protected byte _ID;
    [SerializeField] protected bool solid = true;
    [Header("Texture Values")]
    [SerializeField] private int backFaceTexture;
    [SerializeField] private int frontFaceTexture;
    [SerializeField] private int topFaceTexture;
    [SerializeField] private int bottomFaceTexture;
    [SerializeField] private int leftFaceTexture;
    [SerializeField] private int rightFaceTexture;
    #region Getters
    public new string name{get{return _name;}}
    public byte ID{get{return _ID;}}
    public bool isSolid{get{return solid;}}
    #endregion
    public static implicit operator Block(byte ID){
        return World.GetBlock(ID);
    }
    public static implicit operator byte(Block block){
        return block.ID;
    }
    public int CompareTo(Block other){
        return ID - other.ID;
    }
    public int GetTextureID(int faceIndex){

        switch (faceIndex)
        {
            case 0:
                return backFaceTexture;
            case 1:
                return frontFaceTexture;
            case 2:
                return topFaceTexture;
            case 3:
                return bottomFaceTexture;
            case 4:
                return leftFaceTexture;
            case 5:
                return rightFaceTexture;
            default:
                Debug.LogWarning($"Invalid face index: {faceIndex}");
                return topFaceTexture;
        }
    }
    private void Awake() {
        Debug.Log("ScriptableObj "+_name);
        if (_ID == 0 && _name == "New Block")
        _ID = (byte)Resources.LoadAll<Block>("Blocks").Length;
    }
}
