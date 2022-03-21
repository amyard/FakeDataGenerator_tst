# FakeDataGenerator

Generate the json file with data

## Features
- Generate custom amount of data
- Saving data into json file for testing purpose

## Development

Type in console:
```sh
dotnet run
```
It will generate 100 Entities into json file.

(optional) Or you can provide your number for generating the entities:
```sh 
dotnet run 1000
```
It will generate the file with 1000 entities.


### Amount of data and file size:
```shaderlab
2000 entities   - 1.6864   Mb.
7000 entoties   - 5.9255   Mb.
11000 entities  - 9.3388   Mb.
50000 entities  - 42.2918  Mb.
111000 entities - 93.8802  Mb.
250000 entities - 211.6148 Mb.
500000 entities - 423.0647 MB,
```