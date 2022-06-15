using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LimitedInventory : Inventory
{
    public Item[] limitToItems;
    #region Constructors
    public LimitedInventory(int size) : base(size){
        limitToItems = new Item[maxSize];
    }
    public LimitedInventory(int size, Item[] limitToItems) : base(size){
        this.limitToItems = limitToItems;
    }
    public LimitedInventory(ItemStack[] defaultInv) : base(defaultInv){
        limitToItems = new Item[maxSize];
    }
    public LimitedInventory(ItemStack[] defaultInv, Item[] limitToItems) : base(defaultInv){
        this.limitToItems = limitToItems;
    }
    public LimitedInventory(int size, IList<ItemStack> defaultInv) : base(size, defaultInv){
        limitToItems = new Item[maxSize];
    }
    public LimitedInventory(int size, IList<ItemStack> defaultInv, Item[] limitToItems) : base(size, defaultInv){
        this.limitToItems = limitToItems;
    }
    #endregion
    public void SetLimit(Item limitToItem, int limitIndex){
        limitToItems[limitIndex] = limitToItem;
    }
    public bool CompatibleWithSlot(Item item, int slot){
        if (limitToItems[slot] == null)
            return true;
        if (item == limitToItems[slot])
            return true;
        return false;
    }
    #region Overrides
    ///<summary>
    ///Returns the first index of this inventory where the given item can be placed.
    ///</summary>
    public override int FindItemStackable(Item item, int startSearchIndex = 0)
    {
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (CompatibleWithSlot(item, i)){
                if (IsNull(i))
                    return i;
                if (!items[i].isFull)
                    return i;
            }
        }
        return -1;
    }
    ///<summary>
    ///Returns the first index of this inventory where the given item can be placed.
    ///</summary>
    public override int FindItemStackable(ItemStack itemStack, int startSearchIndex = 0)
    {
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (CompatibleWithSlot(itemStack.item, i)){
                if (IsNull(i))
                    return i;
                if (!items[i].isFull && itemStack.Compatible(items[i]))
                    return i;
            }
        }
        return -1;
    }
    ///<summary>
    ///Always returns -1.
    ///</summary>
    public override int FirstEmptyIndex(int startSearchIndex = 0)
    {
        return -1;
    }
    #endregion
}
