using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{

    [SerializeField] private Player platform;
    [SerializeField] private bool rescue = false;

    private void Update()
    {
        if (this.rescue)
        {
            Rescued();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Platform")
        { 
            this.rescue = true;
            var rigidBody = GetComponent<Rigidbody2D>();
            var collider = GetComponent<Collider2D>();
            Destroy(rigidBody);
            Destroy(collider);

            GameObject[] rescuedAnimals = GameObject.FindGameObjectsWithTag("Animal");
            for(int i = 0; i < rescuedAnimals.Length; ++i)
            {
                var rescue = rescuedAnimals[i].GetComponent<Animal>().rescue;
                if (rescue)
                    platform.animalsRescued++;
            }
        }
    }

    //glue to platform
    private void Rescued()
    {
        var positionX = platform.transform.position.x;
        var positionY = platform.transform.position.y;
        gameObject.transform.position = new Vector2(positionX + 0.3f, positionY + 0.6f);
        gameObject.transform.rotation = Quaternion.identity;
    }
}
