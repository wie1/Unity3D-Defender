using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    //Multiplier of the spaceship's speed
    public float playerSpeed = 1;

    //Spaceship controls
    private float moveStrength = 0;
    private float moveAngle = 0;
    public GameObject joystick;

    //Current velocity of the spaceship
    private Vector3 playerVelocity = new Vector3();

    //The amount that the spaceship's velocity will change
    private Vector3 velocityDelta = new Vector3();

    public GameObject playerCamera;
    public GameObject level;
    public GameObject projectile;
    public GameObject explosionEffect;
    public GameObject lifeBG;
    public GameObject mapObject;
    public GameObject map;
    private GameObject playerOnMap;
    private GameObject gameCtrl;
    private int HP = 3;
    private float immortalityTimer = 0f;
    private float explosionTimer = 1.3f;

    private bool controlling = false;
    private bool exploding = false;
    public bool immortal = false;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Astronaut(Clone)")
        {
            if (col.gameObject.transform.position.y > -90 && !col.gameObject.GetComponent<AstronautScript>().abducted)
            {
                Destroy(col.gameObject);
                gameCtrl.GetComponent<GameController>().AddScore(200);
                gameCtrl.GetComponent<GameController>().savedAstronauts++;
            }
        }
        if (col.gameObject.name == "EnemyShip(Clone)")
        {
            if (!col.gameObject.GetComponent<EnemyScript>().exploding)
            TakeDamage();
        }
        if (col.gameObject.name == "Bosspart(Clone)")
        {
            if (!col.gameObject.transform.parent.GetComponent<BossScript>().exploding)
            {
                TakeDamage();
                if (col.gameObject.GetComponent<BossParts>().partType == partTypes.BossCore)
                {
                    exploding = true;
                    var newExplosion = Instantiate(explosionEffect, transform.position - new Vector3(0, 0, 100), Quaternion.identity);
                    newExplosion.transform.SetParent(transform);
                } 
            }
        }

    }
    public void FireLaser()
    {
        var newProjectile = Instantiate(projectile, transform.position, Quaternion.Euler(-90, 0, 0));
        //Decide whether the projectile should go left or right based on the ship's movement
        if (velocityDelta.x < 0)
        {
            newProjectile.GetComponent<ProjectileScript>().Direction = new Vector3(-700, 0, 0);
        }
        else
        {
            newProjectile.GetComponent<ProjectileScript>().Direction = new Vector3(700, 0, 0);
        }
    }
    void Start()
    {
        gameCtrl = GameObject.Find("GameController");
        playerOnMap = Instantiate(mapObject, transform.position/3 * (Screen.height / 600f) + map.transform.position,Quaternion.identity);
        playerOnMap.transform.SetParent(map.transform);
        playerOnMap.GetComponent<Image>().color = Color.green;
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
            gameCtrl.GetComponent<GameController>().GameOver();
        }

        if (immortal)
        {
            immortalityTimer -= Time.deltaTime;
            if (immortalityTimer <= 0)
            {
                GetComponent<SpriteRenderer>().enabled = true;
                immortal = false;
            }
            else
            {
                if (Mathf.RoundToInt(immortalityTimer * 8) % 2 == 0)
                {
                    GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
        
        //Check if the player starts controlling the ship
        
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (Vector2.Distance(Input.mousePosition, joystick.transform.position) < 50)
            {
                controlling = true;
            }
        }

        /*
        ////////////////////////JOYSTICK CONTROLS////////////////////////

        if (Input.GetKeyUp(KeyCode.Mouse0))
            controlling = false;

        //Check if the player stops controlling the ship
        if (controlling == false)
        {
            joystick.transform.position = joystick.transform.parent.position;
        }

        if (controlling)
        {
            //Calculate the angle and strength based on the positions of the joysticks default position (transform.parent) and the mouse position
            moveStrength = Vector2.Distance(joystick.transform.parent.position, Input.mousePosition);
            moveAngle = Mathf.Atan2(Input.mousePosition.y - joystick.transform.parent.position.y, Input.mousePosition.x - joystick.transform.parent.position.x);
            //Limit the joystick's distance from it's default position.
            if (moveStrength > 100)
            {
                moveStrength = 100;
            }
            joystick.transform.position = new Vector2(Mathf.Cos(moveAngle) * moveStrength + joystick.transform.parent.position.x, Mathf.Sin(moveAngle) * moveStrength + joystick.transform.parent.position.y);

            playerVelocity += new Vector3((joystick.transform.position.x - joystick.transform.parent.position.x) / 3f * Time.deltaTime, (joystick.transform.position.y - joystick.transform.parent.position.y)*4f * Time.deltaTime,0) * 7.3f;
            velocityDelta = new Vector3((joystick.transform.position.x - joystick.transform.parent.position.x) / 3f * Time.deltaTime, (joystick.transform.position.y - joystick.transform.parent.position.y)*4f * Time.deltaTime, 0) * 7.2f;
        }
        */


        ////////////////////////KEYBOARD CONTROLS////////////////////////
        if (Input.GetKey(KeyCode.W))
        {
            playerVelocity.y += 3000 * Time.deltaTime;
            velocityDelta.y = 3000 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerVelocity.y -= 3000 * Time.deltaTime;
            velocityDelta.y = -3000 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            playerVelocity.x += 300 * Time.deltaTime;
            velocityDelta.x = 200 * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            playerVelocity.x -= 300 * Time.deltaTime;
            velocityDelta.x = -200 * Time.deltaTime;
        }
        //Firing a laser
        if (Input.GetKeyDown(KeyCode.Space))
            FireLaser();

        playerVelocity.x = Mathf.Clamp(playerVelocity.x, -200, 200);
        playerVelocity.y = Mathf.Clamp(playerVelocity.y, -150, 150);

        //Reduce the spaceship's velocity
        if (playerVelocity.x < 0)
        {
            playerVelocity.x += 80 * Time.deltaTime * playerSpeed;
            playerVelocity.x = Mathf.Clamp(playerVelocity.x, -200, 0);
        }
        if (playerVelocity.x > 0)
        {
            playerVelocity.x -= 80 * Time.deltaTime * playerSpeed;
            playerVelocity.x = Mathf.Clamp(playerVelocity.x, 0, 200);
        }
        if (playerVelocity.y < 0)
        {
            playerVelocity.y += 1000 * Time.deltaTime * playerSpeed;
            playerVelocity.y = Mathf.Clamp(playerVelocity.y, -120, 0);
        }
        if (playerVelocity.y > 0)
        {
            playerVelocity.y -= 1000 * Time.deltaTime * playerSpeed;
            playerVelocity.y = Mathf.Clamp(playerVelocity.y, 0, 120);
        }

        //If as a result the ship will not move out of the screen, move the player according to vertical velocity
        if (transform.position.y + playerVelocity.y * Time.deltaTime < 50 && transform.position.y + playerVelocity.y * Time.deltaTime > -90)
        {
            transform.position += new Vector3(0, playerVelocity.y) * Time.deltaTime;
        }

        moveLevelLayers(new Vector3(-playerVelocity.x, 0) * Time.deltaTime);
        
        //If ship changes direction, move the ship and level accordingly
        if (velocityDelta.x < 0 && transform.position.x < 120)
        {
            transform.localScale = new Vector3(-12, 14, 1);
            transform.position += new Vector3(Mathf.Abs(velocityDelta.x) * (120 - transform.position.x) / 2f + 50, 0) * Time.deltaTime;

        moveLevelLayers(new Vector3(Mathf.Abs(velocityDelta.x) * (120 - transform.position.x) / 2f + 50, 0) * Time.deltaTime);
        } else if (velocityDelta.x > 0 && transform.position.x > -120)
        {
            transform.localScale = new Vector3(12, 14, 1);
            transform.position += -new Vector3(Mathf.Abs(velocityDelta.x) * (transform.position.x + 120) /2f + 50, 0) * Time.deltaTime;
            moveLevelLayers(-new Vector3(Mathf.Abs(velocityDelta.x) * (transform.position.x + 120) / 2f + 50, 0) * Time.deltaTime);
        }

        playerOnMap.transform.position = transform.position / 3 * (Screen.height / 600f) + map.transform.position;
    }

    public void TakeDamage()
    {
        if (!immortal)
        {
            if (HP > 0)
            {
                Destroy(lifeBG.transform.GetChild(0).gameObject);
                HP--;
                immortal = true;
                immortalityTimer = 2f;
            }
            else
            {
                if (!exploding)
                {
                    exploding = true;
                    var newExplosion = Instantiate(explosionEffect, transform.position - new Vector3(0, 0, 100), Quaternion.identity);
                    newExplosion.transform.SetParent(transform);
                }
            }
        }
    }

    private void moveLevelLayers(Vector3 layerMovement)
    {
        for (int i = 0; i < level.transform.childCount; i++)
        {
            if (i < 2)
            {
                level.transform.GetChild(i).position += layerMovement;
            } else
            {
                level.transform.GetChild(i).position += layerMovement * 1.25f;
            }
        }
    }
}
