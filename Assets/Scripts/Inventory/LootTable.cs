using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class LootTable
{
    public LootChance[] lootChances;
    public List<LootItem> TakeFromTable(){
        List<LootItem> drops = new List<LootItem>();
        foreach (LootChance chance in lootChances)
        {
            if (chance.chance >= Random.value)
                drops.Add(chance.GetLoot());
        }
        return drops;
    }
}
[System.Serializable]
public class LootItem{
    public Item item;
    [Min(1)] public int amount;
    public LootItem(Item item, int amount){
        this.item = item;
        this.amount = amount;
    }
    public static explicit operator ItemStack(LootItem loot){
        return loot.item.NewStack(loot.amount);
    }
    public override string ToString()
    {
        return item+"*"+amount;
    }
}
[System.Serializable]
public class LootChance{
    public Item item;
    [Min(1)] public int amount;
    [Tooltip("Leave as 0 to not randomize amount at all")] public int maxAmount;
    [Range(0, 1)] public float chance = 1;
    public LootChance(Item item, int amount, float chance, int maxAmount = 0){
        this.item = item;
        this.amount = amount;
        this.chance = chance;
        this.maxAmount = maxAmount;
    }
    public LootItem GetLoot(){
        LootItem loot = new LootItem(item, amount);
        if (maxAmount > amount)
            loot.amount = Random.Range(amount, maxAmount+1);
        return loot;
    }
}
