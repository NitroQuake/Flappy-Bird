using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject pipes;
    private Vector2 pos = new Vector2(10, 0);
    [SerializeField] GameObject canvas;
    private TextMeshProUGUI scoreText;
    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GameObject.Find("Score").GetComponent<TextMeshProUGUI>();
        Time.timeScale = 1;
        InvokeRepeating("spawnPipes", 0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnPipes()
    {
        float y = Random.Range(-2.5f, 2.5f);
        pos.y = y;
        Instantiate(pipes, pos, pipes.transform.rotation);
    }

    public void updateScore()
    {
        score++;
        scoreText.SetText(score.ToString());
    }

    public void turnOnCanvas()
    {
        canvas.SetActive(true);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
