using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace SpringMatchEditor {
	
	public class DialogUI : MonoBehaviour
	{
		[MyUTKElementAttr("Dialog")]
		private VisualElement _dialog;
		[MyUTKElementAttr("DialogDesc")]
		private Label _desc;
		[MyUTKElementAttr("DialogOkButton")]
		private Button _button;
		
		private Action _okAction;
		
		// Start is called before the first frame update
		void Start()
		{
			Utils.InitUTK(this);
			_button.RegisterCallback<ClickEvent>(evt => {
				_dialog.style.display = DisplayStyle.None;
				_okAction?.Invoke();
			});
		}

		public void Show(string desc, Action showAction, Action okAction) {
			_desc.text = desc;
			_okAction = okAction;
			_dialog.style.display = DisplayStyle.Flex;
			showAction?.Invoke();
		}
	}

}
