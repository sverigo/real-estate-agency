#include "Header.h"

void TrasposeAndMultiply(double** matrix1, double** matrix2, int n, bool useOmp);

void LaunchComplexMultiplication(int n, bool useOmp)
{
	if (useOmp)
		Log("\nOMP ON");
	else
		Log("\nOMP OFF");
	Log("Matrix: " + to_string(n) + " x " + to_string(n));

	double** matrix1 = RandomMatrix(n, n);
	double ** matrix2 = RandomMatrix(n, n);

	TrasposeAndMultiply(matrix1, matrix2, n, useOmp);

	DeallocateMatrix(matrix1, n, n);
	DeallocateMatrix(matrix2, n, n);
}

void TrasposeAndMultiply(double** matrix1, double** matrix2, int n, bool useOmp)
{
	double** tmp = new double*[n];
	for (int i = 0; i < n; i++)
	{
		tmp[i] = new double[n];
	}

	double** result = new double*[n];
	for (int i = 0; i < n; i++)
	{
		result[i] = new double[n];
	}

	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();

	watch->Start();

	#pragma omp parallel if(useOmp)
	{
		#pragma omp for
		for (int i = 0; i < n; ++i)
			for (int j = 0; j < n; ++j)
				tmp[i][j] = matrix2[j][i];

		#pragma omp for
		for (int i = 0; i < n; ++i)
			for (int j = 0; j < n; ++j)
				for (int k = 0; k < n; ++k)
					result[i][j] += matrix1[i][k] * tmp[j][k];

	}

	Log("Time ellapsed: " + to_string(watch->ElapsedTicks));
	watch->Reset();

	DeallocateMatrix(result, n, n);
	DeallocateMatrix(tmp, n, n);
}