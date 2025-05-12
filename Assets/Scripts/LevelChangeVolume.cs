using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChangeVolume : MonoBehaviour
{
    [SerializeField] LevelsManager.Scenes SceneToSwitchTo;
    bool triggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (triggered || !other.CompareTag("Player"))
        {
            return;
        }
        if (GameplayController.Get().GetCurrentZone().HasNecessaryInteractionLeft())
        {
            ChatManager.Get().PlayNotDoneWithZoneChat();
            return;
        }
        triggered = true;
        switch (SceneToSwitchTo)
        {
            case LevelsManager.Scenes.Surface:
                LevelsManager.Get().LoadSurfaceCavesScene();
                break;
            case LevelsManager.Scenes.HigherCave:
                LevelsManager.Get().LoadHigherCavesScene();
                break;
            case LevelsManager.Scenes.LowerCave:
                LevelsManager.Get().LoadLowerCavesScene();
                break;
            default:
                LevelsManager.Get().LoadSurfaceCavesScene();
                break;
        }
    }
    
}
