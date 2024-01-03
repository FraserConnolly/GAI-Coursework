using com.cyborgAssets.inspectorButtonPro;
using GCU.FraserConnolly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDebug : MonoBehaviour
{
    public bool showGrid = false;
    public Map map { get; private set; }
    private GameObject  mapRendererObject;
    private MapRenderer mapRenderer;

    [ProButton]
    private void NewMap()
    {
        map = new Map();

        MakeNewMapRenderer();

        mapRenderer.Initialise(map.GetMapData(), Map.MapWidth, Map.MapHeight, showGrid: this.showGrid);

        BuildNodes();
    }

    [ProButton]
    private void BuildNodes()
    {
        if ( map == null )
        {
            NewMap();
        }

        Node.ClearNodes();
        Debug.Log($"Starting node build.");
        Node.GetAllNodes(map);
        Debug.Log($"Node build complete.");

    }

    private void MakeNewMapRenderer()
    {
        const string mapRendererName = "Map Renderer";

        var del = GameObject.FindObjectsByType<MapRenderer>(FindObjectsSortMode.None);
        foreach (var item in del)
        {
            DestroyImmediate(item.gameObject);
        }

        mapRendererObject = new GameObject(mapRendererName);
        mapRenderer = mapRendererObject.AddComponent<MapRenderer>();

        mapRendererObject.transform.Rotate(new Vector3(90, 0, 0));
    }

}
