using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum partTypes { BossShield, UfoSpawner, BossLaser, BossCore };

public class BossParts : MonoBehaviour
{
    public GameObject enemy;
    public GameObject laser;
    private GameObject gameCtrl;
    public partTypes partType = 0;

    void Start()
    {
        gameCtrl = GameObject.Find("GameController");
    }
    public void FireLaser()
    {
        var newLaser = Instantiate(laser, transform.position, Quaternion.Euler(-90, 0, 0));
        newLaser.GetComponent<ProjectileScript>().laserSpawner = gameObject;
        newLaser.GetComponent<ProjectileScript>().type = ProjectileType.firedByBoss;
    }
    public void SpawnUfo()
    {
        var newEnemy = Instantiate(enemy, transform.position, Quaternion.identity);
        newEnemy.transform.SetParent(GameObject.Find("SpaceObjects").transform);
        newEnemy.GetComponent<EnemyScript>()._type = 1;
        gameCtrl.GetComponent<GameController>().enemyList.Add(newEnemy);


        var newEnemyOnMap = Instantiate(gameCtrl.GetComponent<GameController>().enemyOnMap, newEnemy.transform.position / 7f + GameObject.Find("Map").transform.position, Quaternion.identity);
        newEnemyOnMap.transform.SetParent(GameObject.Find("Map").transform);
        gameCtrl.GetComponent<GameController>().enemyOnMapList.Add(newEnemyOnMap);
    }
}
