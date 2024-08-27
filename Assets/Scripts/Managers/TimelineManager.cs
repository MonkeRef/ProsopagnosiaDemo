using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour {

    private const string IS_TELEPORTING = "IsTeleporting";
    private const string IS_DONE_TELEPORTING = "IsDoneTeleporting";
    public static TimelineManager Instance { get; private set; }

    public event EventHandler OnBreakfastEnded;
    public event EventHandler<OnCutSceneEndedEventArgs> OnCutsceneEnded;

    public class OnCutSceneEndedEventArgs : EventArgs {
        public int cutsceneIndex;
    }

    [SerializeField] private Animator fadeTransition;

    public List<PlayableDirector> directors;

    private bool isDirectorStarted = false;
    private bool isMinigameTime = false;
    private int index = 0;
    private float directorTimer;

    // OnCutsceneTriggered
    // Play Certain director
    // Then destroy that director/gameobject after finished

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        DialogueQueuer.Instance.OnDialogueEnded += DialogueQueuer_OnDialogueEnded;
    }

    private void DialogueQueuer_OnDialogueEnded (object sender, EventArgs e) {
        StopDirector();
    }

    private void Update () {
        if (isDirectorStarted) {
            directorTimer -= Time.deltaTime;
            if (directorTimer <= 0f) {
                StopDirector();
            }
        }
    }
    public void BreakfastCutscenes () {
        StartCoroutine(FadeTransition(0));
        GameObject topChair = GameObject.Find("TopChairForCutscene");
        topChair.layer = LayerMask.NameToLayer("Default");
    }

    public void GrandpaOpeningCutscene () {
        StartCoroutine(FadeTransition(1));
    }

    public void GrandpaConversationCutscene () {
        StartCoroutine(FadeTransition(2));
    }

    public void MotherCameDownCutscene () {
        StartCoroutine(FadeTransition(3));
    }

    public void SleepingCutscene () {
        StartCoroutine(FadeTransition(4));
    }

    public void BullyNightmareCutscene () {
        StartCoroutine(FadeTransition(5));
    }

    // Save for Day 2
/*    public void NightmareCutscene () {
        StartCoroutine(FadeTransition(5));
    }*/

    private void StopDirector () {
        isDirectorStarted = false;
        directors[index].Stop();

        if (isMinigameTime && !GameHandler.Instance.IsMinigamePlayed()) {
            MinigameManager.Instance.SetMinigameStartTrue(); // Starts minigame
            GameHandler.Instance.SetStateMinigameStart();  // Change state and move character
            isMinigameTime = false;
        } else {
            OnCutsceneEnded?.Invoke(this, new OnCutSceneEndedEventArgs {
                cutsceneIndex = index
            });
            // Else, Go to gamehandler
            // After every cutscene, move the player position as u wish
        }

        // Need a condition too, so not after the first cutscene will trigger minigame
        // Answer : use isMinigameTime, make it into true if want minigame to be played
    }
    IEnumerator FadeTransition (int index) {
        fadeTransition.SetTrigger(IS_TELEPORTING);
        yield return new WaitForSeconds(1);
        directors[index].Play();
        directorTimer = (float)directors[index].duration; // Get duration of the animation/timeline

        this.index = index;
        isDirectorStarted = true;
        fadeTransition.SetTrigger(IS_DONE_TELEPORTING);
    }

    public bool GetDirectorState () {
        return isDirectorStarted;
    }

    public void SetIsMinigameTime(bool isMinigameTime) {
        this.isMinigameTime = isMinigameTime;
    }
}
