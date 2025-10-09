using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class VersionChecker : MonoBehaviour
{
    [Header("Game Version Settings")]
    public string currentVersion = "Beta 1.0"; // versi lokal game kamu
    public string versionFileURL = "https://yourwebsite.com/version.txt";

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
            string latestVersion = request.downloadHandler.text.Trim();

            if (latestVersion != currentVersion)
            {
                Debug.Log($"Update tersedia! Versi terbaru: {latestVersion}");
                if (updatePanel != null)
                {
                    updatePanel.SetActive(true);
                    updateText.text = $"Versi kamu: {currentVersion}\nVersi terbaru: {latestVersion}\n\nSilakan perbarui game kamu!";
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
}
