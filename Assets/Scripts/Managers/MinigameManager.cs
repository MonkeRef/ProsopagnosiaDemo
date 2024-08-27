using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinigameManager : MonoBehaviour
{
    // Customer System Manager
    // Game Serving time : 3 mins
    // Customer will spawn every time between a random time of 10-15 seconds.

    // 2nd Game type
    // Make the store only for take out
    // Customer will spawn infront of the door entrance and walk to infront of the long counter
    // And then player need to interact to ask what do they want
    // After 2-3 seconds, they will either want normal tea or mixed tea.
    // Player make the tea with plastic cup, put it on the counter (If can, player need to bag it first, then the counter only can accept bagged teas.)
    // If wrong, customer angry and mom happiness--;
    // If right, customer happy and mom happiness++;
    // Mom certain happiness at the end will trigger specific dialogue, like happy dialogue, telling story if interact, etc
    // Then customer leave

    private const string IS_TELEPORTING = "IsTeleporting";
    private const string IS_DONE_TELEPORTING = "IsDoneTeleporting";

    public static MinigameManager Instance { get; private set; }

    [SerializeField] private Animator fadeTransition;
    [SerializeField] private DeliveryRecipeSO deliveryRecipeSO;

    public event EventHandler OnMinigameEnded;
    public event EventHandler OnCustomerSpawn;
    public event EventHandler OnTeaDelivered;

    private KitchenObjectsSO waitingKitchenObject;

    private float customerSpawnTimer = 5f; // original 15f;
    private float minigameTimer = 25f; // Original 90f
    private int customerAmount = 0;
    private bool minigameStart = false;
    private bool correctTeaDelivered = false;
    private bool isThereCustomer = false;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        DeliveryCounter.Instance.OnTeaDialogue += DeliveryCounter_OnTeaDialogue;
        // Show DeliveryUI too
    } 

    private void DeliveryCounter_OnTeaDialogue (object sender, DeliveryCounter.OnTeaDialogueArgs e) {
        DialogueQueuer.Instance.TeaConversationDialogue(
            gameObject.GetComponent<DialogueGetter>().GetNames(),
            gameObject.GetComponent<DialogueGetter>().GetIntercolutorDialogues(),
            gameObject.GetComponent<DialogueGetter>().GetSecondIntercolutorDialogues(),
            e.isNormalTea
       );
    }

    public void SetMinigameStartTrue () {
        minigameStart = true;
        BotMovementManager.Instance.SetMotherhouldBeMoving(false);
    }

    private void Update () {
        if (minigameStart) {
            minigameTimer -= Time.deltaTime;
            if (minigameTimer < 0) { // If minigame Ended
                minigameStart = !minigameStart;
                StartCoroutine(FadeTransitionWhileTeleporting());
                BotMovementManager.Instance.SetCustomerStartMovingIn(false);
                BotMovementManager.Instance.SetCustomerStartsMovingOut(false);
                // + Hide Delivery UI
                for(int x  = 0; x <= customerAmount; x++) {
                    GameObject gameObject = GameObject.FindWithTag("Customer");
                    Destroy(gameObject);
                }
                customerAmount = 0;
                OnMinigameEnded?.Invoke(this, EventArgs.Empty);
            }else if(customerAmount == 0) {
                customerSpawnTimer -= Time.deltaTime;
                if (customerSpawnTimer <= 0f && !isThereCustomer) {
                    // No customer, then add
                    customerSpawnTimer = UnityEngine.Random.Range(5f, 10f);
                    customerAmount++;
                    isThereCustomer = true;

                    waitingKitchenObject = deliveryRecipeSO.kitchenObjectRecipeList[UnityEngine.Random.Range(0, deliveryRecipeSO.kitchenObjectRecipeList.Count)];
                    Debug.Log(waitingKitchenObject.name);
                    OnCustomerSpawn?.Invoke(this, EventArgs.Empty);
                }else {
                    // There is customer
                }
            }
        }
    }

    public void DeliveryRecipe (KitchenObjectsSO kitchenObject) {
        correctTeaDelivered = false;
        // KitchenObject delivered
        // Check if the same KitchenObjectsSO
        if (kitchenObject == waitingKitchenObject) { // Check if the kitchenObject name is the same as wanted for delivery name
            correctTeaDelivered = true;
            // Add happiness meter
        } else {
            // Reduce happiness meter
        }
        GameInput.Instance.DisablePlayerMovementAndInteract();
        GameInput.Instance.EnableSkipDialogue();// Enable skip dialogue/text interaction
        DialogueUI.Instance.Show();

        // Show dialogue
        DialogueQueuer.Instance.TeaClosingDialogue(
                gameObject.GetComponent<DialogueGetter>().GetNames(),
                gameObject.GetComponent<DialogueGetter>().GetIntercolutorDialogues(),
                gameObject.GetComponent<DialogueGetter>().GetSecondIntercolutorDialogues(),
                correctTeaDelivered
        );
        BotMovementManager.Instance.SetCustomerStartsMovingOut(true);
        customerAmount--;
    }

    public KitchenObjectsSO GetWaitingKitchenObject () {
        return waitingKitchenObject;
    }

    IEnumerator FadeTransitionWhileTeleporting () {
        GameInput.Instance.DisablePlayerMovementAndInteract();
        fadeTransition.SetTrigger(IS_TELEPORTING);
        yield return new WaitForSeconds(1);
        fadeTransition.SetTrigger(IS_DONE_TELEPORTING);
        GameInput.Instance.EnablePlayerMovementAndInteract();
    }
    
    public void SetIsThereCustomer (bool isThereCustomer) {
        this.isThereCustomer = isThereCustomer;
    }

}
