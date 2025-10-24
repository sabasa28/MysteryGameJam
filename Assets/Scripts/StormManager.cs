using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormManager : MonoBehaviour
{
    [SerializeField] GameObject[] windParticles;
    [SerializeField] GameObject[] stormParticles;
    [SerializeField] GameObject[] fakeStormParticles; //las que se muestran cuando estas adentro de la nave
    [SerializeField] Material windowsGlass;
    bool isStormActive = false;

    private void Update()
    {

    }
    private void Start()
    {
        LevelsManager levelsManager = LevelsManager.Get();
        OnEnterSurfaceScene(!levelsManager.GoingUp, levelsManager.persistentData.isReturning);
    }

    void OnEnterSurfaceScene(bool isPlayerInside, bool isReturning)
    {
        ChangeStormState(isReturning);
        ChangeGlassSorting(isPlayerInside);
    }

    void ChangeStormState(bool violent)
    {
        foreach (GameObject windParticle in windParticles)
        {
            windParticle.SetActive(!violent);
        }

        foreach (GameObject stormParticle in stormParticles)
        {
            stormParticle.SetActive(violent);
        }

        foreach (GameObject fakeStormParticle in fakeStormParticles)
        {
            fakeStormParticle.SetActive(false); //no podemos entrar al stage desde adentro de la nave y que haya tormenta
        }

        isStormActive = violent;
    }

    public void OnEnterOrExitShip(bool enter)
    {
        ChangeGlassSorting(enter);
        if (isStormActive)
        {
            foreach (GameObject stormParticle in stormParticles)
            {
                stormParticle.SetActive(!enter);
            }

            foreach (GameObject fakeStormParticle in fakeStormParticles)
            {
                fakeStormParticle.SetActive(enter);
            }
        }
    }

    void ChangeGlassSorting(bool glassOnTopOfDust)
    {
        float newVal = glassOnTopOfDust ? 0 : -1;
        windowsGlass.SetFloat("_QueueOffset", newVal);
    }
}
