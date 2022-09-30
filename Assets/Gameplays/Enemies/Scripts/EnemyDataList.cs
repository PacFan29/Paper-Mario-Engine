using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDataList", menuName = "ScriptableObjects/Create EnemyData")]
public class EnemyDataList : ScriptableObject
{
    // 敵の情報(EnemyData)をまとめたリスト
    public List<EnemyData> enemyDatas = new List<EnemyData>();

    /// <summary>
    /// 敵の情報(1体ずつの情報)
    /// </summary>
    [Serializable]              // この属性情報を忘れないようにしてください。設定しないとインスペクターに表示されません。
    public class EnemyData {　　// 敵の情報です。【１】で設定した順番に登録できるように書いています。他にも追加したい敵の情報があれば、ここに追加してください。
        public GameObject Prefab;
        public string Name;
        public int Level;
        public int MaxHP;
        public int Power;
        public int Defense;
        public int minCoins = 0;
        public int maxCoins = 1;
        public int baseEXP = 0;
        public int[] abnormalRates = {
            100, //眠り
            100, //グルグル
            100, //混乱
            100, //ミニミニ
            100, //ストップ
            100, //フニャフニャ
            100, //炎
            100, //氷
            100, //逃げる
            100, //吹き飛ばし
            100  //KO
        };
        
        [TextArea]
        public string[] tattleData;
        public bool isTattled;
    }
}
