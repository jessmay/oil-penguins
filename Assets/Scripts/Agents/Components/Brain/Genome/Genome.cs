using UnityEngine;
using System;
using System.Collections;

public abstract class Genome : IComparable<Genome> {

	public double[][][] weights;
	protected NeuralNet brain;
	protected float feelerLength;

	public Genome(double[][][] weights) {
		this.weights = weights;

		brain = new NeuralNet(weights);

		feelerLength = getDefaultLengthOfFeelers();
	}

	public Genome() {

		brain = new NeuralNet(getNumberOfInputs(), getNumberOfOutputs(), getNumberOfLayers(), getNumberOfNeuronsPerLayer());

		weights = brain.getWeights();

		feelerLength = getDefaultLengthOfFeelers();
	}

	public abstract void initialize(Agent agent);

	public abstract int getNumberOfInputs();
	public abstract int getNumberOfOutputs();
	public abstract int getNumberOfLayers();
	public abstract int getNumberOfNeuronsPerLayer();

	public abstract double getFiredValue();
	public abstract float getLengthOfFeelers(Agent agent);
	public abstract float getDefaultLengthOfFeelers();
	public abstract int getNumberOfFeelers();
	public abstract int getViewAngle();

	public abstract double[] sense<A>(A agent) where A : Agent, ITarget;
	public abstract double[] think(Agent agent, double[] senses);
	public abstract void act(Agent agent, double[] thoughts);

	public abstract void update(TestableAgent agent);

	public abstract void OnCollisionEnter(Collision2D collision);
	public abstract void OnCollisionExit(Collision2D collision);

	public double fitness;
	public double totalFitness;
	public abstract double calculateFitness();

	public abstract void reset();
	public abstract void endOfTarget();
	public abstract void endOfTests();

	protected void moveToTestStart(TestableAgent agent) {

		agent.transform.position = agent.map.cellIndexToWorld(agent.map.HumanSpawnPoints[Options.geneticAlgorithm.currTarget]);//agent.startPosition;//agent.map.cellIndexToWorld(agent.map.getRandomHumanSpawn());
		Quaternion rotation = Quaternion.LookRotation(agent.transform.forward, Vector3.zero - (agent.transform.position));//agent.map.cellIndexToWorld(agent.transform.position)
		
		if(!Options.mapName.Equals("TrainingMap"))
			agent.turn(rotation.eulerAngles.z - agent.transform.rotation.eulerAngles.z);

	}
	
	public abstract string getDebugInformation();


	//Sort genomes in decreasing fitness (highest first)
	public int CompareTo(Genome other) {
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


	public static Genome createGenome(Type genome, double[][][] weights) {
		return (Genome)Activator.CreateInstance(genome, new System.Object[]{weights});
	}

	public static Genome createGenome(Type genome) {
		return (Genome)Activator.CreateInstance(genome);
	}

	//Given two parents, return a child that is a cross of these two genomes.
	//Multi point crossover.
	public static Genome[] multiPointCrossover<G> (G g1, G g2, double crossoverRate) where G : Genome{

		Type GenomeType = g1.GetType();

		//Return parents if over crossover rate.
		if(UnityEngine.Random.value > crossoverRate) {
			Genome[] ret = { createGenome(GenomeType, g1.weights), createGenome(GenomeType, g2.weights)};
			
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
		
		Genome[] genomes = { createGenome(GenomeType, weights1), createGenome(GenomeType, weights2)};
		
		return genomes;
	}
	
	//Given two parents, return two children that are crosses of these two genomes.
	//Single point crossover.
	public static Genome[] singlePointCrossover<G> (G g1, G g2, double crossoverRate) where G : Genome {

		Type GenomeType = g1.GetType();
		
		if(UnityEngine.Random.value > crossoverRate) {
			Genome[] ret = { createGenome(GenomeType, g1.weights), createGenome(GenomeType, g2.weights)};
			
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
		
		Genome[] genomes = { createGenome(GenomeType, weights1), createGenome(GenomeType, weights2)};
		
		return genomes;
	}


	public string save() {

		string contents = GetType().Name + " " + feelerLength + "\n";

		contents += getWeightsAsString();

		return contents;
	}


	public static Genome load(string contents) {

		string[] header = contents.Substring(0, contents.IndexOf("\n")).Split(new char[]{' '});

		string typeName = header[0];

		contents = contents.Substring(contents.IndexOf("\n")+1);

		double[][][] weights = createWeightsFromString(contents.Split(new char[]{'\n'}));

		Genome genome = createGenome(Type.GetType(typeName), weights);

		if(header.Length > 1)
			genome.feelerLength = Convert.ToSingle(header[1]);

		return genome;
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
