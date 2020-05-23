using System.Collections.Generic;

[System.Serializable]
public class LevelResult
{
    public string[] types = { };
    public int[] counts = { };

    public int getCountOf(string type)
    {
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].Equals(type))
            {
                return counts[i];
            }
        }
        return 0;
    }

    public void increaseCountOf(string type)
    {
        for (int i = 0; i < types.Length; i++)
        {
            if (types[i].Equals(type))
            {
                counts[i] += 1;
                return;
            }
        }

        string[] prevTypes = types;
        types = new string[types.Length + 1];
        prevTypes.CopyTo(types, 0);

        int[] prevCounts = counts;
        counts = new int[counts.Length + 1];
        prevCounts.CopyTo(counts, 0);

        types[types.Length - 1] = type;
        counts[counts.Length - 1] = 1;
    }

    public static LevelResult generateLevelResult(Day day, List<HistoryEntry> animals, AnalyseSystem analyseSystem, AnimalTypes animalTypes)
    {
        LevelResult levelResult = new LevelResult();

        foreach (HistoryEntry entry in animals)
        {
            countAnimal(day, entry.animal, entry.action, levelResult, analyseSystem, animalTypes);
        }

        return levelResult;
    }

    private static void countAnimal(Day currentDay, Animal animal, string action, LevelResult levelResult, AnalyseSystem analyseSystem, AnimalTypes animalTypes)
    {
        bool isPredator = animalTypes.getTypeById(animal.typeId).predator || animalTypes.getTypeById(animal.voice).predator || animalTypes.getTypeById(animal.footprints).predator;

        if (action == "call" && isPredator)
        {
            levelResult.increaseCountOf("predatorCalled");
            return;
        }
        if (action == "accept" && isPredator)
        {
            levelResult.increaseCountOf("predatorAccepted");
            return;
        }
        if (action == "deny" && isPredator)
        {
            return;
        }

        foreach (Rule rule in currentDay.rules)
        {
            foreach (string noValue in rule.noValues)
            {
                if (animal.typeId.Equals(noValue) && action.Equals("accept"))
                {
                    levelResult.increaseCountOf("predatorAccepted");
                    return;
                }

                if (noValue.Equals("infected") && !analyseSystem.isAnalyseGood(animal) && action.Equals("accept"))
                {
                    levelResult.increaseCountOf("illAccepted");
                    return;
                }
            }
        }

        if (analyseSystem.isAnalyseGood(animal) && action.Equals("deny"))
        {
            levelResult.increaseCountOf("healthyDenied");
            return;
        }

        switch (animal.typeId)
        {
            case "sheep":
                levelResult.increaseCountOf("sheep");
                break;
            case "cow":
                levelResult.increaseCountOf("cow");
                break;
            case "pig":
                levelResult.increaseCountOf("pig");
                break;
            default:
                levelResult.increaseCountOf("goodAccepted");
                break;
        }
    }
}
