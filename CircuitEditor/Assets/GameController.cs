using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Texture2D notGateTex;
    public Texture2D bufferGateTex;
    public Texture2D wireTex;
    public Texture2D emptyTex;
    public GameObject outputPlane;

    private CircuitEditor circuitEditor;



    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;
        circuitEditor = new CircuitEditor(notGateTex, bufferGateTex, wireTex, emptyTex, outputPlane);
    }

    // Update is called once per frame
    void Update()
    {
        circuitEditor.Tick();
    }
}