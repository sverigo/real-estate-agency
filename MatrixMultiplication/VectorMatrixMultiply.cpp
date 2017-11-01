#include "Header.h"

void MultiplicateVectorMatrix(double** matrix, double* vector, int m, int n, bool useOmp);

void LaunchVectorMultiplication(int m, int n, bool useOmp)
{
	if (useOmp)
		Log("OMP ON");
	else
		Log("OMP OFF");

	Log("Matrix: " + to_string(m) + " x " + to_string(n));

	double** matrix = RandomMatrix(m, n);
	double* vector = RandomArray(m);
	
	MultiplicateVectorMatrix(matrix, vector, m, n, useOmp);

	DeallocateMatrix(matrix, m, n);
	delete[] vector;

	Log("\n");
}

void MultiplicateVectorMatrix(double** matrix, double* vector, int m, int n, bool useOmp)
{
	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();

	watch->Start();

	#pragma omp parallel if (useOmp)
	{
		#pragma omp for 
		for (int i = 0; i < m; i++)
		{
			double sum = 0;
			for (int j = 0; j < n; j++)
			{
				sum += matrix[i][j] * vector[i];
			}

			vector[i] = sum;
		}
	}

	Log("Time ellapsed: " + to_string(watch->ElapsedTicks));
	watch->Reset();
}