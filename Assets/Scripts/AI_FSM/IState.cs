using UnityEngine;
using System.Collections;

public interface IState<T>
{
    void Enter(T e);
    void Run(T e);
    void Exit(T e);
}
