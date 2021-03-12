using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLevelsProvider : MonoBehaviour, ILevelProvider
{
    [SerializeField] private bool activateNewScene;

    private bool is2StageLoadded;
    
    private int currentScene = -1;
    public int LevelsCount => SceneManager.sceneCountInBuildSettings - 1;

    public void LoadLevel(int number) => MenuUI.Instance.BlackScreenFadeIn(() => StartCoroutine(LoadCor(number)));
    public void Reload() => MenuUI.Instance.BlackScreenFadeIn(() => StartCoroutine(ReloadCor()));

    private void Start()
    {
        is2StageLoadded = false;
        GameC.Instance.Load2Stage += OnLoad2Stage;
    }

    private void OnDestroy()
    {
        GameC.Instance.Load2Stage -= OnLoad2Stage;
    }

    private void OnLoad2Stage()
    {
        StartCoroutine(Load2Stage());
    }

    private IEnumerator Load2Stage()
    {

        is2StageLoadded = true;

        
        var loadAO = SceneManager.LoadSceneAsync(currentScene + 1, LoadSceneMode.Additive);
        yield return loadAO;
        

        MenuUI.Instance.restartButton.interactable = true;

    }

    private IEnumerator LoadCor(int number)
    {
        yield return StartCoroutine(Unload());
        currentScene = number + 1;
        yield return StartCoroutine(Load());

        MenuUI.Instance.BlackScreenFadeOut();
    }

    private IEnumerator ReloadCor()
    {
        yield return StartCoroutine(Unload());
        yield return StartCoroutine(Load());

        MenuUI.Instance.BlackScreenFadeOut();
    }

    private IEnumerator Unload()
    {
        if (currentScene > 0)
        {
            var unloadAO = SceneManager.UnloadSceneAsync(currentScene);
            yield return unloadAO;
            if (is2StageLoadded)
            {
                is2StageLoadded = false;
                var unload = SceneManager.UnloadSceneAsync(currentScene + 1);
                yield return unload;
            }
            
        }
    }

    private IEnumerator Load()
    {

        var loadAO = SceneManager.LoadSceneAsync(currentScene, LoadSceneMode.Additive);
        yield return loadAO;
        
        if (activateNewScene)
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));

        MenuUI.Instance.restartButton.interactable = true;
    }
}