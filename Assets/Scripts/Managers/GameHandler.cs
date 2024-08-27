using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private const string IS_TELEPORTING = "IsTeleporting";
    private const string IS_DONE_TELEPORTING = "IsDoneTeleporting";

    public static GameHandler Instance { get; private set; }

    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    public event EventHandler OnEndDayAvaiable;

    [SerializeField] private Camera gameCamera;
    [SerializeField] private Animator fadeTransition;

    // Change to List of gameobject for maps, setactive whenever the player isn;t there.
    [SerializeField] private GameObject sleepingBackground;
    [SerializeField] private GameObject minigameMap;
    [SerializeField] private GameObject dayEndTrigger;

    public List<GameObject> daysGameObjects;
    public List<GameObject> strikethroughGameObjects;

    private int currentRoutineFinished = 0;
    private bool isMinigamePlayed = false;
    private bool isGamePaused = false;

    private enum State {
        Day1,
        Day2,
        Day5,
        Day9,
        Day15,
        Minigame
    }

    private State state;
    private State tempState;

    private void Awake () {
        Time.timeScale = 1f;

        state = State.Day1;
        Instance = this;

    }

    private void Start () {
        DisableGameObject();
        
        minigameMap.SetActive(false);
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        PlayerDialogueHandler.Instance.OnDayChange += PlayerDialogueHandler_OnDayChange;
        MinigameManager.Instance.OnMinigameEnded += MinigameManager_OnMinigameEnded;
        TimelineManager.Instance.OnCutsceneEnded += TimelineManager_OnCutsceneEnded;
        DialogueQueuer.Instance.OnDayEndEnabled += DialogueQueuer_OnDayEndEnabled;
        // + On minigame Ended
    }

    private void DialogueQueuer_OnDayEndEnabled (object sender, EventArgs e) {
        dayEndTrigger.SetActive(true);
        DialogueQueuer.Instance.SetEndDayState(false);
    }

    private void TimelineManager_OnCutsceneEnded (object sender, TimelineManager.OnCutSceneEndedEventArgs e) {
        switch (e.cutsceneIndex) {
            case 1:
                // If grandpa opening done
                // Play convo dialogue
                TimelineManager.Instance.GrandpaConversationCutscene();
                break;
            case 2:
                // If convo done, play mother came down
                TimelineManager.Instance.MotherCameDownCutscene();
                break;
            case 3:
                // If mother and grandpa convo done
                Player.Instance.teleportLocationHandler("BottomFloorToTopFloorTP");
                Mother.Instance.SetCharacterPosition(new Vector2(3.1f, -12.3f));
                Grandpa.Instance.SetCharacterPosition(new Vector2(3.1f, -15.3f));
                BotMovementManager.Instance.SetMotherhouldBeMoving(false);
                // Move player to on top
                // Move Grandpa to -3.1f, -15.3f
                // Move Mother to 3.1f, 0.3f
                // Remove table utensils
                break;
            case 4: // Dream
                // Play bully dream
                gameCamera.transform.position = new Vector3(-100f, 0, -10f);

                TimelineManager.Instance.BullyNightmareCutscene();
                break;
            case 5: // BullyNightmare
                // After bullyNightmare
                sleepingBackground.SetActive(false);
                Player.Instance.SetCharacterPosition(new Vector2(-14.5f, 0f));
                gameCamera.transform.position = new Vector3(-17f, 0, -10f);
                break;
        }

    }

    private void MinigameManager_OnMinigameEnded (object sender, EventArgs e) {
        state = tempState;
        minigameMap.SetActive(false);
        StartCoroutine(FadeTransition());
    }

    public void SetStateMinigameStart () {
        tempState = state;
        state = State.Minigame;
        minigameMap.SetActive(true);
        isMinigamePlayed = true;
        // + Show delivery UI
        StartCoroutine(FadeTransition());
    }

    IEnumerator FadeTransition () {
        GameInput.Instance.DisablePlayerMovementAndInteract();
        GameObject player = GameObject.Find("Player");
        GameObject gameCamera = GameObject.Find("Main Camera");
        fadeTransition.SetTrigger(IS_TELEPORTING);
        yield return new WaitForSeconds(1);
        if(state == State.Minigame) {
            player.transform.position = new Vector2(16f, -12f); // Move player to minigame Map
            gameCamera.transform.position = new Vector3(17f, -13f, -10f); // Move Camera to MitchBedroom
        } else {
            player.transform.position = new Vector2(-3.5f, -12f); // Move player to bottomFloor Map
            gameCamera.transform.position = new Vector3(0, -13f, -10f); // Move Camera to bottomFloor
        }
        Player.Instance.ResetCharacterVisualPosition();
        fadeTransition.SetTrigger(IS_DONE_TELEPORTING);
        GameInput.Instance.EnablePlayerMovementAndInteract();
    }

    private void PlayerDialogueHandler_OnDayChange (object sender, EventArgs e) {

        if(state  == State.Day1) { // Only activated when player touch bed to sleep. Check current day state and change it into next day
            state = State.Day2;
            daysGameObjects[0].gameObject.SetActive(false); // Set Day 1 GameObject to false
            daysGameObjects[1].gameObject.SetActive(true);// Set Day 5 GameObject to true
            /*GameObject gameObjectTemp = daysGameObjects[1]; // Set Day 2 GameObject to true
            gameObjectTemp.SetActive(true);*/
            // isMinigamePlayed = true; // Remove this after testing
            TimelineManager.Instance.SleepingCutscene();
        } else if(state == State.Day2) {
            state = State.Day5;
            daysGameObjects[1].gameObject.SetActive(false); // Set Day 2 GameObject to false
            daysGameObjects[2].gameObject.SetActive(true);// Set Day 5 GameObject to true

        } else if(state == State.Day5) {
            state = State.Day9;
            daysGameObjects[2].gameObject.SetActive(false); // Set Day 5 GameObject to false
            daysGameObjects[3].gameObject.SetActive(true);// Set Day 9 GameObject to true
        } else if (state == State.Day9) {
            state = State.Day15;
            daysGameObjects[3].gameObject.SetActive(false); // Set Day 9 GameObject to false
            daysGameObjects[4].gameObject.SetActive(true);// Set Day 15 GameObject to true
        }
        dayEndTrigger.SetActive(false);
        Player.Instance.SetCharacterPosition(new Vector2(-100f, 0f));
        isMinigamePlayed = false;
        SetStrikthroughGameObjectInvisble();
    }

    private void GameInput_OnPauseAction (object sender, EventArgs e) {
        TogglePauseGame();
    }

    private void DisableGameObject () {
        // GameObjects already assigned publicly

        foreach (GameObject gameObject in daysGameObjects) { 
            gameObject.SetActive(false); // Disable every gameObject in List
        }
        daysGameObjects[0].gameObject.SetActive(true); // For day 1, make it true
        
        dayEndTrigger.SetActive(false);

        foreach (GameObject gameObject in strikethroughGameObjects) {
            gameObject.SetActive(false); // Disable every gameObject in List
        }
    }

    public void TogglePauseGame () {
        isGamePaused = !isGamePaused;
        if (isGamePaused) {
            Time.timeScale = 0f; // Pause everything
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 1f; // Unpause 
            OnGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool GetCurrentStateDay1 () {
        return state == State.Day1;
    }
    public bool GetCurrentStateDay2 () {
        return state == State.Day2;
    }
    public bool GetCurrentStateDay5 () {
        return state == State.Day5;
    }
    public bool GetCurrentStateDay9 () {
        return state == State.Day9;
    }
    public bool GetCurrentStateDay15 () {
        return state == State.Day15;
    }

    public void SetStrikethroughGameObjectVisible (string interactedGameObject) {
        if (interactedGameObject == "Sink") {
            interactedGameObject = "BrushTeethStrikethrough";
        } else if (interactedGameObject == "Window") {
            interactedGameObject = "WindowStrikethrough";
        } else if (interactedGameObject == "Bathtub") {
            interactedGameObject = "ShowerStrikethrough";
        }else if (interactedGameObject == "Cupboard") {
            interactedGameObject = "ClothesStrikethrough";
        }

        foreach (GameObject gameObject in strikethroughGameObjects) { // Check, if one of GO from strikethroughgameobject == interacted, then setactive the strikethrough true
            if(gameObject.name == interactedGameObject) {
                if (!gameObject.activeSelf) { // If gameObject is inactive, add to the count
                    currentRoutineFinished++;
                }
                gameObject.SetActive(true);
                break;
            }
            // If every gameObject.setActive is true, then call OnOneTimeDialogue, destroying the gameObject
            // Dont forget to destroy RoutineBarrier too
        } 
    }

    private void SetStrikthroughGameObjectInvisble () {
        RoutineUI.Instance.Show();

        foreach (GameObject gameObject in strikethroughGameObjects) {
            gameObject.SetActive(false); // Disable every gameObject in List
        }
        RoutineUI.Instance.Hide();
        currentRoutineFinished = 0;
    }

    public int GetCurrentRoutineFinished () {
        return currentRoutineFinished;
    }

    public bool IsMinigamePlayed () {
        return isMinigamePlayed;
    }
}
