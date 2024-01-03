using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GCU.FraserConnolly.PCG
{
    public class TerrainRenderer : MonoBehaviour
    {
        private static TerrainRenderer _instance;

        public static Vector3 OffsetPoint = Vector3.zero;
        
        public static float GetYOnTerrain( Vector3 position )
        {
            if ( _instance == null )
            {
                return OffsetPoint.y;
            }

            return _instance._terrain.SampleHeight(position);
        }


        [SerializeField]
        private TerrainData _terrainDataPrefab;
        [SerializeField]
        private Material _defaultMaterial;

        Terrain _terrain;
        TerrainData _terrainData;

        [Serializable]
        private class TerrainLayerDetails
        {
            public Map.Terrain Terrain;
            public int TerrainLayer;
        }

        [SerializeField]
        private TerrainLayerDetails[] _terrainLayerDetails = Array.Empty<TerrainLayerDetails>();

        [SerializeField]
        private GameObject treeTilePrefab;

        private void Awake()
        {
            if ( _instance == null )
            {
                _instance = this;
                return;
            }

            Destroy(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            var map = GameData.Instance.Map;

            var go = new GameObject("Terrain");
            go.transform.SetParent(transform, false);

            _terrain = go.AddComponent<Terrain>();
            var collider = go.AddComponent<TerrainCollider>();

            _terrainData = Instantiate(_terrainDataPrefab);
            _terrainData.size = new Vector3(100, 100, 100);

            Debug.Log($"Terrain tree count: {_terrainData.treeInstanceCount}");

            _terrain.terrainData = _terrainData;
            collider.terrainData = _terrainData;
            _terrain.materialTemplate = _defaultMaterial;

            for (int i = 0; i < _terrainData.terrainLayers.Length; i++)
            {
                Debug.Log($"Layer {i} {_terrainData.terrainLayers[i].name}");
                DetailMapCutoff(_terrain, i);
            }

            SetSplatMap(_terrain, _terrainLayerDetails, map);

            OffsetPoint = _terrain.transform.position;

            var treeParent = new GameObject("Trees");
            treeParent.transform.SetParent(transform, false);

            PlantTress(treeParent.transform, map);
        }

        //https://alastaira.wordpress.com/2013/11/14/procedural-terrain-splatmapping/
        void SetSplatMap(Terrain terrain, IReadOnlyCollection<TerrainLayerDetails> layerDetails, Map map)
        {
            // Get a reference to the terrain data
            TerrainData terrainData = terrain.terrainData;

            // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
            float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

            for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
                for (int x = 0; x < terrainData.alphamapWidth; x++)
                {
                    // Normalise x/y coordinates to range 0-1 
                    float y_01 = (float)y / (float)terrainData.alphamapHeight;
                    float x_01 = (float)x / (float)terrainData.alphamapWidth;

                    // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                    float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapResolution), Mathf.RoundToInt(x_01 * terrainData.heightmapResolution));

                    // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                    Vector3 normal = terrainData.GetInterpolatedNormal(y_01, x_01);

                    // Calculate the steepness of the terrain
                    float steepness = terrainData.GetSteepness(y_01, x_01);

                    // Setup an array to record the mix of texture weights at this point
                    float[] splatWeights = new float[terrainData.alphamapLayers];

                    Vector2Int mapCoordinates = new Vector2Int(
                        (int)(y_01 * Map.MapHeight),
                        (int)(x_01 * Map.MapWidth) );

                    Map.Terrain terrainAtPoint = map.GetTerrainAt(mapCoordinates.x, mapCoordinates.y);

                    foreach (var layerDetail in layerDetails)
                    {
                        if (layerDetail.Terrain != terrainAtPoint)
                        {
                            continue;
                        }

                        splatWeights[layerDetail.TerrainLayer] = 1f;
                    }


                    // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                    float z = splatWeights.Sum();

                    // Loop through each terrain texture
                    for (int i = 0; i < terrainData.alphamapLayers; i++)
                    {

                        // Normalize so that sum of all texture weights = 1
                        splatWeights[i] /= z;

                        // Assign this point to the splatmap array
                        splatmapData[x, y, i] = splatWeights[i];
                    }
                }
            }

            // Finally assign the new splatmap to the terrainData:
            terrainData.SetAlphamaps(0, 0, splatmapData);
        }

        // Set all pixels in a detail map to zero.
        void DetailMapCutoff(Terrain t, int layer = 0) //, float threshold)
        {

            // Get all of layer zero.
            var map = t.terrainData.GetDetailLayer(0, 0, t.terrainData.detailWidth, t.terrainData.detailHeight, layer);

            // For each pixel in the detail map...
            for (int y = 0; y < t.terrainData.detailHeight; y++)
            {
                for (int x = 0; x < t.terrainData.detailWidth; x++)
                {
                    map[x, y] = 0;

                    //// If the pixel value is below the threshold then
                    //// set it to zero.
                    //if (map[x, y] < threshold)
                    //{
                    //    map[x, y] = 0;
                    //}
                }
            }

            // Assign the modified map back.
            t.terrainData.SetDetailLayer(0, 0, layer, map);
        }

        void PlantTress(Transform treeParent, Map map)
        {
            Vector3 gridOffset = new Vector3(0.5f, 0, 0.5f);

            if ( treeTilePrefab == null )
            {
                return;
            }

            for(int x = 0; x< Map.MapWidth; x++)
            {
                for(int y = 0; y< Map.MapHeight; y++)
                {
                    if ( map.GetTerrainAt ( x, y ) != Map.Terrain.Tree )
                    {
                        continue;
                    }

                    Vector3 position = new Vector3(x, 0f, y);
                    position += gridOffset;
                    position += OffsetPoint;

                    position.y += GetYOnTerrain(position);

                    var tree = Instantiate(treeTilePrefab, position, Quaternion.identity, treeParent);
                }
            }
        }

    }
}