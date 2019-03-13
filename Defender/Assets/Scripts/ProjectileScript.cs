using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum ProjectileType { firedByPlayer, firedByEnemy, firedByBoss };
public class ProjectileScript : MonoBehaviour
{
    public Vector3 Direction = new Vector3();
    private GameObject gameCtrl;
    public GameObject explosionEffect;
    public GameObject laserSpawner;

    public ProjectileType type = 0;
    private float lifeTimer = 0f;
    private float maxLife = 0.3f;
    private void Start()
    {
        switch (type)
        {
            case ProjectileType.firedByPlayer:
                maxLife = 0.35f;
                break;
            case ProjectileType.firedByEnemy:
                maxLife = 2f;
                break;
            case ProjectileType.firedByBoss:
                maxLife = 2f;
                break;

        }
        gameCtrl = GameObject.Find("GameController");
    }

    void OnCollisionEnter(Collision col)
    {
        if (type == ProjectileType.firedByPlayer)
        {
            if (col.gameObject.name.Contains("Boss"))
            {
                if (col.gameObject.GetComponent<BossParts>().partType == partTypes.BossShield)
                {
                    Destroy(col.gameObject);
                    Destroy(gameObject);
                }
                if (col.gameObject.GetComponent<BossParts>().partType == partTypes.BossCore)
                {
                    if (!col.gameObject.transform.parent.gameObject.GetComponent<BossScript>().exploding)
                    {
                        var newExplosion = Instantiate(explosionEffect, col.gameObject.transform.position - new Vector3(0, 0, 100), Quaternion.identity);
                        newExplosion.transform.SetParent(col.gameObject.transform);
                        newExplosion.transform.localScale = new Vector3(200, 200, 1);
                        Destroy(gameObject);
                        col.gameObject.transform.parent.gameObject.GetComponent<BossScript>().exploding = true;
                        gameCtrl.GetComponent<GameController>().AddScore(800);
                    }
                }
            }
            if (col.gameObject.name == "EnemyShip(Clone)")
            {
                if (col.gameObject.GetComponent<EnemyScript>().Abducting)
                {
                    if (col.gameObject.GetComponent<EnemyScript>().abductObj.transform.position.y < -70)
                    {
                        col.gameObject.GetComponent<EnemyScript>().abductObj.transform.position = new Vector3(col.gameObject.GetComponent<EnemyScript>().abductObj.transform.position.x, -90, col.gameObject.GetComponent<EnemyScript>().abductObj.transform.position.z);
                    }
                    col.gameObject.GetComponent<EnemyScript>().abductObj.GetComponent<AstronautScript>().abducted = false;
                    col.gameObject.GetComponent<EnemyScript>().Abducting = false;
                }
                if (!col.gameObject.GetComponent<EnemyScript>().exploding)
                {
                    gameCtrl.GetComponent<GameController>().enemyList.Remove(col.gameObject);
                    Destroy(gameCtrl.GetComponent<GameController>().enemyOnMapList[0]);
                    gameCtrl.GetComponent<GameController>().enemyOnMapList.RemoveAt(0);
                    var newExplosion = Instantiate(explosionEffect, col.gameObject.transform.position - new Vector3(0, 0, 100), Quaternion.identity);
                    newExplosion.transform.SetParent(col.gameObject.transform);
                    col.gameObject.GetComponent<EnemyScript>().exploding = true;
                    Destroy(gameObject);
                    if (!gameCtrl.GetComponent<GameController>().bossSpawned)
                    {
                        gameCtrl.GetComponent<GameController>().AddScore(100);
                        gameCtrl.GetComponent<GameController>().killedEnemies++;
                    }
                }
            }
        }
        else if (type == ProjectileType.firedByEnemy || type == ProjectileType.firedByBoss)
        {
            if (col.gameObject.name == "Player")
            {
                col.gameObject.GetComponent<PlayerScript>().TakeDamage();
            }

        }
    }
    void Update()
    {
        if (lifeTimer > maxLife)
        {
            Destroy(gameObject);
        }
        if (type != ProjectileType.firedByBoss)
        {
            transform.position += Direction * Time.deltaTime;
        }
        else
        {
            transform.localScale += new Vector3(50, 0, 0) * Time.deltaTime;
            transform.position = laserSpawner.transform.position;
        }
        lifeTimer += Time.deltaTime;
    }
}
