using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
