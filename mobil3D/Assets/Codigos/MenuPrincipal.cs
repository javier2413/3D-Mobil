using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Configuración de Escenas")]
    public string nombreEscenaJuego = "NivelMercado"; // El nombre exacto de tu escena de juego

    // Este método lo llamará el botón de Jugar
    public void Jugar()
    {
        Debug.Log("Iniciando Punto Núcleo...");
        SceneManager.LoadScene("Playground");
    }

    // Este método lo llamará el botón de Salir
    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Esto cierra la app en el móvil
    }
}
