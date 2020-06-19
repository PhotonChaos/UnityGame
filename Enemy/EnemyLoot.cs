using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [System.Serializable]
    public struct ItemDrop
    {
        public EnumItemType item;
        public int countMin;
        public int countMax;
        public float dropChance;
    }

    public List<ItemDrop> Loot;

    private void OnDestroy() {
        List<EnumItemType> drops = new List<EnumItemType>();

        for(int i = 0; i < Loot.Count; i++) {
            for (int j = 0; j < Loot[i].countMin; j++) {
                drops.Add(Loot[i].item);
            }

            for(int j = 0; j < Loot[i].countMax - Loot[i].countMin; j++) {
                if(Random.value < Loot[i].dropChance) {
                    drops.Add(Loot[i].item);
                }
            }
        }

        if(drops.Count == 0) {
            drops.Add(EnumItemType.POINT);
        }

        foreach(EnumItemType item in drops) {
            StageController.DropItem(item, transform.position);
        }
    }
}
