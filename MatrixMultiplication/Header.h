#pragma once
#include <omp.h>
#include <string>
#include <fstream>
#include <iostream>
#include <vector>
#include <list>
#using <System.dll>

using namespace std;

double** RandomMatrix(int m, int n);
void DeallocateMatrix(double** matrix, int m, int n);
void Log(string str);
double MultiplyMatrix(double** matrix, int m, int n, int k, bool useOmp);
double** EmptyMatrix(int m, int n);
double** RandomZeroOneMatrix(int m, int n);
double* RandomArray(int n);

void LaunchMatrixCalculations(int m, int n, int k, bool useOmp);
void LaunchNonZeroSelection(int m, int n, bool useOmp);
void LaunchArrayNumbers(int n, bool useOmp);
void LaunchArrayAvgNeghbours(int n, int count, bool useOmp);
void LaunchVectorMultiplication(int m, int n, bool useOmp);

int LaunchCriticalSectionComparison(int n, bool useOmp);
void LaunchPairwiseSum(int n, bool useOmp);
int LaunchPrefixSum(int n, bool useOmp);
void LaunchComplexMultiplication(int n, bool useOmp);
void LaunchMinSearch(int n, bool useOmp);
int LaunchBubbleSort(int n, bool useOmp);