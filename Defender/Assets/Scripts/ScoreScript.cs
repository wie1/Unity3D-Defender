using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScript : MonoBehaviour
{

    public Text headerText;
    public Text detailText;

    public void ShowScore(int currentLevelScore, int totalScore, int enemiesDestroyed, int astronautsSaved, bool bossKilled)
    {
        headerText.text = "Total score: " + totalScore + "\nThis level: " + currentLevelScore;
        if (enemiesDestroyed > 0)
        {
            detailText.text += "Enemies destroyed: " + enemiesDestroyed + " x 100 \n";
        }
        if (astronautsSaved > 0)
        {
            detailText.text += "Astronauts saved: " + astronautsSaved + " x 200 \n";
        }
        if (bossKilled)
        {
            detailText.text += "Boss: 800";
        }
    }

    public void GameOverScreen(int totalScore)
    {
        headerText.text = "Game Over\nTotal score: " + totalScore;
        detailText.text = "Press space to restart game";
    }
}
