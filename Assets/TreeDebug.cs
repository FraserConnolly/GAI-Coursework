using com.cyborgAssets.inspectorButtonPro;
using GCU.FraserConnolly;
using GCU.FraserConnolly.AI.Navigation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDebug : MonoBehaviour
{


    [ProButton]
    private void generateTrees()
    {
        deleteTrees();

        Map map = NodeDebug.getMap();

        var islands = Node.GetIsland(Map.Terrain.Tree);

        foreach (var forest in islands)
        {
            
        }
    }

    [ProButton]
    private void deleteTrees()
    {
        var trees = GameObject.FindGameObjectsWithTag("Tree");

        foreach (var tree in trees) 
        { 
            DestroyImmediate(tree);
        }
    }

    private void OnDrawGizmosSelected()
    {

        // Convert the local coordinate values into world
        // coordinates for the matrix transformation.
        Gizmos.matrix = transform.localToWorldMatrix; 
        
        var islands = Node.GetIsland(Map.Terrain.Tree);

        if (islands == null)
        {
            return;
        }

        foreach (var forest in islands)
        {
            Vector2 centreOfGravity = Vector2.zero;
            
            foreach (var tree in forest.Nodes)
            {
                centreOfGravity += tree.Coordinate;
            }

            centreOfGravity /= forest.Size;

            Gizmos.DrawSphere(centreOfGravity, 1f);
        }
    }
}
