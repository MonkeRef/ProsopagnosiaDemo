using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    public static TeleportTrigger Instance { get; private set; }

    // Top Floor TPs
    [SerializeField] private Transform topTP;
    [SerializeField] private Transform mitchRoomTopFloorTP;
    [SerializeField] private Transform bathroomTopFloorTP;

    // Bottom Floor TPs
    [SerializeField] private Transform bottomTP;
    [SerializeField] private Transform bottomFloorBasementTP;

    // ETCs TPs
    [SerializeField] private Transform mitchBedroomTP;
    [SerializeField] private Transform bathRoomTP;
    [SerializeField] private Transform basementTP;

    private void Awake () {
        Instance = this;
    }

    public Transform GetTopTPlocation () {
        return topTP;
    }
    public Transform GetMitchTopFloorTP () {
        return mitchRoomTopFloorTP;
    }
    public Transform GetBathroomTopFloorTP () {
        return bathroomTopFloorTP;
    }

    public Transform GetBottomFloorTPLocation () {
        return bottomTP;
    }
    public Transform GetBasementBottomFloorTPLocation () {
        return bottomFloorBasementTP;
    }

    public Transform GetBasementFloorTP () {
        return basementTP;
    }
    public Transform GetMitchBedRoomFloorTP () {
        return mitchBedroomTP;
    }
    public Transform GetBathroomTP () {
        return bathRoomTP;
    }
}
