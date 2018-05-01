using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitLogicCompiler
{
    private TileData[,] circuitArray;
    private int inputWidth;
    private int inputHeight;
    
    public List<TileCoordinates[]> wireGroups;
    public TileCoordinates[] wireGroupCoordinatesArray;
    public List<TileCoordinates> notGates;
    public List<TileCoordinates> bufferGates;
    public Dictionary<int, TileCoordinates[]> wireGroupCoordinates;
    public TileCoordinates[] notGateCoordinates;
    public TileCoordinates[] bufferGateCoordinates;
    public Dictionary<int, TileCoordinates[]> wireGroupInputs;
    public TileCoordinates[] notGateInputs;
    public TileCoordinates[] bufferGateInputs;
    private bool[,] compiledTiles;
    public int wireGroupLength = 0;
    public int notGateLength = 0;
    public int bufferGateLength = 0;



    public void Reset(int inputWidth, int inputHeight, TileData[,] circuitArray)
    {

        this.circuitArray = circuitArray;
        this.inputWidth = inputWidth;
        this.inputHeight = inputHeight;


        wireGroupInputs = new Dictionary<int, TileCoordinates[]>();
        wireGroupCoordinates = new Dictionary<int, TileCoordinates[]>();

    wireGroups = new List<TileCoordinates[]>();
        notGates = new List<TileCoordinates>();
        bufferGates = new List<TileCoordinates>();

        int inputHeightCache = inputHeight;

        int inputWidthCache = inputWidth;

        compiledTiles = new bool[inputWidthCache, inputHeightCache];

        wireGroupLength = 0;
        notGateLength = 0;
        bufferGateLength = 0;

        for (int x = 0; x < inputWidthCache; x++)
        {
            for (int y = 0; y < inputHeightCache; y++)
            {
                TileCoordinates[] tilesToCompile = new TileCoordinates[(inputWidthCache+2) * (inputHeightCache+2)];

                switch (circuitArray[x, y].type)
                {
                    case TileData.Type.Wire:
                        if (compiledTiles[x, y] == false)
                        {
                            //Compiling found wire
                            tilesToCompile[0] = new TileCoordinates(x, y);
                            int tileToCompileIndex = 1;
                            int provenWireTileIndex = 0;
                            
                            List<TileCoordinates> tempWireCoordinates = new List<TileCoordinates>();

                            int tempX = x;
                            int tempY = y;

                            for (int i = 0; i < (inputWidthCache * inputHeightCache); i++) {

                                tempX = tilesToCompile[i].xCoordinate;
                                tempY = tilesToCompile[i].yCoordinate;

                                if (tileToCompileIndex <= i)
                                {
                                    break;
                                }
                                else if (0 <= tempX && tempX <= (inputWidthCache - 1) && 0 <= tempY && tempY <= (inputHeightCache - 1))
                                {
                                    if (circuitArray[tempX, tempY].type == circuitArray[x, y].type) //shit code
                                    {
                                        if (compiledTiles[tempX, tempY] == false)
                                        {
                                            compiledTiles[tempX, tempY] = true;

                                            //adding wires coordinate to a temporary list
                                            tempWireCoordinates.Add(new TileCoordinates(tempX, tempY));

                                            provenWireTileIndex++;
                                            
                                            if (tempX < (inputWidthCache - 1))
                                            {
                                                tilesToCompile[tileToCompileIndex++] = new TileCoordinates((tempX + 1), tempY);
                                            }
                                            if (0 < tempX)
                                            {
                                                tilesToCompile[tileToCompileIndex++] = new TileCoordinates((tempX - 1), tempY);
                                            }
                                            if (tempY < (inputHeightCache - 1))
                                            {
                                                tilesToCompile[tileToCompileIndex++] = new TileCoordinates(tempX, (tempY + 1));
                                            }
                                            if (0 < tempY)
                                            {
                                                tilesToCompile[tileToCompileIndex++] = new TileCoordinates(tempX, (tempY - 1));
                                            }
                                        }
                                    }
                                }
                            }

                            //converting the temporary list to a permanent array
                            wireGroupCoordinatesArray = new TileCoordinates[provenWireTileIndex];
                            for (int i = 0; i < provenWireTileIndex; i++)
                            {
                                wireGroupCoordinatesArray[i] = tempWireCoordinates[i];
                            }

                            wireGroups.Add(wireGroupCoordinatesArray);
                            wireGroupLength++;
                        }
                        break;

                    case TileData.Type.NotGate:

                        notGates.Add(new TileCoordinates(x, y));
                        notGateLength++;

                        break;

                    case TileData.Type.BufferGate:

                        bufferGates.Add(new TileCoordinates(x, y));
                        bufferGateLength++;

                        break;
                    default:
                        break;
                }
            }
        }
        //compiling wire groups list to wire groups dictionary
        int wireGroupIndexForCoordinates = 0;
        foreach (TileCoordinates[] group in wireGroups)
        {
            TileCoordinates[] wireGroupCoordinatesArray = new TileCoordinates[group.Length];
            int i = 0;
            foreach (TileCoordinates wireTile in group)
            {
                wireGroupCoordinatesArray[i++] = wireTile;
            }
            wireGroupCoordinates.Add(wireGroupIndexForCoordinates++, wireGroupCoordinatesArray);
        }

        notGateCoordinates = new TileCoordinates[notGateLength];
        for (int i = 0; i < notGateLength; i++)
        {
            notGateCoordinates[i] = notGates[i];
        }


        bufferGateCoordinates = new TileCoordinates[bufferGateLength];
        for (int i = 0; i < bufferGateLength; i++)
        {
            bufferGateCoordinates[i] = bufferGates[i];
        }

        //compiling wireGroups inputs
        int iB = 0;
        foreach (TileCoordinates[] group in wireGroups)
        {

            TileCoordinates[] wireGroupInputsArrayTemp = new TileCoordinates[group.Length * 4];
            int i = 0;
            foreach (TileCoordinates wireTile in group)
            {
                int tempX = wireTile.xCoordinate;
                int tempY = wireTile.yCoordinate;

                if (tempX < (inputWidthCache - 1) && circuitArray[(tempX + 1), tempY].rotation == 2)
                {
                    switch (circuitArray[(tempX + 1), tempY].type)
                    {
                        case TileData.Type.NotGate:
                            wireGroupInputsArrayTemp[i++] = new TileCoordinates((tempX + 1), tempY);
                            break;

                        case TileData.Type.BufferGate:
                            wireGroupInputsArrayTemp[i++] = new TileCoordinates((tempX + 1), tempY);
                            break;

                        default:
                            break;
                    }

                }
                if (0 < tempX && circuitArray[(tempX - 1), tempY].rotation == 0)
                {
                    switch (circuitArray[(tempX - 1), tempY].type)
                    {
                        case TileData.Type.NotGate:
                            wireGroupInputsArrayTemp[i++] = new TileCoordinates((tempX - 1), tempY);
                            break;

                        case TileData.Type.BufferGate:
                            wireGroupInputsArrayTemp[i++] = new TileCoordinates((tempX - 1), tempY);
                            break;

                        default:
                            break;
                    }

                }
                if (tempY < (inputHeightCache - 1) && circuitArray[tempX, (tempY + 1)].rotation == 1)
                {
                    switch (circuitArray[tempX, (tempY + 1)].type)
                    {
                        case TileData.Type.NotGate:
                            wireGroupInputsArrayTemp[i++] = new TileCoordinates(tempX, (tempY + 1));
                            break;

                        case TileData.Type.BufferGate:
                            wireGroupInputsArrayTemp[i++] = new TileCoordinates(tempX, (tempY + 1));
                            break;

                        default:
                            break;
                    }

                }
                if (0 < tempY && circuitArray[tempX, (tempY - 1)].rotation == 3)
                {
                    switch (circuitArray[tempX, (tempY - 1)].type)
                    {
                        case TileData.Type.NotGate:
                            wireGroupInputsArrayTemp[i++] = new TileCoordinates(tempX, (tempY - 1));
                            break;

                        case TileData.Type.BufferGate:
                            wireGroupInputsArrayTemp[i++] = new TileCoordinates(tempX, (tempY - 1));
                            break;

                        default:
                            break;
                    }

                }


            }
            TileCoordinates[] wireGroupinputArray = new TileCoordinates[i];
            for (int iT = 0; iT < i; iT++)
            {
                wireGroupinputArray[iT] = wireGroupInputsArrayTemp[iT];
            }


            wireGroupInputs.Add(iB++, wireGroupinputArray);
        }

        //compiling gates inputs
        notGateInputs = new TileCoordinates[notGateLength];
        for (int i = 0; i < notGateLength; i++)
        {
            int tempX = notGateCoordinates[i].xCoordinate;
            int tempY = notGateCoordinates[i].yCoordinate;
            
            switch (circuitArray[tempX, tempY].rotation)
            {
                case 0:
                    notGateInputs[i] = InputVadility(notGateCoordinates[i], new TileCoordinates((tempX - 1), tempY));
                    break;

                case 1:
                    notGateInputs[i] = InputVadility(notGateCoordinates[i], new TileCoordinates(tempX, (tempY + 1)));
                    break;

                case 2:
                    notGateInputs[i] = InputVadility(notGateCoordinates[i], new TileCoordinates((tempX + 1), tempY));
                    break;

                case 3:
                    notGateInputs[i] = InputVadility(notGateCoordinates[i], new TileCoordinates(tempX, (tempY - 1)));
                    break;
                default:
                    notGateInputs[i] = new TileCoordinates(inputWidthCache, 0);
                    break;
            }
    
        }
        


        //compiling buffer inputs
        bufferGateInputs = new TileCoordinates[bufferGateLength];
        for (int i = 0; i < bufferGateLength; i++)
        {
            int tempX = bufferGateCoordinates[i].xCoordinate;
            int tempY = bufferGateCoordinates[i].yCoordinate;

            switch (circuitArray[tempX, tempY].rotation)
            {
                case 0:
                    bufferGateInputs[i] = InputVadility(bufferGateCoordinates[i], new TileCoordinates((tempX - 1), tempY));
                    break;

                case 1:
                    bufferGateInputs[i] = InputVadility(bufferGateCoordinates[i], new TileCoordinates(tempX, (tempY + 1)));
                    break;

                case 2:
                    bufferGateInputs[i] = InputVadility(bufferGateCoordinates[i], new TileCoordinates((tempX + 1), tempY));
                    break;

                case 3:
                    bufferGateInputs[i] = InputVadility(bufferGateCoordinates[i], new TileCoordinates(tempX, (tempY - 1)));
                    break;
                default:
                    bufferGateInputs[i] = new TileCoordinates(inputWidthCache, 0);
                    break;
            }
            Debug.Log("bufferGateInputs" + bufferGateInputs[i].xCoordinate + ", " + bufferGateInputs[i].yCoordinate);
        }
    }

    private TileCoordinates InputVadility(TileCoordinates tileCoordinates, TileCoordinates inputCoordinates)
    {
        int tempTileX = tileCoordinates.xCoordinate;
        int tempTileY = tileCoordinates.yCoordinate;
        int tempInputX = inputCoordinates.xCoordinate;
        int tempInputY = inputCoordinates.yCoordinate;
        
        TileCoordinates returnCoordinates;

        if(tempInputX < 0 || (inputWidth - 1) < tempInputX || tempInputY < 0 || (inputHeight - 1) < tempInputY)
        {
            return new TileCoordinates(inputWidth, 0);


        }

        TileData inputData = circuitArray[tempInputX, tempInputY];

        switch (inputData.type)
        {
            case TileData.Type.Wire:
                returnCoordinates = new TileCoordinates(tempInputX, tempInputY);
                return returnCoordinates;
                break;

            case TileData.Type.NotGate:
            case TileData.Type.BufferGate:

                if (tempTileX < tempInputX)
                {
                    if (inputData.rotation == 2)
                    {
                        returnCoordinates = new TileCoordinates(tempInputX, tempInputY);
                        return returnCoordinates;
                    }
                    else
                    {
                        return new TileCoordinates(inputWidth, 0);
                    }
                }
                if (tempTileX > tempInputX)
                {
                    if (inputData.rotation == 0)
                    {
                        returnCoordinates = new TileCoordinates(tempInputX, tempInputY);
                        return returnCoordinates;
                    }
                    else
                    {
                        return new TileCoordinates(inputWidth, 0);
                    }
                }
                if (tempTileY < tempInputY)
                {
                    if (inputData.rotation == 1)
                    {
                        returnCoordinates = new TileCoordinates(tempInputX, tempInputY);
                        return returnCoordinates;
                    }
                    else
                    {
                        return new TileCoordinates(inputWidth, 0);
                    }
                }
                if (tempTileY > tempInputY)
                {
                    if (inputData.rotation == 3)
                    {
                        returnCoordinates = new TileCoordinates(tempInputX, tempInputY);
                        return returnCoordinates;
                    }
                    else
                    {
                        return new TileCoordinates(inputWidth, 0);
                    }
                }

                break;

            default:
                break;
        }

        return new TileCoordinates((inputWidth), 0);
    }
}