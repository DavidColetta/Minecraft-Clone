using System;
using System.Collections.Generic;
public class Inventory
{
    #region Variables
    protected readonly ItemStack[] items;
    public delegate void OnItemChanged();
    ///<summary>
    ///Invoked when this inventory has been changed.
    ///</summary>
    public event OnItemChanged onItemChangedCallback;
    ///<summary>
    ///The number of slots in the inventory.
    ///</summary>
    public readonly int maxSize;
    ///<summary>
    ///All slots at this index and beyond are empty.
    ///</summary>
    protected int currentSize;
    ///<summary>
    ///Returns true if there are no items in the inventory.
    ///</summary>
    public bool isEmpty{get{return currentSize == 0;}}
    #endregion
    #region Constructors
    ///<summary>
    ///A class which stores InvItems.
    ///</summary>
    public Inventory(int size){
        maxSize = size;
        items = new ItemStack[maxSize];
        currentSize = 0;
    }
    ///<summary>
    ///A class which stores InvItems. Initialized to given array.
    ///</summary>
    public Inventory(ItemStack[] defaultInv){
        maxSize = defaultInv.Length;
        items = defaultInv;
        DecreaseCurrentSize(maxSize);
    }
    ///<summary>
    ///A class which stores InvItems. Initialized to a copy of given array. Items will not be copies.
    ///</summary>
    public Inventory(int size, IList<ItemStack> defaultInv) : this(size){
        int min = UnityEngine.Mathf.Min(size, defaultInv.Count);
        for (int i = 0; i < min; i++){
            if (defaultInv[i] != null)
                items[i] = defaultInv[i];
        }
        DecreaseCurrentSize(min);
    }
    public Inventory(int size, IList<LootItem> defaultInv) : this(size){
        foreach (LootItem loot in defaultInv)
        {
            Add(loot.item, loot.amount);
        }
    }
    #endregion
    #region Add
    ///<summary>
    ///Adds an ItemStack to the inventory. Returns the number of items left over.
    ///Will return 0 if all items successfully added.
    ///</summary>
    public int Add(Item item, int amount){
        int index = FindItemStackable(item);
        if (index != -1) 
            return PAddAtIndex(item, amount, index);
        return PAddAtIndex(item, amount, FirstEmptyIndex());
    }
    ///<summary>
    ///Adds an ItemStack to the inventory. Returns the number of items left over.
    ///Will return 0 if all items successfully added.
    ///Can fill slots other than the given index if amount added is more than one stack.
    ///</summary>
    public int AddAtIndex(Item item, int amount, int index){
        if (!IsValidIndex(index)) {
            return amount;
        }
        if (IsNull(index)){
            if (amount <= item.maxStackCount) {
                SetItem(item, amount, index);
                InvokeOnItemChanged();
                return 0;
            } else {
                SetItem(item, amount, index);
                items[index].Fill();
                return Add(item, amount - item.maxStackCount);
            }
        } else if (item == items[index].item){
            if (items[index].Add(amount)){
                //If same type and stack not too large, increase stack
                InvokeOnItemChanged();
                return 0;
            } else {
                int slotsRemaining = items[index].spaceRemaining;
                items[index].Fill();
                return Add(item, amount - slotsRemaining);
            }
        } else {
            return Add(item, amount);
        }
    }
    private int PAddAtIndex(Item item, int amount, int index){
        if (!IsValidIndex(index)) {
            InvokeOnItemChanged();
            return amount;
        }
        if (IsNull(index)){
            if (amount <= item.maxStackCount) {
                SetItem(item, amount, index);
                InvokeOnItemChanged();
                return 0;
            } else {
                SetItem(item, amount, index);
                items[index].Fill();
                return PAddAtIndex(item, amount - item.maxStackCount, FirstEmptyIndex(index+1));
            }
        } else {
            if (items[index].Add(amount)){
                //If same type and stack not too large, increase stack
                InvokeOnItemChanged();
                return 0;
            } else {
                int slotsRemaining = items[index].spaceRemaining;
                items[index].Fill();
                int stackableIndex = FindItemStackable(item, index+1);
                if (stackableIndex != -1)
                    return PAddAtIndex(item, amount - slotsRemaining, stackableIndex);
                return PAddAtIndex(item, amount - slotsRemaining, FirstEmptyIndex());
            }
        }
    }
    ///<summary>
    ///Adds an ItemStack to the inventory. Returns the number of items left over.
    ///Will return 0 if all items successfully added. (Copy Recommended)
    ///</summary>
    public int Add(ItemStack itemStack){
        //Debug.Log("Adding "+itemStack+" at "+index);
        int index = FindItemStackable(itemStack);
        if (index != -1) 
            return PAddAtIndex(itemStack, index);
        return PAddAtIndex(itemStack, FirstEmptyIndex());
    }
    ///<summary>
    ///Adds an ItemStack to the inventory. Returns the number of items left over.
    ///Will return 0 if all items successfully added. Can fill slots other
    ///than the given index if amount added is more than one stack. (Copy Recommended)
    ///</summary>
    public int AddAtIndex(ItemStack itemStack, int index){
        if (!IsValidIndex(index)) {
            InvokeOnItemChanged();
            return itemStack.count;
        }
        if (IsNull(index)){
            SetItem(itemStack, index);
            InvokeOnItemChanged();
            return 0;
        } else if (itemStack.Compatible(items[index])){
            if (items[index].Add(itemStack.count)){
                //If same type and stack not too large, increase stack
                InvokeOnItemChanged();
                return 0;
            } else {
                itemStack.Add(-items[index].spaceRemaining);
                items[index].Fill();
                return Add(itemStack);
            }
        } else {
            return Add(itemStack);
        }
    }
    private int PAddAtIndex(ItemStack itemStack, int index){
        if (!IsValidIndex(index)) {
            InvokeOnItemChanged();
            return itemStack.count;
        }
        if (IsNull(index)){
            SetItem(itemStack, index);
            InvokeOnItemChanged();
            return 0;
        } else {
            if (items[index].Add(itemStack.count)){
                //If same type and stack not too large, increase stack
                InvokeOnItemChanged();
                return 0;
            } else {
                itemStack.Add(-items[index].spaceRemaining);
                items[index].Fill();
                int stackableIndex = FindItemStackable(itemStack, index + 1);
                if (stackableIndex != -1) 
                    return PAddAtIndex(itemStack, stackableIndex);
                return PAddAtIndex(itemStack, FirstEmptyIndex(index + 1));
            }
        }
    }
    private void SetItem(Item item, int amount, int index){
        items[index] = item.NewStack(amount);
        if (currentSize <= index) currentSize = index + 1;
        //Remember to invoke OnItemChangedCallback
    }
    private void SetItem(ItemStack itemStack, int index){
        items[index] = itemStack;
        if (currentSize <= index) currentSize = index + 1;
        //Remember to invoke OnItemChangedCallback
    }
    #endregion
    #region Remove
    ///<summary>
    ///Removes an amount of items from the inventory. Returns true if successful.
    ///If returns false, no items were removed.
    ///</summary>
    public bool Remove(Item item, int amount){
        return RemoveAtIndex(FindItemReversed(item), amount);
    }
    ///<summary>
    ///Removes an amount of items from the given index. Returns true if successful.
    ///If returns false, no items were removed. Can remove items
    ///outside the given index if amount to remove is greater than the amount at the index.
    ///</summary>
    public bool RemoveAtIndex(int index, int amount){
        if (!IsValidIndex(index) || IsNull(index)) return false;
        if (PRemoveAtIndex(index, amount, index - 1, index + 1)) {
            InvokeOnItemChanged();
            return true;
        } else 
            return false;
    }
    private bool PRemoveAtIndex(int index, int amount, int minSearch, int maxSearch){
        //onItemChangedCallback is called in the RemoveAtIndex function.
        if (items[index].Add(-amount)){
            return true;
        } else if (items[index].count == amount){
            DeleteItem(index);
            return true;
        } else {
            int foundItem = FindItemReversed(items[index].item, minSearch);
            if (foundItem != -1) minSearch = foundItem - 1;
            else {
                foundItem = FindItem(items[index].item, maxSearch);
                if (foundItem != -1) maxSearch = foundItem + 1;
                else return false;
            }
            //If item found other than one at index
            if (PRemoveAtIndex(foundItem, amount - items[index].count, minSearch, maxSearch)){
                DeleteItem(index);
                return true;
            } else
                return false;
        }
    }
    ///<summary>
    ///Removes an amount of items from the inventory. If amount to remove
    ///is more than availible amount, it will still remove all that it can.
    ///</summary>
    public void RemoveOverflow(Item item, int amount){
        PRemoveAtIndexOverflow(FindItemReversed(item), amount);
    }
    private void PRemoveOverflow(Item item, int amount, int startSearchIndex){
        PRemoveAtIndexOverflow(FindItemReversed(item, startSearchIndex), amount);
    }
    ///<summary>
    ///Removes an amount of items from the given index. If amount to remove
    ///is more than availible amount, it will still remove all that it can.
    ///Can remove items outside given index if amount to remove is greater
    ///than the amount of items at the index.
    ///</summary>
    public void RemoveAtIndexOverflow(int index, int amount){
        if (!IsValidIndex(index) || IsNull(index)) {
            return;
        }
        if (!items[index].Add(-amount)){
            Item item = items[index].item;
            int newAmount = amount - items[index].count;
            DeleteItem(index);
            if (newAmount > 0){
                RemoveOverflow(item, newAmount);
                return;
            }
        }
        InvokeOnItemChanged();
    }
    private void PRemoveAtIndexOverflow(int index, int amount){
        if (!IsValidIndex(index)) {
            InvokeOnItemChanged();
            return;
        }
        if (!items[index].Add(-amount)){
            Item item = items[index].item;
            int newAmount = amount - items[index].count;
            DeleteItem(index);
            if (newAmount > 0){
                PRemoveOverflow(item, newAmount, index - 1);
                return;
            }
        }
        InvokeOnItemChanged();
    }
    private void DeleteItem(int index){
        items[index] = null;
        if (currentSize == index + 1) {
            DecreaseCurrentSize(currentSize);
        }
        //Remember to invoke OnItemChangedCallback
    }
    #endregion
    #region Move
    ///<summary>
    ///Swaps the ItemStack at fromIndex from this inventory with the ItemStack at toIndex in the toInv inventory.
    ///</summary>
    public void SwapIndexes(int fromIndex, int toIndex, Inventory toInv){
        ItemStack temp = toInv.Get(toIndex);
        if (IsNull(fromIndex))
            toInv.Delete(toIndex);
        else
            toInv.Set(items[fromIndex], toIndex);
        if (temp == null)
            Delete(fromIndex);
        else
            Set(temp, fromIndex);
    }
    ///<summary>
    ///Swaps the ItemStack at fromIndex with the ItemStack at toIndex.
    ///</summary>
    public void SwapIndexes(int fromIndex, int toIndex){
        ItemStack temp = items[toIndex];
        if (IsNull(fromIndex))
            DeleteItem(toIndex);
        else
            SetItem(items[fromIndex], toIndex);
        if (temp == null)
            Delete(fromIndex);
        else
            Set(temp, fromIndex);
    }
    ///<summary>
    ///Moves the ItemStack at fromIndex from this inventory to toIndex in the toInv inventory.
    ///Will only move partially if toIndex already has items in it. Will swap if
    ///stacks are not compatible.
    ///</summary>
    public void MoveFromIndexToIndex(int fromIndex, int toIndex, Inventory toInv){
        MoveAmountFromIndexToIndex(fromIndex, items[fromIndex].count, toIndex, toInv);
    }
    ///<summary>
    ///Moves the ItemStack at fromIndex from this inventory to toIndex in the toInv inventory.
    ///Will only move partially if toIndex already has items in it.
    ///Returns false if amount not found. Will swap if stacks are not compatible.
    ///</summary>
    public bool MoveAmountFromIndexToIndex(int fromIndex, int amount, int toIndex, Inventory toInv){
        if (amount > items[fromIndex].count) return false;
        if (toInv.IsNull(toIndex)){
            ItemStack copy = items[fromIndex].Copy();
            copy.SetCount(amount);
            toInv.AddAtIndex(copy, toIndex);
            RemoveAtIndexOverflow(fromIndex, amount);
            return true;
        }
        if (toInv.Get(toIndex).Compatible(items[fromIndex])){
            RemoveAtIndexOverflow(fromIndex, toInv.Get(toIndex).spaceRemaining);
            toInv.Get(toIndex).Fill();
            toInv.InvokeOnItemChanged();
            return true;
        }
        SwapIndexes(fromIndex, toIndex, toInv);
        return true;
    }
    ///<summary>
    ///Moves the ItemStack at fromIndex from this inventory to the toInv inventory.
    ///Will only move partially if toIndex already has items in it.
    ///Returns remainder.
    ///</summary>
    public int MoveAmountFromIndex(int fromIndex, int amount, Inventory toInv){
        ItemStack copy = items[fromIndex].Copy();
        if (!copy.SetCount(amount)) return amount;
        int remainder = toInv.Add(copy);
        RemoveAtIndexOverflow(fromIndex, amount - remainder);
        return remainder;
    }
    ///<summary>
    ///Moves the ItemStack at fromIndex from this inventory to the toInv inventory.
    ///Will only move partially if toIndex already has items in it. Returns remainder.
    ///</summary>
    public int MoveFromIndex(int fromIndex, Inventory toInv){
        int remainder = toInv.Add(items[fromIndex].Copy());
        RemoveAtIndexOverflow(fromIndex, items[fromIndex].count - remainder);
        return remainder;
    }
    ///<summary>
    ///Moves an amount of items from this inventory to the toInv.
    ///Returns remainder.
    ///</summary>
    public int MoveAmount(Item item, int amount, Inventory toInv){
        return PMoveAmount(item, amount, toInv, currentSize - 1);
    }
    private int PMoveAmount(Item item, int amount, Inventory toInv, int startSearchIndex){
        int found = FindItemReversed(item, startSearchIndex);
        if (found == -1) return amount;
        if (amount <= item.maxStackCount)
            return MoveAmountFromIndex(found, amount, toInv);
        amount -= items[found].count;
        int remainder = MoveFromIndex(found, toInv);
        if (remainder > 0)
            return remainder;
        return PMoveAmount(item, amount, toInv, startSearchIndex - 1);
    }
    ///<summary>
    ///Moves an all items of given type from this inventory to the toInv.
    ///</summary>
    public void MoveAll(Item item, Inventory toInv){
        PMoveAll(item, toInv, currentSize - 1);
    }
    private void PMoveAll(Item item, Inventory toInv, int startSearchIndex){
        int found = FindItemReversed(item, startSearchIndex);
        if (found == -1) return;
        if (MoveFromIndex(found, toInv) > 0)
            return;
        PMoveAll(item, toInv, startSearchIndex - 1);
    }
    #endregion
    #region Search
    public int Find(Func<ItemStack,bool> func, int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (!IsNull(i))
                if (func(items[i]))
                    return i;
        }
        return -1;
    }
    ///<summary>
    ///Returns the first index of this item in the inventory, starting at startSearchIndex.
    ///Returns -1 if item not found.
    ///</summary>
    public int FindItem(Item item, int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (GetItem(i) == item)
                return i;
        }
        return -1;
    }
    ///<summary>
    ///Returns the last index of this item in the inventory.
    ///</summary>
    public int FindItemReversed(Item item){
        return FindItemReversed(item, currentSize-1);
    }
    ///<summary>
    ///Returns the last index of this item in the inventory, starting backwards from startSearchIndex.
    ///</summary>
    public int FindItemReversed(Item item, int startSearchIndex){
        if (startSearchIndex >= currentSize) startSearchIndex = currentSize - 1;
        for (int i = startSearchIndex; i >= 0; i--)
        {
            if (GetItem(i) == item)
                return i;
        }
        return -1;
    }
    ///<summary>
    ///Returns the first empty index in the inventory, starting from startSearchIndex.
    ///</summary>
    public virtual int FirstEmptyIndex(int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = startSearchIndex; i < maxSize; i++)
        {
            if (IsNull(i)) return i;
        }
        return -1;
    }
    ///<summary>
    ///Returns the number of empty indexes in the inventory.
    ///</summary>
    public int CountEmptyIndexes(int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        int count = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (IsNull(i)) count++;
        }
        return count + (maxSize - currentSize);
    }
    ///<summary>
    ///Returns the number of this item in the inventory.
    ///Counts each item in the stack.
    ///</summary>
    public int CountItems(Item item, int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        int count = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (GetItem(i) == item)
                count += items[i].count;
        }
        return count;
    }
    ///<summary>
    ///Returns the number of items in the inventory, starting from startSearchIndex.
    ///Counts each item in the stack.
    ///</summary>
    public int CountAll(int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        int count = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (!IsNull(i)){
                count += items[i].count;
            }
        }
        return count;
    }
    ///<summary>
    ///Returns true if the inventory contains at least the given amount of items.
    ///</summary>
    public bool Contains(Item item, int amount, int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        int count = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (GetItem(i) == item){
                count += items[i].count;
                if (count >= amount) return true;
            }
        }
        return false;
    }
    ///<summary>
    ///Returns true if the inventory contains at least the given amount of compatible items.
    ///</summary>
    public bool Contains(ItemStack itemStack, int startSearchIndex = 0){
        return Contains(itemStack, itemStack.count, startSearchIndex);
    }
    ///<summary>
    ///Returns true if the inventory contains at least the given amount of compatible items.
    ///</summary>
    public bool Contains(ItemStack itemStack, int amount, int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        int count = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (!IsNull(i) && items[i].Compatible(itemStack)){
                count += items[i].count;
                if (count >= amount) return true;
            }
        }
        return false;
    }
    ///<summary>
    ///Returns the first index of this item in the inventory whose stack is not full, starting at startSearchIndex.
    ///</summary>
    public virtual int FindItemStackable(Item item, int startSearchIndex = 0){
        if (item.maxStackCount == 1) return -1;
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (!IsNull(i)){
                if (!items[i].isFull && items[i].item == item){
                    return i;
                }
            }
        }
        return -1;
    }
    ///<summary>
    ///Returns the first index of this item in the inventory who is compatible and also not full.
    ///</summary>
    public virtual int FindItemStackable(ItemStack itemStack, int startSearchIndex = 0){
        if (itemStack.item.maxStackCount == 1) return -1;
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (!IsNull(i)){
                if (!items[i].isFull && items[i].Compatible(itemStack)){
                    return i;
                }
            }
        }
        return -1;
    }
    ///<summary>
    ///Returns true if all slots in the inventory have an item. 
    ///It is possible that not all stacks are full.
    ///</summary>
    public bool IsFull(int startSearchIndex = 0){
        if (currentSize != maxSize) return false;
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = maxSize - 1; i >= startSearchIndex; i--)
        {
            if (IsNull(i)) return false;
        }
        return true;
    }
    ///<summary>
    ///Returns true if all slots in the inventory are full. 
    ///All stacks must also be full.
    ///</summary>
    public bool IsTruelyFull(int startSearchIndex = 0){
        if (currentSize != maxSize) return false;
        if (startSearchIndex < 0) startSearchIndex = 0;
        for (int i = maxSize - 1; i >= startSearchIndex; i--)
        {
            if (IsNull(i) || !items[i].isFull) return false;
        }
        return true;
    }
    ///<summary>
    ///Returns the number of slots that have items in them.
    ///Ignores stack size.
    ///</summary>
    public int SlotsFilled(int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        int count = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (!IsNull(i)) count++;
        }
        return count;
    }
    ///<summary>
    ///Returns the percentage of slots that have items in them.
    ///Ignores stack size.
    ///</summary>
    public float PercentSlotsFilled(int startSearchIndex = 0){
        return (float)(SlotsFilled(startSearchIndex)) / maxSize;
    }
    ///<summary>
    ///Returns the percent that the inventory has been filled to.
    ///Accounts for stack size.
    ///</summary>
    public float PercentFilled(int startSearchIndex = 0){
        if (startSearchIndex < 0) startSearchIndex = 0;
        float count = 0;
        for (int i = startSearchIndex; i < currentSize; i++)
        {
            if (!IsNull(i)) count += items[i].fillPercent;
        }
        return count / maxSize;
    }
    #endregion
    #region Set/Get
    ///<summary>
    ///Returns the ItemStack at the given index.
    ///</summary>
    public ItemStack Get(int index){
        return items[index];
    }
    ///<summary>
    ///Returns the item at the given index.
    ///</summary>
    public Item GetItem(int index){
        return items[index]?.item;
    }
    ///<summary>
    ///Sets the index ti ItemStack. (Copy Recommended)
    ///</summary>
    public void Set(ItemStack itemStack, int index){
        SetItem(itemStack, index);
        InvokeOnItemChanged();
    }
    ///<summary>
    ///Sets the index to an itemStack of stack size amount.
    ///</summary>
    public void Set(Item item, int amount, int index){
        SetItem(item, amount, index);
        InvokeOnItemChanged();
    }
    ///<summary>
    ///Sets the index to null.
    ///</summary>
    public void Delete(int index){
        DeleteItem(index);
        InvokeOnItemChanged();
    }
    ///<summary>
    ///Returns whether the given index is within the bounds of the inventory.
    ///</summary>
    public bool IsValidIndex(int index){
        return (index >= 0 && index < maxSize);
    }
    ///<summary>
    ///Returns true if the ItemStack at the given index is null.
    ///</summary>
    public bool IsNull(int index){
        return items[index] == null;
    }
    ///<summary>
    ///Returns a copy of all the items in the inventory. Includes null items.
    ///</summary>
    public ItemStack[] Copy(){
        ItemStack[] ans = new ItemStack[maxSize];
        for (int i = 0; i < currentSize; i++)
        {
            if (!IsNull(i))
                ans[i] = items[i].Copy();
        }
        return ans;
    }
    ///<summary>
    ///Returns a copy of all the items in the inventory up until the current size. Includes null items.
    ///</summary>
    public ItemStack[] CopyCurrentSize(){
        ItemStack[] ans = new ItemStack[currentSize];
        for (int i = 0; i < currentSize; i++)
        {
            if (!IsNull(i))
                ans[i] = items[i].Copy();
        }
        return ans;
    }
    ///<summary>
    ///Returns a list copy of all the items in the inventory. Excludes null items.
    ///</summary>
    public List<ItemStack> CopyList(int startIndex = 0){
        List<ItemStack> ans = new List<ItemStack>();
        if (startIndex < 0) startIndex = 0;
        for (int i = startIndex; i < currentSize; i++)
        {
            if (!IsNull(i))
                ans.Add(items[i].Copy());
        }
        return ans;
    }
    private void DecreaseCurrentSize(int pastSize){
        for (int i = pastSize - 1; i >= 0; i--){
            if (!IsNull(i)){
                currentSize = i + 1;
                return;
            }
        }
        currentSize = 0;
    }
    ///<summary>
    ///Tells the inventory an item has been updated.
    ///</summary>
    public void InvokeOnItemChanged(){
        onItemChangedCallback?.Invoke();
    }
    public override string ToString()
    {
        string ans = "";
        int i;
        for (i = 0; i < currentSize && i < maxSize - 1; i++)
        {
            if (!IsNull(i)){
                ans += items[i]+", ";
            } else {
                ans += "null, ";
            }
        }
        for (int j = i; j < maxSize - 1; j++)
        {
            ans += "null, ";
        }
        if (items[maxSize - 1] != null){
            ans += items[maxSize - 1]+";";
        } else {
            ans += "null;";
        }
        return ans;
    }
    #endregion
}
