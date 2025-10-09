using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class VersionChecker : MonoBehaviour
{
    [Header("Game Version Settings")]
    public string currentVersion = "Beta 1.0.1";
    private string versionFileURL = "https://raw.githubusercontent.com/Sightyyy/Anton/refs/heads/main/version.txt";

    [Header("UI References")]
    public GameObject updatePanel;
    public TMP_Text updateText;

    private void Start()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            StartCoroutine(CheckForUpdate());
        }
        else
        {
            Debug.Log("Offline – skip pengecekan update");
        }
    }

    private IEnumerator CheckForUpdate()
    {
        UnityWebRequest request = UnityWebRequest.Get(versionFileURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string latestVersion = Regex.Replace(request.downloadHandler.text, "<.*?>", "").Trim();
            Debug.Log("Response dari server: " + request.downloadHandler.text);
            if (latestVersion != currentVersion)
            {
                Debug.Log($"Update tersedia! Versi terbaru: {latestVersion}");

                if (updatePanel != null && updateText != null)
                {
                    updatePanel.SetActive(true);
                    updateText.text =
                        "Update Available!\n" +
                        "Please update the game!";
                }
            }
            else
            {
                Debug.Log("Game sudah versi terbaru.");
            }
        }
        else
        {
            Debug.LogWarning("Gagal memeriksa versi: " + request.error);
        }
    }
    public void OpenDownloadPage()
    {
        Application.OpenURL("https://sightyy.itch.io/anton");
        Application.Quit();
    }

}
