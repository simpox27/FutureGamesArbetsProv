using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircuitOutputMesh
{

    Vector3 localAjustment;

    public void Reset(int inputWidth, int inputHeight, float dividedByCircuitScale, GameObject outputPlane)
    {
        
        localAjustment = new Vector3((-inputWidth * 0.5f * dividedByCircuitScale), 0, (-inputHeight * 0.5f * dividedByCircuitScale));

        Vector3[] outputMeshVerticesArray = new Vector3[inputWidth * inputHeight * 4];
        int[] outputMeshTrianglesArray = new int[inputWidth * inputHeight * 6];

        //indexing mesh vertices
        for (int iX = 0; iX < inputWidth; iX++)
        {
            for (int iY = 0; iY < inputHeight; iY++)
            {
                int tileIndex = iX * inputHeight + iY;

                Vector3 tempPosition = new Vector3(iX * dividedByCircuitScale, 0, iY * dividedByCircuitScale);
                outputMeshVerticesArray[tileIndex * 4] = tempPosition + localAjustment;

                tempPosition = new Vector3(iX * dividedByCircuitScale, 0, (iY + 1) * dividedByCircuitScale);
                outputMeshVerticesArray[tileIndex * 4 + 1] = tempPosition + localAjustment;

                tempPosition = new Vector3((iX + 1) * dividedByCircuitScale, 0, (iY + 1) * dividedByCircuitScale);
                outputMeshVerticesArray[tileIndex * 4 + 2] = tempPosition + localAjustment;

                tempPosition = new Vector3((iX + 1) * dividedByCircuitScale, 0, iY * dividedByCircuitScale);
                outputMeshVerticesArray[tileIndex * 4 + 3] = tempPosition + localAjustment;
            }
        }
        //triangulate Vector3[] med vertices index
        for (int iX = 0; iX < inputWidth; iX++)
        {
            for (int iY = 0; iY < inputHeight; iY++)
            {
                int tileIndex = iX * inputHeight + iY;


                outputMeshTrianglesArray[tileIndex * 6] = tileIndex * 4;
                outputMeshTrianglesArray[tileIndex * 6 + 1] = tileIndex * 4 + 1;
                outputMeshTrianglesArray[tileIndex * 6 + 2] = tileIndex * 4 + 2;
                outputMeshTrianglesArray[tileIndex * 6 + 3] = tileIndex * 4;
                outputMeshTrianglesArray[tileIndex * 6 + 4] = tileIndex * 4 + 2;
                outputMeshTrianglesArray[tileIndex * 6 + 5] = tileIndex * 4 + 3;
            }
        }
        Mesh outputMesh = new Mesh();
        outputMesh.vertices = outputMeshVerticesArray;
        outputMesh.triangles = outputMeshTrianglesArray;
        outputMesh.RecalculateNormals();
        outputMesh.RecalculateBounds();

        outputPlane.GetComponent<MeshFilter>().sharedMesh = outputMesh;
    }
}
