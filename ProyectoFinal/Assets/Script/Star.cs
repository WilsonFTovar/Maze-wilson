using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public GameObject winPanel; // Panel de "WIN"

    private void Start()
    {
        // Asegúrate de que el panel de "WIN" esté desactivado al inicio del juego
        if (winPanel != null)
        {
            winPanel.SetActive(false);
        }

        // Configurar la posición, rotación y escala de la estrella
        // transform.position = new Vector3(Random.Range(-10, 10), 0.5f, Random.Range(-10, 10)); // Posición aleatoria en X y Z, Y fijo en 0.5
        transform.rotation = Quaternion.Euler(-90f, 0f, 0f); // Rotación
        transform.localScale = new Vector3(0.2f, 0.2f, 0.2f); // Escala
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mostrar el panel de victoria y detener el juego
            Debug.Log("El jugador ha escapado del laberinto.");
            if (winPanel != null)
            {
                winPanel.SetActive(true);
            }

            // Destruir la estrella
            Destroy(gameObject);
        }
    }
}
