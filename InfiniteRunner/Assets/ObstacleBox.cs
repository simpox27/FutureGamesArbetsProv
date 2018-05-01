using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBox
{

    private GameObject[] obstacleBoxArray;
    private GameObject playerModell;
    private float previousThickness = 1;
    private GameObject lastObstacle;
    float maxDistance = 7;
    float nextDistance;
    GameObject playerEyeTarget;



    public ObstacleBox(GameObject[] obstacleBoxArray, GameObject playerModell)
    {
        nextDistance = 1 + Random.value * maxDistance;

        this.obstacleBoxArray = obstacleBoxArray;
        this.playerModell = playerModell;
        for (int i = 0; i < 6; i++)
        {
            obstacleBoxArray[i].transform.position = new Vector3(i * 3f, 0f, 0f);
            lastObstacle = obstacleBoxArray[i];
        }
    }

    public void Spawning(float probabilityForBox, Vector3 obstacleVelocity, float weakGravity, float strongGravity, float jumpSpeed)
    {
        float obstacleHeightMax = lastObstacle.transform.position.y + jumpSpeed * nextDistance / -obstacleVelocity.x + 0.5f * weakGravity * Mathf.Pow(nextDistance / -obstacleVelocity.x, 2);
        float obstacleHeightMin = lastObstacle.transform.position.y + 0.5f * strongGravity * Mathf.Pow(nextDistance / -obstacleVelocity.x, 2);
        if (obstacleHeightMin < lastObstacle.transform.position.y - 5)
        {
            obstacleHeightMin = lastObstacle.transform.position.y - 5;
        }

        if (lastObstacle.transform.position.x < (20f - nextDistance))
        {
            foreach (GameObject obstacle in obstacleBoxArray)
            {
                if (obstacle.transform.position.x < -10)
                {
                    obstacle.transform.position = new Vector3(20f, obstacleHeightMin + (obstacleHeightMax - obstacleHeightMin) * Random.value, 0f);
                    lastObstacle = obstacle;
                    nextDistance = 1 + Random.value * maxDistance;
                    break;
                }
            }
        }
    }

    public void Movement(Vector3 obstacleVelocity)
    {
        foreach (GameObject obstacle in obstacleBoxArray)
        {
            obstacle.transform.position += obstacleVelocity * Time.deltaTime;
        }
    }

    public CollisionOutcome Collision(float thickness, Vector3 obstacleVelocity, Vector3 velocity)
    {
        float playerModelY = playerModell.transform.position.y;

        CollisionOutcome outcome = new CollisionOutcome(0, 0, 0);

        foreach (GameObject obstacle in obstacleBoxArray)
        {
            float thicknessCombined = playerModell.transform.localScale.y / 2 + obstacle.transform.localScale.y / 2;
            Vector3 obstaclePosition = obstacle.transform.position;

            if (obstaclePosition.x < 1
                && obstaclePosition.x > -1
                && obstaclePosition.y < playerModelY + thicknessCombined
                && obstaclePosition.y > playerModelY - thicknessCombined)
            {
                //Debug.Log("collision");
                if (thickness > previousThickness)
                {
                    float vRatio = -velocity.y / obstacleVelocity.x;
                    float dRatio = ((playerModelY - 0.25f) - (obstaclePosition.y + obstacle.transform.localScale.y / 2f))
                    / ((playerModell.transform.position.x + playerModell.transform.localScale.x / 2f) - (obstaclePosition.x - obstacle.transform.localScale.x / 2f));
                    // Debug.Log("vRatio: " + vRatio + "    dRatio: " + dRatio);

                    if (vRatio <= dRatio)
                    {
                        outcome = new CollisionOutcome(obstaclePosition.y, playerModelY - obstaclePosition.y - obstacle.transform.localScale.y / 2f - 0.25f, 2);
                    }
                    else
                    {
                        outcome = new CollisionOutcome(obstaclePosition.y, 0, -1);
                    }
                }
                else
                {
                    float vRatio = -velocity.y / obstacleVelocity.x;
                    float dRatio = ((playerModelY - playerModell.transform.localScale.y / 2f) - (obstaclePosition.y + obstacle.transform.localScale.y / 2f))
                    / ((playerModell.transform.position.x + playerModell.transform.localScale.x / 2f) - (obstaclePosition.x - obstacle.transform.localScale.x / 2f));




                    /* Debug.Log("playerY: " + playerModelY + "    playerScaleY: " + playerModell.transform.localScale.y / 2 + "    obstacleY: " + obstaclePosition.y
                         + "    obstacleScaleY: " + obstacle.transform.localScale.y / 2 + "    playerX: " + playerModell.transform.position.x + "    playerScaleX: "
                         + playerModell.transform.localScale.x / 2 + "    obstacleX: " + obstaclePosition.x + "    obstacleScaleX: " + obstacle.transform.localScale.x / 2);
                     Debug.Log("distanceY: " + ((playerModelY - playerModell.transform.localScale.y / 2) - (obstaclePosition.y + obstacle.transform.localScale.y / 2)));
                     Debug.Log("distanceX: " + ((playerModell.transform.position.x + playerModell.transform.localScale.x / 2) - (obstaclePosition.x - obstacle.transform.localScale.x / 2)));
                     */
                    //  Debug.Log("no jump:   vRatio: " + vRatio + "    dRatio: " + dRatio);

                    if (vRatio <= dRatio)
                    {
                        outcome = new CollisionOutcome(obstaclePosition.y, playerModelY - obstaclePosition.y - obstacle.transform.localScale.y / 2f - 0.25f, 1);
                    }
                    else
                    {
                        outcome = new CollisionOutcome(obstaclePosition.y, 0, -1);
                    }
                }

            }
        }
        previousThickness = thickness;
        return outcome;
    }
    public void EyeTracking(Rigidbody2D[] obstacleEyeArray, Rigidbody2D playerEyeLeft, Rigidbody2D playerEyeRight)
    {
        foreach (Rigidbody2D eye in obstacleEyeArray)
        {
            Vector2 forceDirection = new Vector2(-eye.transform.position.x, playerModell.transform.position.y - eye.transform.position.y);
            eye.AddForce(forceDirection.normalized * 2, ForceMode2D.Force);
        }
        playerEyeTarget = null;
        foreach (GameObject obstacle in obstacleBoxArray)
        {
            if((obstacle.transform.position.y - playerModell.transform.position.y)/ (obstacle.transform.position.x + 3) < 1 && (obstacle.transform.position.y - playerModell.transform.position.y)/ (obstacle.transform.position.x + 3) > -1 && obstacle.transform.position.x>-2)
            {
                if(playerEyeTarget == null)
                {
                    playerEyeTarget = obstacle;
                }
                else if(obstacle.transform.position.x < playerEyeTarget.transform.position.x)
                {
                    playerEyeTarget = obstacle;
                }
            }
        }
        Vector2 leftForceDirection = new Vector2(playerEyeTarget.transform.position.x- playerEyeLeft.transform.position.x, playerEyeTarget.transform.position.y - playerEyeLeft.transform.position.y);
        Vector2 rightForceDirection = new Vector2(playerEyeTarget.transform.position.x - playerEyeRight.transform.position.x, playerEyeTarget.transform.position.y - playerEyeRight.transform.position.y);
        playerEyeLeft.AddForce(leftForceDirection.normalized * 2, ForceMode2D.Force);
        playerEyeRight.AddForce(rightForceDirection.normalized * 2, ForceMode2D.Force);
    }
}
