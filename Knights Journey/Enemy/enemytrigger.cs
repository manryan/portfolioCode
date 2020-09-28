using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemytrigger : MonoBehaviour {

    public float health = 100;

        void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.tag == "Player")
        {
            health -= 50f;
            GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, health / 100);
            if (health <= 0)
                gameObject.SetActive(false);
        }
    }
}
