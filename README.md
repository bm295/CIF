# MultiThreadOddEven (.NET 10 / C# 14)

This repository contains a small console demo app with three options:

1. **OddEvenProgram** – prints odd and even numbers from two threads in alternating order.
2. **InlineMethodProgram** – compares a regular method and an aggressively inlined method.
3. **InKeywordProgram** – demonstrates the `in` keyword by passing a readonly struct by readonly reference.

## Prerequisites

- .NET 10 SDK (preview)

## Run the demo

From the repository root:

```bash
dotnet run --project Application/Application.csproj
```

Then choose an option number from the console menu.

## Build

```bash
dotnet build MultiThreadOddEven.sln
```
