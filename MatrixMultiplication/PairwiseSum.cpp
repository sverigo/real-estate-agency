#include "Header.h"

std::vector<double> PairwiseSum(std::vector<double> vector, bool useOmp);
double PairwiseSumLoop(double* arr, int n, bool useOmp);

void LaunchPairwiseSum(int n, bool useOmp) {
	if (useOmp)
		Log("\nOMP ON");
	else
		Log("\nOMP OFF");
	Log("Array length: " + to_string(n));
	
	double *arr = RandomArray(n);
	Log("Result: " + to_string(PairwiseSumLoop(arr, n, useOmp)));
	delete[] arr;
}

/*std::vector<double> PairwiseSum(std::vector<double> vector, bool useOmp)
{
	std::vector<double> pairVector;
	#pragma omp parallel if(useOmp)
	{
		#pragma omp for 
		for (int i = 0; i < vector.size(); i += 2)
		{
			double sum = 0;
			if (i + 1 == vector.size())
				sum = vector[i];
			else
				sum = vector[i] + vector[i + 1];

			#pragma omp critical
			pairVector.push_back(sum);
		}
	}
	return pairVector;
}*/

double PairwiseSumLoop(double* arr, int n, bool useOmp) {
	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();

	watch->Start();

	std::vector<double> vector;
	vector.assign(arr, arr + n);

	/*while (vector.size != 1) {
		vector = PairwiseSum(vector, useOmp);
	}*/
		

	Log("Time ellapsedL " + to_string(watch->ElapsedTicks));
	watch->Reset();

	return vector.at(0);
}