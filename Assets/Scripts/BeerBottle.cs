using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BeerBottle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<Rigidbody> allParts = new List<Rigidbody>();
    public void Shatter()
    {
            foreach (Rigidbody part in allParts)
        {
            part.isKinematic = false;
        }
    }
}
