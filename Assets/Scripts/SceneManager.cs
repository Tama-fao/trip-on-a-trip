using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Diese Methode wird durch das Animation Event aufgerufen
    public void ChangeScene()
    {
        SceneManager.LoadScene("Scene 2"); // N�chste Szene laden (Index oder Name anpassen)
    }
 
}
