using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleporterHandler : MonoBehaviour {

    public static PlayerTeleporterHandler Instance { get; private set; }

    private const string IS_TELEPORTING = "IsTeleporting";
    private const string IS_DONE_TELEPORTING = "IsDoneTeleporting";

    [SerializeField] private Animator fadeTransition;

    private string tpLocation;

    private void Awake () {
        Instance = this;
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        tpLocation = collision.gameObject.name;
        if (collision.CompareTag("Teleporter")) { 
            StartCoroutine(FadeTransitionWhileTeleporting(tpLocation));
        }
    }

    IEnumerator FadeTransitionWhileTeleporting (string tpLocation) {
        GameInput.Instance.DisablePlayerMovementAndInteract();
        fadeTransition.SetTrigger(IS_TELEPORTING);
        yield return new WaitForSeconds(1);
        Player.Instance.teleportLocationHandler(tpLocation);
        fadeTransition.SetTrigger(IS_DONE_TELEPORTING);
        GameInput.Instance.EnablePlayerMovementAndInteract();
    }

}
