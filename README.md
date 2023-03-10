# Matheus Penido Dev Assessment

As this is an assessment, I would like to comment on some points, because it could help in the assessment process.

This project consists of two parts:

1 - A graph problem where it is necessary to find every possible path between nodes.

2:- Given an Srt file, it is required to be able to shift the time by a given value in a timespan.

# Where files are saved after the Subtitle Sync?

devassessment\SubtitleTimeshift.Tests\bin\Debug\netcoreapp3.1

# Methodologies used during this process:

Both projects are developed using TDD and OO.

For the graph problem, recursion was used. It is well-known that in graph theory there is an algorithm called DFS used to find any paths between two points inside a graph. However, for this exercise, I tried my own solution because it is an assessment of reasoning on problems. In the real world, the chosen option would be a known and used algorithm to solve this situation.

For the Sync SRT, I chose to convert the SRT file first to a model inside a data structure, just to separate the things between read and write. However, it is known that it is possible to be straightforward and simply read and write the data with the timespan shifted up.

# Problems found during the execution of SYNC ASSESSMENT:

-Out-of-sync time on line 5820. -Timespan format used in the assert file with '.' instead of ',' in the timespan format. -Different timespan formats being used in the SRT file (0:00:00,000 and 00:00:00,000), for example. -Jumps in the number of each subtitle section (1365 to 1367):

1365 02:08:40,166 --> 02:08:42,126 <i>adalah pilihan yang aku tinggalkan pada kau.</i>.

1367 02:09:20,141 --> 02:08:4,346 Subtitle by : Amir Barzagli

# Problems found during the execution of GRAPH ASSESSMENT:

The return used on the provided interface was IObservable<IEnumerable<T>>, where the purpose is to provide every route between two points inside a graph. The problem is that with this return, I am only able to bring a list of type T, and if we have many paths, I will only be able to provide one list of T. If it is possible to know the type T at compile-time, and if it were a string, it would be okay. I could have a list of strings, with each string representing a found path between two nodes.

To fix this, I created two approaches. One is to change IObservable<IEnumerable<T>> to IObservable<IEnumerable<string>> and return the printed path for each node. The other approach was to change IObservable<IEnumerable<IEnumerable<T>>>, but I kept it commented inside the code.

Thank you for this assessment!
