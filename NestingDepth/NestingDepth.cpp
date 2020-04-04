#include <iostream>
#include <vector>
#include <string>
#include <algorithm>

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
	std::string input; cin >> input;
	std::vector<int> numbers;
	for (int i = 0; i < input.size(); ++i)
	{
		numbers.push_back(input[i] - '0');
	}

	const char closingBracket = ')';
	const char openingBracket = '(';
	std::string output;
	auto arraySize = numbers.size();

	std::reverse(std::begin(numbers), std::end(numbers));

	int closedNumber = 0;
	for (int i = 0; i < arraySize; ++i)
	{
		int n = numbers[i];
		int n_prev = i == 0
			? 0
			: numbers[i - 1];
		int n_next = i == arraySize - 1 ? 0 : numbers[i + 1];

		if (n_prev == 0 || n_prev < n)
		{
			while (n != closedNumber)
			{
				++closedNumber;
				output += closingBracket;
			}
		}

		output += std::to_string(n);

		if (n_next == 0)
		{
			while (closedNumber)
			{
				output += openingBracket;
				--closedNumber;
			}
		}
		else if (n > n_next)
		{
			for (int p = 0; p < n - n_next; ++p)
			{
				output += openingBracket;
				--closedNumber;
			}
		}
	}

	std::reverse(std::begin(output), std::end(output));

	cout << "Case #" << caseNumber << ": " << output << std::endl;
}
