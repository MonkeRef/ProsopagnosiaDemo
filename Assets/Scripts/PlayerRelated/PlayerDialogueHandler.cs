using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDialogueHandler : MonoBehaviour
{
    public static PlayerDialogueHandler Instance { get; private set; }

    public event EventHandler OnDayChange;

    private string dialogueTriggeredName;
    private GameObject dialogueGameObjectName;
    // private Collider2D collision;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        DialogueQueuer.Instance.OnOneTimeDialogue += DialogueQueuer_OnOneTimeDialogue;
    }

    private void DialogueQueuer_OnOneTimeDialogue (object sender, EventArgs e) {
        Destroy(dialogueGameObjectName);
    }

    private void OnTriggerEnter2D (Collider2D collision) { // Dialog Otomatis
        dialogueTriggeredName = collision.gameObject.name;
        if (collision.CompareTag("Dialogue")) {
            GameInput.Instance.DisablePlayerMovementAndInteract();
            GameInput.Instance.EnableSkipDialogue();// Enable skip dialogue/text interaction
            DialogueUI.Instance.Show();
            DialogueShowBasedOnNameHandler(dialogueTriggeredName);
        } else if (collision.CompareTag("EndDay")) {
            OnDayChange?.Invoke(this, EventArgs.Empty);
        } else if (collision.CompareTag("Cutscene")) {
            /*if(collision.name == "Day1GrandpaOpeningDialogue") {
                TimelineManager.Instance.GrandpaOpeningCutscene();
            }*/
        } else if (collision.CompareTag("NextScene")) {
            Debug.Log("Called");
            Loader.Load(Loader.Scene.EndingScene);
        }
    }

    public void DialogueShowBasedOnNameHandler (string dialogueTriggeredName) { // Get Dialogue Content from GameObject
        dialogueGameObjectName = GameObject.Find(dialogueTriggeredName);
        DialogueQueuer.Instance.StartDialogue(
            dialogueGameObjectName.GetComponent<DialogueGetter>().GetNames(),
            dialogueGameObjectName.GetComponent<DialogueGetter>().GetIntercolutorDialogues(),
            dialogueGameObjectName.GetComponent<DialogueGetter>().GetSecondIntercolutorDialogues(),
            dialogueTriggeredName
        );
    }

}
