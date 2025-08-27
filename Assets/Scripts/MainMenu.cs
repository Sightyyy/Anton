using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Image transitionImage;
    [SerializeField] private List<Button> levelButtons;
    [SerializeField] private List<GameObject> lockedImages;
    [SerializeField] private List<string> levelSceneNames = new List<string> { "Plains", "Night Woods", "Volcano Hills" };
    private float transitionTime = 1.0f;

    private AudioCollection audioCollection;
    private Animator menuAnimator;

    private void Awake()
    {
        audioCollection = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioCollection>();

        GameObject mainMenuObject = GameObject.Find("Main Menu");
        if (mainMenuObject == null)
        {
            Debug.LogError("Main Menu object not found in the scene!");
        }
        else
        {
            menuAnimator = mainMenuObject.GetComponent<Animator>();
            if (menuAnimator == null)
                Debug.LogError("Animator component not found on Main Menu object!");
        }

        //InitializeLevelButtons();
    }

    private void Start()
    {
        audioCollection.PlayBGM(audioCollection.mainMenu);
        InitializeLevelButtons();
    }

    private void InitializeLevelButtons()
    {
        if (levelButtons.Count != levelSceneNames.Count || lockedImages.Count != levelSceneNames.Count)
        {
            Debug.LogError("Level buttons, locked images, and scene names counts don't match!");
            return;
        }

        Debug.Log($"Levels Completed: {DataManager.Instance.LevelsCompleted}");

        int completedLevels = DataManager.Instance.LevelsCompleted;

        for (int i = 0; i < levelButtons.Count; i++)
        {
            bool levelUnlocked = (i <= completedLevels);

            levelButtons[i].interactable = levelUnlocked;

            lockedImages[i].SetActive(!levelUnlocked);

            string sceneName = levelSceneNames[i];
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => PlayGame(sceneName));
        }
    }

    public void PlayGame(string sceneName)
    {
        StartCoroutine(TransitionToScene(sceneName));
    }

    public void QuitGame()
    {
        StartCoroutine(TransitionAndQuit());
    }

    // --- Animator Trigger Functions ---

    public void TriggerToLevel()
    {
        TriggerAnimator("To Level");
    }

    public void TriggerFromLevel()
    {
        TriggerAnimator("From Level");
    }

    public void TriggerToSettings()
    {
        TriggerAnimator("To Settings");
    }

    public void TriggerFromSettings()
    {
        TriggerAnimator("From Settings");
    }

    private void TriggerAnimator(string triggerName)
    {
        if (menuAnimator != null)
        {
            menuAnimator.SetTrigger(triggerName);
        }
        else
        {
            Debug.LogWarning("Menu Animator is not assigned.");
        }
    }

    // --- Scene & Transition ---

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(FadeFromBlack());
    }

    private IEnumerator TransitionAndQuit()
    {
        yield return StartCoroutine(FadeToBlack());
        Application.Quit();
    }

    private IEnumerator FadeToBlack()
    {
        transitionImage.gameObject.SetActive(true);

        for (float t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(0, 1, t / transitionTime);
            SetImageAlpha(alpha);
            yield return null;
        }

        SetImageAlpha(1);
    }

    private IEnumerator FadeFromBlack()
    {
        for (float t = 0.0f; t < transitionTime; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(1, 0, t / transitionTime);
            SetImageAlpha(alpha);
            yield return null;
        }

        SetImageAlpha(0);
        transitionImage.gameObject.SetActive(false);
    }

    private void SetImageAlpha(float alpha)
    {
        if (transitionImage != null)
        {
            Color color = transitionImage.color;
            color.a = alpha;
            transitionImage.color = color;
        }
        else
        {
            Debug.LogError("Transition Image is not assigned!");
        }
    }
}
