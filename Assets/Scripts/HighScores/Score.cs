using UnityEngine;
using System;
using System.Collections;

public class Score : IComparable<Score> {

	public int waveReached {get; private set;}
	public int killedHumans {get; private set;}
	public float timePlayed {get; private set;}
	private DateTime dayScored;

	public Score(int waveReached, int killedHumans, float timePlayed, DateTime dayScored) {

		this.waveReached = waveReached;
		this.killedHumans = killedHumans;
		this.timePlayed = timePlayed;
		this.dayScored = dayScored;
	}
	
	//Sort scores based on decreasing number of waves,
	// decreasing number of killed humans,
	// increasing time played to complete waves,
	// increasing date
	public int CompareTo(Score other) {

		int ret = other.waveReached - waveReached;

		if(ret == 0) {
			ret = other.killedHumans - killedHumans;
		}

		if(ret == 0) {
			ret = timePlayed.CompareTo(other.timePlayed);
		}

		if(ret == 0) {
			ret = dayScored.CompareTo(other.dayScored);
		}

		return ret;
	}
	
	public string save() {
		return waveReached +" " +killedHumans +" " + timePlayed + " " + dayScored.Ticks;
	}

	public static Score load(string contents) {
		string[] values = contents.Split(new char[] {' '});
		return new Score(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToSingle(values[2]), new DateTime(Convert.ToInt64(values[3]))); 
	}
}
