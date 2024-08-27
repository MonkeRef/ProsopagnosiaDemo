using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grandpa : MonoBehaviour, ICharacter {

    public static Grandpa Instance { get; private set; }

    [SerializeField] private GameObject grandpaVisual;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        transform.position = new Vector2(17f, -23f);
    }

    public void ResetCharacterVisualPosition () {
        grandpaVisual.transform.position = Vector3.zero;
    }

    public void SetCharacterPosition (Vector2 newPos) {
        transform.position = newPos;
    }

}
