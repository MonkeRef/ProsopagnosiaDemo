using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueQueuer : MonoBehaviour
{
    private const string IS_TELEPORTING = "IsTeleporting";
    private const string IS_DONE_TELEPORTING = "IsDoneTeleporting";
    public static DialogueQueuer Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image mitchImage;
    [SerializeField] private Image oppositeImage;

    public Sprite[] mitchSpriteLists;
    public Sprite[] momSpriteLists;
    public Sprite[] miscsSpriteLists;

    [SerializeField] private Animator fadeTransition;

    public Queue<Sprite> mitchSpriteDialogue;
    public Queue<Sprite> oppositeSpriteDialogue;
    public Queue<string> sentences;
    public Queue<string> names;

    public event EventHandler OnOneTimeDialogue;
    public event EventHandler OnDialogueEnded;
    public event EventHandler OnDayEndEnabled;
    public event EventHandler<OnDialogueActivationArgs> OnDialogueActivation;

    public class OnDialogueActivationArgs : EventArgs {
        public string currentDialogueGameObjectName;
    }

    private float dialogueTimer = 5f;
    private float dialogueTimerMax = 5f;

    private bool isTalking = false;
    private bool isFirstTimeDialogue = true;
    private bool isOneTimeThing = true;
    private bool isMitchSpriteListNeeded = false;
    private bool isOppositeSpriteListNeeded = false;
    private bool isCutsceneNeeded = false;
    private bool isEndDayEnabled = false;

    private GameObject cutsceneDialogue;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        sentences = new Queue<string>();
        names = new Queue<string>();
        mitchSpriteDialogue = new Queue<Sprite>();
        oppositeSpriteDialogue = new Queue<Sprite>();

        GameInput.Instance.OnSkipDialogueAction += GameInput_OnSkipDialogueAction;
    }


    private void GameInput_OnSkipDialogueAction (object sender, EventArgs e) {
        DisplayNextSentence();
    }

    private void Update () {
        if (isTalking) {
            if (isFirstTimeDialogue) {
                isFirstTimeDialogue = false;
                DisplayNextSentence();
            } else {
                dialogueTimer -= Time.deltaTime;
                if (dialogueTimer <= 0f) {
                    DisplayNextSentence();
                }
            }
        }
    }


    public void DreamCutsceneDialogue () {
        cutsceneDialogue = GameObject.Find("Day1DreamDialogue");

        dialogueTimerMax = 3f;
        PrepareToStartDialogue();
    }

    public void BullyNightmareCutsceneDialogue () {
        cutsceneDialogue = GameObject.Find("Day1BullyNightmareDialogue");

        dialogueTimerMax = 2f;
        PrepareToStartDialogue();
        // For testing, comment below
        GameInput.Instance.DisableSkipDialogue();
    }

    /*public void NightmareFollowMomDialogue () {
        cutsceneDialogue = GameObject.Find("Day1NightmareFollowMomDialogue");

        dialogueTimerMax = 2.6f;
        PrepareToStartDialogue();
        GameInput.Instance.EnablePlayerMovementAndInteract();
    }*/

    public void BreakfastCutsceneDialogue () { // Used on Breakfast Timeline
        cutsceneDialogue = GameObject.Find("Day1BreakfastDialogue");

        dialogueTimerMax = 3f;
        PrepareToStartDialogue();
    }

    public void GrandpaOpeningCutsceneDialogue () { // Used on Grandpa Timeline
        cutsceneDialogue = GameObject.Find("Day1GrandpaOpeningDialogue");
        dialogueTimerMax = 4f;
        PrepareToStartDialogue();
    }

    public void GrandpaConversationCutsceneDialogue () { // Used on Grandpa Timeline
        cutsceneDialogue = GameObject.Find("Day1GrandpaConversationDialogue");

        dialogueTimerMax = 7f;
        PrepareToStartDialogue();
    }

    public void MotherAndGrandpaCutsceneDialogue () {
        cutsceneDialogue = GameObject.Find("Day1MotherCameDownDialogue");

        dialogueTimerMax = 4f;
        PrepareToStartDialogue();
    }

    private void PrepareToStartDialogue () {
        GameInput.Instance.DisablePlayerMovementAndInteract();
        GameInput.Instance.EnableSkipDialogue();// Enable skip dialogue/text interaction
        DialogueUI.Instance.Show();
        StartDialogue(
            cutsceneDialogue.GetComponent<DialogueGetter>().GetNames(),
            cutsceneDialogue.GetComponent<DialogueGetter>().GetIntercolutorDialogues(),
            cutsceneDialogue.GetComponent<DialogueGetter>().GetSecondIntercolutorDialogues(),
            cutsceneDialogue.name
        );
    }

    public void TeaConversationDialogue (string[] currentNames, string[] mitchDialogues, string[] oppositeDialogues, bool isNormalTea) {
        isTalking = true;
        sentences.Clear();

        SetActiveEverything();

        // Mitch opening
        mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]); // Talking
        oppositeSpriteDialogue.Enqueue(miscsSpriteLists[2]); // Neutral
        int index = UnityEngine.Random.Range(0, 3); // Mitch Opening
        names.Enqueue(currentNames[0]);
        sentences.Enqueue(mitchDialogues[index]);

        // NPC asking for tea
        mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]); // Neutral
        oppositeSpriteDialogue.Enqueue(miscsSpriteLists[3]); // Talking
        names.Enqueue(currentNames[1]);
        if (isNormalTea) {
            index = UnityEngine.Random.Range(0, 2);
            sentences.Enqueue(oppositeDialogues[index]);
        } else {
            index = UnityEngine.Random.Range(3, 5);
            sentences.Enqueue(oppositeDialogues[index]);
        }

        // Mitch says please wait for a bit
        mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
        oppositeSpriteDialogue.Enqueue(miscsSpriteLists[2]); // Neutral
        index = UnityEngine.Random.Range(4, 6);
        names.Enqueue(currentNames[0]);
        sentences.Enqueue(mitchDialogues[index]);
    }


    public void TeaClosingDialogue (string[] currentNames, string[] mitchDialogues, string[] oppositeDialogues, bool isSuccess) {
        isTalking = true;
        sentences.Clear();

        SetActiveEverything();

        int index = 0;
        if(isSuccess) { // Right tea delivered
            index = UnityEngine.Random.Range(6, 8); // Opposite Dialogue say thanks
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
        } else { // Wrong tea delivered
            index = UnityEngine.Random.Range(9, 11); // Opposite Dialogue say whatever
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[3]);
        }

        oppositeSpriteDialogue.Enqueue(miscsSpriteLists[3]); // Talking
        // Opposite Dialogue 
        names.Enqueue(currentNames[1]);
        sentences.Enqueue(oppositeDialogues[index]);

        if (isSuccess) {
            index = UnityEngine.Random.Range(7, 8); // mitch Dialogue say thanks
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
        } else {
            index = UnityEngine.Random.Range(9, 10); // mitch Dialogue say sorry
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[3]);
        }

        oppositeSpriteDialogue.Enqueue(miscsSpriteLists[2]);
        // Mitch Dialogue
        names.Enqueue(currentNames[0]);
        sentences.Enqueue(mitchDialogues[index]);
    }

    public void StartDialogue (string[] currentNames, string[] mitchDialogues, string[] oppositeDialogues, string dialogueGOTriggeredName) {
        isTalking = true;
        sentences.Clear();

        // Check the gameobject tag and compare it to start which dialogue
        GameObject dialogueGameObjectName = GameObject.Find(dialogueGOTriggeredName);

        // Check the Day State, Check the tag, and call the enqueue function

        if (dialogueGameObjectName.CompareTag("Dialogue")) {
            if (GameHandler.Instance.GetCurrentStateDay1()) {
                Day1Dialogues(currentNames, mitchDialogues, oppositeDialogues, dialogueGOTriggeredName);
            }else if(GameHandler.Instance.GetCurrentStateDay2()) {
                Day2Dialogues(currentNames, mitchDialogues, oppositeDialogues, dialogueGOTriggeredName);
            } else if (GameHandler.Instance.GetCurrentStateDay5()) {
                // Day5Dialogues(currentNames, mitchDialogues, oppositeDialogues, dialogueGOTriggeredName);
            } else if (GameHandler.Instance.GetCurrentStateDay9()) {
                // Day9Dialogues(currentNames, mitchDialogues, oppositeDialogues, dialogueGOTriggeredName);
            } else if (GameHandler.Instance.GetCurrentStateDay15()) {
                // Day15Dialogues(currentNames, mitchDialogues, oppositeDialogues, dialogueGOTriggeredName);
            }
        } else if (dialogueGameObjectName.CompareTag("InteractableDialogue")){
            // Interactable Object Dialogue Sequence
            InteractableObjectDialogue(currentNames, mitchDialogues, oppositeDialogues, dialogueGOTriggeredName);
        }else if (dialogueGameObjectName.CompareTag("Cutscene")) {
            // Cutscene Dialogue
            CutsceneObjectDialogue(currentNames, mitchDialogues, oppositeDialogues, dialogueGOTriggeredName);
        }

    }

    private void CutsceneObjectDialogue (string[] currentNames, string[] mitchDialogues, string[] oppositeDialogues, string dialogueGOTriggeredName) {
        switch (dialogueGOTriggeredName) {
            case "Day1BreakfastDialogue":
                SetActiveEverything();

                // How was the breakfast
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[0]);

                // It was delicious
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[0]);

                // And?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[1]);

                //...
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[1]);

                // Thank you mom.
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[2]);

                // You are welcome
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[2]);

                // Time to open up the tea store
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[3]);

                // Okay.
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[3]);
                break;
            case "Day1GrandpaOpeningDialogue":
                SetActiveEverything();

                // Hey kid
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[0]);

                // Good evening Sir
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[0]);

                // Ow, I'm just checking out
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[1]);

                // I just moved in not long ago
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[2]);

                // Welcome to our store! Please have a seat
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[1]);

                // I'll give you a tea
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[2]);

                // Thank you
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[3]);

                Destroy(cutsceneDialogue);
                break;
            case "Day1GrandpaConversationDialogue":
                SetActiveEverything();

                // After getting Tea cutscene
                // OM : Thank you! (for the tea)
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[0]);

                // M : Enjoy the tea
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[0]);

                // M : Anyway, Where did u come from?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[1]);

                // OM : I came from city nearby
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[1]);

                // M : Oh, did you move in alone?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[2]);

                // OM : Yes, I'm retired
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[2]);

                // OM : Here is our family photo
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[3]);

                // OM : Small one is my granddaughter, she pretty?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[4]);

                // M : I guess?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[3]);

                // OM : Wdym? she aint good enough?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[5]);

                // M : I'm sorry, i didnt mean that
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[4]);

                // M : It's just that, i have condition
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[5]);

                // OM : What's that?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[6]);

                // M : Ok this gnna take some time, do you have time?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[6]);

                // OM : Of course, i have all time.
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[7]);

                // M : In short, hard to recognize
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[7]);

                // M : My condition is severe
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[8]);

                // OM : I'm sorry to hear that, since when?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[8]);

                // M : I have this condition since born
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[9]);

                // OM : Is it curable?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[9]);

                // M : Nah, my mother consulted with doctor
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[10]);

                // M : So i accept and move on
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[11]);

                // OM : I hope it doesn't interfere with ur life
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[10]);

                // M : ...
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[12]);

                // M : Thanks for listening
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[13]);

                // Coincidentally, mom came down
                Destroy(cutsceneDialogue);
                break;
            case "Day1MotherCameDownDialogue":
                SetActiveEverything();
                // Mom : Mitch, I'm back, you can go up now.
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[0]);

                // Mom : Oh. Do we have a customer?
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
                names.Enqueue(currentNames[1]);
                sentences.Enqueue(oppositeDialogues[1]);

                // M : No he just moved in
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[0]);

                // Mom : Oh welcome to my store! I'm lyria
                mitchSpriteDialogue.Enqueue(momSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[2]);
                sentences.Enqueue(oppositeDialogues[2]);

                // OM : Hello nice to meet you, I'm michael
                mitchSpriteDialogue.Enqueue(momSpriteLists[2]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[3]);
                sentences.Enqueue(oppositeDialogues[3]);

                // M : Oh we have similiar name! I'm Mitch
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[1]);

                // M : Anyway, since mom is here, i go up
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[0]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[2]);

                // M : Nice to meet you pops.
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[3]);

                // OM : Nice to meet you too.
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[1]);
                names.Enqueue(currentNames[3]);
                sentences.Enqueue(oppositeDialogues[4]);

                isEndDayEnabled = true;
                break;
            case "Day1DreamDialogue":
                SetActiveFalseMitchImage();
                SetActiveFalseOppositeImage();
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[0]);

                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[1]);

                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[2]);

                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[3]);

                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[4]);

                break;
            case "Day1BullyNightmareDialogue":
                SetActiveEverything();

                // 8-9
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(null);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[0]);

                // 9-10
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(null);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[1]);
                // M : ? 10-11
                mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[2]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[2]);

                int index;
                for (int x = 0; x < 8; x++) { // 11-19
                    index = UnityEngine.Random.Range(1, 7);
                    if(x == 4) {
                        mitchSpriteDialogue.Enqueue(miscsSpriteLists[4]);
                        sentences.Enqueue(oppositeDialogues[2]);
                    } else {
                        mitchSpriteDialogue.Enqueue(miscsSpriteLists[4]);
                        sentences.Enqueue(oppositeDialogues[x]);
                    }
                    names.Enqueue(currentNames[index]);
                    oppositeSpriteDialogue.Enqueue(miscsSpriteLists[4]);
                }

                // 19-20
                mitchSpriteDialogue.Enqueue(miscsSpriteLists[4]);
                oppositeSpriteDialogue.Enqueue(miscsSpriteLists[4]);
                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[4]);

                break;
        }
    }


    private void InteractableObjectDialogue (string[] currentNames, string[] mitchDialogues, string[] oppositeDialogues, string dialogueGOTriggeredName) {
        if (dialogueGOTriggeredName == "BasementDialogue") {

            SetActiveTrueMitchImage();
            SetActiveFalseOppositeImage();

            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[0]);

            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[1]);
            OnOneTimeDialogue?.Invoke(this, EventArgs.Empty);
            OnDialogueActivation?.Invoke(this, new OnDialogueActivationArgs {
                currentDialogueGameObjectName = dialogueGOTriggeredName
            });;

            GameObject topChair = GameObject.Find("TopChairForCutscene");
            topChair.layer = LayerMask.NameToLayer("InteractableObject");
            // Maybe use ordinary function call? instead of event?
            // Research (Delegate (Event) vs Method (Function)):
            // Delegate is faster, but it is highly recommended not to make a several same delegate, can cause memory leak
        }
    }

    private void DisplayNextSentence () {
        dialogueTimer = dialogueTimerMax;
        isTalking = false;

        if (sentences.Count == 0) {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

        string name = names.Dequeue();
        nameText.text = name;

        if (isOppositeSpriteListNeeded) { // True because use Enqueue
            Sprite sprite = oppositeSpriteDialogue.Dequeue();
            if(sprite == null) {
                oppositeImage.color = new Color(0f, 0f, 0f, 0f);
            } else {
                oppositeImage.color = new Color(255f, 255f, 255f, 255f);
                oppositeImage.sprite = sprite;
            }
        }

        if (isMitchSpriteListNeeded) {
            Sprite mitchSprite = mitchSpriteDialogue.Dequeue();
            mitchImage.sprite = mitchSprite;
        }
    }

    IEnumerator TypeSentence (string sentence) {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return null;
            isTalking = true;
        }
    }

    public void EndDialogue () {
        nameText.text = "";
        dialogueText.text = "X TO CONTINUE";
        dialogueTimerMax = 5f;
        oppositeImage.sprite = null;

        GameInput.Instance.EnablePlayerMovementAndInteract();
        GameInput.Instance.DisableSkipDialogue(); // Disable skip interaction
        DialogueUI.Instance.Hide();
        isFirstTimeDialogue = true;
        isOppositeSpriteListNeeded = false;
        isMitchSpriteListNeeded = false;

        if (TimelineManager.Instance.GetDirectorState()) { // If in cutscene
            OnDialogueEnded?.Invoke(this, EventArgs.Empty);
        }
        if(isCutsceneNeeded == true) {
            TimelineManager.Instance.GrandpaOpeningCutscene();
            isCutsceneNeeded = false;
        }
        if(isEndDayEnabled == true) { 
            OnDayEndEnabled?.Invoke(this, EventArgs.Empty);
        }
    }

    private void SetActiveFalseMitchImage () {
        mitchImage.enabled = false;
    }
    private void SetActiveTrueMitchImage () {
        mitchImage.enabled = true;
    }
    private void SetActiveFalseOppositeImage () {
        oppositeImage.enabled = false;
    }
    private void SetActiveTrueOppositeImage () {
        oppositeImage.enabled = true;
    }

    public void SetEndDayState(bool isEndDayEnabled) {
        this.isEndDayEnabled = isEndDayEnabled;
    }

    private void SetActiveEverything () {
        SetActiveTrueMitchImage();
        SetActiveTrueOppositeImage();
        isOppositeSpriteListNeeded = true;
        isMitchSpriteListNeeded = true;
    }
    private void Day1Dialogues (string[] currentNames, string[] mitchDialogues, string[] oppositeDialogues, string dialogueGOTriggeredName) {
        // Need to Enqueue berdasarkan objek yang di find
        if (dialogueGOTriggeredName == "MotherDoorDialogue") {
            SetActiveTrueMitchImage();
            SetActiveFalseOppositeImage();
            mitchImage.sprite = mitchSpriteLists[0];

            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[0]);

            OnOneTimeDialogue?.Invoke(this, EventArgs.Empty);
        } else if (dialogueGOTriggeredName == "OpeningDialogue") {
            SetActiveFalseOppositeImage();
            SetActiveFalseMitchImage();

            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[0]);

            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[0]);

            OnOneTimeDialogue?.Invoke(this, EventArgs.Empty); // Will destroy GameObject after triggered
        } else if (dialogueGOTriggeredName == "RoutineBarrierDialogue") {
            SetActiveFalseOppositeImage();
            if (GameHandler.Instance.GetCurrentRoutineFinished() >= 4) {
                OnOneTimeDialogue?.Invoke(this, EventArgs.Empty);
                // Destroy routine barrier too
                GameObject routineBarrier = GameObject.Find("RoutineBarrier");
                Destroy(routineBarrier);

                RoutineUI.Instance.Hide();
            } else {
                if (isOneTimeThing) {
                    isOneTimeThing = false;
                    RoutineUI.Instance.Show();
                }
                SetActiveTrueMitchImage();
                mitchImage.sprite = mitchSpriteLists[0];

                names.Enqueue(currentNames[0]);
                sentences.Enqueue(mitchDialogues[0]);
            }
        } else if (dialogueGOTriggeredName == "GoToBasementDialogue") {
            SetActiveTrueOppositeImage();
            SetActiveTrueMitchImage();
            isOppositeSpriteListNeeded = true;

            mitchImage.sprite = mitchSpriteLists[0];

            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[0]); // Mitch please take..

            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[0]); // Sure, but

            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[1]); // You should

            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[1]); // Why

            oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[2]); // Because you will

            oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[2]); // ...

            oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[3]); // Ok

            OnOneTimeDialogue?.Invoke(this, EventArgs.Empty);
        } else if (dialogueGOTriggeredName == "WindowDialogue" || dialogueGOTriggeredName == "FamilyPitcureDialogue") {
            SetActiveTrueMitchImage();
            SetActiveFalseOppositeImage();
            mitchImage.sprite = mitchSpriteLists[0];
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[0]);

            if (dialogueGOTriggeredName == "WindowDialogue") {
                GameHandler.Instance.SetStrikethroughGameObjectVisible(dialogueGOTriggeredName);
            }
            OnOneTimeDialogue?.Invoke(this, EventArgs.Empty);
        } else if (dialogueGOTriggeredName == "ThankYouDialogue") {
            SetActiveEverything();

            mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[0]); // Thank you mitch

            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[0]); // You're Welcome

            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[1]); // Let's eat now

            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[1]); // Sure

            OnOneTimeDialogue?.Invoke(this, EventArgs.Empty);
        }else if (dialogueGOTriggeredName == "AfterWorkDialogue") {
            SetActiveEverything();

            // *Well, that was tiring*
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[0]);

            // I'm gonna got to my bedroom
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[1]);

            // ...
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[0]);

            // Sure
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[1]);

            OnOneTimeDialogue?.Invoke(this, EventArgs.Empty);

        } else if (dialogueGOTriggeredName == "AfterFirstDayMinigameDialogue") {
            SetActiveEverything();

            // So..
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[0]);

            // So what have u been doing
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[1]);

            // Is it not boring
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[2]);

            // Not really
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[0]);

            // I have been watching utub
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[1]);

            // Fun facts?
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[3]);

            // Of course
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[2]);

            // Did you know that the art of
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[3]);

            // No, i didn't
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[4]);

            // How about another one
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[5]);

            // Sure, did you know that tea is popular?
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[4]);

            // Of course i know
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[6]);

            // Wait, you didn't know?
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[7]);

            // No... i thought
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[5]);

            // Well, at least you get new knowledges
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[8]);

            // You know...
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[9]);

            // We can try going out sometime
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[10]);

            // We don't need to go far away
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[11]);

            // Mitch : ...
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[6]);

            // I don't feel like going out
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[7]);

            // Mom : ...
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[0]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[12]);

            // Alright that's fine
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[13]);

            // Anyway, restroom
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[0]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[14]);

            // Mitch : Sure
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[1]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[2]);
            names.Enqueue(currentNames[0]);
            sentences.Enqueue(mitchDialogues[8]);

            // Brb
            mitchSpriteDialogue.Enqueue(mitchSpriteLists[2]);
            oppositeSpriteDialogue.Enqueue(momSpriteLists[1]);
            names.Enqueue(currentNames[1]);
            sentences.Enqueue(oppositeDialogues[15]);

            isCutsceneNeeded = true;
            OnOneTimeDialogue?.Invoke(this, EventArgs.Empty);
        }
    }


    private void Day2Dialogues (string[] currentNames, string[] mitchDialogues, string[] oppositeDialogues, string dialogueGOTriggeredName) {
        // if(dialogueGOTriggeredName == "")
    }
}
