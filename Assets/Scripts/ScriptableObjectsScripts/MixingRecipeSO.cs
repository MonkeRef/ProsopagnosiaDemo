using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MixingRecipeSO : ScriptableObject {

    public KitchenObjectsSO input;
    public KitchenObjectsSO output;
    public int mixingProgressMax;
}
