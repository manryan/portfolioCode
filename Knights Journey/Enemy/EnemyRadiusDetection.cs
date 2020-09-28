using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRadiusDetection : MonoBehaviour {

    public Enemy enemy;

    public LayerMask wall;
    void OnTriggerEnter2D(Collider2D c)
    {

        if (c.gameObject.tag == "Player")
        {
            Ray ray = new Ray(transform.position, c.transform.position - transform.position);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Vector3.Distance(c.transform.position,transform.position), wall);
            if (hit.collider == null)
            {
                if (enemy.enemyState != EnemyState.Wait)
                {
                    enemy.manager.addEnemyToARing(enemy);
                    enemy.nav.isStopped = false;

                    //       movement[0] = 2;
                    //  target = c.transform;
                    //  nav.destination = target.position;
                    //      enemy.enemyState = EnemyState.Chasing;
                }
                //  nav.agent.stoppingDistance = 2;
                enemy.alerted = true;
            }
            //   StartCoroutine(startAttacking());
        }
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            enemy.alerted = false;
            if(enemy.enemyState == EnemyState.Chasing)
                enemy.switching();
            if (enemy.posRef != null)
                enemy.manager.leaveSpot(enemy);
        }
    }

}
