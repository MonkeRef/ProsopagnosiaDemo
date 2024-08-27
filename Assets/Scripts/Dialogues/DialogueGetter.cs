using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueGetter : MonoBehaviour{

    public static DialogueGetter Instance { get; private set; }

    public string[] names;

    [TextArea(3, 10)]
    public string[] mitchDialogues;
    [TextArea(3, 10)]
    public string[] oppositeDialogues;

    private void Awake () {
        Instance = this;
    }

    public string[] GetNames () {
        return names;
    }

    public string[] GetIntercolutorDialogues () {
        return mitchDialogues;
    }

    public string[] GetSecondIntercolutorDialogues () {
        return oppositeDialogues;
    }
}
