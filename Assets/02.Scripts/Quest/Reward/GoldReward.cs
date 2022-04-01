using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Reward/Gold", fileName = "GoldReward_")]
public class GoldReward : Reward
{
    public override void Give(Quest quest)
    {
        PlayerStatus playerStatus = Managers.Game.GetPlayer().GetComponent<PlayerStatus>();
        playerStatus.gold += Quantity;
    }
}
