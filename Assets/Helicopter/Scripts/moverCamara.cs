using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moverCamara : MonoBehaviour
{
    public float alturaCamara = 200;
    public GameObject objetivo;

    void Update() {
        transform.position = new Vector3(objetivo.transform.position.x, alturaCamara, objetivo.transform.position.z+100);
    }
}
