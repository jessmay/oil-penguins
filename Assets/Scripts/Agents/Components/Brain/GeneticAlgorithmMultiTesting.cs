using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GeneticAlgorithmMultiTesting : MonoBehaviour {
	
	public Genome[] population {get; private set;}
	public Genome[] prevPopulation {get; private set;}
	public int populationSize {get; private set;}
	
	public const double mutationRate = 0.2;
	public const double crossoverRate = 0.8;
	
	public int generation {get; private set;}
	public int populationIndex {get; private set;}
	
	private double totalFitness;
	
	public int tick {get; private set;}
	public int currTarget {get; private set;}
	
	public double bestFitness {get; private set;}
	public Genome mostFit {get; private set;}


	public static int TICKS_PER_GENOME() {
		int value = 700 * (Options.mapName.Equals("TrainingMap")?1:5);
		return value;
	}
	//public const int TARGETS_PER_GENOME() { ;

	[HideInInspector]
	public TrainingAgent[] testSubjects;

	[HideInInspector]
	public GameMap gameMap;

	void Awake() {
		Options.Testing = true;
		Options.geneticAlgorithm = this;
	}

	// Use this for initialization
	void Start ()
	{
		
		gameMap = GetComponent<GameMap>();

		if(Options.populationName == null){

			Type genome = Type.GetType(Options.genomeType == null? "Explorer1Genome": Options.genomeType);//FiveLongFeelerGenome//Explorer1Genome
			populationSize = 50;

			population = new Genome[populationSize];
			
			for (int currGenome = 0; currGenome < populationSize; ++currGenome) {
				
				population[currGenome] = Genome.createGenome(genome);
			}
			
			generation = 0;
		}
		else  {

			string fileName = Options.GADirectory +"/" +Options.populationName+".txt";

			Debug.Log ("Loading Population from file "+fileName +".");

			loadPopulationFromString(File.ReadAllText(fileName));
		}

		prevPopulation = null;

		testSubjects = new TrainingAgent[populationSize];

		for(int currIndividual = 0; currIndividual < populationSize; ++currIndividual) {

			//Vector3 location = gameMap.map.cellIndexToWorld(gameMap.map.HumanSpawnPoints[currTarget]);
			//Quaternion rotation = Quaternion.LookRotation(transform.forward, Vector3.zero - location);
			Quaternion rotation = gameMap.map.getSpawnAngle(currTarget);
			gameMap.spawnHumanImmediate(gameMap.map.HumanSpawnPoints[currTarget], Options.mapName.Equals("TrainingMap")?gameMap.Human.transform.rotation: rotation, population[currIndividual]);
			
			testSubjects[currIndividual] = gameMap.HumansOnMap[currIndividual].GetComponent<TrainingAgent>();
		}


		initialize();
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		//End of current target test.
		if (++tick == TICKS_PER_GENOME()) {
			
			//Move to next target.
			currTarget = (currTarget+1) % gameMap.map.HumanSpawnPoints.Count;

			for(int currIndividual = 0; currIndividual < populationSize; ++currIndividual) {
				//Notify agent of end of target test
				testSubjects[currIndividual].brain.endOfTarget();
				
				//calculate fitness and add to genomes current total.
				population[currIndividual].totalFitness += population[currIndividual].calculateFitness();
				
				//Get new test location
				Vector3 location = gameMap.map.cellIndexToWorld(gameMap.map.HumanSpawnPoints[currTarget]);
				Quaternion rotation = gameMap.map.getSpawnAngle(currTarget);//Quaternion.LookRotation(transform.forward, Vector3.zero - location);
				
				//Reset the agent to new starting values.
				testSubjects[currIndividual].reset(location, Options.mapName.Equals("TrainingMap")?gameMap.Human.transform.rotation: rotation);

			}

			//Finished testing current genome
			if(currTarget == 0) {

				bestFitness = 0;

				for(int currIndividual = 0; currIndividual < populationSize; ++currIndividual) {
					//Notify agent
					testSubjects[currIndividual].brain.endOfTests();
					
					//Calculate fitness for current genome
					population[currIndividual].fitness = population[currIndividual].totalFitness/gameMap.map.HumanSpawnPoints.Count;
					
					//Add fitness to total.
					totalFitness += population[currIndividual].fitness;
					
					//Debug.Log("Population["+currIndividual+"] " +population[currIndividual].fitness);
					
					//Save the best fitness for the generation.
					bestFitness = Math.Max(bestFitness, population[currIndividual].fitness);
					
					//Move to next genome
					++populationIndex;
					
					//Replace weights in agent with new genome's weights.
					testSubjects[currIndividual].replaceBrain(population[currIndividual%populationSize]);

				}

				//End of one generation
				//Debug.Log("Generation "+generation +" completed");
					
				createNewPopulation();
				
				populationIndex = 0;
				totalFitness = 0;
				++generation;

				for(int currIndividual = 0; currIndividual < populationSize; ++currIndividual) {
					testSubjects[currIndividual].replaceBrain(population[currIndividual%populationSize]);
					population[currIndividual%populationSize].initialize(testSubjects[currIndividual]);
				}
			}
			
			tick = 0;
		}
	}

	bool showLoadMenu = false;
	Vector2 scrollPosition;
	void OnGUI () {

		int width = 300;
		int height = 50;

		Time.timeScale = GUI.HorizontalSlider (new Rect (Screen.width/2 - width/2, 25, width, height), Time.timeScale, 0.0f, 5.0f);
		
		GUI.color = Color.black;
		GUI.Label(new Rect(Screen.width/2 - width/2, 25 + height, width, height*2), "Generation["+generation+"]\nPrevious best: " +bestFitness +"\nTarget "+(currTarget+1) +"/" +gameMap.map.HumanSpawnPoints.Count);

		GUI.color = Color.white;
		//load/save buttons here

		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height - (25 + 2*height), width, height), "Save Population")) {

			string fileName = Options.GADirectory +"/"+DateTime.Now.ToString().Replace('/','-').Replace(':','.') + ".txt";
			Debug.Log("Saving population to file "+fileName +".");
			File.WriteAllText(fileName, getPopulationAsAString(prevPopulation));

		}

		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height - (25 + height), width, height), "Save Best Individual") && mostFit != null) {

			//string fileName = Options.GenomeDirectory +"/"+DateTime.Now.ToString().Replace('/','-').Replace(':','.') + ".txt";
			string fileName = mostFit.GetType().Name;
			int count = 1;

			while (File.Exists(Options.GenomeDirectory +"/" + fileName + count.ToString("D2") + ".txt")) {
				++count;
			}

			fileName = Options.GenomeDirectory +"/" + fileName + count.ToString("D2") + ".txt";

			Debug.Log("Saving most fit to file "+fileName +".");
			File.WriteAllText(fileName, mostFit.save());
		}

//		if(GUI.Button(new Rect(Screen.width/2 - width/2, Screen.height - (25 + height), width, height), "Load Population")) {
//
//			showLoadMenu = !showLoadMenu;
//			scrollPosition = Vector2.zero;
//		}

		if(showLoadMenu) {

			string path = Options.GADirectory;
			
			//If the path does not exist, create it.
			if(!Directory.Exists(path)) 
				Directory.CreateDirectory(path);
			
			//Get all files in the current directory.
			string[] fileList = Directory.GetFiles(path + "/");
			List<string> fileNames = new List<string>();
			
			//Loop through each file in the directory
			for (int currFile = 0; currFile < fileList.Length; ++currFile) {
				
				//Skip all hidden files (example: .DS_Store)
				if(fileList[currFile].Remove(0,fileList[currFile].LastIndexOf('/')+1)[0] == '.')
					continue;
				
				//Add file name to the list of file names
				//Path and directory information is removed first.
				fileNames.Add(fileList[currFile].Remove(fileList[currFile].LastIndexOf('.')).Remove(0,fileList[currFile].LastIndexOf('/')+1));
			}
			
			fileList = fileNames.ToArray();


			int startX = Screen.width/2 + width/2;
			int startY = Screen.height - (25 + 2*height);
			int sWidth = 600;
			int buttonHeight = 50;

			int scrollBarWidth = 16;
			int maxFilesPerScroll = 2;

			//SrollView background
			GUI.Box(new Rect (startX, startY, sWidth/2, maxFilesPerScroll*buttonHeight),GUIContent.none);
			

			//Scrollable view to display all file options to the user.
			scrollPosition = GUI.BeginScrollView (new Rect (startX, startY, sWidth/2, maxFilesPerScroll*buttonHeight), scrollPosition, new Rect (0, 0, sWidth/2-scrollBarWidth, buttonHeight * fileList.Length));
			
			//Button for each file in the directory
			for (int currMap = 0; currMap < fileList.Length; ++currMap) {
				
				//Return filename if button is clicked.
				if(GUI.Button (new Rect(0, (0+currMap)*(buttonHeight), sWidth/2-(fileList.Length> maxFilesPerScroll?scrollBarWidth:0), buttonHeight), fileList[currMap])) {

					string fileName = path +"/" +fileList[currMap]+".txt";
					Debug.Log ("Loading Population from file "+fileName +".");
					loadPopulationFromString(File.ReadAllText(fileName));


					//testSubjects.reset();


					showLoadMenu = false;
				}
			}
			
			// End the scroll view that we began above.
			GUI.EndScrollView ();
		}
	}

	
	//Initialize genetic algorithm.
	private void initialize() {
		
		populationIndex = 0;
		tick = 0;
		currTarget = 0;
		totalFitness = 0;
		bestFitness = 0;
		mostFit = population[0];

		//testSubjects.replaceBrain(population[populationIndex]);
	}

	
	//Get the genome that is currently being tested.
	public Genome getCurrentGenome() {
		return population[populationIndex];
	}
	
	
	//Reset the current genome.
	public void resetCurrentGenome() {
		
		currTarget = 0;
		population[populationIndex].totalFitness = 0;
	}


	//Create a new population selecting and crossing genomes of the previous population.
	private void createNewPopulation() {
		
		//Sort the population by fitness.
		Array.Sort(population);
		
		//Save the fittests genome
		mostFit = population[0];//new Genome(population[0]);
		
		//Create a new population.
		Genome[] newPopulation = new Genome[populationSize];
		
		//Save the top genomes from the previous population.
		int keepBestNum = 5;
		elitism(newPopulation, keepBestNum);
		
		//Crossover until new population is full.
		for (int currGenome = keepBestNum; currGenome < populationSize; ++currGenome) {
			
			//Select two parents at random through a roulette wheel and cross them.
			Genome[] children = Genome.multiPointCrossover(rouletteWheelSelection(), rouletteWheelSelection(), crossoverRate);
			
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

		//Save previous population
		prevPopulation = population;

		//Save over old population.
		population = newPopulation;
	}
	
	
	//Save the top genomes from the previous population into the new population.
	private void elitism(Genome[] newPopulation, int topBest) {
		
		string fittest = "";
		
		for (int currBest = 0; currBest < topBest; ++currBest) {
			fittest += population[currBest].fitness +" ";
			newPopulation[currBest] = Genome.createGenome(population[currBest].GetType(), population[currBest].weights); 
		}
		
		//Debug.Log ("Saving these fitness values: "+fittest);
	}
	
	
	//Select a genome at random
	private Genome rouletteWheelSelection() {
		
		//Select random value.
		double loc = UnityEngine.Random.value * totalFitness;
		double currTotalFitness = 0;
		
		//Find individual that holds that value.
		int index;
		for (index = 0; index < populationSize && currTotalFitness < loc; ++index) {
			currTotalFitness += population[index].fitness;
		}
		
		//Return selected individual.
		return Genome.createGenome(population[index%populationSize].GetType(), population[index%populationSize].weights); ; //population[index%populationSize];
	}
	
	
	private static char separator = '|';
	
	//Return the entire population as a string.
	//Used in saving the current population.
	public string getPopulationAsAString(Genome[] population = null) {

		int generation = this.generation-1;

		if(population == null) {
			population = this.population;
			++generation;
		}

		string pop = population.Length + "\n" +generation +"\n" +separator;
		
		for (int currGenome = 0; currGenome < population.Length; ++currGenome) {
			pop += population[currGenome].save() + (currGenome +1 == population.Length? "":""+separator);
		}
		
		return pop;
	}
	
	
	//Create a population from a string.
	//Used in loading
	public void loadPopulationFromString(string contents) {
		
		char[] separators1 = new char[1];
		separators1[0] = separator;
		string[] c = contents.Split(separators1);
		
		char[] separators2 = new char[1];
		separators2[0] = '\n';
		
		string[] init = c[0].Split(separators2);
		
		int populationSize = Convert.ToInt32(init[0]);
		int generationNumber = Convert.ToInt32(init[1]);
		
		Genome[] population = new Genome[populationSize];
		
		for (int currGenome = 0; currGenome < populationSize; ++currGenome) {
			population[currGenome] = Genome.load(c[1+currGenome]);
		}

		this.populationSize = populationSize;
		this.generation = generationNumber;
		this.population = population;

		initialize();

		//testSubject.reset();
	}
}

