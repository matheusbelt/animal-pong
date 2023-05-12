using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareObstacle : MonoBehaviour
{
    public bool _hasRigidBody = false;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            transform.parent.GetComponent<obstacleManager>().CollisionDetected(this, collision.gameObject);
        }
        else if (collision.gameObject.tag == "Obstacle")
        {
            //this code works on the boxes that are falling from the sky.
            if(collision.gameObject.GetComponent<Rigidbody2D>() != null && !_hasRigidBody)
            {
                ResetBox(collision.gameObject);
            }
        }
    }

    private void ResetBox(GameObject box)
    {
        var rigid = box.GetComponent<Rigidbody2D>();
        Destroy(rigid);
        box.GetComponent<SquareObstacle>()._hasRigidBody = false;
    }
}
