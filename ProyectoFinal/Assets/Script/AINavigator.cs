using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovimientoCuandoCerca : MonoBehaviour
{
    public Transform objetivo; // El objetivo al que seguirá el objeto (jugador)
    public float distanciaActivacion = 2f; // La distancia a la que el objeto se activará
    public float distanciaDesactivacion = 4f; // La distancia a la que el objeto se desactivará
    public float distanciaAtaque = 0.5f; // Distancia a la que el objeto atacará
    public float velocidadMovimiento = 0.5f; // Velocidad de movimiento del objeto
    public float tasaAtaque = 1f; // Tiempo entre ataques
    public float dano = 10f; // Daño infligido por ataque

    private bool activado = false;
    private Animator zombie;
    private float siguienteTiempoDeAtaque = 0f;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        if (objetivo == null)
        {
            objetivo = GameObject.FindGameObjectWithTag("Player").transform;
        }

        zombie = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (objetivo == null)
        {
            Debug.LogError("No se ha asignado un objetivo en el script MovimientoCuandoCerca.");
            return;
        }

        // Calcular la distancia entre este objeto y el objetivo
        float distancia = Vector3.Distance(transform.position, objetivo.position);

        // Verificar si la distancia es menor que la distancia de activación
        if (distancia < distanciaActivacion)
        {
            activado = true;
        }
        // Verificar si la distancia es mayor que la distancia de desactivación
        else if (distancia > distanciaDesactivacion)
        {
            activado = false;
        }

        // Si está activado, mover el objeto hacia el objetivo
        if (activado)
        {
            if (distancia > distanciaAtaque)
            {
                MoverHaciaObjetivo();
            }
            else
            {
                AtacarObjetivo();
            }
        }
        else
        {
            // Detener las animaciones y el movimiento cuando el zombi no está activado
            navMeshAgent.SetDestination(transform.position);
            zombie.SetBool("isWalking", false);
            zombie.SetBool("isAttacking", false);
        }
    }

    private void MoverHaciaObjetivo()
    {
        // Mover el zombi usando el NavMeshAgent
        navMeshAgent.SetDestination(objetivo.position);

        // Activar la animación de caminar
        zombie.SetBool("isWalking", true);
        zombie.SetBool("isAttacking", false);
    }

    private void AtacarObjetivo()
    {
        // Detener el movimiento del zombi
        navMeshAgent.SetDestination(transform.position);

        // Activar la animación de ataque
        zombie.SetBool("isWalking", false);
        zombie.SetBool("isAttacking", true);

        // Infligir daño al objetivo a intervalos regulares
        if (Time.time >= siguienteTiempoDeAtaque)
        {
            Vida playerHealth = objetivo.GetComponent<Vida>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(dano);
            }
            siguienteTiempoDeAtaque = Time.time + 1f / tasaAtaque;
        }
    }
}
