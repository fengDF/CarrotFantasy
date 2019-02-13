using UnityEngine;

public class MonsterBuilder : IBuilder<Monster>
{
    public int monsterID;
    // private GameObject monster;
   
    public void GetData(Monster productClass)
    {
        productClass.monsterID = monsterID;
        // 怪物的数值都是简单设计
        productClass.hp = monsterID * 100;
        productClass.currentHP = productClass.hp;
        productClass.initMoveSpeed = (float)monsterID/3 + 1;
        productClass.moveSpeed = productClass.initMoveSpeed;
        productClass.prize = 50 + monsterID * 50;
    }

    public void GetOtherResource(Monster productClass)
    {
        productClass.GetMonsterProperty();
    }

    public GameObject GetProduct()
    {
        GameObject go = GameController.Instance.GetItem("Monster");
        Monster monster = GetProductClass(go);
        GetData(monster);
        GetOtherResource(monster);
        return go;
    }

    public Monster GetProductClass(GameObject gameObject)
    {
        return gameObject.GetComponent<Monster>();
    }
}
