using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : MonoBehaviour
{
    [SerializeField]
    public FoodType FoodType;
}

public enum FoodType { pizza, hotdog };
