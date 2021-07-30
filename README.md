# autorizador_nubank
Code Challenge em processo seletivo de Engenheiro de Software para o Nubank.

## Decisões técnicas e arquiteturais
Optei pelo uso do C# pois é minha linguagem preferida, bem como a IDE Visual Studio.
Tentei adotar alguns conceitos do paradigma funcional, como funções puras (sem efeitos colaterais) e transparência referencial. Porém, utilizei também classes e objetos, mas tentei mantê-las imutáveis.
Devido a simplicidade, todas as funções e o programa estão em um arquivo só (Program.cs), e as classes estão na pasta Models.

## Frameworks/ Bibliotecas
Utilizei as seguintes bibliotecas:
- Newtonsoft Json para poder serializar e deserializar a entrada e saída (Json)
- System para utilizar os tipos de dados primários e também criar coleções tipadas (List)
- LINQ para filtrar as coleções de maneira mais simples. 

## Instruções para compilar e executar
- É necessário ter o .NET 5.0 instalado na máquina. Caso não tenha, fazer o download em https://dotnet.microsoft.com/download
- Após fazer a instalação, abra o terminal na pasta 'autorizador_nubank/app' e execute os seguintes comandos:

```
dotnet --version
dotnet build
dotnet test
dotnet run < operations
```

Caso não queira passar o arquivo 'operations', é possível informar cada linha individualmente executando apenas:

```
dotnet run
```


### Para build com docker:

```
docker build -t authorize -f Dockerfile .
docker run -i authorize < operations
```

ou 

```
docker run -i authorize
```


# Notas adicionais
No arquivo 'operations' já tem massa de testes para todas as operações que o programa deve realizar, com exceção da violação 'card-not-active'. Para fazer esse teste, é necessário iniciar o programa novamente e executar as 2 linhas a seguir:

* *{"account": {"active-card": false, "available-limit": 175}}* *
* *{"transaction": {"merchant": "Burger King", "amount": 20, "time": "2019-02-13T11:00:00.000Z"}}* *


