using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class obstacleManager : MonoBehaviour
{

    [SerializeField] protected Ball ballClass;
    [SerializeField] protected Bomb bombClass;

    [Header("Power ups")]
    //intanciate variables
    GameObject newBall;
    protected GameObject currentBall;
    public BallManager ballManager;
    public float radius;
    public List<GameObject> bombRadius = new List<GameObject>();

    [Header("Create Map")]
    public int maxPowerUps = 6;
    public List<GameObject> powerUpsList = new List<GameObject>();
    [SerializeField] private Transform _spawnBoxReference;
    [SerializeField] private GameObject _box;
    [SerializeField] private int _line;
    [SerializeField] private float _gap;
    [SerializeField] private int _spriteOrder;

    [Header("Timer")]
    public float _timer;
    public float _moveInterval;
    [SerializeField] private TextMeshProUGUI _timerText;



    private void Start()
    {
        RandomizePowerUps();
        _timer = _moveInterval;
    }

    private void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _timer = _moveInterval;
            SpawnBoxLine();
            MoveObjectsDown();
        }
        UpdateTimerText();
    }


    #region MAP

    //Create a script that will add more boxes on top of the top boxes
    //it has to change the sorting layer so it's not on top
    //maybe the boxes will drop and will lose rigid body once they stop? Or they will spawn on top regardless?

    public void SpawnBoxLine()
    {
        var spawnLine = _line;

        var initialPositionX = _spawnBoxReference.position.x;
        var initialPositionY = _spawnBoxReference.position.y + 2;


        while (spawnLine > 0)
        {

            var initialPosition = new Vector2(initialPositionX, initialPositionY);
            var newSquare = Instantiate(_box, initialPosition, _box.transform.rotation, gameObject.transform);
            newSquare.AddComponent<Rigidbody2D>();
            var rigid = newSquare.GetComponent<Rigidbody2D>();
            rigid.gravityScale = 0.3f;
            rigid.mass = 0;

            //rigid.maxDepenetrationVelocity = 1;

            //change the bool to make the collision code on squareObstacle work
            newSquare.GetComponent<SquareObstacle>()._hasRigidBody = true;

            var renderer = newSquare.GetComponent<SpriteRenderer>();
            renderer.sortingOrder = _spriteOrder;

            initialPositionX += _gap;
            spawnLine--;
        }
        _spriteOrder--;
        MoveObjectsDown();
    }

    void MoveObjectsDown()
    {
        var gapMove = _gap / 2;
        // Move objects in "obstacles" layer down
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Obstacle"))
        {
            obj.transform.Translate(Vector2.down * gapMove);
        }

        // Move objects in "animals" layer down
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Animal"))
        {
            obj.transform.Translate(Vector2.down * gapMove);
        }
    }

    private void UpdateTimerText()
    {
        if (_timerText != null)
        {
            int seconds = Mathf.FloorToInt(_timer);
            _timerText.text = "Timer: " + seconds.ToString();
        }
    }


    private void RandomizePowerUps()
    {
        while (maxPowerUps > 0)
        {
            GameObject[] squareArray = GameObject.FindGameObjectsWithTag("Obstacle");
            var randomNumber = Random.Range(0, squareArray.Length);
            var chosenSquare = squareArray[randomNumber];

            var randomPowerUp = Random.Range(0, powerUpsList.Count);
            var powerUpChosen = powerUpsList[randomPowerUp];
            var powerUp = Instantiate(powerUpChosen, chosenSquare.transform.position, chosenSquare.transform.rotation, gameObject.transform);
            
            Destroy(squareArray[randomNumber]);

            maxPowerUps--;
        }

    }

    

    #endregion



    #region PowerUPS & Collisions
    //function to access the child squares to know if they collided or not
    public void CollisionDetected(SquareObstacle squareObstacle, GameObject ball)
    {
        currentBall = ball;
        if (squareObstacle.tag == "Obstacle")
        {
            Destroy(squareObstacle.gameObject);
        }

        if (squareObstacle.tag == "Bomb")
        {
            ExplosionRadius(squareObstacle.gameObject.transform.position, radius, squareObstacle.gameObject);
        }
        if (squareObstacle.tag == "Multiplier")
        {
            BallMultiplier(squareObstacle.gameObject);
            Destroy(squareObstacle.gameObject);
        }
    }


    //bomb function
    void ExplosionRadius(Vector2 center, float radius, GameObject bomb)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);
        var bombFired = bomb.GetComponent<Bomb>();
        bombFired.isFired = true;

        //add everything to a list and clean the list
        foreach (Collider2D col in hitColliders)
        {
            if (col.gameObject.tag != "Ball" && col.gameObject.tag != "Wall" && col.gameObject.tag != "Animal")
            {
                if(bombRadius.Contains(col.gameObject) == false)
                {
                    bombRadius.Add(col.gameObject);
                }
            }
        }
        ExplosionDamage();
    }

    public void ExplosionDamage()
    {
        //run through the bomb radius list that was created to find other bombs or other powerups
        //col = the square that the ball hit
        foreach (var col in bombRadius)
        {
            if (col.gameObject.tag == "Bomb")
            {
                var bombScript = col.GetComponent<Bomb>();
                if (bombScript.isFired == false)
                {
                    bombScript.isFired = true;
                    ExplosionRadius(col.gameObject.transform.position, radius, col.gameObject);
                    return;
                }
            }
            else if (col.gameObject.tag == "Multiplier")
            {
                BallMultiplier(col);
            }
            Destroy(col.gameObject);
        }
        bombRadius.Clear();
    }


    //when hitting a ball multiplier new balls arrive from that point down with random X directions
    public void BallMultiplier(GameObject obj)
    {
        var objScript = obj.GetComponent<BallMultiplier>();
        var range = Random.Range(2, 5);
        while (range > 0)
        {
            if (objScript.isFired == false)
            {
                //instanciate new balls, random from 2 to 4
                //tenho que pegar a variavel da bola que tocou no quadrado em questao p se a bola original morrer nao dar erro
                newBall = Instantiate(currentBall);
                ballManager.ballsAlive.Add(newBall);
                StartCoroutine(ballClass.BallStart(newBall));
            }
            range--;
        }
        objScript.isFired = true;
    }

    #endregion
}
