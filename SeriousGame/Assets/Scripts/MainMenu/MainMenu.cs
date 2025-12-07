using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void LoadMazeGame()
   {
      SceneManager.LoadScene("Maze");
   }

   public void LoadRhythmGame()
   {
      SceneManager.LoadScene("Rhythm");
   }

   public void QuitGame()
   {
      Application.Quit();
   }
}
