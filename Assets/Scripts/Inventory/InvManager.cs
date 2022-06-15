using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvManager : MonoBehaviour
{
    public LootTable table;
    public static InvManager instance;
    public static Inventory inventory;
    [SerializeField] InventoryDisplay inventoryDisplay;
    public static ItemStack heldItemStack{get; private set;}
    public LootItem removeItem;
    private void Awake() {
        instance = this;
        inventory = new Inventory(20);
        inventoryDisplay.Initialize(inventory);
        //inventory.onItemChangedCallback += OnItemChanged;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)){
            List<LootItem> loot = table.TakeFromTable();
            for (int i = 0; i < loot.Count; i++)
            {
                inventory.Add(loot[i].item, loot[i].amount);
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab)){
            inventoryDisplay.gameObject.SetActive(!inventoryDisplay.gameObject.activeSelf);
            inventoryDisplay.enabled = inventoryDisplay.gameObject.activeInHierarchy;
        }
        if (Input.GetKeyDown(KeyCode.Backspace)){
            inventory.Remove(removeItem.item, removeItem.amount);
        }
    }
    private void OnItemChanged(){
        Debug.Log("Inventory is "+Mathf.Round(inventory.PercentFilled()*100)+"% full!");
    }
    public static void SlotClicked(Inventory inv, int index){
        if (heldItemStack == null){
            //Pick up item
            if (inv.IsNull(index)) return;
            Hold(inv, index);
        } else {
            if (inv.IsNull(index) || inv.Get(index).Compatible(heldItemStack)){
                inv.AddAtIndex(heldItemStack, index);
                ClearHold();
            } else {
                ItemStack temp = heldItemStack;
                Hold(inv, index);
                inv.AddAtIndex(temp, index);
            }
        }
    }
    public static void Hold(Inventory inv, int index){
        heldItemStack = inv.Get(index);
        inv.Delete(index);
    }
    public static void ClearHold(){
        heldItemStack = null;
    }
}
