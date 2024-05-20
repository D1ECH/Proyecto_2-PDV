using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using weka.classifiers.trees;
using weka.classifiers.evaluation;
using weka.core;
using java.io;
using java.lang;
using java.util;
using weka.classifiers.functions;
using weka.classifiers;
using weka.core.converters;

public class NPCBaloncesto : MonoBehaviour
{
    weka.classifiers.trees.M5P saberPredecirFuerza;
    weka.core.Instances casosEntrenamiento;
    private string ESTADO = "Sin conocimiento";
    public GameObject pelota;
    public GameObject aro;
    GameObject InstanciaPelota;
    float mejorFuerzaX, mejorFuerzaY;
    public float valorMaximoFx = 10, valorMaximoFy = 18, pasoFx = 1;
    Rigidbody r;
    private string archivoExperiencias = "Assets/Experiencias.arff";

    void Start()
    {
        // Verifica que el prefab de la pelota esté asignado
        if (pelota == null)
        {
            print("El prefab de la pelota no está asignado.");
            return;
        }

        VerificarOcrearArchivo();
        if (ESTADO == "Sin conocimiento") StartCoroutine(Entrenamiento());
    }

    void VerificarOcrearArchivo()
    {
        if (!System.IO.File.Exists(archivoExperiencias))
        {
            System.IO.File.WriteAllText(archivoExperiencias, "@relation experiencias\n\n@attribute Fx numeric\n@attribute Fy numeric\n@attribute Distancia numeric\n\n@data\n");
        }
    }

    IEnumerator Entrenamiento()
    {
        try
        {
            casosEntrenamiento = new weka.core.Instances(new java.io.FileReader(archivoExperiencias));
        }
        catch (Exception e)
        {
            print("Error al leer el archivo de experiencias: " + e.Message);
            yield break;
        }

        print("ENTRENAMIENTO: crea una tabla con las fuerzas utilizadas y distancias alcanzadas");
        print("Datos de entrada: Fx variables de 1 a " + valorMaximoFx + ", Fy variables de 1 a " + valorMaximoFy);

        if (casosEntrenamiento.numInstances() < 10)
        {
            for (float Fx = 1; Fx <= valorMaximoFx; Fx += pasoFx)
            {
                for (float Fy = 1; Fy <= valorMaximoFy; Fy += pasoFx)
                {
                    print($"Lanzando pelota con Fx = {Fx}, Fy = {Fy}");

                    InstanciaPelota = Instantiate(pelota);
                    Rigidbody rb = InstanciaPelota.GetComponent<Rigidbody>();

                    if (rb == null)
                    {
                        print("La instancia de la pelota no tiene un componente Rigidbody.");
                        yield break;
                    }

                    rb.AddForce(new Vector3(Fx, Fy, 0), ForceMode.Impulse);

                    float startTime = Time.time; // Time when the wait started
                    float waitTime = 5f; // Max time to wait

                    // Espera a que la pelota caiga por debajo de y = 0 o hasta que pase el tiempo máximo
                    yield return new WaitUntil(() =>
                    {
                        print($"Posición de la pelota: y = {rb.transform.position.y}" + $" x = {rb.transform.position.x}");
                        return rb.transform.position.y < 0 || Time.time - startTime > waitTime;
                    });

                    if (rb.transform.position.y >= 0)
                    {
                        print("La pelota no cayó dentro del tiempo esperado.");
                    }

                    print($"Pelota cayó: posición final x = {rb.transform.position.x}, y = {rb.transform.position.y}");

                    Instance casoAaprender = new Instance(casosEntrenamiento.numAttributes());
                    casoAaprender.setDataset(casosEntrenamiento);
                    casoAaprender.setValue(0, Fx);
                    casoAaprender.setValue(1, Fy);
                    casoAaprender.setValue(2, rb.transform.position.x);
                    casosEntrenamiento.add(casoAaprender);

                    rb.isKinematic = true;
                    rb.GetComponent<Collider>().isTrigger = true;
                    Destroy(InstanciaPelota, 1f);
                }
            }
        }

        saberPredecirFuerza = new M5P();
        casosEntrenamiento.setClassIndex(2);
        saberPredecirFuerza.buildClassifier(casosEntrenamiento);

        GuardarCasosEntrenamiento();

        if (casosEntrenamiento.numInstances() >= 10)
        {
            Evaluation evaluador = new Evaluation(casosEntrenamiento);
            evaluador.crossValidateModel(saberPredecirFuerza, casosEntrenamiento, 10, new java.util.Random(1));
            print("El Error Absoluto Promedio durante el entrenamiento fue de " + evaluador.meanAbsoluteError().ToString("0.000000") + " N");
        }

        ESTADO = "Con conocimiento";
    }

    void GuardarCasosEntrenamiento()
    {
        try
        {
            ArffSaver saver = new ArffSaver();
            saver.setInstances(casosEntrenamiento);
            saver.setFile(new java.io.File(archivoExperiencias));
            saver.writeBatch();
        }
        catch (Exception e)
        {
            print("Error al guardar el archivo de experiencias: " + e.Message);
        }
    }

    void FixedUpdate()
    {
        if ((ESTADO == "Con conocimiento"))
        {
            Vector3 objetivo = aro.transform.position;
            float distanciaObjetivo = objetivo.x;

            Instance casoPrueba = new Instance(casosEntrenamiento.numAttributes());
            casoPrueba.setDataset(casosEntrenamiento);
            casoPrueba.setValue(2, distanciaObjetivo);

            double[] predicciones = saberPredecirFuerza.distributionForInstance(casoPrueba);
            mejorFuerzaX = (float)predicciones[0];
            mejorFuerzaY = (float)predicciones[1];

            print("El NPC calcula las fuerzas: Fx = " + mejorFuerzaX + ", Fy = " + mejorFuerzaY);

            InstanciaPelota = Instantiate(pelota);
            r = InstanciaPelota.GetComponent<Rigidbody>();
            r.AddForce(new Vector3(mejorFuerzaX, mejorFuerzaY, 0), ForceMode.Impulse);

            ESTADO = "Acción realizada";
        }
        if (ESTADO == "Acción realizada")
        {
            if (r.transform.position.y < 0)
            {
                print("La pelota lanzada llegó a " + r.transform.position.x + ". El error fue de " + (r.transform.position.x - aro.transform.position.x).ToString("0.000000") + " m");
                r.isKinematic = true;
                ESTADO = "FIN";
            }
        }
    }
}

