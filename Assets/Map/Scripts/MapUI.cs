﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapUI : MonoBehaviour
{
    public Button PopulationButton;
    public TextMeshProUGUI PopulationSizeText;
    public TextMeshProUGUI InfectedText;

    private bool showPopulation;

    void Start()
    {
        PopulationButton.onClick.AddListener(() =>
        {
            MapBehaviour.instance.ShowPopulation(showPopulation);
            showPopulation = !showPopulation;
        });
    }

    private void Update()
    {
        if(RegionBehaviour.RegionSelected != null)
        {
            Region reg = RegionBehaviour.RegionSelected.Region;
            PopulationSizeText.text = reg.city.PopulationSize + "";
            InfectedText.text = reg.city.Infected + "";
        }
    }
}
