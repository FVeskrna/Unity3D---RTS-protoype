using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomizeSliders : MonoBehaviour
{
    public List<Slider> Sliders = new List<Slider>();
    public void Randomize()
    {
        foreach(Slider slider in Sliders)
        {
            slider.value = Random.Range((int)slider.minValue,(int)slider.maxValue);
        }
    }
    
    public void GreenLands()
    {
        foreach(Slider slider in Sliders)
        {
            if(slider.name == "Height")
            slider.value = 100;
            if(slider.name == "Width")
            slider.value = 100;
            if(slider.name == "LakeCount")
            slider.value = 2;
            if(slider.name == "LakeSize")
            slider.value = 30;
            if(slider.name == "MountainCount")
            slider.value = 2;
            if(slider.name == "MountainSize")
            slider.value = 20;
            if(slider.name == "ForestCount")
            slider.value = 3;
            if(slider.name == "ForestSize")
            slider.value = 25;
            if(slider.name == "Vegetation")
            slider.value = 20;
            if(slider.name == "IronOres")
            slider.value = 5;
        }
    }

    public void BigLake()
    {
        foreach(Slider slider in Sliders)
        {
            if(slider.name == "Height")
            slider.value = 100;
            if(slider.name == "Width")
            slider.value = 100;
            if(slider.name == "LakeCount")
            slider.value = 1;
            if(slider.name == "LakeSize")
            slider.value = 90;
            if(slider.name == "MountainCount")
            slider.value = 2;
            if(slider.name == "MountainSize")
            slider.value = 20;
            if(slider.name == "ForestCount")
            slider.value = 3;
            if(slider.name == "ForestSize")
            slider.value = 25;
            if(slider.name == "Vegetation")
            slider.value = 20;
            if(slider.name == "IronOres")
            slider.value = 5;
        }
    }

    public void BigMountain()
    {
        foreach(Slider slider in Sliders)
        {
            if(slider.name == "Height")
            slider.value = 100;
            if(slider.name == "Width")
            slider.value = 100;
            if(slider.name == "LakeCount")
            slider.value = 2;
            if(slider.name == "LakeSize")
            slider.value = 30;
            if(slider.name == "MountainCount")
            slider.value = 1;
            if(slider.name == "MountainSize")
            slider.value = 90;
            if(slider.name == "ForestCount")
            slider.value = 3;
            if(slider.name == "ForestSize")
            slider.value = 25;
            if(slider.name == "Vegetation")
            slider.value = 20;
            if(slider.name == "IronOres")
            slider.value = 5;
        }
    }
}
