# Clyvo Vet API

API RESTful desenvolvida em .net 9.0 para projeto em colaboração com a divisão de medicina veterinária da empresa Clyvo

## Funcionalidades
Sistema completo com CRUD para:
- Animais, Donos, Clínicas, Consultas, Prescrições, Históricos, Sintomas e Veterinários.

## Rotas Principais
- `GET /api/{entidade}`: Lista todos os recursos.
- `GET /api/{entidade}/{id}`: Busca recurso específico.
- `POST /api/{entidade}`: Cria novo recurso.
- `PUT /api/{entidade}/{id}`: Atualiza recurso existente.
- `DELETE /api/{entidade}/{id}`: Remove recurso.

## Instalação e Execução
1. clone o repositório 
```bash 
git clone https://github.com/JoaoHBatistaLeal/clyvovet-csharp.git
```
2. abra o projeto na IDE de sua preferência

3. crie um arquivo `.env` na raiz do projeto e adicione a seguinte variável:
```
DEV_ENV_DB="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<HOST.COM>)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=<SERVICO>)));User Id=<USER>;Password=<SENHA>"
```
4.  no terminal da solução, rode os seguintes comandos:
```dotnet 
dotnet build
dotnet run --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj
```
5. abra o navegador e acesse a documentação via swagger ui
```
http://localhost:PORTA/swagger
```

