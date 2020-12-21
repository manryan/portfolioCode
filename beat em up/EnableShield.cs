using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableShield : MonoBehaviour
{
    public Animator anim;

    public Rigidbody rb;

    public float jumpForce;

    public GameObject shield;

    public HitEntity myHE;

    public Entity player;

    public AudioClip[] clip;

    public AudioSource sfxSource;


    public void enableShield()
    {
        if (!shield.activeInHierarchy)
            shield.SetActive(true);
    }

    public void PlaySingleSound(AudioClip clip)
    {
        sfxSource.clip = clip;
     //   sfxSource.volume = volume;
        sfxSource.Play();
    }

    public virtual void cantcrouchorflip(int num)
    {
        myHE.attackPos = num;
        player.allowed = false;
    }

    public void cantcf()
    {
        myHE.attackPos = 0;
        player.allowed = false;
    }

    public void cancrouchorflip()
    {
        myHE.attackPos = 0;
        player.allowed = true;
        resetAnimatorLayers();
    }

        public virtual void jumpDone()
    {
        anim.SetBool("Jump", false);
    }

    public virtual void addJumpForce()
    {
        resetAnimatorLayersWhenJumping();
        rb.AddForce(transform.root.up * jumpForce, ForceMode.Impulse);
    }


    public void resetAnimatorLayers()
    {
        anim.SetLayerWeight(2, 0);
        if (player.crouch)
        {
        //    anim.SetBool("Crouch", true);
            anim.SetFloat("crouchfloat", 1);
            anim.SetLayerWeight(1, 1);
        }
        else
        {
            anim.SetFloat("crouchfloat", 0);
            anim.SetLayerWeight(1, 0);
        }
        player.enablesecondBox();
    }

    public void resetAnimatorLayersWhenJumping()
    {
        anim.SetLayerWeight(1, 0);
        anim.SetLayerWeight(2, 1);
    }
}
