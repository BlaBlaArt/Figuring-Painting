using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLevelsProvider : MonoBehaviour, ILevelProvider
{
    [SerializeField] private bool activateNewScene;

    private int currentScene = -1;
    public int LevelsCount => SceneManager.sceneCountInBuildSettings - 1;

    public void LoadLevel(int number) => MenuUI.Instance.BlackScreenFadeIn(() => StartCoroutine(LoadCor(number)));
    public void Reload() => MenuUI.Instance.BlackScreenFadeIn(() => StartCoroutine(ReloadCor()));

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