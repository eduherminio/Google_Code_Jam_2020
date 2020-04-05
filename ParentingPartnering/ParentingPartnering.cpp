#include <iostream>
#include <vector>
#include <list>
#include <string>
#include <algorithm>
#include <utility>

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

class Activity
{
public:
    int Number;
    int Start;
    int End;
    std::string Person;

    Activity(int number, int start, int end)
        :Number(number), Start(start), End(end)
    {
    }
};

void HandleTestCase(int caseNumber)
{
    std::string output;
    int numberOfActivities; cin >> numberOfActivities;
    std::vector<Activity> activities; activities.reserve(numberOfActivities);

    for (int i = 0; i < numberOfActivities; ++i)
    {
        int start; cin >> start;
        int end; cin >> end;
        activities.push_back(Activity(i, start, end));
    }


    std::sort(std::begin(activities), std::end(activities), [](const auto& a, const auto& b)
    {
        return a.Start < b.Start;
    });

    bool cameronFree = true;
    bool jamieFree = true;

    auto maxT = std::max_element(std::begin(activities), std::end(activities), [](const auto& a, const auto& b) { return a.End < b.End; })->End;

    for (int t = 0; t < maxT; ++t)
    {
        auto result = std::begin(activities);
        while (result != std::end(activities))
        {
            result = std::find_if(result, std::end(activities), [t](const auto& act) { return act.End == t; });
            if (result != std::end(activities))
            {
                if (result->Person == "C")
                {
                    cameronFree = true;
                }
                else if (result->Person == "J")
                {
                    jamieFree = true;
                }

                ++result;
            }
        }

        result = std::begin(activities);
        while (result != std::end(activities))
        {
            result = std::find_if(result, std::end(activities), [t](const auto& act) { return act.Start == t; });
            if (result != std::end(activities))
            {
                if (cameronFree)
                {
                    cameronFree = false;
                    result->Person = "C";
                }
                else if (jamieFree)
                {
                    jamieFree = false;
                    result->Person = "J";
                }
                else
                {
                    output = "IMPOSSIBLE";
                    break;
                }

                result++;
            }
        }
    }

    if (output != "IMPOSSIBLE")
    {
        std::sort(std::begin(activities), std::end(activities), [](const auto& a, const auto& b)
        {
            return a.Number < b.Number;
        });

        for (const auto& activity : activities)
        {
            output += activity.Person;
        }
    }

    cout << "Case #" << caseNumber << ": " << output << std::endl;
}
