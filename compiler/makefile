
all: solution tests

solution:
	xbuild /p:TargetFrameworkVersion="v4.5"  /p:Configuration=Debug compiler.sln

tests: solution
	mono ./packages/NUnit.ConsoleRunner.3.6.0/tools/nunit3-console.exe NUnit.Tests/bin/Debug/NUnit.Tests.dll

graph: solution
	./Program/bin/Debug/Program.exe; xdot ./Program/bin/Debug/graph.txt

clean:
	xbuild /t:clean
