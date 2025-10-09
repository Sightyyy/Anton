using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    public Image transitionImage;
    public float transitionTime = 1.0f;
    private bool paused = false;
    AudioCollection audioCollection;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject tutorialPanel;
    private PlayerBehavior playerBehavior;
    private Animator playerAnimator;
    private bool gameOverTriggered = false;
    private bool tutorialShown = false;
    public string scene;
    public string world;

    private EnemySpawner enemySpawn;

    private void Awake()
    {
        audioCollection = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioCollection>();
        playerBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Animator>();

        enemySpawn = GetComponent<EnemySpawner>();
        if (enemySpawn == null)
        {
            Debug.LogError("EnemySpawner tidak ditemukan di GameMenu object!");
        }
    }

    private void Start()
    {
        transitionImage.gameObject.SetActive(true);
        if (world == "Plains" && tutorialPanel != null && PlayerPrefs.GetInt("TutorialShown", 0) == 0)
        {
            ShowTutorial();
        }
        else
        {
            StartCoroutine(TransitionOpening());
        }

        if (world == "Plains")
        {
            audioCollection.PlayBGM(audioCollection.plains);
        }
        else if (world == "Cave")
        {
            audioCollection.PlayBGM(audioCollection.cave);
        }
        else if (world == "Dungeon")
        {
            audioCollection.PlayBGM(audioCollection.dungeon);
        }
    }

    private void Update()
    {
        OnPlayerDead();
        CheckWaveComplete();

        if (playerBehavior == null) return;

        if (tutorialShown && tutorialPanel.activeSelf)
        {
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
            {
                HideTutorial();
                return;
            }
        }

        if (!tutorialShown && !playerBehavior.isDead)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button7))
            {
                if (!paused)
                {
                    audioCollection.PlaySFX(audioCollection.buttonClick);
                    Pause();
                }
                else
                {
                    audioCollection.PlaySFX(audioCollection.buttonClick);
                    Resume();
                }
            }
        }
    }

    private void ShowTutorial()
    {
        if (tutorialPanel != null && world == "Plains")
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f;
            tutorialShown = true;

            Image panelImage = tutorialPanel.GetComponent<Image>();
            if (panelImage != null)
            {
                panelImage.raycastTarget = true;
                tutorialPanel.AddComponent<Button>().onClick.AddListener(HideTutorial);
            }
        }
    }

    private void HideTutorial()
    {
        if (tutorialPanel != null)
        {
            transitionImage.gameObject.SetActive(false);
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f;

            tutorialShown = false; // <- tambahin ini

            PlayerPrefs.SetInt("TutorialShown", 1); // ubah ke 1 biar next time nggak muncul lagi
            PlayerPrefs.Save();
        }
    }

    private void OnPlayerDead()
    {
        if (playerBehavior == null || gameOverTriggered) return;

        if (gameOverPanel != null && playerBehavior.isDead)
        {
            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.normalizedTime >= 1.0f)
            {
                gameOverTriggered = true;
                StartCoroutine(GameOverSequence());
            }
        }
    }

    private IEnumerator GameOverSequence()
    {
        yield return StartCoroutine(NormalTransition());
        
        audioCollection.PlayBGM(audioCollection.gameOver);
        Time.timeScale = 0f;
    }

    private void CheckWaveComplete()
    {
        if (enemySpawn == null) return;

        if (enemySpawn.currentWave >= enemySpawn.maxWave && 
            (enemySpawn.enemiesPerWave - enemySpawn.enemiesKilledThisWave) <= 0)
        {
            enemySpawn = null;
            DataManager.Instance.CompleteLevel();
            GoToCutscene(scene);
        }
    }

    public void GoToCutscene(string scene)
    {
        StartCoroutine(TransitionToScene(scene));
    }

    public void ReturnToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        StartCoroutine(TransitionToScene(sceneName));
    }

    public void Restart(string sceneName)
    {
        Time.timeScale = 1f;
        StartCoroutine(TransitionToScene(sceneName));
    }

    private void Pause()
    {
        pausePanel.SetActive(true);
        paused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        pausePanel.SetActive(false);
        paused = false;
        Time.timeScale = 1f;
    }

    private IEnumerator TransitionToScene(string sceneName)
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene(sceneName);
        yield return StartCoroutine(FadeFromBlack());
    }

    private IEnumerator NormalTransition()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(FadeToBlack());
        gameOverPanel.SetActive(true);
        yield return StartCoroutine(FadeFromBlack());
    }

    private IEnumerator TransitionOpening()
    {
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
