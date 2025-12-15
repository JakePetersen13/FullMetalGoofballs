using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : ActionBaseState
{
    public override void EnterState(ActionStateManager actions)
    {
        
    }

    public override void UpdateState(ActionStateManager actions)
    {
        actions.rHandAim.weight = Mathf.Lerp(actions.rHandAim.weight, 1, Time.deltaTime * 10);
        actions.lHandIK.weight = Mathf.Lerp(actions.lHandIK.weight, 1, Time.deltaTime * 10);

        if (Input.GetKeyDown(KeyCode.R) && CanReload(actions))
        {
            Debug.Log("Switching to Reload State");
            actions.SwitchState(actions.Reload);
        }
    }

    bool CanReload(ActionStateManager action)
    {
        if (action.ammo.currentAmmo == action.ammo.clipSize) return false;
        else if (action.ammo.extraAmmo == 0) return false;
        else return true;
    }
}
