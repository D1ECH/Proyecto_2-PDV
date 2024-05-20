using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class CheckpointController : MonoBehaviour
{
    public Checkpoint[] CheckpointsList;
    public LookAtTargetController Arrow;

    private Checkpoint CurrentCheckpoint;
    private int CheckpointId;
    public string nextSceneName; // Nombre de la próxima escena a cargar al llegar al último punto de control

    // Use this for initialization
    void Start ()
	{
        if (CheckpointsList.Length==0) return;

	    for (int index = 0; index < CheckpointsList.Length; index++)
            CheckpointsList[index].gameObject.SetActive(false);

	    CheckpointId = 0;
        SetCurrentCheckpoint(CheckpointsList[CheckpointId]);
	}

    private void SetCurrentCheckpoint(Checkpoint checkpoint)
    {
        if (CurrentCheckpoint != null)
        {
            CurrentCheckpoint.gameObject.SetActive(false);
            CurrentCheckpoint.CheckpointActivated -= CheckpointActivated;
        }

        CurrentCheckpoint = checkpoint;
        CurrentCheckpoint.CheckpointActivated += CheckpointActivated;
        Arrow.Target = CurrentCheckpoint.transform;
        CurrentCheckpoint.gameObject.SetActive(true);
    }

    private void CheckpointActivated()
    {
        CheckpointId++;
        if (CheckpointId >= CheckpointsList.Length)
        {
            CurrentCheckpoint.gameObject.SetActive(false);
            CurrentCheckpoint.CheckpointActivated -= CheckpointActivated;
            Arrow.gameObject.SetActive(false);
            StartCoroutine(LoadNextSceneWithFade()); // Inicia la corutina para cargar la próxima escena con un fundido
            return;
        }

        SetCurrentCheckpoint(CheckpointsList[CheckpointId]);
    }

    IEnumerator LoadNextSceneWithFade()
    {
        // Espera unos segundos antes de cargar la próxima escena
        yield return new WaitForSeconds(1);

        // Desactiva la flecha
        Arrow.gameObject.SetActive(false);

        // Espera un breve periodo de tiempo para dar tiempo a que la pantalla se oscurezca
        yield return new WaitForSeconds(3);

        // Carga la próxima escena con un fundido
        SceneManager.LoadScene(nextSceneName);
    }

// Update is called once per frame
void Update () {
	
	}
}
