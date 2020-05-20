using UnityEngine;
using UnityEngine.UI;

public class AnimalWindowSetup : MonoBehaviour
{
    [SerializeField]
    private Text animalTitle;
    [SerializeField]
    private Text animalName;
    [SerializeField]
    private Text animalType;
    [SerializeField]
    private Image animalImage;

    public void setup(Animal animal, int animalCounter, Images images)
    {
        animalTitle.text = string.Format("ANIMAL #{0}", animalCounter + 1);
        animalName.text = animal.name;
        animalType.text = animal.typeName;
        animalImage.sprite = images.getImageById(animal.iconId);
    }
}
