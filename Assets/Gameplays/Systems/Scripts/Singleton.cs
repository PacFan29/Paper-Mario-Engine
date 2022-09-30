using UnityEngine;
using UnityEngine.SceneManagement;

public class Singleton : MonoBehaviour {

	// インスタンスが存在するか？
	static bool existsInstance = false;

	// 初期化
	void Awake () {
		// インスタンスが存在するなら破棄する
		if (existsInstance)
		{
			Destroy(gameObject);
			return;
		}

		// 存在しない場合
		// 自身が唯一のインスタンスとなる
		existsInstance = true;
		DontDestroyOnLoad(gameObject);
	}

    void Update() {
		if (GameManager.Death) {
			SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
			existsInstance = false;
		} else {
			foreach (Transform obj in this.transform) {
				if (LayerMask.LayerToName(obj.gameObject.layer) != "UI") {
					obj.gameObject.SetActive(SceneManager.GetActiveScene().name != "BattleScene");
				}
			}
		}
    }
}