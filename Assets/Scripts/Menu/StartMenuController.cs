using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HarvestShore.Menu
{
    public static class GameLaunchOptions
    {
        public static bool StartInGameplay { get; private set; }
        public static int SaveSlotIndex { get; private set; }

        public static void RequestStartInGameplay(int saveSlotIndex)
        {
            StartInGameplay = true;
            SaveSlotIndex = Mathf.Max(0, saveSlotIndex);
        }

        public static bool ConsumeStartInGameplay(out int saveSlotIndex)
        {
            saveSlotIndex = SaveSlotIndex;

            if (!StartInGameplay)
                return false;

            StartInGameplay = false;
            SaveSlotIndex = 0;
            return true;
        }

        public static void Clear()
        {
            StartInGameplay = false;
            SaveSlotIndex = 0;
        }
    }

    public class StartMenuController : MonoBehaviour
    {
        [SerializeField] private string persistentSceneName = "PersistentScene";
        [SerializeField] private int defaultSaveSlotIndex;
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button loadGameButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            BindButton(newGameButton, StartNewGame);
            BindButton(loadGameButton, OpenLoadMenu);
            BindButton(exitButton, ExitGame);
        }

        public void StartNewGame()
        {
            GameLaunchOptions.RequestStartInGameplay(defaultSaveSlotIndex);
            SceneManager.LoadScene(persistentSceneName);
        }

        public void OpenLoadMenu()
        {
            GameLaunchOptions.Clear();
            SceneManager.LoadScene(persistentSceneName);
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        private static void BindButton(Button button, UnityAction action)
        {
            if (button == null)
                return;

            button.onClick.RemoveListener(action);

            if (button.onClick.GetPersistentEventCount() == 0)
                button.onClick.AddListener(action);
        }
    }
}
