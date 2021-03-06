using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadMenu : MonoBehaviour
{
    public GameObject DeadMenuUi;

    public void PlayerDead()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        DeadMenuUi.SetActive(true);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("ApocalipticGame");
    }
}
