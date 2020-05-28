public interface IGameListener
{
    void OnGameInit(GameControllerScript game, Day day);
    void OnAnimalCome(GameControllerScript game, Animal animal);
    void OnChoiceDone(GameControllerScript game, Animal animal);
}
