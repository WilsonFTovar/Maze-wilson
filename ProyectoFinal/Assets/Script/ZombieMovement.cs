using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMovement : MonoBehaviour
{
    public Transform player; // El objetivo al que seguirá el zombi (jugador)
    public float activationDistance = 10f; // Distancia a la que el zombi se activará
    public float attackDistance = 2f; // Distancia a la que el zombi atacará
    public float movementSpeed = 2f; // Velocidad de movimiento del zombi
    public float attackRate = 1f; // Velocidad de ataque del zombi
    public float damage = 10f; // Daño que el zombi inflige al jugador

    private bool isActivated = false;
    private Animator animator;
    private float nextAttackTime;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null)
        {
            Debug.LogError("No se ha asignado un jugador en el script ZombieMovement.");
            return;
        }

        // Calcular la distancia entre el zombi y el jugador
        float distance = Vector3.Distance(transform.position, player.position);

        // Verificar si la distancia es menor que la distancia de activación
        if (distance < activationDistance)
        {
            isActivated = true;
        }

        // Mover el zombi hacia el jugador si está activado
        if (isActivated)
        {
            if (distance > attackDistance)
            {
                MoveTowardsPlayer();
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            // Detener las animaciones cuando el zombi no está activado
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
        }
    }

    private void MoveTowardsPlayer()
    {
        // Calcular la dirección hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;

        // Mover el zombi
        transform.position = Vector3.MoveTowards(transform.position, player.position, movementSpeed * Time.deltaTime);

        // Activar la animación de caminar
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    private void AttackPlayer()
    {
        // Activar la animación de ataque
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        if (Time.time >= nextAttackTime)
        {
            Vida playerHealth = player.GetComponent<Vida>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }
}
