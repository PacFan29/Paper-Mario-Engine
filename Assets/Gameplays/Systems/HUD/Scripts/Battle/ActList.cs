using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActList : MonoBehaviour
{
    public BattleManager manager;
    [Header("データ")]
    public ActPatterns actPatterns;
    public StatusSaver status;
    public BadgeData badgeData;
    [Header("その他コンテンツ")]
    public RectTransform contents;
    public RectTransform prefab;
    private RectTransform[] list = new RectTransform[25];
    private int selectId = 0;
    private bool inputMemory = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < list.Length; i++) {
            ActListItem item = list[i].GetComponent<ActListItem>();
            item.selected = (i == selectId);

            if (item.selected) {
                manager.abActionId = item.actPatterns.patterns[item.index].patternIndex;
                manager.FPUsed = item.actPatterns.patterns[item.index].FPNeeded;
            }
        }

        if (Input.GetAxis("Vertical") < 0){
            if (!inputMemory){
                inputMemory = true;
                if (selectId < (list.Length - 1)) selectId++;
            }
        } else if (Input.GetAxis("Vertical") > 0){
            if (!inputMemory){
                inputMemory = true;
                if (selectId > 0) selectId--;
            }
        } else {
            inputMemory = false;
        }

        if (status.FP >= manager.FPUsed && Input.GetButtonDown("A")){
            manager.selected = true;
        }
    }

    void Initialize() {
        foreach (Transform obj in contents) {
            if ( 0 <= obj.gameObject.name.LastIndexOf("Clone") ) {
                Destroy(obj.gameObject);
            }
        }

        List<int> abilityList = Filter(manager.abilityId);
        list = new RectTransform[abilityList.Count];

        list[0] = prefab;
        list[0].GetComponent<ActListItem>().index = abilityList[0];
        list[0].GetComponent<ActListItem>().isPartner = IsPartner(actPatterns.patterns[abilityList[0]].act);

		for(int i = 1; i < abilityList.Count; i++)
		{
			list[i] = GameObject.Instantiate(prefab) as RectTransform;
			list[i].SetParent(contents, false);

			list[i].GetComponent<ActListItem>().index = abilityList[i];
            list[i].GetComponent<ActListItem>().isPartner = IsPartner(actPatterns.patterns[abilityList[i]].act);
		}
    }
    List<int> Filter(int actionIndex) {
        List<int> abilityList = new List<int>();

        for (int i = 0; i < actPatterns.patterns.Count; i++){
            bool fill = false;
            bool isJump = (actionIndex == 0 && actPatterns.patterns[i].act == ActPatterns.Act.Jump);
            bool isHammer = (actionIndex == 1 && actPatterns.patterns[i].act == ActPatterns.Act.Hammer);

            if (isJump) {
                fill = true;
            } else if (isHammer) {
                fill = true;
            }

            switch (actPatterns.patterns[i].condition) {
                case ActPatterns.Conditions.BootsRank:
                //条件がランクの場合（ブーツ）
                fill = (status.BootsRank >= actPatterns.patterns[i].value && isJump);
                break;

                case ActPatterns.Conditions.HammerRank:
                //条件がランクの場合（ハンマー）
                fill = (status.HammerRank >= actPatterns.patterns[i].value && isHammer);
                break;
                
                case ActPatterns.Conditions.PartnerRank:
                //条件がランクの場合（仲間）
                break;

                case ActPatterns.Conditions.Badge:
                //条件がバッジの場合
                bool isFit = true;

                foreach(StatusSaver.BadgeStatus badge in status.badges) {
                    if (actPatterns.patterns[i].value == badge.BadgeIndex && isFit) {
                        fill = badge.isAttaching;

                        foreach (BadgeData.Badges.Effects effects in badgeData.data[actPatterns.patterns[i].value].effects) {
                            if (isJump && effects.parameter == BadgeData.Badges.Parameter.Battle_Jump && fill) {
                                break;
                            } else if (isHammer && effects.parameter == BadgeData.Badges.Parameter.Battle_Hammer && fill) {
                                break;
                            } else {
                                fill = false;
                            }
                        }
                        break;
                    } else {
                        fill = false;
                    }
                }

                if (status.badges.Count <= 0) {
                    fill = false;
                }
                break;
            }

            if (fill) abilityList.Add(i);
        }

        return abilityList;
    }
    bool IsPartner(ActPatterns.Act act) {
        return (
            act == ActPatterns.Act.PartnerAbilities ||
            act == ActPatterns.Act.PartnerTactics
        );
    }
}
