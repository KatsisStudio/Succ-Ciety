using UnityEngine;
using UnityEngine.SceneManagement;

namespace LewdieJam.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public void PlayGame()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void LoadGallery()
        {
            SceneManager.LoadScene("Gallery");
        }
    }
}
