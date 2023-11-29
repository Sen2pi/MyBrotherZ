using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagement : Singleton<SceneManagement>
{
    public string SceneTransitionName { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }
    public void SetTransitionName(string name)
    {
        SceneTransitionName = name;
    }
}
