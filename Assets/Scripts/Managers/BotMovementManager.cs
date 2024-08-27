using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BotMovementManager : MonoBehaviour {
    // Any NPC related movement

    public static BotMovementManager Instance { get; private set; }

    [SerializeField] private GameObject motherNPC;
    [SerializeField] private Transform customerSpawnPoint;
    [SerializeField] private GameObject customerSpriteVisual;
    [SerializeField] private Transform minigameFloorWayPoints;

    public List<GameObject> bottomFloorMomWaypoints;

    private GameObject customerSpawnGameObject;

    private bool motherShouldBeMoving = true;
    private bool customerStartMovingIn = false;
    private bool customerStartMovingOut = false;
    private bool isCustomerDestroyed = false;

    private float movementSpeed = 2;

    private int momIndex = 0;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        MinigameManager.Instance.OnCustomerSpawn += MinigameManager_OnCustomerSpawn;
        // + Event from delivery UI, when tea is delivered to customer
    } 

    private void MinigameManager_OnCustomerSpawn (object sender, System.EventArgs e) {
        CustomerManager();
    }


    // Customer After spawn, checkfor avaibility
    // MoveTowards that is true for avaibility
    // That waypoint has a collision
    // IF collided with customer
    // That waypoint avaibility, becomes false

    private void Update () {
        MotherMovementHandler();

        CustomerMovementHandler();
    }

    private void MotherMovementHandler () {
        if (motherShouldBeMoving) { // Check whether mother should be moving or not
            Vector2 destination = bottomFloorMomWaypoints[momIndex].transform.position;
            Vector2 newPos = Vector2.MoveTowards(motherNPC.transform.position, destination, movementSpeed * Time.deltaTime);
            motherNPC.transform.position = newPos;

            float distance = Vector2.Distance(motherNPC.transform.position, destination);
            if (distance < 0.05f) {
                StartCoroutine(MotherStayingOnSpot());
            }
        }
    }

    private void CustomerMovementHandler () {
        if (customerStartMovingIn) { // Customer start moving into the waypoint
            Vector2 destination = minigameFloorWayPoints.position;
            Vector2 newPos = Vector2.MoveTowards(customerSpawnGameObject.transform.position, destination, movementSpeed * Time.deltaTime); // This working 
            customerSpawnGameObject.transform.position = newPos;

            float distance = Vector2.Distance(customerSpriteVisual.transform.position, destination);
            if (distance < 0.001f) {
                customerStartMovingIn = false; // Stop moving if alrd on waypoint
            }
        } 
        if (customerStartMovingOut) { // Customer start moving out from the waypoint
            customerStartMovingOut = false;
            // Need to find how to destroy customer properly
            MinigameManager.Instance.SetIsThereCustomer(false);
        }
    }
    private void CustomerManager () {
        // Should not instantiate, until customer destroyed
        customerSpawnGameObject = Instantiate(customerSpriteVisual, customerSpawnPoint.position, customerSpawnPoint.rotation);
        isCustomerDestroyed = false;
        customerStartMovingIn = true;
    }

    IEnumerator MotherStayingOnSpot () {
        motherShouldBeMoving = false;
        yield return new WaitForSeconds(5);
        momIndex = Random.Range(0, 3);
        motherShouldBeMoving = true;
    }

    public void SetCustomerStartMovingIn(bool customerStartMovingIn) {
        this.customerStartMovingIn = customerStartMovingIn;
    }

    public void SetMotherhouldBeMoving (bool shouldBeMoving) {
        this.motherShouldBeMoving = shouldBeMoving;
    }

    public void SetCustomerStartsMovingOut (bool customerStartMovingOut) {
        this.customerStartMovingOut = customerStartMovingOut;
    }
}
