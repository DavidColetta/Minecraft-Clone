using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ItemStack
{
    ///<summary>
    ///Returns the item type of this ItemStack.
    ///</summary>
    public Item item{get; protected set;}
    ///<summary>
    ///The current amount in the stack. (ReadOnly)
    ///</summary>
    public int count{get; protected set;}
    ///<summary>
    ///Returns how many more items can be added to this stack before it is full.
    ///</summary>
    public int spaceRemaining{get{return item.maxStackCount - count;}}
    ///<summary>
    ///Returns the percent that the stack is filled to.
    ///</summary>
    public float fillPercent{get{return (float)(count) / item.maxStackCount;}}
    ///<summary>
    ///Returns true if this stack is full.
    ///</summary>
    public bool isFull{get{return count == item.maxStackCount;}}
    ///<summary>
    ///This distinquishes uncompatible ItemStacks of the same item type.
    ///</summary>
    public float ID{get; protected set;}
    ///<summary>
    ///A stack of items. WARNING: StackCount SHOULD be clamped between 1 and maxStackCount.
    ///WARNING: Use Item.NewStack() instead!
    ///</summary>
    public ItemStack(Item item, int stackCount){
        this.item = item;
        count = stackCount;
    }
    ///<summary>
    ///Adds or removes an amount.
    ///Retuns false if cannot add/remove this amount.
    ///</summary>
    public bool Add(int amount){
        return SetCount(count + amount);
    }
    ///<summary>
    ///Combines 2 stacks of the same item.
    ///Retuns false if cannot add these stacks.
    ///</summary>
    public bool Add(ItemStack itemStack){
        if (Compatible(itemStack)) 
            return SetCount(count + itemStack.count);
        return false;
    }
    ///<summary>
    ///Sets the amount in the stack.
    ///Retuns false if new amount is not valid.
    ///</summary>
    public bool SetCount(int newStackCount){
        if (newStackCount <= item.maxStackCount && newStackCount > 0){
            count = newStackCount;
            return true;
        }
        return false;
    }
    ///<summary>
    ///Sets the stack count to the max.
    ///</summary>
    public void Fill(){
        count = item.maxStackCount;
    }
    ///<summary>
    ///Returns true if the itemStacks are compatible for stacking. Ignores the stack counts.
    ///Does NOT guarentee the stacks can be added.
    ///</summary>
    public bool Compatible(ItemStack itemStack){
        if (item == itemStack.item){
            return ID == itemStack.ID;
        }
        return false;
    }
    ///<summary>
    ///Returns a copy of this ItemStack.
    ///</summary>
    public virtual ItemStack Copy(){
        return new ItemStack(item, count);
    }
    public override string ToString()
    {
        return count+"*"+item.name;
    }
}
