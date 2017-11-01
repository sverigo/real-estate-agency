#include "Header.h"

void AvgNeighbourElements(double* arr, int n, int ngCount, bool useOmp);

void LaunchArrayAvgNeghbours(int n, int count, bool useOmp)
{
	if (useOmp)
		Log("OMP ON");
	else
		Log("OMP OFF");
	Log("Array: " + to_string(n));

	double* arr = RandomArray(n);

	Log("Count: " + to_string(count));

	AvgNeighbourElements(arr, n, count, useOmp);

	delete[] arr;

	Log("\n");
}

void AvgNeighbourElements(double* arr, int n, int ngCount, bool useOmp)
{
	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();


	int maxThreadsNum = omp_get_max_threads();
	list<double>* vectors = new list<double>[maxThreadsNum];

	watch->Start();

	#pragma omp parallel if (useOmp) shared(ngCount) num_threads(2)
	{
		int count = 1;
		double sum = 0;
		#pragma omp for schedule(static, ngCount) private(count, sum)
		for (int i = 0; i < n; i++)
		{
			sum+=arr[i];

			if (count % ngCount == 0 || i + 1 == n)
			{
				int threadsIndex = omp_get_thread_num();

				if (vectors[threadsIndex].size() < vectors[threadsIndex].max_size() - 50)
				{
					vectors[threadsIndex].push_back(sum / count);
					count = 0;
					sum = 0;
				}
			}

			count++;
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
}