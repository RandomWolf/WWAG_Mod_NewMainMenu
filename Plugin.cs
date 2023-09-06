using BepInEx;
using Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wizgun;
using System.Collections;
namespace WWAG_NewIntro
{
	// Plugin Info
    [BepInPlugin("org.NikoTheFox.WWAGNewIntro", "New WWAG Main Menu Scene", "0.0.6")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Debug.Log($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
		}
		void OnSceneLoaded(Scene scene, LoadSceneMode mode)
		{
			if (scene.name == "title-screen")
			{

				// When the Scene "Title-Screen" is loaded
				// Add the PluginAnimationHandler to the backgroundCanvas Object
				Debug.Log($"Found scene, loading animation");
				GameObject backgroundCanvas = GameObject.Find("background-canvas");
				backgroundCanvas.AddComponent<PluginAnimationHandler>();

			}
		}
		void OnEnable()
		{
			Debug.Log($"Plugin enabled");
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

	}
	public class PluginAnimationHandler : MonoBehaviour
	{
		private void Awake()
		{
			Debug.Log("In MonoBehavior");
			PlaySpineAnimations();
		}

		void PlaySpineAnimations()
		{
			Debug.Log("Starting");
			// Checking if all the neccesary objecs are loaded and found
			string activeSceneName = SceneManager.GetActiveScene().name;
			if (activeSceneName != "title-screen")
			{
				Debug.Log("Not in the title-screen scene");
				return;
			}
			GameObject backgroundCanvas = GameObject.Find("background-canvas");
			if (backgroundCanvas == null)
			{
				Debug.Log("background-canvas not found");
				return;
			}
			GameObject playerAnimation = backgroundCanvas.transform.Find("player-animation").gameObject;
			if (playerAnimation == null)
			{
				Debug.Log("player-animation not found");
				return;
			}


			Debug.Log($"Trying to invoke corouting ");
			StartCoroutine(StartPlaySequence());
		}
	
		IEnumerator StartPlaySequence()
		{
			Debug.Log("Starting Coroutine");

			string activeSceneName = SceneManager.GetActiveScene().name;
			GameObject backgroundCanvas = GameObject.Find("background-canvas");
			GameObject playerAnimation = backgroundCanvas.transform.Find("player-animation").gameObject;
			yield return new WaitForSeconds(5f);
			Skins.PlayerSkins playerSkins = playerAnimation.GetComponent<Skins.PlayerSkins>();

			// Playing the animations one after another, with a delay
			Debug.Log($"Playing first animation");
			var skeletonAnimation = playerAnimation.GetComponent<Spine.Unity.SkeletonAnimation>();
			var animationStateData = skeletonAnimation.skeletonDataAsset.GetAnimationStateData();
			animationStateData.SetMix("zz-unused/sitting-examples", "actions/sit/chair/02-sit-chair-idle", 0.5f);
			var trackEntry = skeletonAnimation.AnimationState.SetAnimation(0, "zz-unused/sitting-examples", false);
			trackEntry.Complete += (entry) => { skeletonAnimation.AnimationState.SetAnimation(0, "actions/sit/chair/02-sit-chair-idle", true); };

			yield return new WaitForSeconds(20f);
			Debug.Log($"Playing second animation (Sitting -> Gun)");
			var animationStateDataSP = skeletonAnimation.skeletonDataAsset.GetAnimationStateData();
			animationStateDataSP.SetMix("actions/sit/chair/02-sit-chair-idle", "actions/cinematic/view-weapon/view-pistol", 0.5f);
			var trackEntrySP = skeletonAnimation.AnimationState.SetAnimation(0, "actions/sit/chair/02-sit-chair-idle", false);
			trackEntrySP.Complete += (entry) => { skeletonAnimation.AnimationState.SetAnimation(0, "actions/cinematic/view-weapon/view-pistol", true); };

			Debug.Log($"Playing second animation (Gun -> Sitting)");
			yield return new WaitForSeconds(5f);
			var animationStateDataPS = skeletonAnimation.skeletonDataAsset.GetAnimationStateData();
			animationStateDataPS.SetMix("actions/cinematic/view-weapon/view-pistol", "actions/sit/chair/02-sit-chair-idle", 0.5f);
			var trackEntryPS = skeletonAnimation.AnimationState.SetAnimation(0, "actions/cinematic/view-weapon/view-pistol", false);
			trackEntryPS.Complete += (entry) => { skeletonAnimation.AnimationState.SetAnimation(0, "actions/sit/chair/02-sit-chair-idle", true); };


		}
	
	}

}
