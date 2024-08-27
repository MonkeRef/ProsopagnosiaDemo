using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI Instance { get; private set; }

    private bool isOneTimeThing = true;

    private void Awake () {
        gameObject.transform.localScale = Vector3.zero;
        Instance = this;
    }

    public void Show () {
        if(isOneTimeThing) {
            gameObject.transform.localScale = Vector3.one;
            isOneTimeThing = false;
        }
        gameObject.SetActive(true);
    }

    public void Hide () {
        gameObject.SetActive(false);
    }
}
