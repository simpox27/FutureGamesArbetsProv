using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    public Vector3 velocity = new Vector3(0.0f, 10.0f, 0.0f);
    float gravity;
    Vector3 deltaPosition;
    public float thickness = 1;
    ObstacleBox obstacleBox;
    private AudioSource jumpAudio;
    private AudioSource landAudio;
    bool playedLastFrame= false;

    public Player(ObstacleBox obstacleBox, AudioSource jumpAudio, AudioSource landAudio)
    {
        this.obstacleBox = obstacleBox;
        this.jumpAudio = jumpAudio;
        this.landAudio = landAudio;
    }

    public float Movement(GameObject playerModel, Vector3 obstacleVelocity, float weakGravity, float strongGravity, float jumpSpeed)
    {
        if (Input.GetKey(KeyCode.DownArrow))
        {
            gravity = strongGravity;
            thickness = 0.5f;
        }
        else
        {
            gravity = weakGravity;
            thickness = 1f;
        }
        playerModel.transform.localScale = new Vector3(1, thickness);

        velocity += new Vector3(0.0f, gravity * Time.deltaTime, 0.0f);

        deltaPosition = velocity * Time.deltaTime;

        playerModel.transform.position += deltaPosition;

        CollisionOutcome collisionOutcome = obstacleBox.Collision(thickness, obstacleVelocity, velocity);

        bool landMemory = false;

        switch (collisionOutcome.outcome)
        {
            case -1:
                Debug.Log("player fail");
                break;
            case 0:
                Debug.Log("player no touch");
                break;
            case 1:
                Debug.Log("player touchdown");
                playerModel.transform.position = new Vector3(0f, collisionOutcome.obstacleHeight + 0.5f + thickness / 2f, 0f);
                velocity = new Vector3(0.0f, 0.0f, 0.0f);
                if(playedLastFrame == false)
                {
                    landAudio.Play();
                    playedLastFrame = true;
                }
                landMemory = true;
                
                break;
            case 2:
                Debug.Log("player jump");
                playerModel.transform.position = new Vector3(0f, collisionOutcome.obstacleHeight +0.5f+ thickness / 2f, 0f);
                if (jumpSpeed - collisionOutcome.previousHeaight * 2f * jumpSpeed > jumpSpeed)
                {
                    velocity = new Vector3(0.0f, jumpSpeed, 0f);
                }
                else
                {
                    velocity = new Vector3(0.0f, jumpSpeed - collisionOutcome.previousHeaight * 2f * jumpSpeed, 0f);
                }
                jumpAudio.Play();

                if (playedLastFrame == false)
                {
                    landAudio.Play();
                }
                break;
            default:
                Debug.Log("player no touch");
                break;

        }
        if (landMemory == false)
        {
            playedLastFrame = false;
        }

        return playerModel.transform.position.y;
    }
    
    //void Movement()
    //{
    //    if (transform.position.y > floorHeight)
    //    {
    //        velocity += new Vector3(0.0f, gravity * Time.deltaTime, 0.0f);
    //    }

    //    transform.position += velocity * Time.deltaTime;

    //    if (transform.position.y <= floorHeight)
    //    {
    //        transform.position = new Vector3(0.0f, floorHeight, 0.0f);
    //        velocity.y = -velocity.y;
    //    }
    //}
}