using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using Sirenix.OdinInspector;

namespace SpringMatch {
	
	[RequireComponent(typeof(Animator))]
	public class SimpleAnimator : MonoBehaviour
	{
		public AnimationClip clip;
		private PlayableGraph playableGraph;
		private Playable clipPlayable;
		[SerializeField]
		private UnityEngine.Events.UnityEvent onEnd;
		private bool isEnd = true;
		
		// Start is called on the frame when a script is enabled just before any of the Update methods is called the first time.
		protected void Awake()
		{
			playableGraph = PlayableGraph.Create();
			playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
			var playableOutput = AnimationPlayableOutput.Create(playableGraph,
				"Animation",
				GetComponent<Animator>());
			clipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
			Debug.Log($"--- {playableGraph} {clipPlayable}");
			playableOutput.SetSourcePlayable(clipPlayable);
		}
		
		// This function is called when the MonoBehaviour will be destroyed.
		protected void OnDestroy()
		{
			playableGraph.Destroy();
		}
		
		// This function is called when the behaviour becomes disabled () or inactive.
		protected void OnDisable()
		{
			playableGraph.Stop();
		}
		
		[Button]
		public void Play() {
			Debug.Log($"Play {playableGraph} {clipPlayable}");
			isEnd = false;
			playableGraph.Play();
			clipPlayable.SetTime(0);
		}
		
		[Button]
		public void Stop() {
			playableGraph.Stop();
		}
		
		// Update is called every frame, if the MonoBehaviour is enabled.
		protected void Update()
		{
			if (clipPlayable.GetTime() > clip.length) {
				if (!isEnd) {
					Debug.Log($"OnEnd");
					onEnd.Invoke();
				}
				isEnd = true;
			}
			
		}
	}

}
