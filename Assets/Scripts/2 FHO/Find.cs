using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Find : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] FHOSceneManager fhos;
    [SerializeField] Image correctImage;
    [SerializeField] Image correctImageAtAnswer;

    public void Answer()
    {
        if (fhos.isSetting)
            return;

        GetComponent<Button>().interactable = false;
        StartCoroutine(CorrectImageCoroutine());

        fhos.score_add();
    }

    IEnumerator CorrectImageCoroutine()
    {
        float count = 0;
        while (count < 1)
        {
            count += 0.02f;
            correctImage.fillAmount = count;
            correctImageAtAnswer.fillAmount = count;
            yield return null;
        }
    }
}
