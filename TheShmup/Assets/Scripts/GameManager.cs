/*
 * 
 * NOTE:
 *      This script is still a work in progress, however
 *      the IncreaseScore function is needed for the Enemy script.
 *      
 */

using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // Use to Access the Manager
    public static GameManager refer;
    
    // References for spawning
    public GameObject enemy;
    public GameObject boss;
    public GameObject player;
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

    // Spawn and game control
    private bool bossSpawned = false;
    private bool isGameActive = false;
    // Public access to check game state
    public bool isGameRunning
    {
        get { return isGameActive; }
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
        if (isGameActive)
        {
            // Update Score text
            string txt = "Score: ";
            if (highScore < 10)
                txt += "0000";
            else if (highScore < 100)
                txt += "000";
            else if (highScore < 1000)
                txt += "00";
            else if (highScore < 10000)
                txt += "0";
            currScore.text = txt + score.ToString();

            // Check current enemy count for boss spawning
            if (count < needCount)
            {
                spawnTimer += Time.deltaTime;
                if (spawnTimer > nextSpawn)
                {
                    // Spawns Enemy
                    Instantiate(enemy, new Vector3(Random.Range(-spawner.transform.position.x, spawner.transform.position.x), 0, spawner.transform.position.z), Quaternion.Euler(180, 0, 0));
                    nextSpawn = Random.Range(minSpawnTime, maxSpawnTime);
                    spawnTimer = 0;
                }
            }
        }
    }

    // Increases score and count (Called by enemy when hurt)
    public void IncreaseScore(int amount)
    {
        // Checks if games running
        if (isGameActive)
        {
            // Increases the score
            score += amount;
            // Increases the count
            count++;
        }
    }

    // Shows the top score and reload button (Called by player when hurt)
    public void ShowRestart()
    {
        // Switches game state
        isGameActive = false;

        if (score > highScore)
        {
            highScore = score;
        }

        // Sets the score text on the menu (with at least 5 digits)
        string txt = "High Score:\n";
        if (highScore < 10)
            txt += "0000";
        else if (highScore < 100)
            txt += "000";
        else if (highScore < 1000)
            txt += "00";
        else if (highScore < 10000)
            txt += "0";
        menuScore.GetComponent<Text>().text = txt + highScore.ToString();

        currScore.enabled = false;
        menuScore.SetActive(true);
    }

    // Reloads the scene (Called by button)
    public void Reload()
    {
        // Reset variables
        score = 0;
        count = 0;
        needCount = 20;

        currScore.enabled = true;
        menuScore.SetActive(false);

        // Reload player
        Instantiate(player);

        // Switch games state
        isGameActive = true;
    }
}