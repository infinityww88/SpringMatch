using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace SpringMatch.UI {

	public class HeartInfo : MonoBehaviour
	{
		private Tween updateSeq = null;
		[SerializeField]
		private TMPro.TextMeshProUGUI heartNum;
		
		// Start is called before the first frame update
		void Start()
		{
			updateSeq = DOTween.Sequence().AppendCallback(() => {
				PrefsManager.Inst.UpdateHeartNum();
				heartNum.text = $"{PrefsManager.Inst.HeartNum}";
			}).AppendInterval(1f).SetLoops(-1, LoopType.Restart).SetTarget(gameObject);
		}
	}
}

