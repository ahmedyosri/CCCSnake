using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component that defines a Fruit (pickup-able item)
/// </summary>
public class Pickup : MonoBehaviour
{
    /// <summary>
    /// Action fired as the item is pickedup
    /// As the development goes, a more complex pickup behavior may be needed, hence additional components can be added to the parent object and listens to this Action, to control .. may be Animations
    /// </summary>
    public Action OnItemPickedup;

    /// <summary>
    /// The effective score of the current pickup (Fruit)
    /// </summary>
    public int itemScore = 5;

    /// <summary>
    /// How much that pickup should affect the snake's speed
    /// </summary>
    public float speedBonus = 0.75f;
}
