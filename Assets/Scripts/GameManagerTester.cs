using UnityEngine;

public class GameManagerTester : MonoBehaviour
{
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.K))
		{
			GameManager.Instance.WinScreen();
		}

		if (Input.GetKeyDown(KeyCode.L))
		{
			GameManager.Instance.GameOverScreen();
		}
	}
}