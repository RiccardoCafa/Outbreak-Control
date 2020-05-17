﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class City
{
    public static float TotalPopulation = 0;
    public static float Person = 1e-3f;
    public float Population {
        get { return NotInfected + Infected; }
    }
    public float Symptomatic;
    public Queue<float> Asymptomatic;
    public float Infected {
        get {
            float infected = Symptomatic;
            foreach (float a in Asymptomatic)
                infected += a;
            return infected;
        }
    }
    public float NotInfected;
    public int Money;

    public Culture MyCulture;
    public Region Region;

    public City(float population, Region region, Culture myCulture)
    {
        TotalPopulation += population;
        NotInfected = population;
        Symptomatic = 0;
        Money = 0;
        MyCulture = myCulture;
        Region = region;
        Asymptomatic = new Queue<float>();
    }

    public void UpdatePerDay(Virus virus)
    {
        float deaths = Symptomatic * virus.Lethality(Region);
        Symptomatic -= deaths;
        TotalPopulation -= deaths;
        float newInfected = NotInfected * Infected * virus.InfectRate(Region);
        newInfected = newInfected <= NotInfected ? newInfected : NotInfected;

        Asymptomatic.Enqueue(newInfected);
        NotInfected -= newInfected;

        if (Asymptomatic.Count > virus.SerialRangeRnd())
        {
            float value = Asymptomatic.Dequeue();
            if (Symptomatic == 0 && value > 0)
                Region.OnRegionInfected();
            Symptomatic += value;
        }
    }

    public void UpdatePerWeek()
    {
        NotInfected += Population * 0.0005f;
        TotalPopulation += Population * 0.0005f;
    }

    public void Immigrate(
        (float notInfected, float symptomatic, Queue<float> asymptomatic) migrate)
    {
        NotInfected += migrate.notInfected;
        if (Symptomatic == 0 && migrate.symptomatic != 0)
            Region.OnRegionInfected();
        Symptomatic += migrate.symptomatic;
        Queue<float> newAsymptomatic = new Queue<float>();
        if (migrate.asymptomatic != null)
            while (migrate.asymptomatic.Count != 0) newAsymptomatic.
                Enqueue(Asymptomatic.Dequeue() + migrate.asymptomatic.Dequeue());
        while (Asymptomatic.Count != 0)
            newAsymptomatic.Enqueue(Asymptomatic.Dequeue());
        Asymptomatic = newAsymptomatic;
    }

    public void Emigrate(
        (float notInfected, float symptomatic, Queue<float> asymptomatic) migrate)
    {
        NotInfected -= migrate.notInfected;
        Symptomatic -= migrate.symptomatic;
        Queue<float> newAsymptomatic = new Queue<float>();
        if (migrate.asymptomatic != null)
            if (migrate.asymptomatic.Count != 0 && Asymptomatic.Count != 0)
            {
                if (migrate.asymptomatic.Count < Asymptomatic.Count)
                {
                    while (migrate.asymptomatic.Count != 0) newAsymptomatic.
                        Enqueue(Asymptomatic.Dequeue() - migrate.asymptomatic.Dequeue());
                    while (Asymptomatic.Count != 0)
                        newAsymptomatic.Enqueue(Asymptomatic.Dequeue());
                }
                else
                {
                    while (Asymptomatic.Count != 0) newAsymptomatic.
                        Enqueue(Asymptomatic.Dequeue() - migrate.asymptomatic.Dequeue());
                    while (migrate.asymptomatic.Count != 0)
                        newAsymptomatic.Enqueue(migrate.asymptomatic.Dequeue());
                }

                Asymptomatic = newAsymptomatic;
            }
    }

    public static (float notInfected, float symptomatic, Queue<float> asymptomatic)
        PrepareMigrationPerDay(City from, City to)
    {
        float deltaPopulationDensity = from.Population /
            (from.Population + to.Population + 0.0000001f);
        float deltaInfected = from.Symptomatic /
            (from.Symptomatic + to.Symptomatic + 0.0000001f);
        float deltaMoney = to.Money / (from.Money + to.Money + 0.0000001f);
        float migration =
            deltaMoney * 0.5f +
            deltaInfected * 0.3f +
            deltaPopulationDensity * 0.2f;

        if (migration > 0 && migration + to.Population < 1 && migration < from.Population)
        {
            Queue<float> asymptomatic = new Queue<float>();
            foreach (float a in to.Asymptomatic)
                asymptomatic.Enqueue(a * migration * 0.2f);
            return (
                from.NotInfected * migration * 0.2f,
                from.Symptomatic * migration * 0.2f, asymptomatic);
        }
        return (0, 0, null);
    }
}