using UnityEngine;

public class 玩家战斗及目标检测 : 实体战斗及目标检测
{
    [Header("弹反细节")]
    [SerializeField] private float 弹反动作僵直时间;
    public bool 尝试弹反()//bool类型方法必须要有返回值，不然会标红
    {
        bool 已弹反 = false;
        foreach(var target in 获得检测的碰撞体())
        {
            弹反 counterable = target.GetComponent<弹反>();
            //定义接口实例并赋予从碰撞体中筛选出的附加了接口“弹反”的组件
            //如果没有获取到带有弹反接口的组件便为空
            if (counterable != null && counterable.CanBeCountered)
            {
                counterable.HandleCounter(); //运行接口实例的方法
                已弹反 = true;
            }
        }
        return 已弹反;
    }
    public float 获得弹反动作僵直时间()
    {
        return 弹反动作僵直时间;
    }
}
