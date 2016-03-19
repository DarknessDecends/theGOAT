using UnityEngine;

public class LevelManager : MonoBehaviour {

	private PlayerController player;
	private BossController boss;
	private Clock clock;

	void Start() {
		player = PlayerController.instance;
		boss = GameObject.FindObjectOfType<BossController>();
		clock = GameObject.FindObjectOfType<Clock>();

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
		boss.health+=5000;
		boss.maxHealth += 5000;

		//resets his health upon restart
		player.health = player.maxHealth;

		//Reloads the Scene
		UnityEngine.SceneManagement.SceneManager.LoadScene("Level 1");
		
		//resets his position to the spawn
		GameObject.FindObjectOfType<PlayerController>().transform.position = new Vector3(13.5f, -33.5f, -3);

		//Sets the timer back to full
		if (clock != null) {
			clock.time = clock.totalTime * 60f;
		}
	}
	
	public void hardReset() {
		Destroy(player.gameObject);
		LoadLevel("Start Screen");
	}

}
