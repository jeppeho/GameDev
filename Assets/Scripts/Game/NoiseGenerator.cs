using UnityEngine;
using System.Collections;


/**
 * TODO
 * 1. Keep only the PerlinNoise1D method.
 * 2. Map in the end of perlin noise? Add variables. 
 * 3. Get control of variables
 * 4. Make GetSet methods of relevant variables
 * 
 * - Make seed
 * - Varier størrelsen af et sample, til brug ved større og mindre mønstre (eller er det octaves
 * - Det kan nok godt optimeres
 * - Create GetPerlinNoise1D(), where you can input numSamples, numDimensions, numOctaves etc, and get values
 * - make noise array multiple dimensions
 * - Map, normalize and squish noise array dimensions 
 * - Use noise array in LevelGenerator
 * 
 */


public class NoiseGenerator : MonoBehaviour {

	private int numSamples = 128;

	public float[] GetRandomNoise1D(float minMap, float maxMap){

		float[] noise = new float[numSamples];

		//Assign values to the noise array
		for (int sample = 0; sample < numSamples; sample++)
			noise [sample] = GetRandomNumber (); 

		//Map the noise
		noise = Map (noise, minMap, maxMap);

		//Return the noise
		return noise;
	}
		
	/**
	 * Returns an array containing values with perlin noise
	 * Use the parameters to define the noise
	 */
	public float[] GetPerlinNoise1D(int _firstOctave, int _numOctaves, float persistence, float minMap, float maxMap){

		//SetRandomSeed(4);

		float[] noise = new float[numSamples];
		float[] tempNoise = new float[numSamples + 1];

		//FOR EACH OCTAVE
		for (int octave = _firstOctave; octave < _numOctaves; octave++) {

			int frequency = Mathf.FloorToInt (Mathf.Pow (2, octave));
			float amplitude = Mathf.Pow (persistence, octave);
			int interval = numSamples / frequency;

			//Set first value in array
			tempNoise [0] = GetRandomNumber () * amplitude;

			//FOR EACH FREQUENCY SAMPLE
			for (int freqStep = 0; freqStep < frequency; freqStep++) {

				int freqIndex = freqStep * interval;

				//Set random value
				tempNoise [freqIndex + interval] = GetRandomNumber () * amplitude;

				//FOR ALL OTHER SAMPLES
				for (int sample = 0; sample < interval; sample++) {

					//Find the actual index
					int sampleIndex = freqIndex + sample;

					//Create vectors to interpolate between
					Vector2 v1 = new Vector2 (freqIndex, tempNoise [freqIndex]);
					Vector2 v2 = new Vector2 (freqIndex + interval, tempNoise [freqIndex + interval]);

					//Set interpolated value
					noise [freqIndex + sample] += LinearInterpolation (sampleIndex, v1, v2);
				}
			}
		}

		//Map the noise
		noise = Map (noise, minMap, maxMap);

		return noise;
	}

	public void SetRandomSeed(int seed){
		Random.seed = seed;
	}

	private float GetRandomNumber(){
		return Random.value;	//Random.Range (-1f, 1f);
	}

	/**
	 * Interpolates between two vectors, at the point 'sample'
	 */
	private float LinearInterpolation(int sample, Vector2 v1, Vector2 v2){
		return v1.y + (sample - v1.x) * (v2.y - v1.y) / (v2.x - v1.x);
	}


	/**
	 * Maps the inputted array from it's old mapping the specified
	 * @param noise: The array to be remapped
	 * @param newMin: The new minimum value in the array
	 * @param newMax: The new maximum value in the array
	 */
	private float[] Map(float[] noise, float newMin, float newMax){

		float oldMin = 1000000, oldMax = -1000000;

		//Find min and max value
		for(int sample = 0; sample < numSamples; sample++){

			if (noise [sample] < oldMin)
				oldMin = noise [sample];
			if (noise [sample] > oldMax)
				oldMax = noise [sample];
		}

		//Find the stepwise ratio
		float ratio = (newMax - newMin) / (oldMax - oldMin);

		//Map values
		for (int i = 0; i < numSamples; i++)
			noise [i] = (noise [i] - oldMin) * ratio + newMin;

		//Return noise array
		return noise;
	}


	public int GetNumSamples(){
		return numSamples;
	}
		

	public void CreateVisualAxes(){
		GameObject xAxis = GameObject.CreatePrimitive (PrimitiveType.Cube);
		GameObject yAxis = GameObject.CreatePrimitive (PrimitiveType.Cube);
		GameObject zAxis = GameObject.CreatePrimitive (PrimitiveType.Cube);

		xAxis.transform.position = new Vector3 (0, 0, numSamples / 2);
		yAxis.transform.position = new Vector3 (0, 0, numSamples / 2);
		zAxis.transform.position = new Vector3 (0, 0, numSamples / 2);

		xAxis.transform.localScale = new Vector3 (10, 0.5f, 0.5f);
		yAxis.transform.localScale = new Vector3 (0.5f, 5, 0.5f);
		zAxis.transform.localScale = new Vector3 (0.5f, 0.5f, numSamples);
	}


	public void CreateVisualRepresentationSingle(float[] noise, float scaleY){

		for (int sample = 0; sample < noise.Length; sample++) {

			//noise [sample]

			//GET VALUES
			int y = Mathf.FloorToInt ((noise [sample] * scaleY));
			int x = 20;
			float scale = 2;
			float color = (noise [sample] + 1) / 2; //This only goes well when mapped -1 to 1...

			//Create point for graph
			GameObject point = GameObject.CreatePrimitive (PrimitiveType.Cube);
			point.transform.localScale = new Vector3 (scale, scale, scale);
			point.transform.position = new Vector3 (x, y, sample * 1);
			point.GetComponent<Renderer> ().material.color =  new Color(1 - color, color, 0);
		}
	}

//
//	public float[] GetPerlinNoise1D(int _firstOctave, int _numOctaves, float persistence, float minMap, float maxMap){
//
//		float[] noise = new float[numSamples];
//		float[] tempNoise = new float[numSamples + 1];
//
//		//FOR EACH OCTAVE
//		for (int octave = _firstOctave; octave < _numOctaves; octave++) {
//
//			int frequency = Mathf.FloorToInt (Mathf.Pow (2, octave));
//			float amplitude = Mathf.Pow (persistence, octave);
//
//			int interval = numSamples / frequency;
//
//			//FOR EACH FREQUENCY SAMPLE
//			//note that sample only goes to freq, so not actually sample (maybe freqStep
//			for (int sample = 0; sample < frequency; sample++) {
//
//				int freqIndex = sample * interval;
//
//				//assign value at index 0
//				if (sample == 0)
//					tempNoise [freqIndex] = GetRandomNumber () * amplitude;
//
//				tempNoise [freqIndex + interval] = GetRandomNumber () * amplitude;
//
//				//Dont go array out of range
//				//				if (freqIndex + interval < numSamples)
//				//					tempNoise [freqIndex + interval] = GetRandomNumber () * amplitude;
//
//				//FOR ALL OTHER SAMPLES
//				for (int subSample = 0; subSample < interval; subSample++) {
//
//					int index = freqIndex + subSample;
//
//					Vector2 v1 = new Vector2 (freqIndex, tempNoise [freqIndex]);
//					//Vector2 v2;
//					Vector2 v2 = new Vector2 (freqIndex + interval, tempNoise [freqIndex + interval]);
//
//					//Dont go array out of range
//					//					if (freqIndex + interval < numSamples) {
//					//						Debug.Log ("Calling #1");
//					//						v2 = new Vector2 (freqIndex + interval, tempNoise [freqIndex + interval]);
//					//					} else {
//					//						Debug.Log ("Calling #2, because freqIndex + interval = " + (freqIndex + interval));
//					//						Debug.Log ("frequency = " + frequency);
//					//						Debug.Log ("Interval = " + interval);
//					//						Debug.Log ("freqIndex = " + freqIndex);
//					//						v2 = new Vector2 (freqIndex + interval, 0);
//					//					}
//					//Assign value to the array
//					noise [freqIndex + subSample] += LinearInterpolation (index, v1, v2);
//				}
//			}
//		}
//
//		//Map the noise
//		noise = Map (noise, minMap, maxMap);
//
//		return noise;
//	}
		

}
