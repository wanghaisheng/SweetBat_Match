using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

// Define a set of game modes for different game objectives
public enum GameMode { FeedingObjective, ScoringObjective, TimeObjective, CollectionObjective }
public enum GamePlayMode { MovesLimited, TimedMatch }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;  // Static reference to the GameManager instance

    /// Observer Pattern
    [HideInInspector] public UnityEvent<GameMode> OnGameMode;
    [HideInInspector] public UnityEvent OnUniqueMatches;

    public int Level { get { return level; } }  // Public getter for the current level
    // Gets or sets the objective game mode.
    public GameMode GameMode { get { return gameMode; } set { gameMode = value; } }
    // Gets the list of available fruits.
    public List<GameObject> AvailableFruits { get { return availableFruits; } }

    // Gets the maximum feeding objective.
    public int MaxFeedingObjective { get { return maxFeedingObjective; } }
    // Gets the maximum score objective.
    public int MaxScoreObjective { get { return maxScoreObjective; } }

    // Gets the current move counter.
    public int MoveCounter { get { return moveCounter; } }
    // Gets the time to match.
    public float TimeToMatch { get { return timeToMatch; } }

    // Gets or sets whether the game objective is complete.
    public bool ObjectiveComplete { get { return objectiveComplete; } set { objectiveComplete = value; } }

    // Gets the total remaining seconds for the timer.
    public float TotalSeconds { get { return totalSeconds; } }
    // Gets the match objective amount.
    public int MatchObjectiveAmount { get { return matchObjectiveAmount; } }
    // Gets or sets a value indicating whether unique matches are required.
    public bool UniqueMatches { get { return uniqueMatches; } set { uniqueMatches = value; } }

    // Serialized game mode field
    [SerializeField] GameMode gameMode;

    // Private field for the current level
    [SerializeField] int level = 0;
    // List of available fruits
    [SerializeField] List<GameObject> availableFruits;
    // List of upcoming fruits
    [SerializeField] List<GameObject> upcomingFruits;

    [Header("Game Mode")]
    // Move counter for the game mode
    [SerializeField] int moveCounter;
    // Time to match for the game mode
    [SerializeField] float timeToMatch;

    [Header("Feeding Objective")]
    // Maximum feeding objective
    [SerializeField] int maxFeedingObjective;

    [Header("Scoring Objective")]
    // Maximum score objective
    [SerializeField] int maxScoreObjective;

    [Header("Time Objective")]
    [SerializeField] float totalSeconds;
    [SerializeField] int matchObjectiveAmount;

    // Indicates whether the game objective is complete
    bool objectiveComplete;

    bool uniqueMatches;

    /// <summary>
    /// Subscribes to the SceneManager's sceneLoaded event when the script is enabled.
    /// </summary>
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Unsubscribes from the SceneManager's sceneLoaded event when the script is disabled.
    /// </summary>
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (gameMode == GameMode.ScoringObjective || gameMode == GameMode.TimeObjective)
            uniqueMatches = true;
    }

    /// <summary>
    /// Increases the level count and starts the next level routine.
    /// </summary>
    public void NextLevel()
    {
        // Increases the level count
        level++;
        // Starts the next level routine
        StartCoroutine(NextLevelRutiner());
    }

    /// <summary>
    /// Waits for 2 seconds and finds the LevelManager object.
    /// </summary>
    IEnumerator NextLevelRutiner()
    {
        yield return new WaitForSeconds(2);

        // Finds the LevelManager object
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.NextLevel();
    }

    /// <summary>
    /// Called when a scene is loaded, checks if the scene is "Game" and sets the objective game mode.
    /// </summary>
    /// <param name="scene">The scene that was loaded</param>
    /// <param name="mode">The mode used to load the scene</param>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
        {
            OnGameMode?.Invoke(gameMode);
        }
    }

    /// <summary>
    /// Gets a random game play mode from the GamePlayMode enum.
    /// </summary>
    /// <returns>A random game play mode</returns>
    public GamePlayMode GetRandomGamePlayMode()
    {
        int numberOfGamePlayMode = Enum.GetNames(typeof(GamePlayMode)).Length;

        return (GamePlayMode)UnityEngine.Random.Range(0, numberOfGamePlayMode);
    }
}