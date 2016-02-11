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

    public void resetLevel() {
        //resets his health upon restart
        GameObject.FindObjectOfType<PlayerController>().health = GameObject.FindObjectOfType<PlayerController>().maxHealth;

        //Reloads the Scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        
        //resets his position to the spawn
        GameObject.FindObjectOfType<PlayerController>().transform.position = new Vector3(0, 0, -3);

        //Sets the timer back to full
        GameObject.FindObjectOfType<Clock>().time = GameObject.FindObjectOfType<Clock>().totalTime*60f;
    }

}
