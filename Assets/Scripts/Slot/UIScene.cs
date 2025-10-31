using UnityEngine;

public enum UISceneName
{
    Title,
    Game,
    Pose,
    Result,
    Ranking,
}

[System.Serializable]
public class UIScene
{
    public UISceneName name;
    public GameObject rootObject;

    public UISceneName GetName()
    {
        return name;
    }

    public GameObject GetRootObject()
    {
        return rootObject;   
    }
}