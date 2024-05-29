using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Vida : MonoBehaviour
{
    public float vidaa = 100;
    public float tasaDisminucion = 1.0f;
    public float danoFuego = 5.9f;

    public Image barraDeVida;
    public GameObject gameOverPanel;

    // Start is called before the first frame update
    void Start()
    {
        barraDeVida.fillAmount = vidaa / 100;
        gameOverPanel.SetActive(false); 
    }

    public void TakeDamage(float amount)
    {
        vidaa -= amount;
        vidaa = Mathf.Clamp(vidaa, 0, 100);
        barraDeVida.fillAmount = vidaa / 100;

        if (vidaa <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("El jugador ha muerto.");
        // Mostrar el panel de "Game Over"
        gameOverPanel.SetActive(true);
        // Pausar el juego
        Time.timeScale = 0f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Statue")
        {
            vidaa -= tasaDisminucion * Time.deltaTime;
            vidaa = Mathf.Clamp(vidaa, 0, 100);
            barraDeVida.fillAmount = vidaa / 100;
        }
    }
}
