using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UpdateTextFromSlider : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TMP_Text>().text = gameObject.transform.parent.GetComponent<Slider>().value.ToString();
    }
}
