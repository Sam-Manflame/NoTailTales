using UnityEngine;
using UnityEngine.UI;

public class AnimalWindowSetup : MonoBehaviour, IGameListener
{
    [SerializeField]
    private Images images;

    [SerializeField]
    private Text animalTitle;
    [SerializeField]
    private Text animalName;
    [SerializeField]
    private Text animalType;
    [SerializeField]
    private Image animalImage;

    public void OnAnimalCome(GameControllerScript game, Animal animal)
    {
        setup(animal, game.getAnimalCounter(), images);
    }

    public void OnGameInit(GameControllerScript game, Day day)
    {

    }

    public void setup(Animal animal, int animalCounter, Images images)
    {
        animalTitle.text = string.Format("ANIMAL #{0}", animalCounter + 1);
        animalName.text = animal.name;
        animalType.text = animal.typeName;
        animalImage.sprite = images.getImageById(animal.iconId);
    }
}
