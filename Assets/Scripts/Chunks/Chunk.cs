using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public byte[,,] blocks;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshFilter meshFilter;
    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    public bool isActive{get{return gameObject.activeSelf;} set{gameObject.SetActive(value);}}
    public Vector3Int getCoords;
    public Vector2Int chunkCoords;
    private void Awake() {
        blocks = new byte[VoxelData.chunkWidth, VoxelData.chunkHeight, VoxelData.chunkWidth];
        getCoords = World.GetCoords(transform.position);
        chunkCoords = World.GetChunkCoords(getCoords);
        PopulateChunk();
    }
    private void Start() {
        LoadChunk();
    }
    public void LoadChunk() {
        RunOnEach(AddVoxelDataToChunk);

        CreateMesh();
    }
    public void PopulateChunk(){
        for (int y = 0; y < VoxelData.chunkHeight; y++){
            for (int x = 0; x < VoxelData.chunkWidth; x++){
                for (int z = 0; z < VoxelData.chunkWidth; z++){
                    blocks[x,y,z] = World.inst.GetVoxel(getCoords + new Vector3Int(x,y,z));
                }
            }
        }
    }
    #region Mesh
    bool CheckVoxelSolid(Vector3Int pos){
        if (pos.y < 0 || pos.y >= VoxelData.chunkHeight) return false;
        byte block;
        if (IsInChunk(pos)){
            block = blocks[pos.x,pos.y,pos.z];
        }
        else{
            /*if (!World.TryGetBlock(pos+getCoords, out block)){
                block = 1;World.inst.GetVoxel(pos+getCoords);
            }*/
            block = World.inst.GetVoxel(pos+getCoords);
        }
        return ((Block)block).isSolid;
    }
    void AddVoxelDataToChunk(Vector3Int pos){
        byte blockID = Get(pos);
        if (blockID == 0) return;
        for (int i = 0; i < 6; i++){
            if (!CheckVoxelSolid(pos + VoxelData.faceChecks[i])){
                for (int j = 0; j < 4; j++){
                    vertices.Add((pos+VoxelData.voxelVerts[VoxelData.voxelTris[i,j]])*VoxelData.blockSize);
                }
                AddTexture(World.GetBlock(blockID).GetTextureID(i));
                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex+1);
                triangles.Add(vertexIndex+2);
                triangles.Add(vertexIndex+2);
                triangles.Add(vertexIndex+1);
                triangles.Add(vertexIndex+3);
                vertexIndex += 4;
            }
        }
    }
    void CreateMesh(){
        Mesh mesh = new Mesh{
            vertices = vertices.ToArray(),
            triangles = triangles.ToArray(),
            uv = uvs.ToArray()
        };
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }
    void AddTexture(int textureID){
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);
        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;
        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x,y));
        uvs.Add(new Vector2(x,y+VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x+VoxelData.NormalizedBlockTextureSize,y));
        uvs.Add(new Vector2(x+VoxelData.NormalizedBlockTextureSize,y+VoxelData.NormalizedBlockTextureSize));
    }
    #endregion
    #region Get/Set
    public byte Get(Vector3Int coords){
        return Get(coords.x,coords.y,coords.z);
    }
    public byte Get(int x, int y, int z){
        if (IsInChunk(x,y,z))
            return blocks[x,y,z];
        return 0;
    }
    public bool IsInChunk(int x, int y, int z){
        return x >= 0 && x < VoxelData.chunkWidth && y >= 0 && y < VoxelData.chunkHeight && z >= 0 && z < VoxelData.chunkWidth;
    }
    public bool IsInChunk(Vector3Int coords){
        return IsInChunk(coords.x, coords.y, coords.z);
    }
    #endregion
    #region AllBlocks
    public void SetAllBlocks(byte block){
        for (int y = 0; y < VoxelData.chunkHeight; y++){
            for (int x = 0; x < VoxelData.chunkWidth; x++){
                for (int z = 0; z < VoxelData.chunkWidth; z++){
                    blocks[x,y,z] = block;
                }
            }
        }
    }
    public void RunOnEach(Action<byte> method){
        for (int x = 0; x < VoxelData.chunkWidth; x++) {
            for (int y = 0; y < VoxelData.chunkHeight; y++) {
                for (int z = 0; z < VoxelData.chunkWidth; z++) {
                    method(blocks[x,y,z]);
                }
            }
        }
    }
    public void RunOnEach(Action<byte,Vector3Int> method){
        for (int x = 0; x < VoxelData.chunkWidth; x++) {
            for (int y = 0; y < VoxelData.chunkHeight; y++) {
                for (int z = 0; z < VoxelData.chunkWidth; z++) {
                    method(blocks[x,y,z], new Vector3Int(x,y,z));
                }
            }
        }
    }
    public void RunOnEach(Action<Vector3Int> method){
        for (int x = 0; x < VoxelData.chunkWidth; x++) {
            for (int y = 0; y < VoxelData.chunkHeight; y++) {
                for (int z = 0; z < VoxelData.chunkWidth; z++) {
                    method(new Vector3Int(x,y,z));
                }
            }
        }
    }
    public void SetEach(Func<byte> method){
        for (int x = 0; x < VoxelData.chunkWidth; x++) {
            for (int y = 0; y < VoxelData.chunkHeight; y++) {
                for (int z = 0; z < VoxelData.chunkWidth; z++) {
                    blocks[x,y,z] = method();
                }
            }
        }
    }
    public void SetEach(Func<Vector3Int,byte> method){
        for (int x = 0; x < VoxelData.chunkWidth; x++) {
            for (int y = 0; y < VoxelData.chunkHeight; y++) {
                for (int z = 0; z < VoxelData.chunkWidth; z++) {
                    blocks[x,y,z] = method(new Vector3Int(x,y,z));
                }
            }
        }
    }
    public byte FindFirst(Func<byte,bool> method){
        for (int x = 0; x < VoxelData.chunkWidth; x++) {
            for (int y = 0; y < VoxelData.chunkHeight; y++) {
                for (int z = 0; z < VoxelData.chunkWidth; z++) {
                    if (method(blocks[x,y,z])) return blocks[x,y,z];
                }
            }
        }
        return default(byte);
    }
    public Vector3Int FindCoords(Func<byte,bool> method){
        for (int x = 0; x < VoxelData.chunkWidth; x++) {
            for (int y = 0; y < VoxelData.chunkHeight; y++) {
                for (int z = 0; z < VoxelData.chunkWidth; z++) {
                    if (method(blocks[x,y,z])) return new Vector3Int(x,y,z);
                }
            }
        }
        return new Vector3Int(-1,-1,-1);
    }
    public bool Contains(Func<byte,bool> method){
        for (int x = 0; x < VoxelData.chunkWidth; x++) {
            for (int y = 0; y < VoxelData.chunkHeight; y++) {
                for (int z = 0; z < VoxelData.chunkWidth; z++) {
                    if (method(blocks[x,y,z])) return true;
                }
            }
        }
        return false;
    }
    #endregion
}
