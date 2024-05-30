using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	[CreateAssetMenu(menuName="SpringMatch/SpringConfig", fileName="SpringConfig", order=-1)]
	public class SpringConfig : ScriptableObject
	{
		[BoxGroup("Tween Duration")]
		public float gridSlotMoveDuration = 0.2f;
		[BoxGroup("Tween Duration")]
		public float girdSlotShrinkDuration = 0.2f;
		
		[BoxGroup("Tween Duration")]
		public float slotSlotDuration = 0.1f;
		
		[BoxGroup("Tween Duration")]
		public float slotExtraMoveDuration = 0.3f;
		[BoxGroup("Tween Duration")]
		public float slotExtraShrinkDuration = 0.3f;
		
		[BoxGroup("Tween Duration")]
		public float holeGridDuration = 0.2f;
		
		[BoxGroup("Tween Duration")]
		public float colliderRadius = 0.35f;
		
		[BoxGroup("Tween Duration")]
		public int colliderGenerateDelayFrame = 3;
		
		[BoxGroup("Tween Duration")]
		public float jumpDelay = 0.1f;
		
		[BoxGroup("Move Curve Factor")]
		public float headHeightFactor = 0.2f;
		[BoxGroup("Move Curve Factor")]
		public float headHeightOffset = 0;
		[BoxGroup("Move Curve Factor")]
		public float footHeightFactor = 0;
		[BoxGroup("Move Curve Factor")]
		public float footHeightOffset = 1;
		[BoxGroup("Move Curve Factor")]
		public float handHeightFactor = 0.2f;
		[BoxGroup("Move Curve Factor")]
		public float handHeightOffset = 0;
		
		[BoxGroup("Tween Height")]
		public float shrinkToShrinkMaxHeight = 2f;
		[BoxGroup("Tween Height")]
		public float shrinkHeight = 0.4f;
		[BoxGroup("Tween Height")]
		public float gridSlotAutoHeightFactor = 0.7f;
		[BoxGroup("Tween Height")]
		public float extraSlotAutoHeightFactor = 1f;
		[BoxGroup("Tween Height")]
		public float slotSlotAutoHeightFactor = 1.5f;
		[BoxGroup("Tween Height")]
		public float maxAutoHeight = 5f;
		[BoxGroup("Tween Height")]
		public float extraHeight = 3f;
		
		[BoxGroup("Spring LOD")]
		public List<string> springPoolKeys;
		[BoxGroup("Spring LOD")]
		public List<float> springInitLength;
		[BoxGroup("Spring LOD")]
		public Vector2 lodRange;
		
		public AnimationCurve footLerpFactorCurve;
		public AnimationCurve handLerpFactorCurve;
		public AnimationCurve headLerpFactorCurve;
		
		[BoxGroup("Spring Scale")]
		public float maxScale = 3;
		[BoxGroup("Spring Scale")]
		public float minScale = 1;
		[BoxGroup("Spring Scale")]
		public float scaleFactor = 1f;
	}

}
