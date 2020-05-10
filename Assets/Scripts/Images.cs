using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Images : MonoBehaviour
{
    public List<ImageHolder> images = new List<ImageHolder>();

    public Sprite getImageById(string imageId)
    {
        foreach( ImageHolder holder in images)
        {
            if (holder.imageId.Equals(imageId))
            {
                return holder.image;
            }
        }
        throw new System.Exception("Can't find image with id: " + imageId);
    }
}
