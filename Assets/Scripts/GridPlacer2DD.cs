using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class GridPlacer2D : MonoBehaviour
{
    public static GridPlacer2D Instance;

    public Tilemap terrainMap;
    public Tilemap obstacleMap;
    public LayerMask clickableLayer;

    void Awake()
    {
        Instance = this;
    }

    void Update() { }
}
