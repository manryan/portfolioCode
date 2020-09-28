﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[System.Serializable]
public class PlayerData{

    public float health;

    public float x;

    public float y;

    public int saveIndex;

    public DateTime time;

    public int saving;

    public string lastSceneSaved;

    public List<string> scenesUnlocked;

    public List<string> fastTravelSpots;

    public PlayerData (id player)
    {
        health = player.health;
        saveIndex = player.saveIndex;

        x = player.x;
        y = player.y;
        saving = player.saving;
        lastSceneSaved = player.lastSceneSaved;
        scenesUnlocked = player.scenesUnlocked;
        fastTravelSpots = player.fastTravelSpots;
        time = DateTime.Now;
    }
}
