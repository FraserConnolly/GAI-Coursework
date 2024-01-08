using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class is intended to provide sub lists of agents using spacial partitioning,
/// however it has not yet been implemented but it is kept in the project as a placeholder.
/// </summary>
public class SpacialPartitioning : MonoBehaviour
{
    static List<GameObject> _allAgents = new List<GameObject>();

    public static IReadOnlyList<GameObject> GetAllAlliedAgents()
    {
        return GameData.Instance.allies.Where(a => a != null).ToList();
    }

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
