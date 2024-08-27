using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CuttingCounter;

public class MixerCounter : BaseCounter, IHasProgress {

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    public enum State {
        Idle,
        Mixing,
        Mixed
    }

    [SerializeField] private MixingRecipeSO[] mixingRecipeSOArray;

    private State state;
    private float mixingTimer;
    private MixingRecipeSO mixingRecipeSO;

    private void Start () {
        state = State.Idle;
    }

    private void Update () {
        if (HasKitchenObject()) {
            switch (state) {
                case State.Idle:
                    break;
                case State.Mixing:
                    mixingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = mixingTimer / mixingRecipeSO.mixingProgressMax
                    });

                    if (mixingTimer > mixingRecipeSO.mixingProgressMax) {
                        // Mixed

                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(mixingRecipeSO.output, this);

                        state = State.Mixed;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Mixed:
                    break;
            }
        }
    }

    public override void Interact (Player player) {
        if (!HasKitchenObject()) {
            // There is no KitchenObject here
            if (player.HasKitchenObject()) {
                // Player is carrying something
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectsSO())) {
                    // Player carrying something that can be Mixed
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    mixingRecipeSO = GetMixingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectsSO());

                    state = State.Mixing;
                    mixingTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = mixingTimer / mixingRecipeSO.mixingProgressMax
                    });
                }
            } else {
                // Player not carrying anything
            }
        } else {
            // There is kitchenObject
            if (player.HasKitchenObject()) {
                // Player is carrying something
            } else {
                GetKitchenObject().SetKitchenObjectParent(player);
                // Player is not carrying anything

                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput (KitchenObjectsSO inputKitchenObjectSO) {
        MixingRecipeSO mixingRecipeSO = GetMixingRecipeSOWithInput(inputKitchenObjectSO);
        return mixingRecipeSO != null;
    }

    private KitchenObjectsSO GetOutputForInput (KitchenObjectsSO inputKitchenObjectSO) {
        MixingRecipeSO mixingRecipeSO = GetMixingRecipeSOWithInput(inputKitchenObjectSO);
        if (mixingRecipeSO != null) {
            return mixingRecipeSO.output;
        } else {
            return null;
        }
    }

    private MixingRecipeSO GetMixingRecipeSOWithInput (KitchenObjectsSO inputKitchenObjectSO) {
        foreach (MixingRecipeSO mixingRecipeSO in mixingRecipeSOArray) {
            if (mixingRecipeSO.input == inputKitchenObjectSO) {
                return mixingRecipeSO;
            }
        }
        return null;
    }

}
