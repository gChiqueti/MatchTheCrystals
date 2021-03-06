﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Gem : MonoBehaviour
{
    public enum GemType {
        Coconut,
        Bread,
        Milk,
        Crystal,
        Apple,
        Orange,
        Broccoli,
    }

    public GemType gem;
    public Point position = new Point(0, 0);

    public static GemType getRandomGemType() {
        GemType gemType = (GemType) UnityEngine.Random.Range(0, (float)Enum.GetValues(typeof(GemType)).Cast<GemType>().Max());
        return gemType;
    }
}
