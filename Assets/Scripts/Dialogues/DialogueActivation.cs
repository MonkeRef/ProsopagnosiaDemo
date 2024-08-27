using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueActivation : MonoBehaviour {
// Purpose of the Script : Activating dialogue/trigger/cutscene after certain dialogue/action

    public static DialogueActivation Instance { get; private set; }

    public GameObject[] listedGameObjectsWithDialogues; // Put GameObjects that with dialogues need to be activated first.
    private GameObject currentGameObject;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        foreach (GameObject gameObjects in listedGameObjectsWithDialogues) {
            gameObjects.SetActive(false);
        }

        DialogueQueuer.Instance.OnDialogueActivation += DialogueQueuer_OnDialogueActivation;
        MinigameManager.Instance.OnMinigameEnded += MinigameManager_OnMinigameEnded;
    }

    private void MinigameManager_OnMinigameEnded (object sender, System.EventArgs e) {
        ShowCurrentGameObject(1); // Activate Day1AfterMinigame
        ShowCurrentGameObject(2); // Activate Day1AfterWork
    }

    private void DialogueQueuer_OnDialogueActivation (object sender, DialogueQueuer.OnDialogueActivationArgs e) {
        // Should change in the future, use foreach and One IF only
        if (e.currentDialogueGameObjectName == "BasementDialogue") {
            ShowCurrentGameObject(0);
        }
    }

    private void ShowCurrentGameObject (int index) {
        currentGameObject = listedGameObjectsWithDialogues[index];
        currentGameObject.SetActive(true);
    }
}
