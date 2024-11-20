using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Commands
{
    public class LoadMainScreenCommand : ICommand
    {
        const string MainSceneName = "Main";
        public async UniTask ExecuteAsync()
        {
            SceneManager.LoadSceneAsync(MainSceneName, LoadSceneMode.Single);
            await UniTask.CompletedTask;
        }
    }
}
