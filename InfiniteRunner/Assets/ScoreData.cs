using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreData
{
    private float score;

    public ScoreData()
    {
        score = 0f;
    }

    public float Tick(Vector3 obstacleVelocity)
    {
        score += -obstacleVelocity.x * Time.deltaTime;
        if (score > PlayerPrefs.GetFloat("highscore", 0f))
        {
            PlayerPrefs.SetFloat("highscore", score);
        }
        Debug.Log("highscore: " + PlayerPrefs.GetFloat("highscore", 0));
        Debug.Log("score: " + score);

        return score;
    }
}
