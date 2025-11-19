using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    public static void moveToNextLevels(string level)
    {
        SceneManager.LoadScene(level);
    }

    private void OnTriggerEnter(Collider other)
    {
       
        StartCoroutine(endingMent(8));
    }

    IEnumerator endingMent(int time)
    {
        NarratorSystem.Instance.TriggerEvent(NarratorState.Ending);
        yield return new WaitForSeconds(time);
        moveToNextLevels("Level1");
    }
}
