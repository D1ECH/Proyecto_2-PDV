using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC1 : MonoBehaviour
{
    private NPC1Conversacion npc1Conversacion;

    public GameObject prefabLadrillo;
    public GameObject helicoptero;
    private int filas = 5;
    private float radio = 8f;
    public float alturaLadrillo;
    public float anchoLadrillo;

    private bool construido = false;

    void Start()
    {
        npc1Conversacion = GetComponent<NPC1Conversacion>();
        alturaLadrillo = prefabLadrillo.transform.localScale.y;
        anchoLadrillo = prefabLadrillo.transform.localScale.x;
    }

    public IEnumerator ConstruirPared()
    {
        Vector3 centro = helicoptero.transform.position;

        float perimetro = 2 * Mathf.PI * radio;
        int columnas = Mathf.FloorToInt(perimetro / anchoLadrillo);

        for (int i = 0; i < filas; i++)
        {
            for (int j = 0; j < columnas; j++)
            {
                float angulo = j * Mathf.PI * 2 / columnas;
                float x = Mathf.Cos(angulo) * radio + centro.x;
                float z = Mathf.Sin(angulo) * radio + centro.z;
                float y = i * alturaLadrillo + centro.y;

                Vector3 posicion = new Vector3(x, y, z);
                GameObject ladrillo = Instantiate(prefabLadrillo, posicion, Quaternion.identity);

                // Asegurarse de que el ladrillo tenga la rotación correcta
                ladrillo.transform.LookAt(new Vector3(centro.x, y, centro.z));
                ladrillo.transform.Rotate(0, 90, 0);
            }

            // Esperar medio segundo antes de crear la siguiente fila
            yield return new WaitForSeconds(0.5f);
        }
    }

    void Update()
    {
        if (!npc1Conversacion.Conversando() && !construido && !npc1Conversacion.permiso)
        {
            StartCoroutine(ConstruirPared());
            construido = true;
        }
    }
}
