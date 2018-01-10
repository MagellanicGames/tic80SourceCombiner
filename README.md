# tic80SourceCombiner
Enables use of multiple .lua files by combining them into a single source file.

## How-to

Open in Visual studio, build and copy to .exe to the folder where your .lua tic source files reside.  This is a very early and
basic implementation so includes can only be in one file.

Like a typical .lua for the tic 80, define the properties but they must be surrounded with the following syntax:

```
#define tic80Properties
-- title: MyGameTitle
-- author: Yourname/handle
-- descr: A really fun game where you win
-- script: lua
#endDefine
```

After the properties, define your files to be included:

```
#defineIncludes
-- #include Level.lua
-- #include Enemies.lua
-- #include Scores.lua
#endDefine
```

And then after that, simply write your usual code:

```
function TIC()
  
end
```


## Command line

To use on the command line, navigate to the containing folder and use like so:

```
Tic80SourceCombiner.exe mainFileWithIncludes.lua combinedOutputFile.lua
```

## .Bat use

```
start Tic80SourceCombiner.exe mainFileWithIncludes.lua combinedOutputFile.lua
```
