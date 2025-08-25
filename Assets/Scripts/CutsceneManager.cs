using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    AudioCollection audioCollection;
    void Awake()
    {
        audioCollection = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioCollection>();
    }
    void Start()
    {
        audioCollection.PlayBGM(audioCollection.cutscene);
    }
}
