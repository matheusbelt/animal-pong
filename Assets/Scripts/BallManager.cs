using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    //list of balls that are currently alive
    public List<GameObject> ballsAlive = new List<GameObject>();
    public Player player;
    public GameObject originalBall;


    // Update is called once per frame
    void Update()
    {
       if(ballsAlive.Count == 0 && player.life > 0)
        {
            player.DamagePlayer();
        }
    }

    public void DestroyBall(GameObject ball)
    {
        var targetIndex = ballsAlive.IndexOf(ball);
        Destroy(ball);
        ballsAlive.RemoveAt(targetIndex);
    }
}
