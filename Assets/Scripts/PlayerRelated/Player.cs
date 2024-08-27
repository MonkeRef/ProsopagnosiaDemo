using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IKitchenObjectParent, ICharacter {

    public static Player Instance { get; private set; } // Singleton Pattern

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public event EventHandler<OnInteractedObjectChangedArgs> OnInteractedObjectChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    public class OnInteractedObjectChangedArgs : EventArgs {
        public InteractableObject interactedObject;
    }

    [SerializeField] private GameObject playerVisual;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask objectLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private Transform gameCamera;

    public Sprite[] mitchGameplaySprites;

    private int movementSpeed = 5;
    private Vector2 lastInteractDir;
    private Rigidbody2D rb;
    private BaseCounter selectedCounter;
    private InteractableObject interactedObject;
    private KitchenObject kitchenObject;

    // Add optimisation feature :
    // Disable Maps Gameobject if the player isn't there

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        rb = playerVisual.GetComponent<Rigidbody2D>(); // Get playerVisual Rigidbody2D
        gameInput.OnInteractAction += GameInput_OnInteractAction; // When press E
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction; // When press spacebar
    }

    private void GameInput_OnInteractAlternateAction (object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractAction (object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }

        if (interactedObject != null) {
            interactedObject.ObjectInteract(interactedObject);
        }
    }


    private void Update () {
        HandleMovement();
        HandleObjectDetection();
    }
    private void HandleMovement () { // Movement tradisional lebih mudah dimengerti dan dikendalikan (disable W ketika menghadapi wall diatas, etc)
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDir = new Vector3(inputVector.x, inputVector.y, 0f);
        transform.localPosition += moveDir * movementSpeed * Time.deltaTime;


        // Change sprite based on movement
        if(moveDir.x > 0f) {
            playerVisual.GetComponent<SpriteRenderer>().sprite = mitchGameplaySprites[3]; // Right
            kitchenObjectHoldPoint.transform.localPosition = new Vector3(0.46f, -0.12f, 0);
        }else if(moveDir.x < 0f) {
            playerVisual.GetComponent<SpriteRenderer>().sprite = mitchGameplaySprites[2]; // Left
            kitchenObjectHoldPoint.transform.localPosition = new Vector3(-0.46f, -0.12f, 0);
        } else if(moveDir.y > 0f) {
            playerVisual.GetComponent<SpriteRenderer>().sprite = mitchGameplaySprites[0]; // Up
            kitchenObjectHoldPoint.transform.localPosition = new Vector3(0, 0.5f, 0);
        } else if(moveDir.y < 0f) {
            playerVisual.GetComponent<SpriteRenderer>().sprite = mitchGameplaySprites[1]; // Down
            kitchenObjectHoldPoint.transform.localPosition = new Vector3(0, -0.5f, 0);
        }
    }

    // Movement ahrus ditambah cek bisa bergerak atau tidak, kalau ngak, waktu minigame, nanti nge bug.

    private void HandleObjectDetection () {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector2 moveDir = new Vector2(inputVector.x, inputVector.y);

        if (moveDir != Vector2.zero) {
            lastInteractDir = moveDir;
        }

        // This is the best one, but isn't working after some time, not consistent, check youtube why?
        // Answer : use rigidbody position instead of transform position
        float detectionDistance = 1f;
        RaycastHit2D hitInfo = Physics2D.Raycast(rb.position, lastInteractDir, detectionDistance, objectLayerMask);

        // Optimal
        if (hitInfo.transform != null) { // raycast kena transform dengan collider dan layermask yang telah ditentukan
            // Ngak bekerja sebelumnya karena counter dengan collider tidak memiliki script counter, sehingga tidak terdeteksi
            // Diperbaiki dengan cara memindahkan collider ke GameObject dengan script counter
            if (hitInfo.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            } else if (hitInfo.transform.TryGetComponent(out InteractableObject interactableObject)) { // Object bisa
                if (interactableObject != interactedObject) {
                    SetInteractedObject(interactableObject);
                }
            } else {
                SetSelectedCounter(null);
                SetInteractedObject(null);
            }
        } else {
            SetSelectedCounter(null);
            SetInteractedObject(null);
        }
    }
    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    private void SetInteractedObject(InteractableObject interactedObject) {
        this.interactedObject = interactedObject;

        OnInteractedObjectChanged?.Invoke(this, new OnInteractedObjectChangedArgs {
            interactedObject = interactedObject
        });
    }

    public Transform GetKitchenObjectFollowTransform () {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject (KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject () {
        return kitchenObject;
    }

    public void ClearKitchenObject () {
        kitchenObject = null;
    }

    public bool HasKitchenObject () {
        return kitchenObject != null;
    }

    public void teleportLocationHandler (string tpLocation) {
        switch (tpLocation) {
            case "TopFloorToBottomFloorTP":
                transform.position = TeleportTrigger.Instance.GetBottomFloorTPLocation().transform.position; // Get Tp position
                transform.position += new Vector3(0, -1f, 0); // Move player a bit to the right so wont stuck on tp
                gameCamera.position = new Vector3(0, -13f, -10f); // Move Camera to BottomFloor
                break;
            case "TopFloorToMitchBedRoomTP":
                transform.position = TeleportTrigger.Instance.GetMitchBedRoomFloorTP().transform.position;
                transform.position += new Vector3(0, -1f, 0);// Move player a bit to the bottom so wont stuck on tp
                gameCamera.position = new Vector3(-17f, 0, -10f); // Move Camera to MitchBedroom
                break;
            case "TopFloorToBathroomTP":
                transform.position = TeleportTrigger.Instance.GetBathroomTP().transform.position;
                transform.position += new Vector3(1f, 0, 0);// Move player a bit to the right so wont stuck on tp
                gameCamera.position = new Vector3(17f, 0, -10f); // Move Camera to Bathroom
                break;
            case "BottomFloorToTopFloorTP":
                transform.position = TeleportTrigger.Instance.GetTopTPlocation().transform.position; // Get Tp position
                transform.position += new Vector3(0, 1.2f, 0);// Move player a bit to the top so wont stuck on tp
                gameCamera.position = new Vector3(0, 0, -10f); // Move Camera to Top floor
                break;
            case "BottomFloorToBasementTP":
                transform.position = TeleportTrigger.Instance.GetBasementFloorTP().transform.position; // Get Tp position
                transform.position += new Vector3(0, -1f, 0);// Move player a bit to the bottom so wont stuck on tp
                gameCamera.position = new Vector3(-17f, -13f, -10f); // Move Camera to Basement
                break;
            case "BasementFloorToBottomFloorTP":
                transform.position = TeleportTrigger.Instance.GetBasementBottomFloorTPLocation().transform.position; // Get Tp position
                transform.position += new Vector3(1f, 0, 0);// Move player a bit to the right so wont stuck on tp
                gameCamera.position = new Vector3(0, -13f, -10f); // Move Camera to BottomFloor
                break;
            case "MitchBedRoomToTopFloorTP":
                transform.position = TeleportTrigger.Instance.GetMitchTopFloorTP().transform.position; // Get Tp position
                transform.position += new Vector3(0, 1f, 0);// Move player a bit to the top so wont stuck on tp
                gameCamera.position = new Vector3(0, 0f, -10f); // Move Camera to TopFloor
                break;
            case "BathroomToTopFloorTP":
                transform.position = TeleportTrigger.Instance.GetBathroomTopFloorTP().transform.position; // Get Tp position
                transform.position += new Vector3(-1f, 0, 0);// Move player a bit to the left so wont stuck on tp
                gameCamera.position = new Vector3(0, 0, -10f); // Move Camera to BottomFloor
                break;
        }
/*        if (tpLocation == "TopFloorToBottomFloorTP") {
            transform.position = TeleportTrigger.Instance.GetBottomFloorTPLocation().transform.position; // Get Tp position
            transform.position += new Vector3(0, -1f, 0); // Move player a bit to the right so wont stuck on tp
            gameCamera.position = new Vector3(0, -13f, -10f); // Move Camera to BottomFloor
        } else if (tpLocation == "TopFloorToMitchBedRoomTP") {
            transform.position = TeleportTrigger.Instance.GetMitchBedRoomFloorTP().transform.position;
            transform.position += new Vector3(0, -1f, 0);// Move player a bit to the bottom so wont stuck on tp
            gameCamera.position = new Vector3(-17f, 0, -10f); // Move Camera to MitchBedroom
        } else if (tpLocation == "TopFloorToBathroomTP") {
            transform.position = TeleportTrigger.Instance.GetBathroomTP().transform.position;
            transform.position += new Vector3(1f, 0, 0);// Move player a bit to the right so wont stuck on tp
            gameCamera.position = new Vector3(17f, 0, -10f); // Move Camera to Bathroom
        } else if (tpLocation == "BottomFloorToTopFloorTP") {
            transform.position = TeleportTrigger.Instance.GetTopTPlocation().transform.position; // Get Tp position
            transform.position += new Vector3(0, 1.2f, 0);// Move player a bit to the top so wont stuck on tp
            gameCamera.position = new Vector3(0, 0, -10f); // Move Camera to Top floor
        } else if (tpLocation == "BottomFloorToBasementTP") {
            transform.position = TeleportTrigger.Instance.GetBasementFloorTP().transform.position; // Get Tp position
            transform.position += new Vector3(0, -1f, 0);// Move player a bit to the bottom so wont stuck on tp
            gameCamera.position = new Vector3(-17f, -13f, -10f); // Move Camera to Basement
        } else if (tpLocation == "BasementFloorToBottomFloorTP") {
            transform.position = TeleportTrigger.Instance.GetBasementBottomFloorTPLocation().transform.position; // Get Tp position
            transform.position += new Vector3(1f, 0, 0);// Move player a bit to the right so wont stuck on tp
            gameCamera.position = new Vector3(0, -13f, -10f); // Move Camera to BottomFloor
        } else if (tpLocation == "MitchBedRoomToTopFloorTP") {
            transform.position = TeleportTrigger.Instance.GetMitchTopFloorTP().transform.position; // Get Tp position
            transform.position += new Vector3(0, 1f, 0);// Move player a bit to the top so wont stuck on tp
            gameCamera.position = new Vector3(0, 0f, -10f); // Move Camera to TopFloor
        } else if (tpLocation == "BathroomToTopFloorTP") {
            transform.position = TeleportTrigger.Instance.GetBathroomTopFloorTP().transform.position; // Get Tp position
            transform.position += new Vector3(-1f, 0, 0);// Move player a bit to the left so wont stuck on tp
            gameCamera.position = new Vector3(0, 0, -10f); // Move Camera to BottomFloor
        }*/
        ResetCharacterVisualPosition();
    }

    public void ResetCharacterVisualPosition () {
        playerVisual.transform.localPosition = Vector3.zero; // Make sure playerVisual transform position is at the right place
        kitchenObjectHoldPoint.transform.localPosition = new Vector3(0.46f, -0.12f, 0); // Make sure kitchenHoldPoint transform position is at the right place
    }

    public void SetCharacterPosition (Vector2 newPos) {
        transform.position = newPos;
    }
}

    /*    private void FlipTransform () {
            Vector3 localScale = gameObject.transform.localScale;
            localScale.x *= -1;
            gameObject.transform.localScale = localScale;

            isFacingRight = !isFacingRight;
        }*/

