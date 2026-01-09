using System.Security.Cryptography;
using UnityEngine;

public class UI_SkillTree : MonoBehaviour
{
    public int skillPoint;

    public bool haveEnoughSkillPoint(int cost)=>skillPoint >= cost;
    public void RemoveSkillPoint(int cost)=>skillPoint -= cost;
}
