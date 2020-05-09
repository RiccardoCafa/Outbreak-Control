﻿using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VirusUI : MonoBehaviour
{
    public TextMeshProUGUI PlagueName;
    public TextMeshProUGUI PlagueDescription;
    public TextMeshProUGUI PlagueSkillName;
    public SkillBranchUI[] SkillBranchs;

    public Button ArrowLeft;
    public Button ArrowRight;

    public Virus MyVirus;
    public Perk[,] perks;

    public int Page = 0;

    private PerkGenerator perkGenerator;

    private void Start()
    {
        ArrowLeft.onClick.AddListener(() =>
        {
            if (MyVirus == null) return;
            Page--;
            if (Page < 0)
            {
                Page = 0;
            }
            UpdateTree();
        });

        ArrowRight.onClick.AddListener(() =>
        {
            if (MyVirus == null) return;
            Page++;
            if (Page >= MyVirus.PerkNumber)
            {
                Page = MyVirus.PerkNumber - 1;
            }
            UpdateTree();
        });
    }

    public void SetVirus(Virus virus, PerkGenerator perkGenerator)
    {
        this.perkGenerator = perkGenerator;
        MyVirus = virus;
        PlagueName.text = virus.Name;
        PlagueDescription.text = virus.ToString();

        perks = new Perk[perkGenerator.SymptomsMaxLevel, perkGenerator.SymptomsMaxLevel];
        perks = perkGenerator.SymptomsPerks;

        UpdateTree();
    }

    public void UpdateTree()
    {
        if (MyVirus == null) return;
        // skill tree
        string Skillkey = MyVirus.Perks.Keys.ElementAt(Page);
        PlagueSkillName.text = Skillkey;
        for(int i = 0; i < SkillBranchs.Length; i++)
        {
            SkillBranchs[i].UpdateSkills(MyVirus.Perks[Skillkey]);
        }
    }
}
