using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement
{
    private float cameraHeight = 0;
    private float cameraSize = 5;
    private float targetCameraSpeed = 0;
    private float targetCameraSizeSpeed = 5;
    private float cameraSpeed = 0;
    private float cameraSizeSpeed = 0;
    private Camera mainCam;
    float lowestPoint = 0;

    public CameraMovement()
    {
        mainCam = Camera.main;
    }

    // Use this for initialization
    public float Position(GameObject[] obstacleBoxArray, GameObject playerModel)
    {
        float highestPoint = playerModel.transform.position.y;

        foreach(GameObject obstacle in obstacleBoxArray)
        {
            if(obstacle.transform.position.x > 0 && obstacle.transform.position.x < 10)
            {
                if (obstacle.transform.position.y > highestPoint)
                {
                    highestPoint = obstacle.transform.position.y;
                }
                else if (obstacle.transform.position.y < lowestPoint)
                {
                    lowestPoint = obstacle.transform.position.y;
                }
            }
        }
        targetCameraSpeed= (highestPoint + lowestPoint) * 0.5f - cameraHeight;
        cameraSpeed = (targetCameraSpeed - mainCam.transform.position.y);
        mainCam.transform.position += new Vector3(0,cameraSpeed * Time.deltaTime *3f);

        targetCameraSizeSpeed = (highestPoint - lowestPoint) * 0.5f + 4;
        cameraSizeSpeed = (targetCameraSizeSpeed - mainCam.orthographicSize);
        mainCam.orthographicSize += cameraSizeSpeed * Time.deltaTime * 3f;

        mainCam.transform.GetChild(0).transform.localScale = new Vector3(mainCam.orthographicSize * 0.1f, mainCam.orthographicSize * 0.1f, 1);

        return lowestPoint;
    }
}
