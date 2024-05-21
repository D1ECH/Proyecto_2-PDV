using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using weka.classifiers.trees;
using weka.classifiers.evaluation;
using weka.core;
using java.io;
using java.lang;
using java.util;
using weka.classifiers.functions;
using weka.classifiers;
using weka.core.converters;

public class AprendizLento_2_incognitas : MonoBehaviour
{
    weka.classifiers.trees.M5P saberPredecirDistancia, saberPredecirFuerzaX;
    weka.core.Instances casosEntrenamiento;
    private string ESTADO = "Sin conocimiento";
    // private string ESTADO = "Con conocimiento";
    public GameObject pelota;
    public GameObject PuntoObjetivo;
    GameObject InstanciaPelota;
    public float valorMaximoFx = 20, valorMaximoFy = 20, paso = 1, Velocidad_Simulacion = 1;
    public float valorMinimoFx = 16, valorMinimoFy = 10;
    float mejorFuerzaX, mejorFuerzaY, distanciaObjetivo;
    Rigidbody r;

    private NPCInteligenteBaloncesto npcInteligenteBaloncesto;
    bool entrenando = false;

    void Start()
    {
        npcInteligenteBaloncesto = GetComponent<NPCInteligenteBaloncesto>();
        Time.timeScale = Velocidad_Simulacion;
        if (ESTADO == "Sin conocimiento" && npcInteligenteBaloncesto.lanzar && !entrenando)
        {
            entrenando = true;
            StartCoroutine("Entrenamiento");
        }
    }

    IEnumerator Entrenamiento()
    {
        casosEntrenamiento = new weka.core.Instances(new java.io.FileReader("Assets/Finales_Experiencias.arff"));    //... u otro con muchas experiencias

        if (casosEntrenamiento.numInstances() < 10)
        {
            print("Datos de entrada: valorMaximoFx=" + valorMaximoFx + " valorMaximoFy=" + valorMaximoFy + "  " + ((valorMaximoFx == 0 || valorMaximoFy == 0) ? " ERROR: alguna fuerza es siempre 0" : ""));
            for (float Fx = valorMinimoFx;  Fx <= valorMaximoFx; Fx = Fx + paso)                      //Bucle de planificación de la fuerza FX durante el entrenamiento
            {
                for (float Fy = valorMinimoFy; Fy <= valorMaximoFy; Fy = Fy + paso)                    //Bucle de planificación de la fuerza FY durante el entrenamiento
                {
                    InstanciaPelota = Instantiate(pelota, this.transform.position, this.transform.rotation);
                    Rigidbody rb = InstanciaPelota.GetComponent<Rigidbody>();              //Crea una pelota física
                    rb.AddForce(new Vector3(-Fx, Fy, 0), ForceMode.Impulse);                //y la lanza con las fuerza Fx y Fy

                    float startTime = Time.time; // Guarda el tiempo inicial
                    bool hitTarget = false;
                    PuntoObjetivo.GetComponent<Collider>().isTrigger = true;

                    yield return new WaitUntil(() =>
                    {
                        // Verifica si el tiempo ha excedido 10 segundos
                        if (Time.time - startTime >= 10f)
                        {
                            return true;
                        }

                        // Verifica si la pelota ha tocado el PuntoObjetivo
                        if (PuntoObjetivo.GetComponent<Collider>().bounds.Intersects(rb.GetComponent<Collider>().bounds))
                        {
                            print("LA PELOTA HA TOCADO LA CANASTA");
                            hitTarget = true;
                            return true;
                        }

                        // Verifica si la pelota ha caído al suelo
                        return rb.transform.position.y <= 10f;
                    });


                    Instance casoAaprender = new Instance(casosEntrenamiento.numAttributes());
                    casoAaprender.setDataset(casosEntrenamiento);                          //crea un registro de experiencia
                    casoAaprender.setValue(0, Fx);                                         //guarda los datos de las fuerzas Fx y Fy utilizadas
                    casoAaprender.setValue(1, Fy);
                    casoAaprender.setValue(2, hitTarget ? 1.0 : 0.0);                      //anota si alcanzó el objetivo
                    casosEntrenamiento.add(casoAaprender);                                 //guarda el registro en la lista casosEntrenamiento
                    rb.isKinematic = true; rb.GetComponent<Collider>().isTrigger = true;   //...opcional: paraliza la pelota
                    Destroy(InstanciaPelota, 1);                                           //...opcional: destruye la pelota

                }                                                                          //FIN bucle de lanzamientos con diferentes de fuerzas
            }

            File salida = new File("Assets/Finales_Experiencias.arff");
            if (!salida.exists())
                System.IO.File.Create(salida.getAbsoluteFile().toString()).Dispose();
            ArffSaver saver = new ArffSaver();
            saver.setInstances(casosEntrenamiento);
            saver.setFile(salida);
            saver.writeBatch();
        }

        //APRENDIZAJE CONOCIMIENTO:  
        saberPredecirFuerzaX = new M5P();                                                //crea un algoritmo de aprendizaje M5P (árboles de regresión)
        casosEntrenamiento.setClassIndex(0);                                             //y hace que aprenda Fx dada la distancia y Fy
        saberPredecirFuerzaX.buildClassifier(casosEntrenamiento);                        //REALIZA EL APRENDIZAJE DE FX A PARTIR DE LA DISTANCIA Y FY

        saberPredecirDistancia = new M5P();                                              //crea otro algoritmo de aprendizaje M5P (árboles de regresión)  
        casosEntrenamiento.setClassIndex(2);                                             //La variable a aprender a calcular la distancia dada Fx e FY                                                                                         
        saberPredecirDistancia.buildClassifier(casosEntrenamiento);                      //este algoritmo aprende un "modelo fisico aproximado"

        ESTADO = "Con conocimiento";

        print(casosEntrenamiento.numInstances() + " espers " + saberPredecirDistancia.toString());

        //EVALUACION DEL CONOCIMIENTO APRENDIDO: 
        if (casosEntrenamiento.numInstances() >= 10)
        {
            casosEntrenamiento.setClassIndex(0);
            Evaluation evaluador = new Evaluation(casosEntrenamiento);                   //...Opcional: si tien mas de 10 ejemplo, estima la posible precisión
            evaluador.crossValidateModel(saberPredecirFuerzaX, casosEntrenamiento, 20, new java.util.Random(1));
            print("El Error Absoluto Promedio con Fx durante el entrenamiento fue de " + evaluador.meanAbsoluteError().ToString("0.000000") + " N");
            casosEntrenamiento.setClassIndex(2);
            evaluador.crossValidateModel(saberPredecirDistancia, casosEntrenamiento, 20, new java.util.Random(1));
            print("El Error Absoluto Promedio con Distancias durante el entrenamiento fue de " + evaluador.meanAbsoluteError().ToString("0.000000") + " m");
        }

        //Estimación de la distancia a la Canasta
        // distanciaObjetivo = (this.transform.position.x - PuntoObjetivo.transform.position.x);
        distanciaObjetivo = 1.0f;
        PuntoObjetivo.GetComponent<Collider>().isTrigger = true;

        /////////////////////////////////////////////////////////////////////////////////////////////

    }


    void FixedUpdate()                                                                                 //DURANTEL EL JUEGO: Aplica lo aprendido para lanzar a la canasta
    {
        if (ESTADO == "Sin conocimiento" && npcInteligenteBaloncesto.lanzar && !entrenando)
        {
            entrenando = true;
            StartCoroutine("Entrenamiento");              //Lanza el proceso de entrenamiento
        }
        else if ((ESTADO == "Con conocimiento") && (distanciaObjetivo > 0) && npcInteligenteBaloncesto.lanzar)
        {
            Time.timeScale = 1;                                                                               //Durante el juego, el NPC razona así... (no juega aún)   
            float menorDistancia = 1e9f;
            print("-- OBJETIVO: LANZAR LA PELOTA A UNA DISTANCIA DE " + distanciaObjetivo + " m.");

            //Si usa dos bucles Fx y Fy con "modelo fisico aproximado", complejidad n^2
            //Reduce la complejidad con un solo bucle FOR, así

            for (float Fy = 1; Fy < valorMaximoFy; Fy = Fy + paso)                                            //Bucle FOR con fuerza Fy, deduce Fx = f (Fy, distancia) y escoge mejor combinacion         
            {
                Instance casoPrueba = new Instance(casosEntrenamiento.numAttributes());
                casoPrueba.setDataset(casosEntrenamiento);
                casoPrueba.setValue(1, Fy);                                                                   //crea un registro con una Fy
                casoPrueba.setValue(2, 1.0);                                                                  //assuming perfect hit
                float Fx = (float)saberPredecirFuerzaX.classifyInstance(casoPrueba);                          //Predice Fx a partir de la distancia y una Fy 
                if ((Fx >= 1) && (Fx <= valorMaximoFx))
                {
                    Instance casoPrueba2 = new Instance(casosEntrenamiento.numAttributes());
                    casoPrueba2.setDataset(casosEntrenamiento);                                                  //Utiliza el "modelo fisico aproximado" con Fx y Fy                 
                    casoPrueba2.setValue(0, Fx);                                                                 //Crea una registro con una Fx
                    casoPrueba2.setValue(1, Fy);                                                                 //Crea una registro con una Fy
                    float prediccionDistancia = (float)saberPredecirDistancia.classifyInstance(casoPrueba2);     //Predice la distancia dada Fx y Fy
                    if (Mathf.Abs(prediccionDistancia - 1) < menorDistancia)                                   //Busca la Fy con una distancia más cercana al objetivo
                    {
                        menorDistancia = Mathf.Abs(prediccionDistancia - 1);                                   //si encuentra una buena toma nota de esta distancia
                        mejorFuerzaX = Fx;                                                                       //de la fuerzas que uso, Fx
                        mejorFuerzaY = Fy;                                                                       //tambien Fy
                        print("RAZONAMIENTO: Una posible acción es ejercer una fuerza Fx=" + mejorFuerzaX + " y Fy= " + mejorFuerzaY + " se alcanzaría una distancia de " + prediccionDistancia);
                    }
                }
            }                                                                                                     //FIN DEL RAZONAMIENTO PREVIO
            if ((mejorFuerzaX == 0) && (mejorFuerzaY == 0))
            {
                // texto.text = "NO SE LANZÓ LA PELOTA: La distancia de "+distanciaObjetivo+" m no se ha alcanzado muchas veces.";
                // print(texto.text);
            }
            else
            {
                InstanciaPelota = Instantiate(pelota, this.transform.position, this.transform.rotation);
                r = InstanciaPelota.GetComponent<Rigidbody>();                                                        //EN EL JUEGO: utiliza la pelota física del juego (si no existe la crea)
                r.AddForce(new Vector3(-mejorFuerzaX, mejorFuerzaY, 0), ForceMode.Impulse);                            //la lanza en el videojuego con la fuerza encontrada
                print("DECISION REALIZADA: Se lanzó pelota con fuerza Fx =" + mejorFuerzaX + " y Fy= " + mejorFuerzaY);
                ESTADO = "Acción realizada";
            }
        }
        if (ESTADO == "Acción realizada")
        {
            if (Vector3.Distance(r.transform.position, PuntoObjetivo.transform.position) <= 1f)                    //cuando la pelota alcanza el objetivo
            {                                                                                                      //escribe la distancia en x alcanzada
                print("La pelota alcanzó el objetivo.");
                print("La pelota lanzada llegó a " + r.transform.position.x + ". El error fue de " + (r.transform.position.x - distanciaObjetivo).ToString("0.000000") + " m");
                r.isKinematic = true;
                ESTADO = "FIN";
            }
        }
    }
}
