using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject playerModel;
    public GameObject obstacleBoxModel;
    private GameObject[] obstacleBoxArray;
    private Rigidbody2D[] obstacleEyeArray;
    public int obstacleBoxAmount = 20;
    Vector3 obstacleVelocity = new Vector3(-8.0f, 0.0f, 0.0f);
    Vector3 spawnLocationPlayer = new Vector3(0.0f, 1.5f, 0.0f);
    Vector3 spawnLocationBox = new Vector3(5f, 0f, 0f);
    private float probabilityForBox = 0.01f;
    private float playerModelY;
    public float weakGravity = -300;
    public float strongGravity = -3000;
    public float jumpSpeed = 20;
    private float lowestPoint;
    private float score = 0;
    private Rigidbody2D playerEyeLeft;
    private Rigidbody2D playerEyeRight;

    private Player player;
    private ObstacleBox obstacleBox;
    private CameraMovement cameraMovement;
    private ScoreData scoreData;
    public AudioSource jumpAudio;
    public AudioSource landAudio;

    void Start()
    {
        jumpAudio = jumpAudio.GetComponent<AudioSource>();
        landAudio = landAudio.GetComponent<AudioSource>();

        playerModel.transform.position = spawnLocationPlayer;
        
        ObstacleInstantiation();
        PlayerEyeInstantiation();
        obstacleBox = new ObstacleBox(obstacleBoxArray, playerModel);
        player = new Player(obstacleBox, jumpAudio, landAudio);
        cameraMovement = new CameraMovement();
        scoreData = new ScoreData();
    }

    void Update()
    {
        //obstacle spawning
        obstacleBox.Spawning(probabilityForBox, obstacleVelocity, weakGravity, strongGravity, jumpSpeed);

        //movement
        obstacleBox.Movement(obstacleVelocity);

        playerModelY = player.Movement(playerModel, obstacleVelocity, weakGravity, strongGravity, jumpSpeed);

        //camera
        lowestPoint = cameraMovement.Position(obstacleBoxArray, playerModel);

        if (Input.GetKey(KeyCode.Space) || playerModel.transform.position.y < lowestPoint - 5)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        score = scoreData.Tick(obstacleVelocity);

        obstacleBox.EyeTracking(obstacleEyeArray, playerEyeLeft, playerEyeRight);
    }

    void ObstacleInstantiation()
    {
        obstacleBoxArray = new GameObject[obstacleBoxAmount];
        obstacleEyeArray = new Rigidbody2D[obstacleBoxAmount * 2];

        for (int i = 0; i < obstacleBoxArray.Length; i++)
        {
            obstacleBoxArray[i] = Instantiate(obstacleBoxModel);
            obstacleBoxArray[i].transform.position = new Vector3(-20, 0, 0);

            obstacleEyeArray[i*2] = obstacleBoxArray[i].transform.GetChild(0).GetChild(0).GetComponent<Rigidbody2D>();
            obstacleEyeArray[i * 2 + 1] = obstacleBoxArray[i].transform.GetChild(1).GetChild(0).GetComponent<Rigidbody2D>();
        }
    }

    void PlayerEyeInstantiation()
    {
        playerEyeLeft = playerModel.transform.GetChild(0).GetChild(0).GetComponent<Rigidbody2D>();
        playerEyeRight = playerModel.transform.GetChild(1).GetChild(0).GetComponent<Rigidbody2D>();
    }
}
