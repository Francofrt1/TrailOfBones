using FishNet.Managing.Server;
using FishNet.Object;
using Multiplayer.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestGenerator : BaseNetworkBehaviour
{
    [SerializeField] private List<GameObject> treePrefab;

    public void GenerateForest()
    {
        try
        {
            int i = 0;
            Terrain terrain = Terrain.activeTerrain;
            int grassIndex = 0;
            float minDistanceTrees = 3f;
            GameObject treeContainer = GameObject.Find("TreeContainer");
            while (i < 3800)
            {
                Vector3 randomPos = GetRandomPositionOnTerrain(terrain);
                bool isOnGrass = IsOnGrass(randomPos, terrain, grassIndex);
                bool isFarEnough = IsPositionFarEnough(randomPos, minDistanceTrees);
                if (isOnGrass && isFarEnough)
                {
                    int treeCount = treePrefab.Count - 1;
                    int randomTreeIndex = UnityEngine.Random.Range(0, treeCount);
                    SpawnTree(randomTreeIndex, randomPos, terrain.transform);
                    i++;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"GenerateForest error {ex.Message}");
        }
    }

    public void SpawnTree(int randomTreeIndex, Vector3 randomPos, Transform transform)
    {
        var tree = Instantiate(treePrefab[randomTreeIndex], randomPos, Quaternion.identity, transform);
        ServerManager.Spawn(tree);
    }

    Vector3 GetRandomPositionOnTerrain(Terrain terrain)
    {

        Vector3 terrainSize = terrain.terrainData.size;

        float randomX = UnityEngine.Random.Range(0, terrainSize.x);
        float randomZ = UnityEngine.Random.Range(0, terrainSize.z);

        float height = terrain.SampleHeight(new Vector3(randomX, 0, randomZ));

        return new Vector3(randomX, height, randomZ) + terrain.transform.position;
    }

    bool IsOnGrass(Vector3 worldPosition, Terrain terrain, int grassTextureIndex)
    {

        Vector3 terrainLocalPos = worldPosition - terrain.transform.position;

        Vector2 normalizedPos = new Vector2(
            terrainLocalPos.x / terrain.terrainData.size.x,
            terrainLocalPos.z / terrain.terrainData.size.z
        );


        int alphaX = (int)(normalizedPos.x * terrain.terrainData.alphamapWidth);
        int alphaY = (int)(normalizedPos.y * terrain.terrainData.alphamapHeight);

        float[,,] alphaMap = terrain.terrainData.GetAlphamaps(alphaX, alphaY, 1, 1);


        return alphaMap[0, 0, grassTextureIndex] > 0.5f;
    }

    bool IsPositionFarEnough(Vector3 position, float minDistance)
    {
        Vector3 halfExtents = new Vector3(minDistance / 2f, minDistance / 2f, minDistance / 2f);


        Collider[] nearbyColliders = Physics.OverlapBox(
            position,
            halfExtents,
            Quaternion.identity,
            LayerMask.GetMask("TerrainElements")
        );

        return nearbyColliders.Length == 0;
    }

    protected override void RegisterEvents()
    {
        GenerateForest();
    }

    protected override void UnregisterEvents()
    {
    }
}
