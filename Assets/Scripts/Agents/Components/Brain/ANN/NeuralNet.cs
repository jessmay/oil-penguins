/*
Joshua Linge
NeuralNet.cs

2014-03-17
*/

using UnityEngine;
using System.Collections;

public class NeuralNet {

	public int numInputs {get; private set;}
	public int numOutputs {get; private set;}
	public int numLayers {get; private set;}
	public int numNeuronsPerLayer {get; private set;}

	NeuronLayer[] layers;

	public NeuralNet(int numInputs, int numOutputs, int numLayers, int numNeuronsPerLayer) {

		this.numInputs = numInputs;
		this.numOutputs = numOutputs;

		//Additional layer for output neurons
		this.numLayers = numLayers+1;
		this.numNeuronsPerLayer = numNeuronsPerLayer;

		constructNetwork();
	}

	public NeuralNet(double[][][] weights) {

		numInputs = weights[0][0].Length-1;
		numOutputs= weights[weights.Length-1].Length;

		numLayers = weights.Length;
		numNeuronsPerLayer = numLayers == 1? numOutputs: weights[0].Length;

		//Debug.Log(numInputs +" " +numOutputs + " "+ numLayers + " " + numNeuronsPerLayer);

		constructNetwork();

		replaceWeights(weights);
	}


	//Creates a network of neurons based on the initialized values of the neural network.
	private void constructNetwork() {
		
		layers = new NeuronLayer[numLayers];
		
		//Create first layer
		layers[0] = new NeuronLayer((numLayers == 1? numOutputs: numNeuronsPerLayer), numInputs);
		
		//If more than one layer, create inner layers.
		if (numLayers > 1) {
			
			//Create all inner layers
			for (int currLayer = 1; currLayer < numLayers-1; ++currLayer) {
				layers[currLayer] = new NeuronLayer(numNeuronsPerLayer, numNeuronsPerLayer);
			}
			
			//Create output layer
			layers[numLayers-1] = new NeuronLayer(numOutputs, numNeuronsPerLayer);
		}
	}


	//Given a set of inputs, go through each neuron and return the results.
	public double[] fire (double[] inputs) {

		double[] outputs = inputs;

		for (int currLayer = 0; currLayer < numLayers; ++currLayer) {
			outputs = layers[currLayer].fire(outputs);
		}

		return outputs;
	}


	//Replace the weights of all neurons in the network with the given weights.
	public void replaceWeights(double[][][] newWeights) {
		
		for (int currLayer = 0; currLayer < numLayers; ++currLayer) {
			layers[currLayer].replaceWeights(newWeights[currLayer]);
		}
	}


	//Create random weights based on the given size of the neural network.
	public static double[][][] createRandomWeights(int numInputs, int numOutputs, int numLayers, int numNeuronsPerLayer) {
		++numLayers;
		double[][][] weights = new double[numLayers][][];
		
		//Create first layer
		weights[0] = NeuronLayer.createRandomWeights((numLayers == 1? numOutputs: numNeuronsPerLayer), numInputs);
		
		//If more than one layer, create inner layers.
		if (numLayers > 1) {
			
			//Create all inner layers
			for (int currLayer = 1; currLayer < numLayers-1; ++currLayer) {
				weights[currLayer] = NeuronLayer.createRandomWeights(numNeuronsPerLayer, numNeuronsPerLayer);
			}
			
			//Create output layer
			weights[numLayers-1] = NeuronLayer.createRandomWeights(numOutputs, numNeuronsPerLayer);
		}
		
		return weights;
	}


	//Get the weights of all the neurons in this neural netowrk.
	public double[][][] getWeights() {
		
		double[][][] weights = new double[numLayers][][];

		for (int currLayer = 0; currLayer < numLayers; ++currLayer) {
			weights[currLayer] = layers[currLayer].getWeights();
		}
		
		return weights;
	}


	//Randomize all the weights of each neuron in the neural network.
	public void randomize() {

		foreach (NeuronLayer l in layers) {
			l.randomize();
		}
	}
}
