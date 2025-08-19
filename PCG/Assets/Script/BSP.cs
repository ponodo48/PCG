using Assets;
using System.Drawing;
using UnityEngine;

public class BSP : MonoBehaviour
{
    [SerializeField] private GameObject tile;
    [SerializeField] private int width = 50;
    [SerializeField] private int height = 50;
    [SerializeField] private int min = 5;
    
    public int[,] map;
    public Nodo root;

    void Start()
    {
        map = new int[width, height];
        BSPstart(map, 0, 0, width, height);
    }

    public void GenerateMap()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        map = new int[width, height];
        BSPstart(map, 0, 0, width, height);
    }

    Vector2 BSPstart(int[,] map, int x, int y, int w, int h)
    {
        if (w <= min || h <= min)
        {
            return BuildRoom(map, x, y, w, h);
        }

        bool divideHorizontally = Random.Range(0, 2) == 0;
        Vector2 roomCenterA;
        Vector2 roomCenterB;

        if (divideHorizontally && w > min * 2)
        {
            int cut = Random.Range(min, w - min);
            roomCenterA = BSPstart(map, x, y, cut, h);
            roomCenterB = BSPstart(map, x + cut, y, w - cut, h);
        }
        else if (!divideHorizontally && h > min * 2)
        {
            int cut = Random.Range(min, h - min);
            roomCenterA = BSPstart(map, x, y, w, cut);
            roomCenterB = BSPstart(map, x, y + cut, w, h - cut);
        }
        else
        {
            return BuildRoom(map, x, y, w, h);
        }

        ConnectRooms(map, roomCenterA, roomCenterB);
        return (roomCenterA + roomCenterB) / 2f;
    }

    Vector2 BuildRoom(int[,] map, int x, int y, int w, int h)
    {
        int wroom = Random.Range(w - 2, w);
        int hroom = Random.Range(h - 2, h);

        int startX = x + 1;
        int startY = y + 1;

        for (int i = startX; i < startX + wroom; i++)
        {
            for (int j = startY; j < startY + hroom; j++)
            {
                map[i, j] = 1;
                Instantiate(tile, new Vector3(i, j, 0), Quaternion.identity, gameObject.transform);
            }
        }

        float centerX = startX + wroom / 2f;
        float centerY = startY + hroom / 2f;

        return new Vector2(centerX, centerY);
    }

    void ConnectRooms(int[,] map, Vector2 a, Vector2 b)
    {
        int x1 = Mathf.RoundToInt(a.x);
        int y1 = Mathf.RoundToInt(a.y);
        int x2 = Mathf.RoundToInt(b.x);
        int y2 = Mathf.RoundToInt(b.y);

        for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x++)
        {
            map[x, y1] = 1;
            Instantiate(tile, new Vector3(x, y1, 0), Quaternion.identity, transform);
        }

        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
        {
            map[x2, y] = 1;
            Instantiate(tile, new Vector3(x2, y, 0), Quaternion.identity, transform);
        }
    }
}
