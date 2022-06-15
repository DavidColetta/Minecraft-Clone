using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Inv/Items/Item", order = 0)]
public class Item : ScriptableObject
{
    [SerializeField] protected string _name = "New Item";
    [SerializeField] protected Sprite _sprite;
    [SerializeField, TextArea()] protected string _description;
    [SerializeField, Min(1)] protected int _maxStackCount = 100;
    [SerializeField] protected Rarity _rarity;
    [SerializeField] protected bool _isMaterial;
    #region Getters
    public new string name{get{return _name;}}
    public Sprite sprite{get{return _sprite;}}
    public string description{get{return _description;}}
    public int maxStackCount{get{return _maxStackCount;}}
    public Rarity rarity{get{return _rarity;}}
    public bool isMaterial{get{return _isMaterial;}}
    #endregion
    ///<summary>
    ///Creates a new stack of items. Count is clamped.
    ///</summary>
    public virtual ItemStack NewStack(int count){
        return new ItemStack(this, Mathf.Clamp(count, 1, maxStackCount));
    }
    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary
    }
}
