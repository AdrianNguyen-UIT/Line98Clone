using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileNode
{
    public class SquareNode : BaseNode
    {
        private Dictionary<Directions, BaseNode> diagNeighbors = null;

        public override float DistanceFrom(BaseNode other)
        {
            //Manhattan Distance
            return Mathf.Abs(Pos.x - other.Pos.x) + Mathf.Abs(Pos.y - other.Pos.y);
        }

        public override void CatchNeighbors()
        {
            neighbors = new Dictionary<Directions, BaseNode>();
            diagNeighbors = new Dictionary<Directions, BaseNode>();
            Vector2[] dirs = new Vector2[] 
            { Vector2.up, Vector2.right, Vector2.down, Vector2.left,
            new Vector2(-1, 1),  new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1)};

            int index = 0;
            for (; index < 4; index ++)
            {
                var neighbor = boardManager.FindNode(Pos + dirs[index]);
                if (neighbor)
                    neighbors.Add((Directions)index, neighbor);
            }

            for (; index < 8; index++)
            {
                var neighbor = boardManager.FindNode(Pos + dirs[index]);
                if (neighbor)
                    diagNeighbors.Add((Directions)index, neighbor);
            }
        }

        public override void CheckChainExplode()
        {
            base.CheckChainExplode();

            int colorID = Ball.ColorID;

            CheckDiagDirectionExplode(Directions.TOP_LEFT, colorID);
            CheckDiagDirectionExplode(Directions.BOTTOM_RIGHT, colorID);
            if (gameManager.CheckExplode())
                return;

            CheckDiagDirectionExplode(Directions.TOP_RIGHT, colorID);
            CheckDiagDirectionExplode(Directions.BOTTOM_LEFT, colorID);
            if (gameManager.CheckExplode())
                return;

            gameManager.ClearExplodeKeys();
        }

        protected override void CheckDirectionExplode(Directions dir, int ballColorID)
        {
            base.CheckDirectionExplode(dir, ballColorID);
        }


        private void CheckDiagDirectionExplode(Directions dir, int ballColorID)
        {
            if (state != State.GROWUP)
                return;

            if (Ball.ColorID != ballColorID)
                return;

            gameManager.AddExplodeKey(Pos);
            if (diagNeighbors.TryGetValue(dir, out var node))
                ((SquareNode)node).CheckDiagDirectionExplode(dir, ballColorID);
            return;
        }
    }
}