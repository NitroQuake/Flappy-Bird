using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    private Rigidbody2D birdRB;
    [SerializeField] int jumpForce;

    // Start is called before the first frame update
    void Start()
    {
        birdRB = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump");
            birdRB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
