#include "Header.h"

int ArrayNumbersInRange(double* arr, int n, double min, double max, bool useOmp);

void LaunchArrayNumbers(int n, bool useOmp)
{
	if (useOmp)
		Log("OMP ON");
	else
		Log("OMP OFF");
	Log("Array: " + to_string(n));

	double* arr = RandomArray(n);

	double min = std::rand() % 10000;
	double max = 10000 - min + std::rand() % 10000;

	Log("Min: " + to_string(min));
	Log("Max: " + to_string(max));

	Log("Result: " + to_string(ArrayNumbersInRange(arr, n, min, max, useOmp)));

	delete[] arr;

	Log("\n");
}

int ArrayNumbersInRange(double* arr, int n, double min, double max, bool useOmp)
{
	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();
	

	int maxThreadsNum = omp_get_max_threads();
	list<double>* vectors = new list<double>[maxThreadsNum];
	int count = 0;

	watch->Start();

	#pragma omp parallel if (useOmp) shared(count)
	{
		#pragma omp for reduction(+:count) 
		for (int i = 0; i < n; i++)
		{
			if (arr[i] >= min && arr[i] <= max)
			{
				count++;

				int threadsIndex = omp_get_thread_num();
				if (vectors[threadsIndex].size() < vectors[threadsIndex].max_size() - 50)
					vectors[threadsIndex].push_back(arr[i]);
			}
		}
	}

	Log("Time ellapsed: " + to_string(watch->ElapsedTicks));
	watch->Reset();

	list<double>* result = &vectors[0];
	for (int i = 1; i < maxThreadsNum; i++)
	{
		result->insert(result->end(), vectors[i].begin(), vectors[i].end());
	}

	delete[] vectors;

	return count;
}

double* RandomArray(int n)
{
	double* arr = new double[n];
	for (int i = 0; i < n; i++)
	{
		arr[i] = std::rand() % 10000;
	}

	return arr;
}