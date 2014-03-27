/*
Joshua Linge
Genome.cs

2014-03-17
*/

using UnityEngine;
using System;
using System.Collections;

public class GenomeOld : IComparable<GenomeOld> {

	public double[][][] weights;
	public double fitness;
	public double totalFitness;


	//One individual in the genetic algorithm population.
	public GenomeOld (double[][][] w) {

		weights = copyWeights(w);
		fitness = 0;
		totalFitness = 0;
	}


	//Copy constructor.
	public GenomeOld(GenomeOld g) {

		weights = copyWeights(g.weights);
		fitness = 0;
		totalFitness = 0;
	}


	//Sort genomes in decreasing fitness (highest first)
	public int CompareTo(GenomeOld other) 
	{
		return other.fitness.CompareTo(fitness);
	}


	//Given a set of weights, return a copy.
	public static double[][][] copyWeights(double[][][] source) {

		int w = source.Length;
		
		double[][][] weights = new double[w][][];
		
		for (int i = 0; i < weights.Length; ++i) {

			weights[i] = new double[source[i].Length][];
			
			for (int j = 0; j < weights[i].Length; ++j) {
				
				weights[i][j] = new double[source[i][j].Length];
				Array.Copy(source[i][j], weights[i][j], weights[i][j].Length);
			}
		}
		
		return weights;
	}


	//Go through each weight in the neural network and change values on the mutation rate.
	public void mutate(double mutationRate) {

		for (int i = 0; i < weights.Length; ++i) {
			for (int j = 0; j < weights[i].Length; ++j) {
				for (int k = 0; k < weights[i][j].Length; ++k) {
					if(UnityEngine.Random.value < mutationRate)
						weights[i][j][k] += UnityEngine.Random.Range(-1.0f, 1.0f) * 0.6;
				}	
			}
		}
	}


	//Given two parents, return a child that is a cross of these two genomes.
	//Multi point crossover.
	public static GenomeOld[] multiPointCrossover (GenomeOld g1, GenomeOld g2, double crossoverRate) {

		//Return parents if over crossover rate.
		if(UnityEngine.Random.value > crossoverRate) {
			GenomeOld[] ret = { new GenomeOld(g1), new GenomeOld(g2)};
			
			return ret;
		}

		int w = g1.weights.Length;

		double[][][] weights1 = new double[w][][];
		double[][][] weights2 = new double[w][][];

		for (int i = 0; i < weights1.Length; ++i) {
			weights1[i] = new double[g1.weights[i].Length][];
			weights2[i] = new double[g1.weights[i].Length][];

			for (int j = 0; j < weights1[i].Length; ++j) {

				weights1[i][j] = new double[g1.weights[i][j].Length];
				weights2[i][j] = new double[g1.weights[i][j].Length];

				if(UnityEngine.Random.value < .5){
					Array.Copy(g1.weights[i][j], weights1[i][j], weights1[i][j].Length);
					Array.Copy(g2.weights[i][j], weights2[i][j], weights2[i][j].Length);
				}
						
				else {
					Array.Copy(g2.weights[i][j], weights1[i][j], weights1[i][j].Length);
					Array.Copy(g1.weights[i][j], weights2[i][j], weights2[i][j].Length);
				}
			}
		}

		GenomeOld[] genomes = { new GenomeOld(weights1), new GenomeOld(weights2)};
		
		return genomes;
	}

	//Given two parents, return two children that are crosses of these two genomes.
	//Single point crossover.
	public static GenomeOld[] singlePointCrossover (GenomeOld g1, GenomeOld g2, double crossoverRate) {

		
		if(UnityEngine.Random.value > crossoverRate) {
			GenomeOld[] ret = { new GenomeOld(g1), new GenomeOld(g2)};
			
			return ret;
		}

		int w = g1.weights.Length;
		
		double[][][] weights1 = new double[w][][];
		double[][][] weights2 = new double[w][][];

		int rand1 = UnityEngine.Random.Range(0,w);
		int rand2 = UnityEngine.Random.Range(0,g1.weights[rand1].Length);

		for (int i = 0; i < weights1.Length; ++i) {
			weights1[i] = new double[g1.weights[i].Length][];
			weights2[i] = new double[g1.weights[i].Length][];

			for (int j = 0; j < weights1[i].Length; ++j) {
				
				weights1[i][j] = new double[g1.weights[i][j].Length];
				weights2[i][j] = new double[g1.weights[i][j].Length];

				if(i < rand1 || (i == rand1 && j < rand2)) {
					Array.Copy(g1.weights[i][j], weights1[i][j], weights1[i][j].Length);
					Array.Copy(g2.weights[i][j], weights2[i][j], weights2[i][j].Length);
				}
				else {
					Array.Copy(g2.weights[i][j], weights1[i][j], weights1[i][j].Length);
					Array.Copy(g1.weights[i][j], weights2[i][j], weights2[i][j].Length);
				}
			}
		}

		GenomeOld[] genomes = { new GenomeOld(weights1), new GenomeOld(weights2)};

		return genomes;
	}


	//Return the weights for this genome as a string.
	//Used in saving to a file.
	public string getWeightsAsString() {

		string w = "";
		w += weights.Length +"\n";

		for (int currLayer = 0; currLayer < weights.Length; ++currLayer) {

			w += weights[currLayer].Length +"\n";

			for (int currNeuron = 0; currNeuron < weights[currLayer].Length; ++currNeuron) {

				w += weights[currLayer][currNeuron].Length +"\n";

				for (int currWeight = 0; currWeight < weights[currLayer][currNeuron].Length; ++currWeight) {
					w += weights[currLayer][currNeuron][currWeight] + "\n";
				}
			}
		}

		return w;
	}


	//Create the weights array from the given string.
	//Used in loading from a file.
	public static double[][][] createWeightsFromString(string[] w) {

		int index = 0;
		int num1 = Convert.ToInt32 (w[index++]);
		double[][][] weights = new double[num1][][];

		for (int currLayer = 0; currLayer < weights.Length; ++currLayer) {

			int num2 = Convert.ToInt32 (w[index++]);
			weights[currLayer] = new double[num2][];
			
			for (int currNeuron = 0; currNeuron < weights[currLayer].Length; ++currNeuron) {

				int num3 = Convert.ToInt32 (w[index++]);
				weights[currLayer][currNeuron] = new double[num3];
				
				for (int currWeight = 0; currWeight < weights[currLayer][currNeuron].Length; ++currWeight) {

					weights[currLayer][currNeuron][currWeight] = Convert.ToDouble(w[index++]);
				}
			}
		}

		return weights;
	}
}
