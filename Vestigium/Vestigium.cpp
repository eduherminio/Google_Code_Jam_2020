#include <iostream>
#include <unordered_set>

using std::cin;
using std::cout;

void HandleTestCase(int);

int main()
{
	int testCases; cin >> testCases;

	for (int i = 0; i < testCases; ++i)
	{
		HandleTestCase(i + 1);
	}
}

void HandleTestCase(int caseNumber)
{
	int size; cin >> size;
	int matrix[100][100];

	int trace = 0;
	int repeatedRows = 0;
	for (int i = 0; i < size; ++i)
	{
		for (int j = 0; j < size; ++j)
		{
			cin >> matrix[i][j];
			if (i == j)
			{
				trace += matrix[i][j];
			}
		}
		std::unordered_set<int> rowSet(std::begin(matrix[i]), std::begin(matrix[i]) + size);
		if (rowSet.size() < size)
		{
			++repeatedRows;
		}
	}

	int repeatedColumns = 0;
	for (int i = 0; i < size; ++i)
	{
		std::unordered_set<int> columnSet(size);
		for (int j = 0; j < size; ++j)
		{
			columnSet.emplace(matrix[j][i]);
		}
		if (columnSet.size() < size)
		{
			++repeatedColumns;
		}
	}

	cout << "Case #" << caseNumber << ": " << trace << " " << repeatedRows << " " << repeatedColumns << std::endl;
}
