using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] internal float speed;
    internal Vector2 moveInput;
    public Animator playerAnimator;
    internal float moveXValor;
    internal float moveYValor;
    public Rigidbody2D rb;
    public bool cantWalk;
    void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveXValor = moveX;
        moveYValor = moveY;

        moveInput = new Vector2(moveX, moveY).normalized;
    }
    private void FixedUpdate()// Lógica de física que maneja el movimiento del jugador si puede caminar
    {
        if (cantWalk)
        {
            Movimiento();
        }
    }
    private void Movimiento()
    {
        // Mueve la posición del Rigidbody2D basado en la entrada de movimiento, velocidad y el tiempo fijo
            rb.MovePosition(rb.position + moveInput * speed * Time.fixedDeltaTime);
    }
}