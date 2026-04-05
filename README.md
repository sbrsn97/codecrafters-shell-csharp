README.md

# Codecrafters Shell in C#

A small shell implementation written in C# for the Codecrafters Shell challenge.

It supports built-in commands, external executable lookup through `PATH`, redirections, and tab completion.

## Features

- Built-in commands:
  - `echo`
  - `exit`
  - `pwd`
  - `type`
  - `cd`
  - `ls`
- External command execution from `PATH`
- Command parsing with:
  - single quotes
  - double quotes
  - backslash escaping
- Redirections:
  - `>`
  - `1>`
  - `2>`
  - `>>`
  - `1>>`
  - `2>>`
- Tab completion:
  - built-in commands
  - executables from `PATH`
  - multiple-match handling
  - longest common prefix completion

## Project Structure

```text
src/
  Commands/
  Completion/
  IO/
  Models/
  Parsing/
  main.cs
```

## Requirements

- .NET SDK

## Run

```bash
dotnet run
```

## Example Commands

```sh
echo hello
pwd
type echo
type ls
ls
cd ..
echo hello > out.txt
echo world >> out.txt
cat out.txt
```

## Notes

This project was built incrementally while solving the Codecrafters Shell challenge stage by stage.

## License

This project is for learning and challenge-solving purposes.
