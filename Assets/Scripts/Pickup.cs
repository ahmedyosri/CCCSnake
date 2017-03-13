using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public Action OnItemPickedup;

    public int itemScore = 5;

    public float speedBonus = 0.75f;
}
