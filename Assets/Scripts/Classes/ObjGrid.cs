using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class ObjGrid<T>
{
    #region Variables
    public delegate void OnObjectsChanged(int x, int y, int maxX, int maxY);
    public event OnObjectsChanged onObjectsChanged;
    public readonly int width;
    public readonly int height;
    public readonly float cellSize;
    public readonly Vector2 originPos;
    protected readonly T[,] gridArray;
    #endregion
    #region Constructors
    public ObjGrid(int width, int height, Vector2 originPos, float cellSize = 1)
        : this(new T[width,height],originPos,cellSize){}
    public ObjGrid(T[,] defaultArray, Vector2 originPos, float cellSize = 1){
        this.cellSize = cellSize;
        this.originPos = originPos;
        gridArray = defaultArray;
        width = defaultArray.GetLength(0);
        height = defaultArray.GetLength(1);
    }
    #endregion
    #region Conversions
    public Vector2 GetWorldPosition(int x, int y){
        return new Vector2(x, y) * cellSize + originPos;
    }
    public Vector2 GetWorldPosition(Vector2Int coords){
        return (Vector2)(coords) * cellSize + originPos;
    }
    public Vector2Int GetCoords(Vector2 worldPosition){
        worldPosition -= originPos;
        return new Vector2Int(Mathf.FloorToInt(worldPosition.x/cellSize), Mathf.FloorToInt(worldPosition.y/cellSize));
    }
    public bool IsValid(int x, int y){
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    public bool IsValid(Vector2Int coords){
        return IsValid(coords.x, coords.y);
    }
    public Vector2 SnapWorldPosToGrid(Vector2 worldPosition){
        return GetWorldPosition(GetCoords(worldPosition));
    }
    #endregion
    #region Get/Set
    public T Get(int x, int y){
        if (!IsValid(x,y)) return default(T);
        return gridArray[x,y];
    }
    public T Get(Vector2Int coords){
        return Get(coords.x, coords.y);
    }
    public T Get(Vector2 worldPosition){
        return Get(GetCoords(worldPosition));
    }
    public void Set(int x, int y, T value){
        if (!IsValid(x, y)) return;
        gridArray[x,y] = value;
        InvokeObjectChanged(x,y);
    }
    public void Set(Vector2Int coords, T value){
        Set(coords.x, coords.y, value);
    }
    public void Set(Vector2 worldPosition, T value){
        Set(GetCoords(worldPosition), value);
    }
    #endregion
    #region Lambda Expressions
    public void RunOnEach(Action<T> method){
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                method(gridArray[x,y]);
            }
        }
    }
    public void RunOnEach(Action<T,Vector2Int> method){
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                method(gridArray[x,y], new Vector2Int(x,y));
            }
        }
    }
    public void RunOnEach(Action<Vector2Int> method){
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                method(new Vector2Int(x,y));
            }
        }
    }
    public void SetEach(Func<T> method){
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x,y] = method();
            }
        }
    }
    public void SetEach(Func<Vector2Int,T> method){
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                gridArray[x,y] = method(new Vector2Int(x,y));
            }
        }
    }
    public T FindFirst(Func<T,bool> method){
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                if (method(gridArray[x,y])) return gridArray[x,y];
            }
        }
        return default(T);
    }
    public Vector2Int FindCoords(Func<T,bool> method){
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                if (method(gridArray[x,y])) return new Vector2Int(x,y);
            }
        }
        return new Vector2Int(-1,-1);
    }
    public bool Contains(Func<T,bool> method){
        for (int x = 0; x < gridArray.GetLength(0); x++) {
            for (int y = 0; y < gridArray.GetLength(1); y++) {
                if (method(gridArray[x,y])) return true;
            }
        }
        return false;
    }
    #endregion
    #region Misc
    public void InvokeObjectChanged(int x, int y){
        onObjectsChanged?.Invoke(x,y,x,y);
    }
    public void InvokeObjectChanged(Vector2Int coords){
        InvokeObjectChanged(coords.x, coords.y);
    }
    public void InvokeObjectsChanged(int x, int y, int maxX, int maxY){
        onObjectsChanged?.Invoke(x,y,maxX,maxY);
    }
    public void InvokeObjectsChanged(Vector2Int coords, Vector2Int maxCoords){
        InvokeObjectsChanged(coords.x, coords.y, maxCoords.x, maxCoords.y);
    }
    public void DrawGrid(){
        RunOnEach(DrawLine);
        Debug.DrawLine(GetWorldPosition(0,height),GetWorldPosition(width,height), Color.white, 60);
        Debug.DrawLine(GetWorldPosition(width,0),GetWorldPosition(width,height), Color.white, 60);

        void DrawLine(Vector2Int pos){
            int x = pos.x;
            int y = pos.y;
            Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x,y+1), Color.white, 60);
            Debug.DrawLine(GetWorldPosition(x,y),GetWorldPosition(x+1,y), Color.white, 60);
        }
    }
    #endregion
}
