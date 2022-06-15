using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InvSlotDisplay : MonoBehaviour
{
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI stackCountDisplay;
    [SerializeField] private Button button;
    public InventoryDisplay parent{get; private set;}
    public int slotID{get; private set;}
    public void SetSlotIDTo(int i, InventoryDisplay parent){
        slotID = i;
        this.parent = parent;
        if (parent.isActiveAndEnabled) OnItemChanged();
    }
    public void OnItemChanged(){
        ItemStack itemStack = parent.inventory.Get(slotID);
        if (itemStack == null){
            image.sprite = emptySprite;
            stackCountDisplay.text = "";
        } else {
            image.sprite = itemStack.item.sprite;
            if (itemStack.count == 1)
                stackCountDisplay.text = "";
            else
                stackCountDisplay.text = itemStack.count.ToString();
        }
    }
    public void Clicked(){
        InvManager.SlotClicked(parent.inventory, slotID);
    }
}
