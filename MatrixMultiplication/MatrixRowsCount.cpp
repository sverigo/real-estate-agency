#include "Header.h"

int CountMatrixRows(double** matrix, int m, int n, bool useOmp);

void LaunchMatrixRowsCount(int m, int n, bool useOmp)
{
	if (useOmp)
		Log("OMP ON");
	else
		Log("OMP OFF");

	Log("Matrix: " + to_string(m) + " x " + to_string(n));

	double** matrix = RandomZeroOneMatrix(m, n);

	Log("Result: " + to_string(CountMatrixRows(matrix, m, n, useOmp)));

	DeallocateMatrix(matrix, m, n);

	Log("\n");
}

int CountMatrixRows(double** matrix, int m, int n, bool useOmp)
{
	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();
	int count = 0;

	watch->Start();

	#pragma omp parallel if (useOmp) shared(count)
	{
		#pragma omp for reduction(+:count)
		for (int i = 0; i < m; i++)
		{
			if (matrix[i][i] == 0)
			{
				count++;
			}
		}
	}

	Log("Time ellapsed: " + to_string(watch->ElapsedTicks));
	watch->Reset();

	return count;
}