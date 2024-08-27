using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter {

    public static DeliveryCounter Instance { get; private set; }

    public event EventHandler<OnTeaDialogueArgs> OnTeaDialogue;

    private KitchenObjectsSO tempList;
    
    public class OnTeaDialogueArgs : EventArgs {
        public bool isNormalTea;
    }
    private bool isNormalTea = false;
    private bool isCustomerHere = false;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        MinigameManager.Instance.OnCustomerSpawn += MinigameManager_OnCustomerSpawn;
    }

    private void MinigameManager_OnCustomerSpawn (object sender, EventArgs e) {
        isCustomerHere = true;
    }

    public override void Interact (Player player) {
        if (player.HasKitchenObject()){ // Player has KitchenObject
            MinigameManager.Instance.DeliveryRecipe(player.GetKitchenObject().GetKitchenObjectsSO());
            player.GetKitchenObject().DestroySelf();
        } else {

            tempList = MinigameManager.Instance.GetWaitingKitchenObject();

            if (tempList != null && isCustomerHere) { // Belum bisa
                // Customer Exist
                if (tempList.name == "NormalTea") {
                    isNormalTea = true;
                } else {
                    isNormalTea = false;
                }
                GameInput.Instance.DisablePlayerMovementAndInteract();
                GameInput.Instance.EnableSkipDialogue();// Enable skip dialogue/text interaction
                DialogueUI.Instance.Show();
                OnTeaDialogue?.Invoke(this, new OnTeaDialogueArgs {
                    isNormalTea = isNormalTea
                });
            } else {
                Debug.Log("I should talk to myself");
                // Start talking to himself
            }
        }
    }
}
