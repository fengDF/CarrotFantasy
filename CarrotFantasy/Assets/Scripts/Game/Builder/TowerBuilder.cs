using UnityEngine;

/// <summary>
/// 塔的建造者
/// </summary>
public class TowerBuilder : IBuilder<Tower>
{
    public int towerID;
    //private GameObject tower;
    public int towerLevel;

    public void GetData(Tower productClass)
    {
        productClass.towerID = towerID;
    }

    public void GetOtherResource(Tower productClass)
    {
        productClass.GetTowerProperty();
    }

    public GameObject GetProduct()
    {
        GameObject go = GameController.Instance.GetItem("Tower/ID" + towerID + "/TowerSet/" + towerLevel);
        Tower tower = GetProductClass(go);
        GetData(tower);
        GetOtherResource(tower);
        return go;
    }

    public Tower GetProductClass(GameObject gameObject)
    {
        return gameObject.GetComponent<Tower>();
    }
}
