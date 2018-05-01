using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CircuitTexture
{
    private SpriteRenderer rend;
    private Color[] notGateColorArray;
    private Color[] bufferGateColorArray;
    private Color[] wireColorArray;
    private Color[] emptyColorArray;
    private int tileTexWidth;

    public CircuitTexture(
        int tileTexWidth,
        Color[] notGateColorArray,
        Color[] bufferGateColorArray,
        Color[] wireColorArray,
        Color[] emptyColorArray)
    {
        this.notGateColorArray = notGateColorArray;
        this.bufferGateColorArray = bufferGateColorArray;
        this.wireColorArray = wireColorArray;
        this.emptyColorArray = emptyColorArray;

        this.tileTexWidth = tileTexWidth;
    }

    public void Reset(int inputWidth, int inputHeight, TileData[,] circuitArray)
    {
        rend = GameObject.Find("CircuitTexture").GetComponent<SpriteRenderer>();

        //create a texture

        Texture2D tex = new Texture2D(inputWidth * tileTexWidth, inputHeight * tileTexWidth);

        int texWidth = tex.width;

        int texHeight = tex.height;



        Color[] circuitColorArray = new Color[texWidth * texHeight];

        for (int x = 0; x < inputWidth; x++)
        {
            for (int y = 0; y < inputHeight; y++)
            {
                Color[] tileColorArray;

                int xForX = 0;
                int yForX = 0;
                int widthForX = 0;
                int xForY = 0;
                int yForY = 0;
                int widthForY = 0;
                //tex.SetPixel(x, y, Color.green);
                switch (circuitArray[x, y].type)
                {
                    case TileData.Type.Empty:
                        tileColorArray = emptyColorArray;
                        break;
                    case TileData.Type.Wire:
                        tileColorArray = wireColorArray;
                        break;
                    case TileData.Type.NotGate:
                        tileColorArray = notGateColorArray;
                        break;
                    case TileData.Type.BufferGate:
                        tileColorArray = bufferGateColorArray;
                        break;
                    default:
                        tileColorArray = emptyColorArray;
                        break;
                }
                switch (circuitArray[x, y].rotation)
                {
                    case 0:
                        xForX = 1;
                        yForY = 1;
                        break;
                    case 1:
                        yForX = 1;
                        xForY = -1;
                        widthForY = 1;
                        break;
                    case 2:
                        xForX = -1;
                        widthForX = 1;
                        yForY = -1;
                        widthForY = 1;
                        break;
                    case 3:
                        yForX = -1;
                        widthForX = 1;
                        xForY = 1;
                        break;
                    default:
                        xForX = 1;
                        yForY = 1;
                        break;
                }

                for (int tX = 0; tX < tileTexWidth; tX++)
                {
                    for (int tY = 0; tY < tileTexWidth; tY++)
                    {
                        int tilePixelIndex = tX + tY * tileTexWidth;

                        int rX = tX * xForX + tY * yForX + (tileTexWidth - 1) * widthForX;
                        int rY = tX * xForY + tY * yForY + (tileTexWidth - 1) * widthForY;
                        int circuitPixelIndex = x * tileTexWidth + rX + y * tileTexWidth * tileTexWidth * inputWidth + rY * tileTexWidth * inputWidth;

                        Color tileColorPixel = tileColorArray[tilePixelIndex];
                        circuitColorArray[circuitPixelIndex] = tileColorPixel;
                    }
                }
            }
        }
        tex.SetPixels(circuitColorArray);
        tex.Apply();

        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Point;

        //create a sprite
        Sprite newSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

        //assign our procedural sprite to rend.sprite
        rend.sprite = newSprite;
    }
}
