using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public GameObject projectile;
    public GameObject abductObj;
    private GameObject gameCtrl;

    public Sprite mutantSprite;
    private Vector3 direction = new Vector3();

    private float timeToDirChange = 0f;
    private float weaponCooldown = 4f;
    private float explosionTimer = 1.3f;
    private int type = 0;
    private int mutantMoveRandX;
    private int mutantMoveRandY;

    public bool Abducting = false;
    public bool exploding = false;

    //Enemy will change texture if it's type is changed.
    public int _type
    {
        get { return type; }
        set
        {
            type = value;
            if (type == 1)
                GetComponent<SpriteRenderer>().sprite = mutantSprite;
        }
    }


    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Astronaut(Clone)" && type == 0 && col.gameObject.transform.position.y == -90 && !exploding)
        {
            Abducting = true;
            abductObj = col.gameObject;
            abductObj.GetComponent<AstronautScript>().abducted = true;
        }

    }

    void Start()
    {
        gameCtrl = GameObject.Find("GameController");
        // Choose wether the ufo will keep going left or right.
        if (type == 0)
            direction = new Vector3((Random.Range(-1, 1) + 0.5f) * 2, 0, 0);

        player = GameObject.Find("Player");
    }

    void Update()
    {
        if (exploding)
            explosionTimer -= Time.deltaTime;
        if (explosionTimer <= 0.8f && GetComponent<SpriteRenderer>().enabled)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        if (explosionTimer <= 0)
        {
            Destroy(gameObject);
        }

        //The ufo must only move upwards if it is abducting an astronaut
        if (!Abducting)
        {
            // Different simple movement types for different types of enemies.
            switch (type)
            {
                //Basic ufo movement
                case 0:
                    //After a timer, the vertical direction changes.
                    timeToDirChange -= Time.deltaTime;
                    if (timeToDirChange <= 0f)

                    {
                        direction.y = Random.Range(-1, 2);
                        timeToDirChange = Random.Range(1f, 2f);
                    }
                    //If the ufo touches the bottom or the top of the level, it will start going in the opposite direction.
                    if (transform.position.y <= -90)
                    {
                        direction.y = 1;
                    }
                    else if (transform.position.y >= 50)
                    {
                        direction.y = -1;
                    }

                    transform.position += direction * Time.deltaTime * 30;


                    break;


                // Movement for mutant
                case 1:
                    if (timeToDirChange <= 0)
                    {
                        mutantMoveRandX = Random.Range(-20, 21);
                        mutantMoveRandY = Random.Range(-30, 31);
                        timeToDirChange = 1f;
                    }
                    timeToDirChange -= Time.deltaTime;
                    if (Mathf.Abs(transform.position.x - player.transform.position.x) > 20)
                    {
                        if (player.transform.position.x - transform.position.x < 0)
                        {
                            transform.position += new Vector3(-100, 0, 0) * Time.deltaTime;
                        }
                        else
                        {
                            transform.position += new Vector3(100, 0, 0) * Time.deltaTime;
                        }
                        if (player.transform.position.y - transform.position.y < 0)
                        {
                            transform.position += new Vector3(0, -30, 0) * Time.deltaTime;
                        }
                        else
                        {
                            transform.position += new Vector3(0, 30, 0) * Time.deltaTime;
                        }
                    }
                    else
                    {
                        if (player.transform.position.y - transform.position.y < 0)
                        {
                            transform.position += new Vector3(0, -70, 0) * Time.deltaTime;
                        }
                        else
                        {
                            transform.position += new Vector3(0, 70, 0) * Time.deltaTime;
                        }
                        if (player.transform.position.x - transform.position.x < 0)
                        {
                            transform.position += new Vector3(-20, 0, 0) * Time.deltaTime;
                        }
                        else
                        {
                            transform.position += new Vector3(20, 0, 0) * Time.deltaTime;
                        }

                    }
                    transform.position += new Vector3(mutantMoveRandX, mutantMoveRandY, 0) * Time.deltaTime;


                    break;
            }
            //Ufo shoots the player
            if (Vector3.Distance(player.transform.position, transform.position) < 200 && weaponCooldown <= 0 && !exploding)
            {

                var shootangle = Mathf.Atan2(player.transform.position.y - transform.position.y, player.transform.position.x - transform.position.x);
                var newprojectile = Instantiate(projectile, transform.position, Quaternion.Euler(-90, 0, 0));
                newprojectile.transform.SetParent(GameObject.Find("SpaceObjects").transform);
                newprojectile.GetComponent<ProjectileScript>().Direction = new Vector3(Mathf.Cos(shootangle) * 100, Mathf.Sin(shootangle) * 100, 0);
                newprojectile.GetComponent<ProjectileScript>().type = ProjectileType.firedByEnemy;
                weaponCooldown = 4f;
            }
            else if (weaponCooldown > 0)
            {
                weaponCooldown -= Time.deltaTime;
            }
        }
        else
        {
            //if the ufo is abducting, it will go upwards until it reaches the top of the level, the astronaut is then destroyed.
            transform.position += new Vector3(0, 25 * Time.deltaTime, 0);
            abductObj.transform.position = transform.position - new Vector3(0, 30, 0);
            if (transform.position.y > 50)
            {
                Destroy(abductObj);
                Abducting = false;
                type = 1;
                GetComponent<SpriteRenderer>().sprite = mutantSprite;
            }
        }


        //This makes sure the enemy will not go outside the level, and it's position is synced to the level's position.
        if (transform.position.x - Camera.main.transform.position.x < -640)
        {
            transform.position += new Vector3(640 * 2, 0, 0);
        }
        else if (transform.position.x - Camera.main.transform.position.x > 640)
        {
            transform.position -= new Vector3(640 * 2, 0, 0);
        }
    }
}
