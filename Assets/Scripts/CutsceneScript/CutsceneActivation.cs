using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneActivation : MonoBehaviour
{
    public static CutsceneActivation Instance { get; private set; }

    public GameObject[] listedGameObjectsWithDialogues; // Put GameObjects that with dialogues need to be activated first.
    private GameObject currentGameObject;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        foreach (GameObject gameObjects in listedGameObjectsWithDialogues) {
            gameObjects.SetActive(false);
        }

        MinigameManager.Instance.OnMinigameEnded += MinigameManager_OnMinigameEnded;
    }

    private void MinigameManager_OnMinigameEnded (object sender, System.EventArgs e) {
        ShowCurrentGameObject(0); // Day1GrandpaOpeningDialogue
        ShowCurrentGameObject(1); // Day1GrandpaConversationDialogue
        ShowCurrentGameObject(2); // Day1MotherComeDownDialogue
    }

    private void ShowCurrentGameObject (int index) {
        currentGameObject = listedGameObjectsWithDialogues[index];
        currentGameObject.SetActive(true);
    }
}
