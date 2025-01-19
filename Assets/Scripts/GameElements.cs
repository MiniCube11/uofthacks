using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameElements : MonoBehaviour
{
    public void Home()
    {
        SceneManager.LoadScene("Main");
    }
}
