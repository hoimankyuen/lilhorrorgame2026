using System.Collections;
using MoonlightTools.GeneralTools;
using MoonlightTools.UIToolkit;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviourPersistentSingleton<LevelManager>
{
    [Header("Settings")]
    [SerializeField] private string menuSceneName;
    [SerializeField] private string gameSceneName;
    [SerializeField] private float loadSceneFaderDelay;

    private bool loadingScene = false;

    // ======== Functionalities ========

    public void GotoMainMenu()
    {
        LoadScene(menuSceneName);
    }

    public void GotoGame()
    {
        LoadScene(gameSceneName);
    }
    
    private void LoadScene(string sceneName)
    {
        if (loadingScene)
            return;

        loadingScene = true;

        LoadingFader.Instance.Show(() =>
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            StartCoroutine(DelayedHideFader());
        });
    }
    
    private IEnumerator DelayedHideFader()
    {
        yield return new WaitForSeconds(loadSceneFaderDelay);
        LoadingFader.Instance.Hide(() =>
        {
            loadingScene = false;
        });
    }
}