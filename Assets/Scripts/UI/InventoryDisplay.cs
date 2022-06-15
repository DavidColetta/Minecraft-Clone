using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public Inventory inventory{get; private set;}
    private InvSlotDisplay[] slots;
    public void Initialize(Inventory inv) {
        OnDisable();
        inventory = inv;
        slots = transform.GetComponentsInChildren<InvSlotDisplay>();
        if (slots.Length < inventory.maxSize) {Debug.LogError("Not Enough Slots!"); return;}
        if (slots.Length > inventory.maxSize){
            Debug.LogWarning("Too many slots! Resizing from "+slots.Length+" to "+inventory.maxSize);
            for (int i = inventory.maxSize; i < slots.Length; i++)
            {
                slots[i].gameObject.SetActive(false);
            }
            System.Array.Resize<InvSlotDisplay>(ref slots, inventory.maxSize);
        }
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSlotIDTo(i, this);
        }
        if (isActiveAndEnabled) OnEnable();
    }
    private void UpdateUI(){
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].OnItemChanged();
        }
    }
    public void OnEnable() {
        if (inventory != null) {
            inventory.onItemChangedCallback += UpdateUI;
            UpdateUI();
        }
    }
    public void OnDisable() {
        if (inventory != null){
            inventory.onItemChangedCallback -= UpdateUI;
        }
    }
}
