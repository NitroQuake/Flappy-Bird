using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLeft : MonoBehaviour
{
    private Rigidbody2D pipesRB;
    [SerializeField] int speed;
    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        pipesRB = GetComponent<Rigidbody2D>();
        gameController = GameObject.Find("System").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        pipesRB.velocity = Vector2.left * speed;

        if(pipesRB.position.x <= -18.5)
        {
            Destroy(pipesRB.gameObject);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Game Over");
        Time.timeScale = 0;
        gameController.turnOnCanvas();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        gameController.updateScore();
    }

}
