using UnityEngine;
using UnityEngine.Rendering;

public class DiamondSquareGenerator : MonoBehaviour
{

    [SerializeField] private int size = 257;
    [SerializeField] private float roughness = 0.7f;
    [SerializeField] private float heightScale = 20f;
    
    private float[,] heightMap; 

    void Start()
    {
        heightMap = GenerateHeightMap(size, roughness);
        ApplyToTerrain(heightMap);
    }

    float[,] GenerateHeightMap(int size, float roughness)
    {
        float[,] map = new float[size, size];

        map[0, 0] = Random.Range(0f, 1f);
        map[0, size - 1] = Random.Range(0f, 1f);
        map[size - 1, 0] = Random.Range(0f, 1f);
        map[size - 1, size - 1] = Random.Range(0f, 1f);

        int i1 = Mathf.FloorToInt(map[0, 0] * 10000);
        int i2 = Mathf.FloorToInt(map[0, size - 1] * 10000);
        int i3 = Mathf.FloorToInt(map[size - 1, 0] * 10000);
        int i4 = Mathf.FloorToInt(map[size - 1, size - 1] * 10000);

        int seed = i1 ^ i2 ^ i3 ^ i4; //XOR 

        // Inicializamos la semilla global
        Random.InitState(seed);

        Debug.Log("Seed generado: " + seed);

        Random.InitState(12345);

        int stepSize = size - 1;
        
        float scale = roughness;

        while (stepSize > 1)
        {
            int halfStep = stepSize / 2;

            for (int x = halfStep; x < size - 1; x += stepSize)
            {
                for (int y = halfStep; y < size - 1; y += stepSize)
                {
                    //Promedio
                    float prom = (map[x - halfStep, y - halfStep] + map[x - halfStep, y + halfStep] +map[x + halfStep, y - halfStep] + map[x + halfStep, y + halfStep]) / 4.0f;
                    map[x, y] = prom + (Random.value * 2 - 1) * scale;
                }
            }

            for (int x = 0; x < size; x += halfStep)
            {
                for (int y = (x + halfStep) % stepSize; y < size; y += stepSize)
                {
                    float sum = 0f;
                    int count = 0;

                    if (x - halfStep >= 0) { sum += map[x - halfStep, y]; count++; }
                    if (x + halfStep < size) { sum += map[x + halfStep, y]; count++; }
                    if (y - halfStep >= 0) { sum += map[x, y - halfStep]; count++; }
                    if (y + halfStep < size) { sum += map[x, y + halfStep]; count++; }

                    float avg = sum / count;
                    map[x, y] = avg + (Random.value * 2 - 1) * scale;
                }
            }

            stepSize /= 2;
            scale *= roughness;
        }

        return map;
    }

    void ApplyToTerrain(float[,] heightMap)
    {
        Terrain terrain = GetComponent<Terrain>();
        if (terrain != null)
        {
            int terrainSize = heightMap.GetLength(0);
            terrain.terrainData.heightmapResolution = terrainSize;
            terrain.terrainData.size = new Vector3(terrainSize, heightScale, terrainSize);
            terrain.terrainData.SetHeights(0, 0, heightMap);
        }
    }
}
