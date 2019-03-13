using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    public Texture2D BossArchitechture;
    private Color[] BossArchitechtureColors = new Color[16 * 16];
    private List<GameObject> bossParts = new List<GameObject>();
    public List<Texture2D> partTextureList = new List<Texture2D>();

    public GameObject player;
    public GameObject bossPart;
    private GameObject gameCtrl;

    private float explosionTimer = 2f;
    private float movePhase = 90 * Mathf.Deg2Rad;
    private float attackTimer = 4f;

    private int attackType = 0;
    public bool exploding = false;

    private void UfoAttack()
    {
        foreach (GameObject bossObj in bossParts)
        {

            if (bossObj.GetComponent<BossParts>().partType == partTypes.UfoSpawner)
            {
                bossObj.GetComponent<BossParts>().SpawnUfo();
            }
        }
    }
    private void ShootLaser()
    {
        foreach (GameObject bossObj in bossParts)
        {
            if (bossObj.GetComponent<BossParts>().partType == partTypes.BossLaser)
            {
                bossObj.GetComponent<BossParts>().FireLaser();
            }
        }
    }


    void Start()
    {
        gameCtrl = GameObject.Find("GameController");
        player = GameObject.Find("Player");

        BossArchitechtureColors = BossArchitechture.GetPixels();

        for (int x = 0; x < 16; x++)
        {
            for (int y = 0; y < 16; y++)
            {
                //Instantiate boss ship parts in the correct arrangement as drawn in the BossArchitechture image
                if (BossArchitechtureColors[x + y * 16].r == 1)
                {
                    var obj = Instantiate(bossPart, new Vector3(x * 10, y * 10 - 50, 0) + transform.position, Quaternion.Euler(-90, 0, 0));
                    obj.transform.SetParent(transform);
                    obj.GetComponent<Renderer>().material.mainTexture = partTextureList[0];
                    obj.GetComponent<BossParts>().partType = partTypes.BossCore;

                }
                else if (BossArchitechtureColors[x + y * 16].g == 1)
                {
                    var obj = Instantiate(bossPart, new Vector3(x * 10, y * 10 - 50, 0) + transform.position, Quaternion.Euler(-90, 0, 0));
                    obj.transform.SetParent(transform);
                    obj.GetComponent<Renderer>().material.mainTexture = partTextureList[1];
                    obj.GetComponent<BossParts>().partType = partTypes.BossShield;

                }
                else if (BossArchitechtureColors[x + y * 16].b == 1)
                {
                    var obj = Instantiate(bossPart, new Vector3(x * 10, y * 10 - 50, 0) + transform.position, Quaternion.Euler(-90, 0, 0));
                    obj.transform.SetParent(transform);
                    obj.GetComponent<Renderer>().material.mainTexture = partTextureList[2];
                    obj.GetComponent<BossParts>().partType = partTypes.UfoSpawner;
                    bossParts.Add(obj);

                }
                else if ((int)(BossArchitechtureColors[x + y * 16].r * 255) == 127)
                {
                    var obj = Instantiate(bossPart, new Vector3(x * 10, y * 10 - 50, 0) + transform.position, Quaternion.Euler(-90, 0, 0));
                    obj.transform.SetParent(transform);
                    obj.GetComponent<Renderer>().material.mainTexture = partTextureList[3];
                    obj.GetComponent<BossParts>().partType = partTypes.BossLaser;
                    bossParts.Add(obj);

                }

            }
        }
    }


    void Update()
    {
        if (exploding)
        {
            transform.position -= new Vector3(0, 30, 0) * Time.deltaTime;
            explosionTimer -= Time.deltaTime;
            if (explosionTimer <= 0)
            {
                Destroy(gameObject);

                gameCtrl.GetComponent<GameController>().enemyList.Clear();
                gameCtrl.GetComponent<GameController>().enemyOnMapList.Clear();
                gameCtrl.GetComponent<GameController>().ScoreScreen();
            }
        }
        else
        {
            attackTimer += 1f * Time.deltaTime;
            movePhase += 1f * Time.deltaTime;
            transform.position += new Vector3(Mathf.Cos(movePhase) * -3, Mathf.Sin(movePhase), 0) * Time.deltaTime * 40f;
            if (attackTimer >= 5f)
            {
                if (attackType == 0)
                {
                    UfoAttack();
                    attackType = 1;
                }
                else
                {
                    ShootLaser();
                    attackType = 0;
                }
                attackTimer = 0;
            }

        }
        //This makes sure the boss will not go outside the level, and it's position is synced to the level's position.
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
