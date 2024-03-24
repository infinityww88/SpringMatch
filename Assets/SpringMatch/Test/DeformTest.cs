using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class DeformTest : MonoBehaviour
{
	public CurveTest curveTest0;
	public CurveTest curveTest1;
	public CurveTest curveTest2;
	
	[Range(0, 1)]
	[OnValueChanged("OnFactorChange")]
	public float factor;
	
	void OnFactorChange() {
		if (factor == 0) {
			curveTest0.factor = 0;
			curveTest1.factor = 0;
			curveTest2.factor = 0;
		}
		else {
			curveTest0.factor = 0.2f + 0.8f * factor;
			curveTest1.factor = 0.1f + 0.9f * factor;
			curveTest2.factor = factor;
		}
		
		curveTest0.OnFactorChange();
		curveTest1.OnFactorChange();
		curveTest2.OnFactorChange();
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
