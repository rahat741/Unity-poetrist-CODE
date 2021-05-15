/*===============================================================================
Copyright (c) 2017 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

[RequireComponent(typeof(VideoPlayer))]
public class Sahyrlar : MonoBehaviour
{
	#region PRIVATE_MEMBERS

	private VideoPlayer videoPlayer;
	#endregion //PRIVATE_MEMBERS




	#region PUBLIC_MEMBERS

	public Button m_PlayButton;
	public RectTransform m_ProgressBar;

	public GameObject btnZoom;
	public GameObject btnExit;
	public VideoPlayer zoomPlayer;
	public GameObject panel;

	#endregion //PRIVATE_MEMBERS


	#region MONOBEHAVIOUR_METHODS

	void Start()
	{

		videoPlayer = GetComponent<VideoPlayer>();

		// Setup Delegates
		videoPlayer.errorReceived += HandleVideoError;
		videoPlayer.started += HandleStartedEvent;
		videoPlayer.prepareCompleted += HandlePrepareCompleted;
		videoPlayer.seekCompleted += HandleSeekCompleted;
		videoPlayer.loopPointReached += HandleLoopPointReached;

		LogClipInfo();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0)) 
		{

		}
		if (videoPlayer.isPlaying)
		{
			ShowPlayButton(false);
			btnZoom.SetActive (true);
			btnExit.SetActive (false);

			if (videoPlayer.frameCount < float.MaxValue)
			{
				float frame = (float)videoPlayer.frame;
				float count = (float)videoPlayer.frameCount;

				float progressPercentage = 0;

				if (count > 0)
					progressPercentage = (frame / count) * 100.0f;

				if (m_ProgressBar != null)
					m_ProgressBar.sizeDelta = new Vector2((float)progressPercentage, m_ProgressBar.sizeDelta.y);
			}

		}
		else
		{
			ShowPlayButton(true);
		}
	}

	void OnApplicationPause(bool pause)
	{
		Debug.Log("OnApplicationPause(" + pause + ") called.");
		if (pause)
			Pause();
	}

	#endregion // MONOBEHAVIOUR_METHODS


	#region PUBLIC_METHODS

	public void Play()
	{
		Caching.ClearCache ();
		Debug.Log("Play Video");
		PauseAudio(false);
		videoPlayer.GetComponent<MeshRenderer> ().enabled = true;
		videoPlayer.Play();
		zoomPlayer.url = videoPlayer.url;
		zoomPlayer.Play ();
		zoomPlayer.GetComponent<AudioSource> ().enabled = false;
		ShowPlayButton(false);
		btnExit.SetActive (false);
		Button btnZ = btnZoom.GetComponent<Button> ();
		btnZ.onClick.AddListener (TaskOnClick);
	}

	void TaskOnClick()
	{
		panel.SetActive (true);
		btnZoom.SetActive (false);
		btnExit.SetActive (true);

		//zoomPlayer.url = videoPlayer.url;
		videoPlayer.Stop ();
		zoomPlayer.GetComponent<AudioSource> ().enabled = true;
		//zoomPlayer.Play ();
		Button btnE = btnExit.GetComponent<Button> ();
		btnE.onClick.AddListener (TaskExitClick);
	}

	void TaskExitClick(){
		panel.SetActive (false);
		zoomPlayer.Stop ();
		videoPlayer.Play ();
	}

	public void Pause()
	{
		if (videoPlayer)
		{
			Debug.Log("Pause Video");
			PauseAudio(true);
			videoPlayer.Pause();
			ShowPlayButton(true);
		}
	}

	#endregion // PUBLIC_METHODS


	#region PRIVATE_METHODS

	private void PauseAudio(bool pause)
	{
		for (ushort trackNumber = 0; trackNumber < videoPlayer.audioTrackCount; ++trackNumber)
		{
			if (pause)
				videoPlayer.GetTargetAudioSource(trackNumber).Pause();
			else
				videoPlayer.GetTargetAudioSource(trackNumber).UnPause();
		}
	}

	private void ShowPlayButton(bool enable)
	{
		m_PlayButton.enabled = enable;
		m_PlayButton.GetComponent<Image>().enabled = enable;
	}

	// Zoom knopkany gorkezmek ucin
	private void ShowZoomButton(bool enable)
	{
		btnZoom.GetComponent<Button> ().enabled = enable;
		btnZoom.GetComponent<Image>().enabled = enable;
	}

	private void LogClipInfo()
	{
		if (videoPlayer.clip != null)
		{
			string stats =
				"\nName: " + videoPlayer.clip.name +
				"\nAudioTracks: " + videoPlayer.clip.audioTrackCount +
				"\nFrames: " + videoPlayer.clip.frameCount +
				"\nFPS: " + videoPlayer.clip.frameRate +
				"\nHeight: " + videoPlayer.clip.height +
				"\nWidth: " + videoPlayer.clip.width +
				"\nLength: " + videoPlayer.clip.length +
				"\nPath: " + videoPlayer.clip.originalPath;

			Debug.Log(stats);
		}
	}

	#endregion // PRIVATE_METHODS


	#region DELEGATES

	void HandleVideoError(VideoPlayer video, string errorMsg)
	{
		Debug.LogError("Error: " + video.clip.name + "\nError Message: " + errorMsg);
	}

	void HandleStartedEvent(VideoPlayer video)
	{
		Debug.Log("Started: " + video.clip.name);
	}

	void HandlePrepareCompleted(VideoPlayer video)
	{
		Debug.Log("Prepare Completed: " + video.clip.name);
	}

	void HandleSeekCompleted(VideoPlayer video)
	{
		Debug.Log("Seek Completed: " + video.clip.name);
	}

	void HandleLoopPointReached(VideoPlayer video)
	{
		Debug.Log("Loop Point Reached: " + video.clip.name);

		ShowPlayButton(true);
	}

	#endregion //DELEGATES

}
