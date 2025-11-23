using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty { Easy, Hard }

public class MatchSettingsController : MonoBehaviour
{
    public Difficulty difficulty = Difficulty.Easy;
    public byte playerCount = 2;
    public Color chipColor = Color.red;
    
    public void StartMatch()
    {
        SceneManager.LoadScene("Game");
    }
    
    // public void OnDifficultyChanged(int index)
    // {
    //     gameSettings.difficulty = index == 0 ?
    //         GameSettings.Difficulty.Easy :
    //         GameSettings.Difficulty.Hard;
    // }
    //
    // public void OnPlayerCountChanged(int index)
    // {
    //     gameSettings.playerCount = index + 2; // 0->2, 1->3, 2->4
    // }
    //
    // public void OnColorSelected(Color color)
    // {
    //     gameSettings.chipColor = color;
    // }
}
