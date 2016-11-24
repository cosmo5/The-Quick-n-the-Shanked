using UnityEngine;
using System.Collections;

public interface IState
{
    void Enter(AI_Entity e);
    void Run(AI_Entity e);
    void Exit(AI_Entity e);
}
