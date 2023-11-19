using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpacialPartitioning : MonoBehaviour
{
    static List<GameObject> _allAgents = new List<GameObject>();

    public static IReadOnlyList<GameObject> GetAllAgents()
    {
        if ( _allAgents.Any() )
        {
            return _allAgents;
        }

        _allAgents = new List<GameObject>();
        _allAgents.AddRange( GameData.Instance.allies .Where( a => a != null ) );
        _allAgents.AddRange( GameData.Instance.enemies.Where( a => a != null ) );

        return _allAgents;
    }

    private void LateUpdate()
    {
        // ensures that the static list is updated each frame.
        _allAgents.Clear();        
    }
}
