using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGlobalSubscriber
{

}

public interface IGameOverHandler : IGlobalSubscriber
{
    void HandleGameOver();
}

public interface IOnGameOverHandler : IGlobalSubscriber
{
    void HandleOnGameOver();
}
