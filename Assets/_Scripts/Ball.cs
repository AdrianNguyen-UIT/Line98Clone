using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [SerializeField] private GameObject ghost = null;
    private SpriteRenderer spriteRenderer = null;
    private Animator animator = null;
    private int ballColorID = 0;
    public int ColorID => ballColorID;

    public void SetColor(int colorID, Sprite sprite)
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        ballColorID = colorID;
    }

    public void Disappear() => gameObject.SetActive(false);
    public void Appear() => gameObject.SetActive(true);

    public void Grow()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        animator.SetTrigger("Grow");
    }

    public void Select()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        animator.SetBool("Selected", true);
    }

    public void UnSelect()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        animator.SetBool("Selected", false);
    }

    public void Explode()
    {
        //Play explode particle

        gameObject.SetActive(false);
    }

    public Sprite GetSprite()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();

        return spriteRenderer.sprite;
    }

    public void BecomeGhost()
    {
        ghost.SetActive(true);
    }
}
