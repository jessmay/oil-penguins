/*
Joshua Linge
NeuronLayer.cs

2014-03-17
*/

using UnityEngine;
using System.Collections;

public class NeuronLayer {

	public int numNeurons {get; private set;}
	private Neuron[] neurons;

	public NeuronLayer(int numNeurons, int numInputs) {

		this.numNeurons = numNeurons;

		neurons = new Neuron[numNeurons];

		//Initialize each neuron in the layer.
		for(int currNeuron = 0; currNeuron < numNeurons; ++currNeuron) {
			neurons[currNeuron] = new Neuron(numInputs);
		}
	}


	//Given a set of inputs, go through each neuron to retrieve its output.
	public double[] fire (double[] inputs) {

		double[] outputs = new double[numNeurons];

		for(int currNeuron = 0; currNeuron < numNeurons; ++currNeuron) {
			outputs[currNeuron] = neurons[currNeuron].output(inputs);
		}

		return outputs;
	}


	//Replace the weights of each neuron in the layer with the given weights.
	public void replaceWeights(double[][] newWeights) {

		for(int currNeuron = 0; currNeuron < numNeurons; ++currNeuron) {
			neurons[currNeuron].replaceWeights(newWeights[currNeuron]);
		}
	}
	

	//Returns the weights of all the neurons in the layer.
	public double[][] getWeights() {

		double[][] weights = new double[numNeurons][];

		for(int currNeuron = 0; currNeuron < numNeurons; ++currNeuron) {
			weights[currNeuron] = neurons[currNeuron].getWeights();
		}

		return weights;
	}


	//Creates random weights for a neuron layer of the given size and input number.
	public static double[][] createRandomWeights(int numNeurons, int numInputs) {

		double[][] weights = new double[numNeurons][];

		for(int currNeuron = 0; currNeuron < numNeurons; ++currNeuron) {
			weights[currNeuron] = Neuron.createRandomWeights(numInputs, false);
		}

		return weights;
	}


	//Randomize the weights in each neuron in the layer.
	public void randomize() {

		foreach(Neuron n in neurons) {
			n.randomizeWeights();
		}
	}
}
