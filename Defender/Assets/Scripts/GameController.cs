using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    //Prefabs
    public GameObject enemy;
    public GameObject boss;
    public GameObject enemyOnMap;

    private GameObject map;
    private GameObject player;
    private static GameController objectInstance;

    //For keeping track of the enemies
    public List<GameObject> enemyList = new List<GameObject>();

    //For updating the position of the enemies on the gui map
    public List<GameObject> enemyOnMapList = new List<GameObject>();

    private Text scoreCounterText;

    //Counters
    private int enemyAmount = 5;
    private int currentLevelScore;
    private int totalScore;
    private int level = 0;
    private int spawnedEnemies = 0;
    public int killedEnemies = 0;
    public int savedAstronauts = 0;

    //Timers
    private float enemyTime = 4f;
    private float passedTime = 0f;
    private float nextTime = 1f;
    private float menuTimer = 0f;

    //Bools
    public bool bossSpawned = false;
    private bool inMenu = false;
    private bool gameOver = false;

    void Awake()
    {
        //Makes this gameobject be not destroyed even in scene change.
        DontDestroyOnLoad(this);

        if (objectInstance == null)
        {
            objectInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        player = GameObject.Find("Player");
        map = GameObject.Find("Map");
        scoreCounterText = GameObject.Find("ScoreCounter").GetComponent<Text>();
        scoreCounterText.text = "Score: " + currentLevelScore;
    }

    void Update()
    {
        //If is in the score menu, count down time and change scene
        if (inMenu && !gameOver)
        {
            menuTimer -= Time.deltaTime;
            if (menuTimer <= 0f)
            {
                NextLevel();
                menuTimer = 0f;
            }
        }

        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                RestartGame();
        }

        //If the player is not in the menu, enemies will be spawned according to the level's difficulty
        if (!inMenu)
        {
            if (spawnedEnemies < enemyAmount)
            {
                passedTime += Time.deltaTime;
                if (passedTime >= nextTime)
                {
                    passedTime = 0;
                    nextTime = Random.Range(0.2f, enemyTime);
                    spawnedEnemies++;

                    var newEnemy = Instantiate(enemy, new Vector3(Random.Range(320 * -2, 320 * 2), Random.Range(-70, 50), 0), Quaternion.identity);

                    //If the enemy is spawned too close to the player, it is moved further away.
                    if (Mathf.Abs(player.transform.position.x - newEnemy.transform.position.x) < 180)
                    {
                        if (player.transform.position.x - newEnemy.transform.position.x < 0)
                        {
                            newEnemy.transform.position += new Vector3(Mathf.Abs(player.transform.position.x - newEnemy.transform.position.x) - 180, 0, 0) * -1;
                        }
                        else
                        {
                            newEnemy.transform.position += new Vector3(Mathf.Abs(player.transform.position.x - newEnemy.transform.position.x) - 180, 0, 0);
                        }
                    }
                    newEnemy.transform.SetParent(GameObject.Find("SpaceObjects").transform);
                    enemyList.Add(newEnemy);

                    var newEnemyOnMap = Instantiate(enemyOnMap, newEnemy.transform.position / 3f * (Screen.height / 600f) + map.transform.position, Quaternion.identity);
                    newEnemyOnMap.transform.SetParent(map.transform);
                    enemyOnMapList.Add(newEnemyOnMap);
                }
            }
            if (killedEnemies >= enemyAmount)
            {
                //Every other level will have a boss battle in it
                if ((level) % 2 != 0)
                {
                    bossSpawned = false;
                    ScoreScreen();
                }
                else if (!bossSpawned)
                {
                    bossSpawned = true;
                    var newboss = Instantiate(boss, new Vector3(player.transform.position.x + 100, -20, 0), Quaternion.identity);
                    newboss.transform.SetParent(GameObject.Find("SpaceObjects").transform);
                }
            }
            //Updating the position of the enemies on the map
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i].GetComponent<EnemyScript>().Abducting)
                {
                    if (enemyOnMapList[i].GetComponent<Image>().color != Color.red)
                        enemyOnMapList[i].GetComponent<Image>().color = Color.red;
                }
                else
                {
                    if (enemyOnMapList[i].GetComponent<Image>().color != Color.white)
                        enemyOnMapList[i].GetComponent<Image>().color = Color.white;
                }
                if (enemyList[i].GetComponent<EnemyScript>()._type == 1)
                {
                    if (enemyOnMapList[i].GetComponent<Image>().color != Color.yellow)
                        enemyOnMapList[i].GetComponent<Image>().color = Color.yellow;
                }
                enemyOnMapList[i].transform.position = enemyList[i].transform.position / 3f * (Screen.height / 600f) + map.transform.position;
            }
        }
    }
    public void AddScore(int value)
    {
        currentLevelScore += value;
        scoreCounterText.text = "Score: " + currentLevelScore;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            enemyList = new List<GameObject>();
            enemyOnMapList = new List<GameObject>();
            if (!gameOver)
            {
                menuTimer = 3f;
                inMenu = true;
                GameObject.Find("ScoreControl").GetComponent<ScoreScript>().ShowScore(currentLevelScore, totalScore, killedEnemies, savedAstronauts, bossSpawned);
            }
            else
            {
                GameObject.Find("ScoreControl").GetComponent<ScoreScript>().GameOverScreen(totalScore);
                inMenu = true;
            }
        }
        else if (scene.buildIndex == 0)
        {
            player = GameObject.Find("Player");
            map = GameObject.Find("Map");
            scoreCounterText = GameObject.Find("ScoreCounter").GetComponent<Text>();
            scoreCounterText.text = "Score: " + currentLevelScore;
            bossSpawned = false;
        }
    }
    public void ScoreScreen()
    {
        totalScore += currentLevelScore;
        SceneManager.LoadScene(1);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene(0);
        inMenu = false;
        spawnedEnemies = 0;
        savedAstronauts = 0;
        currentLevelScore = 0;
        level++;
        killedEnemies = 0;
        enemyAmount += 10;
        nextTime = 1f;
    }
    public void GameOver()
    {
        totalScore += currentLevelScore;
        SceneManager.LoadScene(1);
        gameOver = true;
    }
    private void RestartGame()
    {
        SceneManager.LoadScene(0);
        inMenu = false;
        gameOver = false;
        spawnedEnemies = 0;
        savedAstronauts = 0;
        currentLevelScore = 0;
        level = 0;
        killedEnemies = 0;
        enemyAmount = 5;
        nextTime = 1f;
    }
}
