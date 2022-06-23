using System;
using UnityEngine;
using UnityEngine.Events;


public class EventManager
{
    public static Action OnMonsterTrigger;
    public static Action OnSafeSpaceTrigger;
    public static void MonsterTrigger()
    {
        OnMonsterTrigger?.Invoke();
    }
    public static void SafeSpaceTrigger()
    {
        OnSafeSpaceTrigger?.Invoke();
    }
    
}
