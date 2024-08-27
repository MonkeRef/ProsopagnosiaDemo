using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour {
    public void ObjectInteract (InteractableObject interactedObject) {
        // Disini, harus membedakan object yang kena interact itu, tag gameobjectnya apa lalu jalankan scriptnya.
        if (interactedObject.CompareTag("InteractableDialogue")) {
            GameInput.Instance.DisablePlayerMovementAndInteract(); // Disable Movement and Interact
            GameInput.Instance.EnableSkipDialogue();// Enable skip dialogue/text interaction
            DialogueUI.Instance.Show();
            PlayerDialogueHandler.Instance.DialogueShowBasedOnNameHandler(interactedObject.name);
        } else if (interactedObject.CompareTag("RoutineGO")) {
            GameHandler.Instance.SetStrikethroughGameObjectVisible(interactedObject.name);
        } else if (interactedObject.CompareTag("Cutscene")) {
           if(interactedObject.name == "TopChairForCutscene") {
                TimelineManager.Instance.BreakfastCutscenes();
                TimelineManager.Instance.SetIsMinigameTime(true);
            }
        }
    }
}
