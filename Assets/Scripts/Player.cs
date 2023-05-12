using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("Game Mechanics")]
    public float speed;
    public int life = 3;
    public int animalLevelCount;
    public int minimalAnimalsRescued;
    public int animalsRescued;

    private Rigidbody2D _platformBody;
    public Ball ball;
    public obstacleManager om;

    protected GameObject ballObject;
    public GameObject ballPrefab;

    protected Vector2 ballPosition;

    public BallManager ballManager;

    [SerializeField] private float downTest;
    [SerializeField] private float upTest;
    private Vector2 startPosition;


    private void Start()
    {
        _platformBody = GetComponent<Rigidbody2D>();

        //get ball game object
        ballObject = GameObject.Find("Ball");

        //get start position
        startPosition = transform.position;

        BallSpawn(ballObject);
    }

    // Update is called once per frame
    void Update()
    {
        #region CONTROLS
        //move left
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _platformBody.velocity = new Vector2(-speed, 0);
        }

        if(Input.GetKeyUp(KeyCode.LeftArrow)) 
        {
            _platformBody.velocity = new Vector2(0, 0);
        }

        //move right
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _platformBody.velocity = new Vector2(speed, 0);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            _platformBody.velocity = new Vector2(0, 0);
        }

        //ball start
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            StartCoroutine(ball.BallStart(ballObject));
        }


        //control for testing stuff
        if (Input.GetKeyUp(KeyCode.F))
        {
            om.SpawnBoxLine();
        }


        #endregion

        //Re-start Ball + game over
        if (ballObject.GetComponent<Ball>().start == false && life > 0)
        {
            ballObject.transform.position = new Vector2(transform.position.x, transform.position.y + 0.3f);
        }
        else if (life <= 0)
        {
            //game over
            Debug.Log("game over");
        }

        //victory
        if(minimalAnimalsRescued <= animalsRescued)
        {
            //victory
            Debug.Log("Victory!");
        }
    }


    #region COLLISIONS
    //Not letting it go out of the zone
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            _platformBody.velocity = Vector2.zero;
        }

        if (collision.gameObject.tag == "Ball")
        {
            Rigidbody2D _rigid = collision.gameObject.GetComponent<Rigidbody2D>();

            //change speed
            ChangeVelocity(_rigid);

            StartCoroutine(PlatformBounce(collision.gameObject));

            //if the speed is too low, increase the speed of ball
            if (_rigid.velocity.y < -speed/2)
            {
                _rigid.velocity = new Vector2(_rigid.velocity.x + speed, _rigid.velocity.y + speed);
            }

            //if the speed is too high, decrease the speed
            if (_rigid.velocity.magnitude > 15)
            {
                _rigid.velocity = new Vector2(_rigid.velocity.x / 2, _rigid.velocity.y / 2);
            }
        }
    }

    private void ChangeVelocity(Rigidbody2D rigid)
    {
        float speedChange = UnityEngine.Random.Range(-1f, 1f);

        if (rigid.velocity.x < rigid.velocity.y)
        {
            //increase or decrease speed to make the game more dynamic
            rigid.velocity = new Vector2(rigid.velocity.x + speedChange, rigid.velocity.y);
        }
        else
        {
            //increase or decrease speed to make the game more dynamic
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y + speedChange);
        }
    }



    private IEnumerator PlatformBounce(GameObject ball)
    {
        var ballStart = ball.GetComponent<Ball>();
        if (ballStart.start) {
            var down = new Vector2(transform.position.x, downTest);
            transform.Translate(down * Time.deltaTime);

            yield return new WaitForSeconds(0.1f);

            var up = new Vector2(transform.position.x, upTest);
            transform.Translate(up * Time.deltaTime);

            yield return new WaitForEndOfFrame();
            if (transform.position.y != startPosition.y)
            {
                transform.position = new Vector2(transform.position.x, startPosition.y);
            }
            yield break;
        }
        yield break;
    }

    #endregion



    //Spawn the ball
    public void BallSpawn(GameObject spawnBall)
    {
        var ball = Instantiate(spawnBall);
        ballObject = ball;
        //add to the alive list
        ballManager.ballsAlive.Add(ball);
        float positionX = transform.position.x;
        float positionY = transform.position.y;
        ballPosition = ball.transform.position =  new Vector2(positionX, positionY + 0.3f);
    }

    //Player lost life
    public void DamagePlayer()
    {
        //create new ball
        BallSpawn(ballPrefab);
        
        //find it
        var ball = GameObject.Find(ballObject.name) as GameObject;

        //set start to false to follow platform
        ball.GetComponent<Ball>().start = false;

        //damage
        life--;
        Debug.Log(life);
    }
}
