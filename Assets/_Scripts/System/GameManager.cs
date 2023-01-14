using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : Singleton<GameManager>, ISaveable
{
    [SerializeField] private BallColorConfig[] ballColorConfigs = null;
    [SerializeField] private GameplayConfig gameplayConfig = null;

    [SerializeField] private GameObject pausePanel = null;
    [SerializeField] private GameObject gameOverPanel = null;

    private Queue<int> colorIndexQueue = null;
    private uint currentScore = 0;
    private uint highScore = 0;
    private Stack<Vector2> explodeKeys = null;

    [HideInInspector]
    public TileNode.BaseNode SelectedNode = null;
    [HideInInspector]
    public TileNode.BaseNode TargetNode = null;

    public void EmptySelectedNode() => SelectedNode = null;
    public void EmptyTargetNode()
    {
        TargetNode = null;
        lineRenderer.positionCount = 0;
    }

    private List<Vector2> waypoints = null;
    private LineRenderer lineRenderer = null;
    private BoardManager boardManager = null;
    private UIManager uiManager = null;
    private MovingBall movingBall = null;

    public bool BlockInput = false;
    private uint currentGhostCount = 0;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        boardManager = BoardManager.Instance;
        uiManager = UIManager.Instance;

        boardManager.Init();
        explodeKeys = new Stack<Vector2>();

        movingBall = MovingBall.Instance;
        movingBall.Init();
        movingBall.OnEndEvent += GrowTarget;

        highScore = 0;
        EmptySelectedNode();
        EmptyTargetNode();
        BlockInput = false;
        Timer.Reset();

        if (!SaveSystem.LoadJSonData())
        {
            //First Play
            NewGame();
        }
    }

    public void NewGame()
    {
        Timer.Reset();
        ClearResources();
        boardManager.CreateNewBoard(gameplayConfig.QueuedCount, gameplayConfig.InitGrowUpCount);
        InitScoreboard();
        Resume();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Timer.Stop();
        Timer.Reset();
        BlockInput = true;
        SetHighScore();
        ClearResources();
        boardManager.ClearResources();
        SaveSystem.SaveJSonData();
    }

    public void Quit()
    {
        SetHighScore();
        Timer.Stop();
        SaveSystem.SaveJSonData();
        Application.Quit();
    }

    public void Pause()
    {
        Timer.Stop();
        BlockInput = true;
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        BlockInput = false;
        Timer.Resume();
        pausePanel.SetActive(false);
    }

    private void ClearResources()
    {
        currentScore = 0;
        colorIndexQueue = new Queue<int>();
    }

    private void Update()
    {
        Timer.Update(Time.deltaTime);
        uiManager.SetTimerText(Timer.Format());
    }

    private void InitScoreboard()
    {
        Timer.Reset();
        var count = gameplayConfig.QueuedCount;
        Sprite[] initNextQueueSprites = new Sprite[count];
        for (int index = 0; index < count; index++)
        {
            int rdmIndex = GetRandomColorIndex();
            colorIndexQueue.Enqueue(rdmIndex);
            initNextQueueSprites[index] = ballColorConfigs[rdmIndex].Sprite;
        }

        uiManager.InitNextQueueBall(count, initNextQueueSprites);
        uiManager.SetCurrentScoreText(currentScore);
        uiManager.SetHighScoreText(highScore);
    }

    private void AttachQueueBallsToBoard()
    {
        var count = gameplayConfig.QueuedCount;
        for (int index = 0; index < count; index++)
        {
            if (boardManager.OutOfActiveNode())
            {
                GameOver();
                return;
            }

            int configIndex = colorIndexQueue.Dequeue();
            boardManager.AttachQueueBallToRandomActiveNode(ballColorConfigs[configIndex]);
        }

        if (boardManager.OutOfActiveNode())
        {
            GameOver();
            return;
        }
    }

    private void MakeNewColorIndexQueue()
    {
        var count = gameplayConfig.QueuedCount;
        while (colorIndexQueue.Count < count)
        {
            int newRandomColorIndex = GetRandomColorIndex();
            colorIndexQueue.Enqueue(newRandomColorIndex);
            uiManager.AddNewQueueImg(ballColorConfigs[newRandomColorIndex].Sprite);
        }
    }

    private int GetRandomColorIndex() => Random.Range(0, ballColorConfigs.Length - 1);

    private void MoveToTarget()
    {
        boardManager.ReleaseGrowUpNode(SelectedNode.Pos);
        SelectedNode.BackToDefault();

        lineRenderer.positionCount = 0;
        BlockInput = true;
        movingBall.Move(SelectedNode.Ball.GetSprite(), waypoints);
    }

    private void GrowTarget()
    {
        boardManager.AddGrowUpNode(TargetNode.Pos);
        TargetNode.AttachGrowUp(SelectedNode.Ball.ColorID, SelectedNode.Ball.GetSprite());
        if (SelectedNode.NodeType == TileNode.BaseNode.Type.GHOST)
            TargetNode.BecomeGhostBall();

        TargetNode.CheckChainExplode();
        BlockInput = false;
        EmptyTargetNode();
        EmptySelectedNode();
    }

    public BallColorConfig GetRandomColorConfig() => ballColorConfigs[GetRandomColorIndex()];

    public Sprite GetSprite(int colorID)
    {
        foreach (var config in ballColorConfigs)
        {
            if (config.ColorID == colorID)
                return config.Sprite;
        }
        return null;
    }

    public void Score()
    {
        if (currentScore == gameplayConfig.MaxScore)
            return;

        currentScore += gameplayConfig.ScorePerExploded;
        uiManager.SetCurrentScoreText(currentScore);
    }

    private void SetHighScore()
    {
        highScore = currentScore;
        uiManager.SetHighScoreText(highScore);
    }

    public void EndTurn()
    {
        MoveToTarget();
        boardManager.GrowQueueBalls();
        AttachQueueBallsToBoard();
        MakeNewColorIndexQueue();
    }

    public void FindPath()
    {
        waypoints = Pathfinding.FindPath(SelectedNode, TargetNode, boardManager.TotalNodeCount);
        if (waypoints == null)
            return;

        var count = waypoints.Count;

        lineRenderer.positionCount = count;
        for (int index = 0; index < count; index++)
        {
            lineRenderer.SetPosition(index, waypoints[index]);
        }
    }

    public void AddExplodeKey(Vector2 key)
    {
        if (!explodeKeys.Contains(key))
            explodeKeys.Push(key);
    }

    public bool CheckExplode()
    {
        bool explode = explodeKeys.Count >= gameplayConfig.ExplodeCount;
        if (explode)
            boardManager.ChainExplode(explodeKeys);
        return explode;
    }

    public bool IsGhost()
    {
        if (currentGhostCount < gameplayConfig.GhostCount)
        {
            var chance = Random.Range(0, 100);
            if (chance <= gameplayConfig.GhostAppearChance)
            {
                currentGhostCount++;
                return true;
            }
            else
                return false;
        }
        return false;
    }


    public void ClearExplodeKeys() => explodeKeys.Clear();


    #region Save Load
    public void PopulateSaveData(SaveData saveData)
    {
        saveData.HighScore = highScore;
        saveData.CurrentScore = currentScore;
        saveData.PlayTime = Timer.GetTime();

        while (colorIndexQueue.Count != 0)
        {
            saveData.ColorIndexQueue.Add(colorIndexQueue.Dequeue());
        }
        boardManager.PopulateSaveData(saveData);
    }

    public void LoadFromSaveData(SaveData saveData)
    {
        ClearResources();
        if (saveData.ColorIndexQueue.Count == 0)
        {
            NewGame();
        }
        else
        {
            List<Sprite> temptList = new List<Sprite>();
            foreach (var data in saveData.ColorIndexQueue)
            {
                colorIndexQueue.Enqueue(data);
                temptList.Add(ballColorConfigs[data].Sprite);
            }
            Sprite[] initNextQueueSprites = temptList.ToArray();
            uiManager.InitNextQueueBall(gameplayConfig.QueuedCount, initNextQueueSprites);
        }

        highScore = saveData.HighScore;
        currentScore = saveData.CurrentScore;
        Timer.SetTime(saveData.PlayTime);

        uiManager.SetCurrentScoreText(currentScore);
        uiManager.SetHighScoreText(highScore);
        boardManager.LoadFromSaveData(saveData);
    }
    #endregion
}
