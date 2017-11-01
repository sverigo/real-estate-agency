#include "Header.h"

#include <ctime>

using namespace std;

void MatrixMultiplying(int k);
void NonZeroElements();
void NumbersRange();
void LaunchMatrixRowsCount(int m, int n, bool useOmp);
void CountMatrixRows();
void AvgArrayNeighbours(int count);
void MultiplicateMatrixVector();

void CriticalSectionComparison();
void PairwiseSum();
void PrefixSum();
void ComplexMatrix();
void MinSearch();
void BubbleSort();

int elemCounts[] = { 100, 1000, 2000, 4000, 6000, 8000, 10000 };

int main(int argc, char* argv[])
{
	if (argc < 3) return -1;
	
	int k = stoi(string(argv[1]));
	int count = stoi(string(argv[2]));

	std::srand(unsigned(std::time(0)));

	//MatrixMultiplying(k);
	//NonZeroElements();
	//NumbersRange();
	//CountMatrixRows();
	//AvgArrayNeighbours(count);
	//MultiplicateMatrixVector();

	//CriticalSectionComparison();
	//PairwiseSum();
	//PrefixSum();
	//ComplexMatrix();
	//MinSearch();
	BubbleSort();

	getchar();
	return 0;
}

void BubbleSort()
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		double time1 = LaunchBubbleSort(elemCounts[i], true);
		double time2 = LaunchBubbleSort(elemCounts[i], false);
		Log("\nArray length: " + to_string(elemCounts[i]));
		Log("OMP OFF / OMP ON : " + to_string(time2 / time1));
	}
}

void MinSearch()
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		LaunchMinSearch(elemCounts[i], true);
		LaunchMinSearch(elemCounts[i], false);
	}
}

void ComplexMatrix()
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		LaunchComplexMultiplication(elemCounts[i], true);
		LaunchComplexMultiplication(elemCounts[i], false);
	}
}

void PrefixSum() {
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		double time1 = LaunchPrefixSum(elemCounts[i], true);
		double time2 = LaunchPrefixSum(elemCounts[i], false);

		Log("\nArray length: " + to_string(elemCounts[i]));
		Log("OMP OFF / OMP ON : " + to_string(time2 / time1));
	}
}

void PairwiseSum() {
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++) {
		LaunchPairwiseSum(elemCounts[i], true);
		LaunchPairwiseSum(elemCounts[i], false);
	}
}

void CriticalSectionComparison() {
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++) {
		double time1 = LaunchCriticalSectionComparison(elemCounts[i], true);
		double time2 = LaunchCriticalSectionComparison(elemCounts[i], false);
		Log("\nArray length: " + to_string(elemCounts[i]));
		Log("OMP OFF / OMP ON : " + to_string(time2 / time1));
	}
}

void MultiplicateMatrixVector()
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		LaunchVectorMultiplication(elemCounts[i], elemCounts[i], true);
		LaunchVectorMultiplication(elemCounts[i], elemCounts[i], false);
		Log("\n");
	}
}

void AvgArrayNeighbours(int count)
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		LaunchArrayAvgNeghbours(elemCounts[i], count, true);
		LaunchArrayAvgNeghbours(elemCounts[i], count, false);
		Log("\n");
	}
}

void CountMatrixRows()		
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		LaunchMatrixRowsCount(elemCounts[i], elemCounts[i], true);
		LaunchMatrixRowsCount(elemCounts[i], elemCounts[i], false);
		Log("\n");
	}
}

void NumbersRange()
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		LaunchArrayNumbers(elemCounts[i], true);
		LaunchArrayNumbers(elemCounts[i], false);
		Log("\n");
	}
}

void NonZeroElements()
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof (*elemCounts); i++)
	{
		LaunchNonZeroSelection(elemCounts[i], elemCounts[i], true);
		LaunchNonZeroSelection(elemCounts[i], elemCounts[i], false);
		Log("\n");
	}
}

void MatrixMultiplying(int k)
{
	for (int i = 0; i < sizeof(elemCounts) / sizeof(*elemCounts); i++)
	{
		LaunchMatrixCalculations(elemCounts[i], elemCounts[i], k, true);
		LaunchMatrixCalculations(elemCounts[i], elemCounts[i], k, false);
		Log("\n");
	}
}

double MultiplyMatrix(double** matrix, int m, int n, int k, bool useOmp)
{
	double result = 0;
	double** newMatrix = EmptyMatrix(m, n);

	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();
	watch->Start();

	for (int i = 0; i < m; i++)
	{
		#pragma omp parallel if (useOmp) 
		{
			#pragma omp for reduction(+:result) nowait
			for (int j = 0; j < n; j++)
			{
				newMatrix[i][j] = matrix[i][j] * k;
				result += newMatrix[i][j];
			}
		}
	}

	
	Log("Time ellapsed: " + to_string(watch->ElapsedTicks));
	watch->Reset();

	DeallocateMatrix(newMatrix, m, n);
	return result;
}

void LaunchMatrixCalculations(int m, int n, int k, bool useOmp)
{
	if(useOmp)
		Log("OMP ON");
	else
		Log("OMP OFF");
	Log("Multiplication coefficient: " + to_string(k));
	Log("Matrix: " + to_string(m) + " x " + to_string(n));

	double** matrix = RandomMatrix(m, n);

	Log("Multiplication result: " + to_string(MultiplyMatrix(matrix, m, n, k, useOmp)));

	DeallocateMatrix(matrix, m, n);

	Log("\n");
}

double** RandomMatrix(int m, int n)
{
	Log("Creating random matrix...");
	double** matrix = new double*[m];
	for (int i = 0; i < m; ++i)
	{
		matrix[i] = new double[n];
		for (int j = 0; j < n; j++)
		{
			matrix[i][j] = std::rand() % 10000;
		}
	}

	return matrix;
}

double** EmptyMatrix(int m, int n)
{
	Log("Creating empty matrix...");
	double** matrix = new double*[m];
	for (int i = 0; i < m; ++i)
	{
		matrix[i] = new double[n];
	}

	return matrix;
}

void DeallocateMatrix(double** matrix, int m, int n)
{
	Log("Deallocating matrix...");
	for (int i = 0; i < n; i++)
	{
		delete[] matrix[i];
	}

	delete[] matrix;
}

void Log(string str)
{
	ofstream logFile;
	logFile.open("log.txt", ios::app);
	cout << str << endl;
	logFile << str << endl;
	logFile.close();
}