using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public void PlayGame()
    {
        //charge le jeu lorque pouton play est press�
        SceneManager.LoadScene(1);
    }
}
