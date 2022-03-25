using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Directions
{
    TOP = 0,
    RIGHT,
    BOTTOM,
    LEFT,
    TOP_LEFT,
    TOP_RIGHT,
    BOTTOM_RIGHT,
    BOTTOM_LEFT
}

namespace TileNode
{
    public abstract class BaseNode : MonoBehaviour, IHeapItem<BaseNode>
    {
        public enum State
        {
            DEFAULT = 0,
            QUEUE,
            GROWUP
        }

        public enum Type
        {
            NORMAL = 0,
            GHOST,
        }

        [SerializeField] private Ball ball = null;
        protected State state = State.DEFAULT;
        public Type NodeType = Type.NORMAL;
        protected GameManager gameManager = null;
        protected BoardManager boardManager = null;

        public Vector2 Pos { get { return transform.position; } set { transform.position = value; } }
        public Ball Ball => ball;
        public State NodeState => state;
        public bool Walkable => state == State.DEFAULT;

        public virtual void Init(Vector2 pos)
        {
            transform.position = pos;
            state = State.DEFAULT;
            NodeType = Type.NORMAL;
            gameManager = GameManager.Instance;
            boardManager = BoardManager.Instance;
        }

        protected virtual void OnMouseEnter()
        {
            if (gameManager.BlockInput)
                return;

            if (!gameManager.SelectedNode)
                return;

            if (gameManager.SelectedNode.Pos == Pos)
                return;

            if (state != State.DEFAULT)
            {
                gameManager.EmptyTargetNode();
                return;
            }

            gameManager.TargetNode = this;
            gameManager.FindPath();
        }

        protected virtual void OnMouseDown()
        {
            if (gameManager.BlockInput)
                return;

            if (gameManager.TargetNode)
            {
                gameManager.EndTurn();
                return;
            }

            if (state == State.GROWUP)
            {
                if (gameManager.SelectedNode)
                {
                    gameManager.SelectedNode.UnselectBall();
                }
                gameManager.SelectedNode = this;
                ball.Select();
            }
        }

        public virtual void AttachQueue(int colorID, Sprite sprite)
        {
            if (state != State.DEFAULT)
                return;

            ball.SetColor(colorID, sprite);
            ball.Appear();

            state = State.QUEUE;
        }

        public virtual void AttachGrowUp(int colorID, Sprite sprite)
        {
            AttachQueue(colorID, sprite);
            ball.Grow();
            state = State.GROWUP;
        }

        public virtual void Grow()
        {
            if (state != State.QUEUE)
                return;

            ball.Grow();
            if (gameManager.IsGhost())
                BecomeGhostBall();
            state = State.GROWUP;
        }

        public virtual void Explode()
        {
            if (state != State.GROWUP)
                return;

            ball.Explode();
            state = State.DEFAULT;
        }

        public virtual void CheckChainExplode()
        {
            int colorID = Ball.ColorID;

            CheckDirectionExplode(Directions.LEFT, colorID);
            CheckDirectionExplode(Directions.RIGHT, colorID);
            if (gameManager.CheckExplode())
                return;

            CheckDirectionExplode(Directions.TOP, colorID);
            CheckDirectionExplode(Directions.BOTTOM, colorID);
            if (gameManager.CheckExplode())
                return;
        }

        protected virtual void CheckDirectionExplode(Directions dir, int ballColorID)
        {
            if (state != State.GROWUP)
                return;

            if (Ball.ColorID != ballColorID)
                return;

            gameManager.AddExplodeKey(Pos);
            if (neighbors.TryGetValue(dir, out var node))
                node.CheckDirectionExplode(dir, ballColorID);
            return;
        }

        public virtual void BackToDefault()
        {
            state = State.DEFAULT;
            ball.Disappear();
        }

        public virtual void ResetNode()
        {
            NodeType = Type.NORMAL;
            BackToDefault();
        }

        public virtual void UnselectBall()
        {
            ball.UnSelect();
        }

        public virtual void BecomeGhostBall()
        {
            NodeType = Type.GHOST;
            ball.BecomeGhost();
        }

        #region pathfinding
        protected Dictionary<Directions, BaseNode> neighbors = null;
        public Dictionary<Directions, BaseNode> Neighbors => neighbors;
        public BaseNode Connection = null;
        public float G = 0;
        public float H = 0;
        public float F => G + H;

        public abstract void CatchNeighbors();
        public abstract float DistanceFrom(BaseNode other);
        #endregion

        #region heap item
        private int heapIndex = 0;
        public int HeapIndex { get { return heapIndex; } set { heapIndex = value; } }

        // Lower F cost means HIGHER priority
        // Same F cost + Lower H cost means HIGHER priority
        public int CompareTo(BaseNode other)
        {
            int compare = F.CompareTo(other.F);
            if (compare == 0)
            {
                compare = H.CompareTo(other.H);
            }
            return compare * -1;
        }
        #endregion
    }
}