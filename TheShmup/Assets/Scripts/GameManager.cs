/*
 * GAME MANAGER
 *      Usable without using GetComponent, simply use GameManager.refer to access public aspects.
 *      Public Functions:
 *          - IncreaseScore(int): Increases score and kill count.
 *          - ResetCount(): Resets the kill count and allows for another boss spawn.
 *          - ShowRestart(): Changes the game state back to menu.
 *          - Reload(): Changes the game state to game and spawns a player object.
 *      Public Variables:
 *          - refer: A reference to itself, use to access public functions and varibles.
 *          - minSpawnTime / maxSpawnTime: Minimum and maximum spawn times for enemies.
 *          - gameState: Retruns currentGameState without being able to change it.
 *          - GAMESTATE: Enum of different game states.
 *      
 */

using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // Difference Game States
    public enum GAMESTATE
    {
        Menu,   // Menu, (Displays high score and start button)
        Game    // Game, actual game.
    };

    // Use to Access the Manager
    public static GameManager refer;
    
    // References for spawning
    public GameObject enemy;
    public GameObject boss;
    public GameObject player;


    public Player playerInstance;

    // Transform for enemy / boss spawning reference,
    // will spawn enemies on X based on the negative and positive value of the transform.
    public Transform spawner;

    // Reference for UI
    public Text currScore;
    public GameObject menuScore;

    // Enemy spawn times
    public float minSpawnTime = 1.0f;
    public float maxSpawnTime = 5.0f;

    // Count of enemies killed
    private int needCount = 20;
    private int count = 0;
    private float spawnTimer = 0;
    private float nextSpawn = 0;

    // Score
    private int score = 0;
    private int highScore = 0;
    int nextUpgrade = 100;

    // Spawn and game control
    private bool bossSpawned = false;
    public GAMESTATE currentGameState = GAMESTATE.Menu;
    // Public access to game state
    public GAMESTATE gameState
    {
        get { return currentGameState; }
    }
    
    void Awake()
    {
        // Checks if refer is NULL
        if (refer == null)
        {
            // Sets refer to this
            refer = this;
        }
        // Checks if refer refers to this
        else if (refer != this)
        {
            // Removes the gameobject
            Destroy(gameObject);
        }
                
        // Loads the last high score if one Score.dat exists
        if (File.Exists(Application.persistentDataPath + "/Score.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/Score.dat", FileMode.Open);
            highScore = (int)bf.Deserialize(file);
            file.Close();
        }

        // Shows the menu
        ShowRestart();
    }

    void OnApplicationQuit()
    {
        // Saves the current High Score
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/Score.dat");
                
        bf.Serialize(file, highScore);
        file.Close();
    }

    void Update()
    {
        // Checks game state
        if (currentGameState == GAMESTATE.Game)
        {
            // Update the score text (with at least 5 digits)
            string txt = "Score: ";
            if (score <= 9)
                txt += "0000";
            else if (score <= 99)
                txt += "000";
            else if (score <= 999)
                txt += "00";
            else if (score <= 9999)
                txt += "0";
            currScore.text = txt + score.ToString();

            // Resets the game if the player is dead
            if (playerInstance == null)
            {
                ShowRestart();
            }

            // Check current enemy count for boss spawning
            if (count < needCount)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer > nextSpawn)
                {
                    // Spawns Enemy
                    Instantiate(enemy, new Vector3(Random.Range(-3, 3), spawner.transform.position.y, spawner.transform.position.z), Quaternion.Euler(0, 180, 0));
                    nextSpawn = Random.Range(minSpawnTime, maxSpawnTime);
                    spawnTimer = 0;
                }
            }
            // Checks if Boss is spawned
            else if (count >= needCount && !bossSpawned)
            {
                // Waits until all enemies are killed
                if (GameObject.FindGameObjectWithTag("Enemy") == null)
                {
                    // Spawns boss after a 2 second delay
                    if (spawnTimer > 2)
                    {
                        Debug.Log("Boss Spawned");
                        // Spawns boss
                        Instantiate(boss, new Vector3(0, spawner.position.y, spawner.position.z), Quaternion.Euler(0, 180, 0));
                        // stop Boss from spawning again
                        bossSpawned = true;
                    }
                    spawnTimer += Time.deltaTime;
                }
            }
            // Checks if the boss has been spawned, and no enemies now exist in the scene
            else if (bossSpawned && GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                // Reset values
                spawnTimer = 0;
                count = 0;
                bossSpawned = false;
            }
        }
    }

    // Increases score and count (Called by enemy and boss when killed)
    public void IncreaseScore(int amount)
    {
        // Checks if games running
        if (currentGameState == GAMESTATE.Game)
        {
            // Increases the score
            score += amount;
            // Increases the count
            count++;
            //every hundred points you get an upgrade
            if (nextUpgrade <= score)
            {
                playerInstance.UpgradeGuns();
                nextUpgrade += 100;
            }
        }
    }

    // Shows the top score and reload button
    public void ShowRestart()
    {
        // Checks game state
        if (currentGameState == GAMESTATE.Game)
        {
            // Switches game state
            currentGameState = GAMESTATE.Menu;

            // Checks and set new high scores
            if (score > highScore)
            {
                highScore = score;
            }

            // Sets the score text on the menu (with at least 5 digits)
            string txt = "High Score:\n";
            if (highScore <= 9)
                txt += "0000";
            else if (highScore <= 99)
                txt += "000";
            else if (highScore <= 999)
                txt += "00";
            else if (highScore <= 9999)
                txt += "0";
            menuScore.GetComponent<Text>().text = txt + highScore.ToString();

            // Change what on the canvas is showing
            currScore.enabled = false;
            menuScore.SetActive(true);
        }
    }

    // Reloads the scene (Called by button)
    public void Reload()
    {
        // Checks game state
        if (currentGameState == GAMESTATE.Menu)
        {
            // Reset variables
            score = 0;
            count = 0;
            needCount = 20;

            // Change what on the canvas is showing
            currScore.enabled = true;
            menuScore.SetActive(false);

            // Reload player
            playerInstance = Instantiate(player).GetComponent<Player>();

            // Switch games state
            currentGameState = GAMESTATE.Game;
        }
    }
}