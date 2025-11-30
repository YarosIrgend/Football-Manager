using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchSettingsController : MonoBehaviour
{
    public GameSettings GameSettings;

    public void Start()
    {
        GameSettings.Difficulty = Difficulty.Easy;
        GameSettings.PlayerCount = 2;
        GameSettings.ChipColor = Color.red;
    }
    
    public void StartMatch()
    {
        SceneManager.LoadScene("Game");
    }
    
    public void OnDifficultyChanged(int index)
    {
        GameSettings.Difficulty = index == 0 ?
            Difficulty.Easy :
            Difficulty.Hard;
        Debug.Log(GameSettings.Difficulty);
    }
    
    public void OnPlayerCountChanged(int index)
    {
        GameSettings.PlayerCount = (byte)(index + 2); // 0->2, 1->3, 2->4
        Debug.Log(GameSettings.PlayerCount);
    }
    
    public void OnColorSelected(int index)
    {
        GameSettings.ChipColor = index switch
        {
            1 => Color.red,
            2 => Color.blue,
            3 => Color.green,
            4 => Color.yellow,
            _ => Color.white
        };
        Debug.Log(GameSettings.ChipColor);
    }
}
