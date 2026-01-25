using System;
using UnityEngine;
[Serializable]
public class Stat_AttackGroup
{
    [cn("¹¥»÷ËÙ¶È")] public Stat attackSpeed;

    [cn("ÎïÀíÉËº¦")] public Stat damage;
    [cn("±©»÷ÂÊ")] public Stat critChance;
    [cn("±©»÷ÉËº¦")] public Stat critPower;
    [cn("»¤¼×Ï÷¼õ")] public Stat armorReduction;//»¤¼×Ï÷¼õ
    //ÔªËØÉËº¦
    [cn("»ğÑæÉËº¦")] public Stat fireDamage;
    [cn("±ùËªÉËº¦")] public Stat iceDamage;
    [cn("À×µçÉËº¦")] public Stat lightningDamage;//ÉÁµçÉËº¦
}
