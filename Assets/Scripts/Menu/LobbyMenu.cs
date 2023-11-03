using UnityEngine;
using UnityEngine.SceneManagement;

namespace LewdieJam.Menu
{
    public class LobbyMenu : MonoBehaviour
    {
        public void LoadGame()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
