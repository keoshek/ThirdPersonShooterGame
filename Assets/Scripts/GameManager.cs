using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject[] enemy;
    [SerializeField] Button restartButton;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI livesText;
    int score;
    int highScore;
    int livesLeft;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        StartCoroutine(EnemySpawn());
        score = 0;
        livesLeft = 3;
        UpdateScore(0);
        UpdateLive(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator EnemySpawn()
    {
        yield return new WaitForSeconds(2);
        int index = Random.Range(0, enemy.Length);
        Vector3 position = new Vector3(Random.Range(25f, 110f), 0.6f, Random.Range(-60f, 15f));
        Instantiate(enemy[index], position, enemy[index].transform.rotation);
        StartCoroutine(EnemySpawn());
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("highScore");
    }

    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        Time.timeScale = 0f;
        SaveHighScore();
    }

    void SaveHighScore()
    {
        if (PlayerPrefs.HasKey("highScore"))
        {
            if (score > PlayerPrefs.GetInt("highScore"))
            {
                highScore = score;
                PlayerPrefs.SetInt("highScore", highScore);
                PlayerPrefs.Save();
            }
        }
        else
        {
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("highScore", highScore);
                PlayerPrefs.Save();
            }
        }
    }

    public void UpdateLive(int lifeToAdd)
    {
        livesLeft += lifeToAdd;
        livesText.text = "Lives: " + livesLeft;
        if (livesLeft == 0)
        {
            GameOver();
        }
    }
}
