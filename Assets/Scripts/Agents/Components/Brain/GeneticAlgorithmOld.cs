/*
Joshua Linge
GeneticAlgorithm.cs

2014-03-17
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GeneticAlgorithmOld {

	public GenomeOld[] population {get; private set;}
	public int populationSize {get; private set;}

	public const double mutationRate = 0.2;
	public const double crossoverRate = 0.8;

	public int generation {get; private set;}
	public int populationIndex {get; private set;}

	private double totalFitness;

	public int tick {get; private set;}
	public int currTarget {get; private set;}

	public double bestFitness {get; private set;}
	public GenomeOld mostFit {get; private set;}


	public const int TICKS_PER_GENOME = 500;

	public GeneticAlgorithmOld (int populationSize, int numInputs, int numOutputs, int numLayers, int numNeuronsPerLayer) {

		this.populationSize = populationSize;
		population = new GenomeOld[populationSize];

		for (int currGenome = 0; currGenome < populationSize; ++currGenome) {

			population[currGenome] = new GenomeOld(NeuralNet.createRandomWeights(numInputs, numOutputs, numLayers, numNeuronsPerLayer));
		}

		generation = 0;

		initialize();
	}

	public GeneticAlgorithmOld (int populationSize, int generation, GenomeOld[] population) {

		this.populationSize = populationSize;
		this.generation = generation;
		this.population = population;

		initialize();
	}


	//Initialize genetic algorithm.
	private void initialize() {

		populationIndex = 0;
		tick = 0;
		currTarget = 0;
		totalFitness = 0;
		bestFitness = 0;
		mostFit = population[0];
	}


	//Get the genome that is currently being tested.
	public GenomeOld getCurrentGenome() {
		return population[populationIndex];
	}


	//Reset the current genome.
	public void resetCurrentGenome() {

		currTarget = 0;
		population[populationIndex].totalFitness = 0;
	}


	//Update the genetic algorithm.
	public void update (TestAgent agent) {

		//End of current target test.
		if (++tick == TICKS_PER_GENOME) {

			//Move to next target.
			currTarget = (currTarget+1) % agent.totalTargets();

			//Notify agent of end of target test
			agent.endOfTarget(currTarget);

			//calculate fitness and add to genomes current total.
			population[populationIndex].totalFitness += agent.calculateFitness();

			//Reset the agent back to starting values.
			agent.reset();

			//Finished testing current genome
			if(!agent.targetsEnabled || currTarget == 0) {

				//Notify agent
				agent.endOfTests();

				//Calculate fitness for current genome
				population[populationIndex].fitness = population[populationIndex].totalFitness/(agent.targetsEnabled?agent.totalTargets():1);

				//Add fitness to total.
				totalFitness += population[populationIndex].fitness;
				
				Debug.Log("Population["+populationIndex+"] " +population[populationIndex].fitness);

				//Save the best fitness for the generation.
				bestFitness = Math.Max(bestFitness, population[populationIndex].fitness);

				//Move to next genome
				++populationIndex;

				//Replace weights in agent with new genome's weights.
				agent.replaceBrain(population[populationIndex%populationSize].weights);

				//End of one generation
				if(populationIndex == populationSize) {
					Debug.Log("Generation "+generation +" completed");
					
					createNewPopulation();
					
					populationIndex = 0;
					totalFitness = 0;
					bestFitness = 0;
					++generation;
				}
			}

			tick = 0;
		}
	}


	//Create a new population selecting and crossing genomes of the previous population.
	private void createNewPopulation() {

		//Sort the population by fitness.
		Array.Sort(population);

		//Save the fittests genome
		mostFit = new GenomeOld(population[0]);

		//Create a new population.
		GenomeOld[] newPopulation = new GenomeOld[populationSize];

		//Save the top genomes from the previous population.
		int keepBestNum = 5;
		elitism(newPopulation, keepBestNum);

		//Crossover until new population is full.
		for (int currGenome = keepBestNum; currGenome < populationSize; ++currGenome) {

			//Select two parents at random through a roulette wheel and cross them.
			GenomeOld[] children = GenomeOld.singlePointCrossover(rouletteWheelSelection(), rouletteWheelSelection(), crossoverRate);

			//Add first child to the population and mutate.
			newPopulation[currGenome] = children[0];
			newPopulation[currGenome].mutate(mutationRate);

			++currGenome;

			//If population is not full, add second child and mutate.
			if(currGenome < populationSize) {
				newPopulation[currGenome] = children[1];
				newPopulation[currGenome].mutate(mutationRate);
			}
		}

		//Save over old population.
		population = newPopulation;
	}


	//Save the top genomes from the previous population into the new population.
	private void elitism(GenomeOld[] newPopulation, int topBest) {

		string fittest = "";

		for (int currBest = 0; currBest < topBest; ++currBest) {
			fittest += population[currBest].fitness +" ";
			newPopulation[currBest] = new GenomeOld(population[currBest]); 
		}

		Debug.Log ("Saving these fitness values: "+fittest);
	}


	//Select a genome at random
	private GenomeOld rouletteWheelSelection() {

		//Select random value.
		double loc = UnityEngine.Random.value * totalFitness;
		double currTotalFitness = 0;

		//Find individual that holds that value.
		int index;
		for (index = 0; index < populationSize && currTotalFitness < loc; ++index) {
			currTotalFitness += population[index].fitness;
		}

		//Return selected individual.
		return new GenomeOld(population[index%populationSize]);
	}


	private static char separator = '|';

	//Return the entire population as a string.
	//Used in saving the current population.
	public string getPopulationAsAString() {

		string pop = populationSize + "\n" +generation +"\n" +separator;
		
		for (int currGenome = 0; currGenome < populationSize; ++currGenome) {
			pop += population[currGenome].getWeightsAsString() + (currGenome +1 == populationSize? "":""+separator);
		}

		return pop;
	}


	//Create a population from a string.
	//Used in loading
	public static GeneticAlgorithmOld loadPopulationFromString(string contents) {

		char[] separators1 = new char[1];
		separators1[0] = separator;
		string[] c = contents.Split(separators1);

		char[] separators2 = new char[1];
		separators2[0] = '\n';

		string[] init = c[0].Split(separators2);

		int populationSize = Convert.ToInt32(init[0]);
		int generationNumber = Convert.ToInt32(init[1]);

		GenomeOld[] population = new GenomeOld[populationSize];

		for (int currGenome = 0; currGenome < populationSize; ++currGenome) {
			population[currGenome] = new GenomeOld(GenomeOld.createWeightsFromString(c[1+currGenome].Split(separators2)));
		}

		return new GeneticAlgorithmOld(populationSize, generationNumber, population);
	}
}
