using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public LootItem loot;
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player"){
            //Debug.Log("Remainder: "+ 
            InvManager.inventory.Add(loot.item, loot.amount);
        }
    }
}
