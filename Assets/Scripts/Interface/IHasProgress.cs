using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasProgress {
    public event EventHandler<OnProgressChangedEventArgs> OnProgressChanged;
    public class OnProgressChangedEventArgs : EventArgs { // Prolly wont be used, since use fill over time
        public float progressNormalized;
    }

}
