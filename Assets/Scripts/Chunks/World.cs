using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public static World inst;
    public BiomeAttribute biome;
    public int seed;
    public static Dictionary<Vector2Int,Chunk> chunks;
    public static List<Vector2Int> activeChunks;
    public GameObject chunkPrefab;
    public GameObject player;
    public Block[] blockTypes;
    public static Dictionary<string, byte> blockNames;
    public static Vector2Int playerChunk;
    void Awake()
    {
        if (inst){
            Destroy(gameObject);
            return;
        }
        inst = this;

        if (blockTypes == null || blockTypes.Length == 0){
            blockTypes = Resources.LoadAll<Block>("Blocks");
            System.Array.Sort(blockTypes); //Make sure Blocks correspond to their IDs
        }
        blockNames = new Dictionary<string, byte>();
        foreach (Block block in blockTypes)
        {
            blockNames.Add(block.name, block.ID);
        }
        chunks = new Dictionary<Vector2Int, Chunk>();
        activeChunks = new List<Vector2Int>();

        Random.InitState(seed);
        GenerateWorld();
        player.transform.position = new Vector3(3,40,3);
        playerChunk = GetChunkCoords(GetCoords(player.transform.position));
    }
    private void Update() {
        Vector2Int newPlayerChunk = GetChunkCoords(GetCoords(player.transform.position));
        if (playerChunk != newPlayerChunk){
            playerChunk = newPlayerChunk;
            //CheckViewDistance();
        }
    }
    #region WorldGen
    public void GenerateWorld(){
        for (int x = -VoxelData.renderDistance; x < VoxelData.renderDistance+1; x++){
            for (int z = -VoxelData.renderDistance; z < VoxelData.renderDistance+1; z++){
                Chunk newChunk = AddChunk(new Vector2Int(x,z));
            }
        }
    }
    public byte GetVoxel(Vector3Int pos){
        //Immutable Pass
        if (pos.y == 0) return 6; //If bottom block return bedrock
        //Basic Terrain Pass
        byte voxel = 0;
        int terrainHeight = Mathf.FloorToInt(biome.terrainHeight*
            Noise.Get2DPerlin(new Vector2(pos.x,pos.z), 10000f, biome.terrainScale))+biome.solidGroundHeight;
        if (pos.y == terrainHeight) voxel = 2;
        else if (pos.y < terrainHeight && pos.y > terrainHeight - 4) voxel = 3;
        else if (pos.y > terrainHeight) return 0;
        else voxel = 1;
        //Second Pass
        if (voxel == 1){
            foreach (Lode lode in biome.lodes)
            {
                if (pos.y > lode.minHeight && pos.y < lode.maxHeight){
                    if (Noise.Get3DPerlin(pos,lode.noiseOffset+10000f,lode.scale,lode.threshold)){
                        voxel = lode.blockID;
                    }
                }
            }
        }
        return voxel;

    }
    public Chunk AddChunk(Vector2Int chunkCoords){
        Vector3Int coords = GetCoordsOfChunk(chunkCoords);
        Chunk newChunk = Instantiate(inst.chunkPrefab, GetWorldPosition(coords),
            Quaternion.identity, inst.transform).GetComponent<Chunk>();
        chunks.Add(chunkCoords, newChunk);
        activeChunks.Add(chunkCoords);
        return newChunk;
    }
    #endregion
    public static bool CheckForVoxel(float x, float y, float z){
        int xCheck = Mathf.FloorToInt(x);
        int yCheck = Mathf.FloorToInt(y);
        int zCheck = Mathf.FloorToInt(z);

        int xChunk = xCheck / VoxelData.chunkWidth;
        int zChunk = zCheck / VoxelData.chunkWidth;

        xCheck -= (xChunk * VoxelData.chunkWidth);
        zCheck -= (zChunk * VoxelData.chunkWidth);

        return ((Block)chunks[new Vector2Int(xChunk,zChunk)].blocks[xCheck, yCheck, zCheck]).isSolid;
    }
    void CheckViewDistance(){
        List<Vector2Int> previouslyActiveChunks = new List<Vector2Int>(activeChunks);
        for (int x = playerChunk.x-VoxelData.renderDistance; x < playerChunk.x+VoxelData.renderDistance+1; x++){
            for (int z = playerChunk.y-VoxelData.renderDistance; z < playerChunk.y+VoxelData.renderDistance+1; z++){
                if (!IsChunkInWorld(new Vector2Int(x,z))){
                    Chunk newChunk = AddChunk(new Vector2Int(x,z));
                } else if (!chunks[new Vector2Int(x,z)].isActive){
                    chunks[new Vector2Int(x,z)].isActive = true;
                    activeChunks.Add(new Vector2Int(x,z));
                }
                for (int i = 0; i < previouslyActiveChunks.Count; i++){
                    if (previouslyActiveChunks[i].Equals(new Vector2Int(x,z)))
                        previouslyActiveChunks.RemoveAt(i);
                }
            }
        }
        foreach (Vector2Int chunkCoord in previouslyActiveChunks){
            chunks[chunkCoord].isActive = false;
            activeChunks.Remove(chunkCoord);
        }
    }
    #region Find Chunks
    public static bool IsChunkInWorld(Vector2Int chunkCoords){
        return chunks.ContainsKey(chunkCoords);
    }
    public static bool TryGetChunk(Vector2Int chunkCoords, out Chunk chunk){
        return chunks.TryGetValue(chunkCoords, out chunk);
    }
    public static bool TryGetChunkAtPos(Vector3Int coords, out Chunk chunk){
        return chunks.TryGetValue(GetChunkCoords(coords), out chunk);
    }
    public static bool TryGetBlock(Vector3Int coords, out byte id){
        if (coords.y < VoxelData.chunkHeight && coords.y >= 0){
            Chunk foundChunk;
            if (TryGetChunkAtPos(coords, out foundChunk)){
                coords = GetCoordsInChunk(coords);
                id = foundChunk.blocks[coords.x,coords.y,coords.z];
                return true;
            }
            if (coords.sqrMagnitude < 3500) print(coords);
            id = 0;
            return false;
        }
        id = 0;
        return true;
    }
    #endregion
    #region Conversions
    public static Vector2Int GetChunkCoords(Vector3Int coords){
        if (coords.x < 0){
            coords.x -= VoxelData.chunkWidth-1;
        } if (coords.z < 0){
            coords.z -= VoxelData.chunkWidth-1;
        }
        return new Vector2Int(coords.x,coords.z) / VoxelData.chunkWidth;
    }
    public static Vector3Int GetCoordsInChunk(Vector3Int coords){
        if (coords.x < 0){
            coords.x *= -1;
        } if (coords.z < 0){
            coords.z *= -1;
        }
        Vector2Int v2 = new Vector2Int(coords.x % VoxelData.chunkWidth,coords.z % VoxelData.chunkWidth);
        return new Vector3Int(v2.x,coords.y,v2.y);
    }
    public static Vector3Int GetCoordsOfChunk(Vector2Int chunkCoords){
        return new Vector3Int(chunkCoords.x,0,chunkCoords.y) * VoxelData.chunkWidth;
    }
    public static Vector3 GetWorldPosition(int x, int y, int z){
        return new Vector3(x, y, z) * VoxelData.blockSize + inst.transform.position;
    }
    public static Vector3 GetWorldPosition(Vector3Int coords){
        return (Vector3)(coords) * VoxelData.blockSize + inst.transform.position;
    }
    public static Vector3Int GetCoords(Vector3 worldPosition){
        worldPosition -= inst.transform.position;
        return new Vector3Int(Mathf.FloorToInt(worldPosition.x/VoxelData.blockSize),
            Mathf.FloorToInt(worldPosition.y/VoxelData.blockSize),
            Mathf.FloorToInt(worldPosition.z/VoxelData.blockSize));
    }
    #endregion
    #region Block Conversions
    public static Block GetBlock(byte ID){
        return inst.blockTypes[ID];
    }
    public static Block GetBlock(string blockName){
        return GetBlock(GetBlockID(blockName));
    }
    public static byte GetBlockID(string blockName){
        return blockNames[blockName];
    }
    public static byte GetRandomBlockID(){
        return (byte)Random.Range(0,inst.blockTypes.Length);
    }
    #endregion
}
