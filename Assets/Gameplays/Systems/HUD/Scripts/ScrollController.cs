using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour {

	[SerializeField]
	RectTransform prefab = null;
	public int count = 10;
	public int pattern = 0;
	private RectTransform[] list = new RectTransform[10];
	private int selectIndex = 0;
	private bool inputMemory = false;

	void Start () 
	{
		list = new RectTransform[count];
		list[0] = prefab;

		for(int i = 1; i < list.Length; i++)
		{
			list[i] = GameObject.Instantiate(prefab) as RectTransform;
			list[i].SetParent(transform, false);

			list[i].GetComponent<ListCommon>().index = i;
		}
	}

	void Update() {
		int length = 0;
		foreach (RectTransform listItem in list) {
			ListCommon thisList = listItem.GetComponent<ListCommon>();

			switch(pattern) {
				case 0:
				//アイテム
				length = thisList.status.items.Count;
				break;
				
				case 1:
				//大事なもの
				break;

				case 2:
				//バッジ
				length = thisList.status.badges.Count;
				break;
			}
			listItem.gameObject.SetActive(length > thisList.index);
			thisList.selected = (true && thisList.index == selectIndex);
		}

		if (true) {
			if (Input.GetAxis("Vertical") < 0){
                if (!inputMemory){
					inputMemory = true;
					if (selectIndex < (length - 1)) selectIndex++;
                }
            } else if (Input.GetAxis("Vertical") > 0){
                if (!inputMemory){
					inputMemory = true;
					if (selectIndex > 0) selectIndex--;
                }
            } else {
                inputMemory = false;
            }
		}
	}
}