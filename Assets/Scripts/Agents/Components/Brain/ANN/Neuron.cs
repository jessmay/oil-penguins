/*
Joshua Linge
Neuron.cs

2014-03-17
*/

using UnityEngine;
using System.Collections;

public class Neuron {

	public int numInputs {get; private set;}
	private double[] weights;

	public static double bias = 1.0;

	public Neuron(int numInputs) {

		//Extra input for bias
		this.numInputs = numInputs+1;

		weights = new double[this.numInputs];

		//Initialize with random weights
		randomizeWeights();
	}


	//Set the weights of this neuron to have random values.
	public void randomizeWeights() {

		weights = createRandomWeights(numInputs);
	}


	//Create an array of random weight values.
	public static double[] createRandomWeights(int numWeights, bool includeBias = true) {

		double[] weights = new double[numWeights + (includeBias?0:1)];

		for (int currWeight = 0; currWeight < weights.Length; ++currWeight) {
			weights[currWeight] = Random.Range(-1.0f, 1.0f);
		}

		return weights;
	}


	//Run the neuron on a given set of input, returning the output.
	public double output (double[] input) {
		double sum = 0;

		for (int currInput = 0; currInput < input.Length; ++currInput) {
			sum += weights[currInput] * input[currInput];
		}

		sum += weights[numInputs-1] * bias;

		return sigmoid((float)sum);
	}


	//Replace the weights of this neuron with the given weights.
	public void replaceWeights(double[] newWeights) {
		weights = newWeights;
	}


	//Returns the weights of this neuron.
	public double[] getWeights() {
		return weights;
	}


	public static double sigmoid(float value, float p = 1.0f) {
		return 1.0f/(1 + Mathf.Exp(-value / p));
	}
}
