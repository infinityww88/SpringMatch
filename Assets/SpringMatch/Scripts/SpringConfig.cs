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
		
		public float shrinkToShrinkMaxHeight = 2f;
		
		public float shrinkHeight = 0.4f;
		
		public float gridSlotAutoHeightFactor = 0.7f;
		public float extraSlotAutoHeightFactor = 1f;
		public float slotSlotAutoHeightFactor = 1.5f;
		
		public float maxAutoHeight = 5f;
	}

}
