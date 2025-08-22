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
    private PlayerBehavior playerBehavior;

    private void Awake()
    {
        audioCollection = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioCollection>();
        playerBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehavior>();
    }


    private void Start()
    {
        transitionImage.gameObject.SetActive(true);
        StartCoroutine(TransitionOpening());
    }

    private void Update()
    {
        OnPlayerDead();

        if (playerBehavior != null) return;

        if (Input.GetKeyDown(KeyCode.Escape) && playerBehavior.isDead!)
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

    private void OnPlayerDead()
    {
        if (gameOverPanel != null && playerBehavior.isDead)
        {
            StartCoroutine(NormalTransition());
            gameOverPanel.SetActive(true);
            audioCollection.PlayBGM(audioCollection.gameOver);
            Time.timeScale = 0f;
        }
    }

    public void ReturnToMainMenu(string sceneName)
    {
        Time.timeScale = 1f;
        StartCoroutine(TransitionToScene(sceneName));
    }

    public void Restart(string sceneName)
    {
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
