using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class EndingTimeline : MonoBehaviour
{
    private float directorTimer;

    [SerializeField] private PlayableDirector endingTimeline;

    private void Start () {
        endingTimeline.Play();
        directorTimer = (float) endingTimeline.duration;
    }

    private void Update () {
        directorTimer -= Time.deltaTime;
        if (directorTimer <= 0f) {
            endingTimeline.Stop();
            Loader.Load(Loader.Scene.MainMenuScene);
        }
    }
}
