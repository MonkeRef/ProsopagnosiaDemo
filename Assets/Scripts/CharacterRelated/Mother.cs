using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mother : MonoBehaviour, ICharacter
{
    public static Mother Instance {  get; private set; }

    [SerializeField] private GameObject motherVisual;

    private void Awake () {
        Instance = this;
    }

    private void Start () {
        transform.position = new Vector2(0f, -12f);
    }
    public void ResetCharacterVisualPosition () {
        motherVisual.transform.position = Vector3.zero;
    }

    public void SetCharacterPosition (Vector2 newPos) {
        transform.position = newPos;
    }
}
