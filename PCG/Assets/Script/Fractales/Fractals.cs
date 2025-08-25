using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Fractals : MonoBehaviour
{
    [SerializeField] int mapX = 50, mapY = 50;
    [SerializeField, Range(1, 20)] int step = 10; // distancia entre puntos base
    [SerializeField, Range(0.1f, 10f)] float heightScale = 2f;
    [SerializeField] float cellSize = 1f;

    private Mesh mesh;

    void Start()
    {
        // 1. Crear un mapa con valores semilla
        float[,] map = new float[mapX, mapY];

        for (int x = 0; x < mapX; x += step)
        {
            for (int y = 0; y < mapY; y += step)
            {
                map[x, y] = Random.Range(0f, 1f); // puntos semilla
            }
        }

        // 2. Suavizar con interpolación bilineal
        map = BilinearInterpolation(map, step);

        // 3. Convertir a un Mesh y asignarlo
        mesh = GenerateMesh(map);
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private float[,] BilinearInterpolation(float[,] mapAux, int step)
    {
        int width = mapAux.GetLength(0);
        int height = mapAux.GetLength(1);

        for (int x = 0; x < width - step; x += step)
        {
            for (int y = 0; y < height - step; y += step)
            {
                float q00 = mapAux[x, y];
                float q10 = mapAux[x + step, y];
                float q01 = mapAux[x, y + step];
                float q11 = mapAux[x + step, y + step];

                for (int i = 0; i <= step; i++)
                {
                    float tx = (float)i / step;
                    for (int j = 0; j <= step; j++)
                    {
                        float ty = (float)j / step;

                        float fxy0 = Mathf.Lerp(q00, q10, tx);
                        float fxy1 = Mathf.Lerp(q01, q11, tx);
                        float fxy = Mathf.Lerp(fxy0, fxy1, ty);

                        mapAux[x + i, y + j] = fxy;
                    }
                }
            }
        }

        return mapAux;
    }

    private Mesh GenerateMesh(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        int vertIndex = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float z = y * cellSize;
                float h = heightMap[x, y] * heightScale;
                float xPos = x * cellSize;

                vertices[vertIndex] = new Vector3(xPos, h, z);
                uvs[vertIndex] = new Vector2((float)x / width, (float)y / height);
                vertIndex++;
            }
        }

        int triIndex = 0;
        for (int x = 0; x < width - 1; x++)
        {
            for (int y = 0; y < height - 1; y++)
            {
                int i = x * height + y;

                // primer triángulo
                triangles[triIndex++] = i;
                triangles[triIndex++] = i + height;
                triangles[triIndex++] = i + 1;

                // segundo triángulo
                triangles[triIndex++] = i + 1;
                triangles[triIndex++] = i + height;
                triangles[triIndex++] = i + height + 1;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        return mesh;
    }
}