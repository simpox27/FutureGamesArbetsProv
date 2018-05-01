using System.Collections.Generic;
using System.Collections;
using UnityEngine.Profiling;
using UnityEngine;


public class CircuitEditor
{

    private float screenHeight;
    private float screenWidth;
    private float cameraToScreen;
    private float screenHalfHeight;

    private float mousePositionX;
    private float mousePositionZ;

    private float mouseSpecialX;
    private float mouseSpecialZ;

    private int mouseCircuitPositionX;
    private int mouseCircuitPositionY;


    private Vector3 cameraPos;
    private Vector3 circuitPos;
    private Vector3 cameraCircuitOffset;
    private Transform cameraTransform;

    public float cameraSpeed = 1f;

    public float circuitScale = 20f;
    public float dividedByCircuitScale;

    public int circuitWidth = 27;
    public int circuitHeight = 27;

    public TileData[,] circuitArray;
    private TileData.Type tileType = TileData.Type.Wire;
    private int tileRotation = 0;

    private CircuitTexture circuitTexture;

    private CircuitLogicCompiler circuitLogicCompiler;
    public Dictionary<int, TileCoordinates[]> wireGroupCoordinates;
    public TileCoordinates[] notGateCoordinates;
    public TileCoordinates[] bufferGateCoordinates;
    public Dictionary<int, TileCoordinates[]> wireGroupInputs;
    public TileCoordinates[] notGateInputs;
    public TileCoordinates[] bufferGateInputs;
    private int wireGroupLength = 0;
    private int notGateLength = 0;
    private int bufferGateLength = 0;

    private int[,] outputArray;
    private int[,] inputArray;

    private CircuitOutputMesh circuitOutputMesh;

    private GameObject outputPlane;

    private Color[] outputMeshColors;

    public CircuitEditor(
        Texture2D notGateTex,
        Texture2D bufferGateTex,
        Texture2D wireTex,
        Texture2D emptyTex,
        GameObject outputPlane)
    {
        circuitArray = new TileData[circuitWidth, circuitHeight];


        dividedByCircuitScale = 1 / circuitScale;

        screenHeight = Screen.height;
        screenWidth = Screen.width;
        float fovRad = Camera.main.fieldOfView * Mathf.PI / 180;
        screenHalfHeight = screenHeight * 0.5f;
        cameraToScreen = screenHalfHeight / Mathf.Tan(fovRad * 0.5f);

        cameraTransform = Camera.main.transform;
        cameraPos = cameraTransform.position;
        circuitPos = GameObject.Find("Circuit").transform.position;
        cameraCircuitOffset = cameraPos - circuitPos;

        circuitTexture = new CircuitTexture(
            emptyTex.width,
            notGateTex.GetPixels(),
            bufferGateTex.GetPixels(),
            wireTex.GetPixels(),
            emptyTex.GetPixels());

        

        circuitLogicCompiler = new CircuitLogicCompiler();

        this.outputPlane = outputPlane;
        circuitOutputMesh = new CircuitOutputMesh();


        circuitReset();
        
        //test setup
        for (int iX = 0; iX < circuitWidth; iX++)
        {
            for (int iY = 0; iY < circuitHeight; iY++)
            {
                float icx = iX - circuitWidth * 0.5f;
                float icy = iY - circuitHeight * 0.5f;
                if (Mathf.Abs(icx) == Mathf.Abs(icy))
                {
                    circuitArray[iX, iY] = new TileData(TileData.Type.Wire, 0);
                }
                else if(Mathf.Abs(icx) >= Mathf.Abs(icy))
                {
                    if(0 < icx)
                    {
                        circuitArray[iX, iY] = new TileData(TileData.Type.BufferGate, 1);
                    }
                    else
                    {
                        circuitArray[iX, iY] = new TileData(TileData.Type.BufferGate, 3);
                    }
                }
                else
                {
                    if (0 < icy)
                    {
                        circuitArray[iX, iY] = new TileData(TileData.Type.BufferGate, 0);
                    }
                    else
                    {
                        circuitArray[iX, iY] = new TileData(TileData.Type.BufferGate, 2);
                    }
                }
            }
            
        }
    }

    

    public void Tick()
    {
        if (Input.anyKey)
        {
            //On click
            if (Input.GetMouseButtonDown(0) == true)
            {
                mousePositionX = Input.mousePosition.x;
                mousePositionZ = Input.mousePosition.y;

                mouseSpecialX = (mousePositionX - screenWidth * 0.5f) / cameraToScreen * cameraCircuitOffset.y + cameraCircuitOffset.x;
                mouseSpecialZ = (mousePositionZ - screenHeight * 0.5f) / cameraToScreen * cameraCircuitOffset.y + cameraCircuitOffset.z;

                mouseCircuitPositionX = Mathf.RoundToInt(mouseSpecialX * circuitScale + circuitWidth * 0.5f - 0.5f);
                mouseCircuitPositionY = Mathf.RoundToInt(mouseSpecialZ * circuitScale + circuitHeight * 0.5f - 0.5f);

                circuitArray[mouseCircuitPositionX, mouseCircuitPositionY] = new TileData(tileType, tileRotation);

                circuitTexture.Reset(circuitWidth, circuitHeight, circuitArray);

                //compiling logicarrays
                circuitLogicCompiler.Reset(circuitWidth, circuitHeight, circuitArray);

                wireGroupCoordinates = circuitLogicCompiler.wireGroupCoordinates;
                notGateCoordinates = circuitLogicCompiler.notGateCoordinates;
                bufferGateCoordinates = circuitLogicCompiler.bufferGateCoordinates;

                wireGroupInputs = circuitLogicCompiler.wireGroupInputs;
                notGateInputs = circuitLogicCompiler.notGateInputs;
                bufferGateInputs = circuitLogicCompiler.bufferGateInputs;

                wireGroupLength = circuitLogicCompiler.wireGroupLength;
                notGateLength = circuitLogicCompiler.notGateLength;
                bufferGateLength = circuitLogicCompiler.bufferGateLength;

                //resetting outputArray
                outputArrayReset(circuitWidth, circuitHeight);
            }



            //Camera movemet
            if (Input.GetKey(KeyCode.W))
            {
                cameraPos = new Vector3(cameraPos.x, cameraPos.y, (cameraPos.z + cameraSpeed * Time.deltaTime));
                cameraTransform.position = cameraPos;
                cameraCircuitOffset = cameraPos - circuitPos;
            }
            if (Input.GetKey("s"))
            {
                Camera.main.transform.position = new Vector3(cameraPos.x, cameraPos.y, (cameraPos.z - cameraSpeed * Time.deltaTime));
                cameraPos = Camera.main.transform.position;
                cameraCircuitOffset = cameraPos - circuitPos;
            }
            if (Input.GetKey("a"))
            {
                Camera.main.transform.position = new Vector3((cameraPos.x - cameraSpeed * Time.deltaTime), cameraPos.y, cameraPos.z);
                cameraPos = Camera.main.transform.position;
                cameraCircuitOffset = cameraPos - circuitPos;
            }
            if (Input.GetKey("d"))
            {
                Camera.main.transform.position = new Vector3((cameraPos.x + cameraSpeed * Time.deltaTime), cameraPos.y, cameraPos.z);
                cameraPos = Camera.main.transform.position;
                cameraCircuitOffset = cameraPos - circuitPos;
            }

            //Circuit scaler
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                circuitHeight += 1;
                CircuitScaler();
                circuitArray = new TileData[circuitWidth, circuitHeight];

                circuitReset();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (circuitHeight > 1)
                {
                    circuitHeight -= 1;
                    CircuitScaler();
                    circuitArray = new TileData[circuitWidth, circuitHeight];
                    circuitReset();
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (circuitWidth > 1)
                {
                    circuitWidth -= 1;
                    CircuitScaler();
                    circuitArray = new TileData[circuitWidth, circuitHeight];
                    circuitReset();
                }
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                circuitWidth += 1;
                CircuitScaler();
                circuitArray = new TileData[circuitWidth, circuitHeight];
                circuitReset();
            }

            //PlacementType/Rotation
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                tileType = TileData.Type.Wire;
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                tileType = TileData.Type.NotGate;
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                tileType = TileData.Type.BufferGate;
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                tileType = TileData.Type.Empty;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                if (tileRotation < 3)
                {
                    tileRotation += 1;
                }
                else
                {
                    tileRotation = 0;
                }
            }
        }
        Profiler.BeginSample("logicLoops");
        //logic outputs
        for (int iX = 0; iX < circuitWidth + 1; iX++)
        {
            for (int iY = 0; iY < circuitHeight; iY++)
            {
                inputArray[iX, iY] = outputArray[iX, iY];
            }
        }
        //inputArray = (int[,])outputArray.Clone();
        Profiler.EndSample();
        //wire outputs
        for (int i = 0; i < wireGroupLength; i++)
        {
            int output = 1;
            foreach (TileCoordinates pos in wireGroupInputs[i])
            {
                output *= inputArray[pos.xCoordinate, pos.yCoordinate];
            }
            foreach (TileCoordinates pos in wireGroupCoordinates[i])
            {
                outputArray[pos.xCoordinate, pos.yCoordinate] = output;
            }
        }

        //notGate outputs
        for (int i = 0; i < notGateLength; i++)
        {

            outputArray[notGateCoordinates[i].xCoordinate, notGateCoordinates[i].yCoordinate] = 1 - inputArray[notGateInputs[i].xCoordinate, notGateInputs[i].yCoordinate];

        }

        //bufferGate outputs
        for (int i = 0; i < bufferGateLength; i++)
        {

            outputArray[bufferGateCoordinates[i].xCoordinate, bufferGateCoordinates[i].yCoordinate] = inputArray[bufferGateInputs[i].xCoordinate, bufferGateInputs[i].yCoordinate];

        }
        
        //outputMesh colors
        for(int iX = 0; iX < circuitWidth; iX++)
        {
            for(int iY = 0; iY < circuitHeight; iY++)
            {
                int outputValue = outputArray[iX, iY];
                outputMeshColors[(iX * circuitHeight + iY) * 4] = new Color (0, 0, 0, outputValue * 0.5f);
                outputMeshColors[(iX * circuitHeight + iY) * 4 + 1] = new Color(0, 0, 0, outputValue * 0.5f);
                outputMeshColors[(iX * circuitHeight + iY) * 4 + 2] = new Color(0, 0, 0, outputValue * 0.5f);
                outputMeshColors[(iX * circuitHeight + iY) * 4 + 3] = new Color(0, 0, 0, outputValue * 0.5f);
            }
        }
        outputPlane.GetComponent<MeshFilter>().sharedMesh.colors = outputMeshColors;
    }



    private void CircuitScaler()
    {
        GameObject.Find("Circuit").transform.localScale = new Vector3(circuitWidth * dividedByCircuitScale, circuitHeight * dividedByCircuitScale, 1);
    }

    private void outputArrayReset(int tempWidth, int tempHeight)
    {
        outputArray = new int[(tempWidth + 1), tempHeight];
        inputArray = new int[(tempWidth+1), tempHeight];

        for (int tempX = 0; tempX < (tempWidth + 1); tempX++)
        {
            for (int tempY = 0; tempY < tempHeight; tempY++)
            {
                outputArray[tempX, tempY] = 1;
            }
        }
    }

    private void circuitReset()
    {
        outputArrayReset(circuitWidth, circuitHeight);
        circuitTexture.Reset(circuitWidth, circuitHeight, circuitArray);
        circuitOutputMesh.Reset(circuitWidth, circuitHeight, dividedByCircuitScale, outputPlane);
        circuitLogicCompiler.Reset(circuitWidth, circuitHeight, circuitArray);

        wireGroupCoordinates = circuitLogicCompiler.wireGroupCoordinates;
        notGateCoordinates = circuitLogicCompiler.notGateCoordinates;
        bufferGateCoordinates = circuitLogicCompiler.bufferGateCoordinates;

        wireGroupInputs = circuitLogicCompiler.wireGroupInputs;
        notGateInputs = circuitLogicCompiler.notGateInputs;
        bufferGateInputs = circuitLogicCompiler.bufferGateInputs;

        wireGroupLength = circuitLogicCompiler.wireGroupLength;
        notGateLength = circuitLogicCompiler.notGateLength;
        bufferGateLength = circuitLogicCompiler.bufferGateLength;

        outputMeshColors = new Color[circuitWidth * circuitHeight * 4];
    }
}
 