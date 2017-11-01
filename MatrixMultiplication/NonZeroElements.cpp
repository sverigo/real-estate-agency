#include "Header.h"

using namespace std;

int CountNonZeroValues(double** matrix, int m, int n, bool useOmp);

void LaunchNonZeroSelection(int m, int n, bool useOmp)
{
	if (useOmp)
		Log("OMP ON");
	else
		Log("OMP OFF");
	Log("Matrix: " + to_string(m) + " x " + to_string(n));

	double** matrix = RandomZeroOneMatrix(m, n);

	Log("Result: " + to_string(CountNonZeroValues(matrix, m, n, useOmp)));

	DeallocateMatrix(matrix, m, n);

	Log("\n");
}

double** RandomZeroOneMatrix(int m, int n)
{
	Log("Creating random '0-1' matrix...");
	double** matrix = new double*[m];
	for (int i = 0; i < m; ++i)
	{
		matrix[i] = new double[n];
		for (int j = 0; j < n; j++)
		{
			matrix[i][j] = std::rand() % 2;
		}
	}
	return matrix;
}

int CountNonZeroValues(double** matrix, int m, int n, bool useOmp)
{
	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();
	

	int count = 0;

	int maxThreadsNum = omp_get_max_threads();
	list<double>* vectors = new list<double>[maxThreadsNum];
	
	watch->Start();
		 
	for (int i = 0; i < m; i++)
	{
		#pragma omp parallel if (useOmp) 
		{
			#pragma omp for reduction(+:count) nowait
			for (int j = 0; j < n; j++)
			{
				if (matrix[i][j] != 0)
				{
					count++;
					int threadsIndex = omp_get_thread_num();
				}
			}
		}
	}

	Log("Time ellapsed: " + to_string(watch->ElapsedTicks));
	watch->Reset();

	list<double>* result = &vectors[0];
	for (int i = 1; i < omp_get_max_threads(); i++)
	{
		result->insert(result->end(), vectors[i].begin(), vectors[i].end());
	}


	delete[] vectors;

	

	return count;
}