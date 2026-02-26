using UnityEngine;
using TMPro;

public class SurvivalTimer : MonoBehaviour
{
    public TMP_Text timerText;
    private float elapsedTime;
    private bool isPlayerAlive = true;

    void Update()
    {
        // Only count time if the game is running (Time.timeScale > 0) 
        // and the player hasn't died yet
        if (isPlayerAlive && Time.timeScale > 0)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopTimer()
    {
        isPlayerAlive = false;
    }
}