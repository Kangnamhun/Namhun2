using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Exp", fileName = "ExpReward_")]
public class ExpReward : Reward
{
    public override void Give(Quest quest)
    {
        PlayerStatus playerStatus = Managers.Game.GetPlayer().GetComponent<PlayerStatus>();
        playerStatus.exp += Quantity;
    }
}