using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter {

    [SerializeField] private KitchenObjectsSO kitchenObjectSO;

    public override void Interact (Player player) {
        if (!HasKitchenObject()) {
            // There is no KitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                // Player not carrying anything
            }
        } else {
            // There is kitchenObject
            if (player.HasKitchenObject()) {
                // Player is carrying something
            } else {
                GetKitchenObject().SetKitchenObjectParent(player);
                // Player is not carrying anything
            }
        }
    }



}
