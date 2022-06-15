using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class Grid3D<T>
{
    #region Variables
    public delegate void OnObjectsChanged(int x, int y, int z);
    public event OnObjectsChanged onObjectsChanged;
    public readonly int width;
    public readonly int height;
    public readonly int length;
    public readonly float cellSize;
    public readonly Vector3 originPos;
    public readonly T[,,] gridArray;
    #endregion
    #region Constructors
    public Grid3D(int width, int height, int length, Vector3 originPos, float cellSize = 1)
        : this(new T[width,height,length],originPos,cellSize){}
    public Grid3D(T[,,] defaultArray, Vector3 originPos, float cellSize = 1){
        this.cellSize = cellSize;
        this.originPos = originPos;
        gridArray = defaultArray;
        width = defaultArray.GetLength(0);
        height = defaultArray.GetLength(1);
        length = defaultArray.GetLength(2);
    }
    #endregion
    #region Conversions
    public Vector3 GetWorldPosition(int x, int y, int z){
        return new Vector3(x, y, z) * cellSize + originPos;
    }
    public Vector3 GetWorldPosition(Vector3Int coords){
        return (Vector3)(coords) * cellSize + originPos;
    }
    public Vector3Int GetCoords(Vector3 worldPosition){
        worldPosition -= originPos;
        return new Vector3Int(Mathf.FloorToInt(worldPosition.x/cellSize),
            Mathf.FloorToInt(worldPosition.y/cellSize),
            Mathf.FloorToInt(worldPosition.z/cellSize));
    }
    public bool IsValid(int x, int y, int z){
        return x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < length;
    }
    public bool IsValid(Vector3Int coords){
        return IsValid(coords.x, coords.y, coords.z);
    }
    public Vector3 SnapWorldPosToGrid(Vector3 worldPosition){
        return GetWorldPosition(GetCoords(worldPosition));
    }
    #endregion
    #region Get/Set
    public T Get(int x, int y, int z){
        if (!IsValid(x,y,z)) return default(T);
        return gridArray[x,y,z];
    }
    public T Get(Vector3Int coords){
        return Get(coords.x, coords.y, coords.z);
    }
    public T Get(Vector3 worldPosition){
        return Get(GetCoords(worldPosition));
    }
    public void Set(int x, int y, int z, T value){
        if (!IsValid(x, y, z)) return;
        gridArray[x,y,z] = value;
        InvokeObjectChanged(x,y,z);
    }
    public void Set(Vector3Int coords, T value){
        Set(coords.x, coords.y, coords.z, value);
    }
    public void Set(Vector3 worldPosition, T value){
        Set(GetCoords(worldPosition), value);
    }
    #endregion
    #region Lambda Expressions
    public void RunOnEach(Action<T> method){
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    method(gridArray[x,y,z]);
                }
            }
        }
    }
    public void RunOnEach(Action<T,Vector3Int> method){
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    method(gridArray[x,y,z], new Vector3Int(x,y,z));
                }
            }
        }
    }
    public void RunOnEach(Action<Vector3Int> method){
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    method(new Vector3Int(x,y,z));
                }
            }
        }
    }
    public void SetEach(Func<T> method){
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    gridArray[x,y,z] = method();
                }
            }
        }
    }
    public void SetEach(Func<Vector3Int,T> method){
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    gridArray[x,y,z] = method(new Vector3Int(x,y,z));
                }
            }
        }
    }
    public T FindFirst(Func<T,bool> method){
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    if (method(gridArray[x,y,z])) return gridArray[x,y,z];
                }
            }
        }
        return default(T);
    }
    public Vector3Int FindCoords(Func<T,bool> method){
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    if (method(gridArray[x,y,z])) return new Vector3Int(x,y,z);
                }
            }
        }
        return new Vector3Int(-1,-1,-1);
    }
    public bool Contains(Func<T,bool> method){
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < length; z++) {
                    if (method(gridArray[x,y,z])) return true;
                }
            }
        }
        return false;
    }
    #endregion
    #region Misc
    public void InvokeObjectChanged(int x, int y, int z){
        onObjectsChanged?.Invoke(x,y,z);
    }
    public void InvokeObjectChanged(Vector3Int coords){
        InvokeObjectChanged(coords.x, coords.y, coords.z);
    }
    public void DrawGrid(float duration = 60f){
        RunOnEach(DrawAxis);
        for (int x = 0; x <= width; x++)
        {
            Debug.DrawLine(GetWorldPosition(x,0,length),GetWorldPosition(x,height,length), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(x,height,0),GetWorldPosition(x,height,length), Color.white, duration);
        }
        for (int y = 0; y <= height; y++)
        {
            Debug.DrawLine(GetWorldPosition(0,y,length),GetWorldPosition(width,y,length), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(width,y,0),GetWorldPosition(width,y,length), Color.white, duration);
        }
        for (int z = 0; z <= length; z++)
        {
            Debug.DrawLine(GetWorldPosition(width,0,z),GetWorldPosition(width,height,z), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(0,height,z),GetWorldPosition(width,height,z), Color.white, duration);
        }

        void DrawAxis(Vector3Int pos){
            Debug.DrawLine(GetWorldPosition(pos.x,pos.y,pos.z),GetWorldPosition(pos.x+1,pos.y,pos.z), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(pos.x,pos.y,pos.z),GetWorldPosition(pos.x,pos.y+1,pos.z), Color.white, duration);
            Debug.DrawLine(GetWorldPosition(pos.x,pos.y,pos.z),GetWorldPosition(pos.x,pos.y,pos.z+1), Color.white, duration);
        }
    }
    #endregion
}
