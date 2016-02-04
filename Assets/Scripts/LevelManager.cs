using UnityEngine;

public class LevelManager : MonoBehaviour {

    public void LoadLevel(string name){
        Debug.Log("New Level Load: " + name);
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
    public void QuitRequest(){
        Debug.Log("Quit Requested");
        Application.Quit();
    }

}
