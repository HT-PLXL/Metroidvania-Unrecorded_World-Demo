using UnityEngine;

public class Skill_Crystal : Skill_Base
{
    [SerializeField][cn("水晶预制体")] private GameObject CrystalPerfab;
    [SerializeField][cn("引爆时间")] private float detonateTime = 2;
    public void CreateCrystal()
    {
        GameObject crystal = Instantiate(CrystalPerfab, transform.position, Quaternion.identity);
        crystal.GetComponent<SkillObject_Crystal>().SetUpCrystal(detonateTime);
    }
}
