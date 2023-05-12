using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb2D;
    public float speed;
    

    public GameObject platform;

    public bool start = false;

   /* //list of balls that are currently alive
    public List<GameObject> ballsAlive = new List<GameObject>();*/

    public BallManager manager;


    private void Update()
    {
        //fixes velocity if ball is too slow
        //FixVelocity();

        //checks if ball is out of the screen and destroys it
        if (gameObject.transform.position.y < -6)
        {
            manager.DestroyBall(gameObject);
        }

    }

   

    //start to move the ball
    public IEnumerator BallStart(GameObject ball)
    {
        int speedX = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
        Rigidbody2D _rigid;

        _rigid = ball.GetComponent<Rigidbody2D>();
        _rigid.velocity = new Vector2(speed * speedX, speed);

        yield return new WaitForEndOfFrame();

        ball.GetComponent<Ball>().start = true;

        yield break;
    }

    //Not letting it go out of the zone
    private void OnCollisionEnter2D(Collision2D collision)
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        if(collision.gameObject.tag != "Ball")
        {
            rb2D = GetComponent<Rigidbody2D>();
            if (rb2D.velocity.y == 0)
            {
                rb2D.velocity += new Vector2(rb2D.velocity.x, speed / 3);
            }
        }
        //ignore collision with other balls
        else
        {
            Collider2D collider2D = GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(collider2D, collision.collider);
        }
    }

    //if the ball is to slow increase speed
    private void FixVelocity()
    {
        var ballVelY = gameObject.GetComponent<Rigidbody2D>().velocity;
        /*if (ballVelY.y > 0 && ballVelY.y < speed)
        {
            ballVelY = new Vector2(ballVelY.x, speed);
        }
        else if (ballVelY.y < 0 && ballVelY.y > -speed)
        {
            ballVelY = new Vector2(ballVelY.x, -speed);
        }*/
        if(ballVelY.magnitude < 10)
        {
            Debug.Log("magnitude");
            ballVelY = new Vector2(ballVelY.x * 2, ballVelY.y * 2);
        }
    }
}
