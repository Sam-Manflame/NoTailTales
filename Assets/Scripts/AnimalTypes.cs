using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalTypes : MonoBehaviour
{
    public List<AnimalType> types = new List<AnimalType>();

    public AnimalType getTypeById(string typeId)
    {
        foreach (AnimalType type in types)
        {
            if (type.typeId.Equals(typeId))
            {
                return type;
            }
        }
        throw new System.Exception("Can't find animal type with id: " + typeId);
    }
}
