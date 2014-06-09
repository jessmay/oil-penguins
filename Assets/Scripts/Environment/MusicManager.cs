using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour {
	
	public AudioSource betweenWaveSound;
	public AudioSource duringWaveSound;

	public WaveManager waveManager;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.F4)) {

			if (Options.playMusic == true) {
				Options.playMusic = false;

				mute();
			}
			else {
				Options.playMusic = true;

				unmute();
			}
		}

	}

	public void playWaveMusic() {

		if(Options.playMusic) {
			duringWaveSound.Play();
			betweenWaveSound.Stop();
		}

	}

	public void playBetweenWavesMusic(){

		if(Options.playMusic) {
			duringWaveSound.Stop();
			betweenWaveSound.Play();
		}
	}

	public void mute() {
		betweenWaveSound.Stop();
		duringWaveSound.Stop();
	}

	public void unmute() {
	
		if(waveManager.betweenWaves) {
			playBetweenWavesMusic();
		}
		else {
			playWaveMusic();
		}

	}

}
