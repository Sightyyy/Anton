using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneTransition : MonoBehaviour
{
    AudioCollection audioCollection;
    Animator animator;
    string targetScene;

    void Awake()
    {
        audioCollection = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioCollection>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        audioCollection.PlayBGM(audioCollection.cutscene);
        animator.SetTrigger("Play");
    }

    void TransitionTo(string sceneName)
    {
        targetScene = sceneName;
        OnTransitionComplete();
    }

    public void OnTransitionComplete()
    {
        SceneManager.LoadScene(targetScene);
    }

    public void GoToPlains()   => TransitionTo("Plains");
    public void GoToCave()     => TransitionTo("Cave");
    public void GoToDungeon()  => TransitionTo("Dungeon");
    public void BackToMainMenu() => TransitionTo("MainMenu");
}
