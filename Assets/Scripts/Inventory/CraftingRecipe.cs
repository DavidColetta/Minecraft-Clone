using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Inv/Recipies/Recipe", order = 10)]
public class CraftingRecipe : ScriptableObject
{
    [SerializeField] protected LootItem[] _materials;
    [SerializeField] protected LootItem _output;
    public LootItem[] materials{get{return _materials;}}
    public LootItem output{get{return _output;}}
    public bool CanCraft(Inventory matInv){
        foreach (LootItem material in _materials)
        {
            if (!matInv.Contains(material.item, material.amount))
                return false;
        }
        return true;
    }
    public bool ConsumeMaterials(Inventory matInv){
        if (!CanCraft(matInv)) return false;
        foreach (LootItem material in materials)
        {
            matInv.RemoveOverflow(material.item, material.amount);
        }
        return false;
    }
}
[CreateAssetMenu(fileName = "Recipe", menuName = "Inv/Recipies/MultiRecipe", order = 11)]
public class MultiOutputRecipe : CraftingRecipe{
    [SerializeField] private LootTable byproducts;
    public Item[] GetPossibleByproducts(){
        Item[] possibleByproducts = new Item[byproducts.lootChances.Length];
        for (int i = 0; i < byproducts.lootChances.Length; i++)
        {
            possibleByproducts[i] = byproducts.lootChances[i].item;
        }
        return possibleByproducts;
    }
    public List<LootItem> GetByproducts(){
        return byproducts.TakeFromTable();
    }
}
