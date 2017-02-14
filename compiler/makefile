SHELL = /bin/bash -e
.PHONY: solution tests clean graph

all: solution tests

solution:
	xbuild /p:TargetFrameworkVersion="v4.5"  /p:Configuration=Debug compiler.sln

tests: solution
	mono ./packages/NUnit.ConsoleRunner.3.6.0/tools/nunit3-console.exe NUnit.Tests/bin/Debug/NUnit.Tests.dll


Program/bin/Debug/graph.dot: solution
	pushd Program/bin/Debug; \
	./Program.exe; \
	popd

graph: Program/bin/Debug/graph.dot
	xdot ./Program/bin/Debug/graph.dot

clean:
	xbuild /t:clean
