SHELL = /bin/bash -e
.PHONY: solution tests clean graph

all: solution tests

solution: Program/bin/Debug/Program.exe

Program/bin/Debug/Program.exe:
	xbuild  /p:Configuration=Debug compiler.sln

tests: solution
	mono ./packages/NUnit.ConsoleRunner.3.6.0/tools/nunit3-console.exe NUnit.Tests/bin/Debug/NUnit.Tests.dll


Program/bin/Debug/graph.dot: solution
	pushd Program/bin/Debug; \
	./Program.exe; \
	popd

cfg: Program/bin/Debug/graph.dot
	xdot ./Program/bin/Debug/graph.dot

dom: Program/bin/Debug/graph.dot
	xdot ./Program/bin/Debug/Dominator.dot

clean:
	xbuild /t:clean
