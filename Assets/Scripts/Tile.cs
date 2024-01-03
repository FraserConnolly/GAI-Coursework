
using System.Collections.Generic;
using UnityEngine;

namespace GCU.FraserConnolly
{
    public struct Tile
    {
        public Node StartCoordinate;
        public Vector2Int Size;
        public List<Node> Participants;

        void AddParticipents ( Node node)
        {
            if ( Participants == null )
            {
                Participants = new List<Node> ();
            }

            Participants.Add (node);
        }
    }
}
