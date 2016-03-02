using UnityEngine;

public class LevelManager : MonoBehaviour {

    private PlayerController player;

    void Start() {
        player = PlayerController.instance;
    }

    public void LoadLevel(string name){
        Debug.Log("New Level Load: " + name);
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
    public void QuitRequest(){
        Debug.Log("Quit Requested");
        Application.Quit();
    }

    public void resetLevel() {

        //Death Score Deduction
        player.Score(-500);

        //resets his health upon restart
        GameObject.FindObjectOfType<PlayerController>().health = GameObject.FindObjectOfType<PlayerController>().maxHealth;

        //Reloads the Scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1");
        
        //resets his position to the spawn
        GameObject.FindObjectOfType<PlayerController>().transform.position = new Vector3(13.5f, -33.5f, -3);

        //Sets the timer back to full
        GameObject.FindObjectOfType<Clock>().time = GameObject.FindObjectOfType<Clock>().totalTime*60f;
    }
    
    public void hardReset() {
        Destroy(player.gameObject);
        LoadLevel("Start Screen");
    }

}
