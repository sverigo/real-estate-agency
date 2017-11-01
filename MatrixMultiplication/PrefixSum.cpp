#include "Header.h"

int PrefixSum(double* arr, int n, bool useOmp);

int LaunchPrefixSum(int n, bool useOmp)
{
	if (useOmp)
		Log("\nOMP ON");
	else
		Log("\nOMP OFF");
	Log("Array length: " + to_string(n));

	double *arr = RandomArray(n);
	int result = PrefixSum(arr, n, useOmp);

	delete[] arr;

	return result;
}

int PrefixSum(double* arr, int n, bool useOmp)
{
	int *partial, *temp;
	int work, num_threads;
	int i, last;

	double* arrCopy = new double[n];
	for (int i = 0; i < n; i++)
	{
		arrCopy[i] = arr[i];
	}

	Log("Starting calculations...");
	System::Diagnostics::Stopwatch^ watch = gcnew System::Diagnostics::Stopwatch();

	watch->Start();

	#pragma omp parallel if (useOmp) default(none) private(i, last) shared(arrCopy, partial, temp, work, n, num_threads)
	{
		#pragma omp single
		{
			num_threads = omp_get_num_threads();
			partial = new int[num_threads];
			temp = new int[num_threads];
			work = n / num_threads + 1; /*sets length of sub-arrays*/
		}

		int threadNum = omp_get_thread_num();
		/*calculate prefix-sum for each subarray*/
		for (i = work * threadNum + 1; i < work * threadNum + work && i < n; i++)
			arrCopy[i] += arrCopy[i - 1];

		partial[threadNum] = arrCopy[i - 1];

		#pragma omp barrier
		/*calculate prefix sum for the array that was made from last elements of each of the previous sub-arrays*/
		for (i = 1; i < num_threads; i <<= 1) {
			if (threadNum >= i)
				temp[threadNum] = partial[threadNum] + partial[threadNum - i];

		#pragma omp barrier

		#pragma omp single
			memcpy(partial + 1, temp + 1, sizeof(int) * (num_threads - 1));
		}
		/*update original array*/
		for (i = work * threadNum; i < (last = work * threadNum + work < n ? work * threadNum + work : n); i++)
			arrCopy[i] += partial[threadNum] - arrCopy[last - 1];


	}

	int result = watch->ElapsedTicks;

	Log("Time ellapsed: " + to_string(watch->ElapsedTicks));
	watch->Reset();

	delete[] arrCopy;

	return result;
}