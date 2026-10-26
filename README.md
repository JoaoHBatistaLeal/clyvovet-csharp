# Clyvo Vet API (ByteCare)

API RESTful corporativa desenvolvida em .NET 9.0 para suporte a medicina veterinaria preventiva e gestao clinica integrada para a Clyvo Vet, estruturada com Clean Architecture, persistencia poliglota (PostgreSQL e MongoDB), inteligencia artificial para recomendacao clinica, observabilidade com OpenTelemetry e esteira automatizada de CI/CD no Azure DevOps.

---

## 1. Integrantes do Grupo (ByteCare)

- **Erik Naoki Miyasato** - RM: 565771 - Turma: 2TDSPJ
- **Gustavo Arthur Carvalho Sartori** - RM: 561650 - Turma: 2TDSPJ
- **Gutemberg Rocha Silva** - RM: 562267 - Turma: 2TDSPJ
- **João Henrique Batista Leal** - RM: 564361 - Turma: 2TDSPJ
- **Juliana da Silva Stigliani** - RM: 561171 - Turma: 2TDSPJ

---

## 2. Visao Geral da Solucao e Beneficios para o Negocio

A solucao Clyvo Vet aborda a fragmentacao do historico clinico e a falta de padronizacao no acompanhamento de saude preventiva dos animais de estimacao. A plataforma unifica a gestao de tutores, pacientes, equipes veterinarias e clinicas parceiras, agregando suporte a decisao clinica por Inteligencia Artificial e persistencia poliglota para analise estruturada e semiestruturada.

### Beneficios Praticos:
- **Centralizacao do Prontuario Veterinario:** Registro unificado de consultas, evolucao de sintomas e prescricoes em banco relacional de alta confiabilidade.
- **Suporte a Decisao Clinica com IA:** Triagem rapida baseada em sintomas relatados, categorizando niveis de urgencia (Baixa, Media, Alta, Emergencia) e sugerindo condutas imediatas.
- **Auditoria e Persistencia NoSQL:** Historico de analises de IA e logs de auditoria gravados em MongoDB sem sobrecarregar as tabelas transacionais relacionais.
- **Navegabilidade Hipermidia (HATEOAS):** Endpoints nivel 3 do Modelo de Richardson com links autodocumentados para acoes de consulta, atualizacao e remocao.
- **Esteira de CI/CD Resiliente:** Integracao continua no Azure DevOps com compilacao automatizada, execucao de testes unitarios e de integracao isolados de conexoes externas e geracao de artefato imutavel.

### Stack Tecnologica Completa:
- **Linguagem e Framework:** C# 13, .NET 9.0 SDK
- **Persistencia Relacional:** Entity Framework Core 9.0, PostgreSQL 16 Alpine, SQLite (testes isolados)
- **Persistencia NoSQL:** MongoDB 7.0 (driver oficial MongoDB.Driver 3.12.0)
- **Seguranca e Criptografia:** ASP.NET Core Authentication JWT Bearer, HMAC-SHA256, Azure Key Vault
- **Observabilidade:** Serilog (Console e Rolling File), OpenTelemetry Tracing e Metrics, Prometheus Exporter, Health Checks nativos do ASP.NET Core
- **Testes Automatizados:** xUnit 2.9, Moq 4.20, Microsoft.AspNetCore.Mvc.Testing (WebApplicationFactory)
- **Containerizacao:** Docker multi-stage build com usuario restrito nao-root (`USER app`)
- **Pipeline e Nuvem:** Azure Pipelines (YAML), Azure Container Registry (ACR), Azure Container Instances (ACI) / Azure Web App

---

## 3. Arquitetura da Solucao e Fluxo CI/CD

### Diagrama de Arquitetura em Nuvem

![Arquitetura da Solucao em Nuvem](docs/architecture/azure_architecture.png)

### Fluxo Integrado de CI/CD (Numerado):
1. **Desenvolvedor:** Realiza codificacao local, executa testes locais (`dotnet test`) e envia os commits com mensagens semanticas para as branches `main` ou `master` no repositorio Git.
2. **Azure Pipelines (Gatilho de CI):** O push na branch principal dispara automaticamente a pipeline declarada no arquivo `azure-pipelines.yml`.
3. **Restauracao e Build (CI):** O agente Microsoft instala o .NET 9 SDK, restaura as dependencias NuGet e realiza o build da solucao com a flag `--configuration Release`.
4. **Execucao de Testes Automatizados (CI):** Os 138 testes (104 unitarios e 34 de integracao) sao executados com coleta de cobertura (`XPlat Code Coverage`). Como os testes de integracao utilizam SQLite em memoria e mock NoSQL, o estagio conclui em segundos sem depender de redes externas ou bancos pre-existentes no agente.
5. **Publicacao do Artefato (CI):** A API e compilada em formato de artefato imutavel (`drop`) e disponibilizada para a esteira de CD.
6. **Entrega Continua (CD):** A pipeline de CD provisiona a infraestrutura no Microsoft Azure ou realiza a atualizacao da imagem de container no Azure Container Registry (ACR).
7. **Deploy em Ambiente Cloud (ACI / Web App):** A aplicacao entra no ar sob o usuario sem privilegios de root (`USER app`), consumindo segredos injetados pelo Azure Key Vault ou `--secure-environment-variables`.
8. **Usuario Final / Veterinario:** Acessa a API via Swagger público ou aplicacao cliente atraves do endereco FQDN exposto pelo Azure.

---

## 4. Estrutura da Clean Architecture e SOLID

A solucao adota os padroes de Clean Architecture com estrita inversao de controle:

- **ClyvoVetChallenge.Domain:** Nucleo da aplicacao sem qualquer dependencia de pacotes externos ou infraestrutura. Contem as entidades fundamentais (`Animal`, `Clinica`, `Consulta`, `Diario`, `Dono`, `Historico`, `Prescricao`, `Sintoma`, `Veterinario`, `HistoricoRecomendacaoIa`), enums de status e excecoes customizadas (`DomainException`, `NotFoundException`, `BusinessRuleException`).
- **ClyvoVetChallenge.Application:** Orquestracao de regras de negocio, DTOs de entrada e saida, parametros de busca paginada, contratos de servico (`IAnimalService`, `IConsultaService`, `IRecomendacaoIaService`, etc.) e interfaces de repositorios (`IRepository<T>`, `IAnimalRepository`, `IConsultaRepository`, `IRecomendacaoIaRepository`). Possui o metodo de extensao `AddApplicationServices()`.
- **ClyvoVetChallenge.Infrastructure:** Implementacao de persistencia relacional via Entity Framework Core (`AppDbContext`, configuracoes Fluent API), persistencia NoSQL MongoDB (`MongoRepository<T>`, `RecomendacaoIaRepository`), servico de geracao de tokens JWT (`TokenService`) e execucao de stored procedures. Possui o metodo de extensao `AddInfrastructureServices()`.
- **ClyvoVetChallenge.Api:** Camada de apresentacao RESTful contendo os Controllers HTTP, configuracao de middlewares (Correlation ID, Serilog, RFC 7807 ProblemDetails GlobalExceptionHandler), politicas de seguranca JWT Bearer, gerador de hipermidia HATEOAS e configuracoes do Swagger.

### Persistencia Poliglota:
- **Relacional (PostgreSQL):** Dados cadastrais e transacionais com integridade referencial estrita e transacoes ACID (Animais, Consultas, Prescricoes, Tutores).
- **NoSQL (MongoDB):** Dados semiestruturados contendo as avaliacoes clinicas e recomendacoes de cuidado geradas pelo servico de IA, alem de logs analiticos e historico evolutivo do paciente.

---

## 5. Documentacao dos Endpoints (OpenAPI / Swagger)

A documentacao interativa completa da API encontra-se exportada no arquivo [`docs/swagger.json`](docs/swagger.json) e pode ser acessada visualmente atraves da rota `/swagger` quando a aplicacao estiver em execucao.

### Tabela Resumo dos Endpoints

| Metodo | Rota | Descricao | Autenticacao |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Auth/login` | Autenticacao do usuario com geracao do token JWT | Anonimo |
| `POST` | `/api/Auth/registrar` | Cadastro de novo usuario no sistema | Anonimo |
| `GET` | `/api/Auth/perfil` | Informacoes do perfil do usuario autenticado | Bearer JWT |
| `GET` | `/api/status` | Marcador de versao e ambiente para esteira de CI/CD | Anonimo |
| `GET` | `/health` | Diagnostico detalhado de saude e dependencias da API | Anonimo |
| `GET` | `/health/ready` | Probe de prontidao (Readiness) para orquestradores | Anonimo |
| `GET` | `/health/live` | Probe de vivacidade (Liveness) para orquestradores | Anonimo |
| `GET` | `/metrics` | Metricas Prometheus exportadas via OpenTelemetry | Anonimo |
| `GET` | `/api/Animal` | Listagem de todos os animais com links HATEOAS | Bearer JWT |
| `GET` | `/api/Animal/paged` | Listagem paginada, filtrada e ordenada com HATEOAS | Bearer JWT |
| `GET` | `/api/Animal/{id}` | Consulta de animal por identificador unico | Bearer JWT |
| `POST` | `/api/Animal` | Cadastro de novo animal com validacao de data | Bearer JWT |
| `PUT` | `/api/Animal/{id}` | Atualizacao de dados cadastrais de animal | Bearer JWT |
| `DELETE`| `/api/Animal/{id}` | Remocao de animal do cadastro | Bearer JWT |
| `GET` | `/api/Consulta` | Listagem de todas as consultas com HATEOAS | Bearer JWT |
| `GET` | `/api/Consulta/paged` | Listagem paginada de consultas com HATEOAS | Bearer JWT |
| `GET` | `/api/Consulta/{id}` | Consulta veterinaria por identificador unico | Bearer JWT |
| `GET` | `/api/Consulta/animal/{animalId}` | Listagem de consultas vinculadas a um animal | Bearer JWT |
| `POST` | `/api/Consulta` | Agendamento de consulta veterinaria | Bearer JWT |
| `PUT` | `/api/Consulta/{id}/resultado` | Atualizacao do laudo, parecer e status da consulta | Bearer JWT |
| `DELETE`| `/api/Consulta/{id}` | Cancelamento ou remocao de consulta | Bearer JWT |
| `POST` | `/api/RecomendacaoIa/avaliar` | Avaliacao clinica por IA e persistencia no MongoDB | Bearer JWT |
| `GET` | `/api/RecomendacaoIa/animal/{animalId}`| Historico de recomendacoes de IA por animal | Bearer JWT |
| `GET` | `/api/RecomendacaoIa` | Historico global de analises de IA persistidas | Bearer JWT |
| `GET` | `/api/Clinica` | Listagem de clinicas veterinarias cadastradas | Bearer JWT |
| `GET` | `/api/Diario` | Listagem de registros do diario de saude | Bearer JWT |
| `GET` | `/api/Dono` | Listagem de tutores de animais cadastrados | Bearer JWT |
| `GET` | `/api/Historico` | Listagem de eventos medicos no prontuario | Bearer JWT |
| `GET` | `/api/Prescricao` | Listagem de prescricoes de medicamentos | Bearer JWT |
| `GET` | `/api/Sintoma` | Catalogo padronizado de sintomas veterinarios | Bearer JWT |
| `GET` | `/api/Veterinario` | Listagem de medicos veterinarios com CRMV | Bearer JWT |

---

## 6. Exemplos de Payloads JSON e Hipermidia HATEOAS

### Autenticacao (`POST /api/Auth/login`)
```json
{
  "username": "admin",
  "password": "SuaSenhaSegura!"
}
```
Resposta com token e expiracao:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expirationMinutes": 120,
  "expiration": "2026-10-26T14:30:00Z",
  "usuario": "admin"
}
```

### Consulta Paginada com HATEOAS (`GET /api/Animal/paged?pageNumber=1&pageSize=2`)
```json
{
  "items": [
    {
      "id": "7ca6469b-869d-4be9-ba4f-ee5a40b955bb",
      "donoId": "a1f59224-cf56-4c47-b352-78a053cbe962",
      "nome": "Pipoca",
      "especie": "Felina",
      "raca": "Siames",
      "sexo": "Femea",
      "dataNascimento": "2022-03-01",
      "peso": 4.20,
      "links": [
        {
          "href": "http://localhost:8080/api/Animal/7ca6469b-869d-4be9-ba4f-ee5a40b955bb",
          "rel": "self",
          "method": "GET"
        },
        {
          "href": "http://localhost:8080/api/Animal/7ca6469b-869d-4be9-ba4f-ee5a40b955bb",
          "rel": "update",
          "method": "PUT"
        },
        {
          "href": "http://localhost:8080/api/Animal/7ca6469b-869d-4be9-ba4f-ee5a40b955bb",
          "rel": "delete",
          "method": "DELETE"
        },
        {
          "href": "http://localhost:8080/api/Animal",
          "rel": "all-animals",
          "method": "GET"
        }
      ]
    }
  ],
  "pageNumber": 1,
  "pageSize": 2,
  "totalCount": 5,
  "totalPages": 3,
  "hasPreviousPage": false,
  "hasNextPage": true,
  "links": [
    {
      "href": "http://localhost:8080/api/Animal/paged?pageNumber=1&pageSize=2",
      "rel": "self",
      "method": "GET"
    },
    {
      "href": "http://localhost:8080/api/Animal/paged?pageNumber=1&pageSize=2",
      "rel": "first-page",
      "method": "GET"
    },
    {
      "href": "http://localhost:8080/api/Animal/paged?pageNumber=3&pageSize=2",
      "rel": "last-page",
      "method": "GET"
    },
    {
      "href": "http://localhost:8080/api/Animal/paged?pageNumber=2&pageSize=2",
      "rel": "next-page",
      "method": "GET"
    }
  ]
}
```

### Recomendacao de IA e NoSQL (`POST /api/RecomendacaoIa/avaliar`)
Requisicao:
```json
{
  "animalId": "7ca6469b-869d-4be9-ba4f-ee5a40b955bb",
  "nomeAnimal": "Pipoca",
  "especie": "Felina",
  "idadeAnos": 2,
  "sintomas": [
    "tosse",
    "dificuldade respiratoria"
  ],
  "observacoesAdicionais": "Apresenta cansaco facil apos brincar"
}
```
Resposta estruturada persistida no MongoDB:
```json
{
  "id": "671cc815d9a9f24823101e4a",
  "animalId": "7ca6469b-869d-4be9-ba4f-ee5a40b955bb",
  "nomeAnimal": "Pipoca",
  "especie": "Felina",
  "diagnosticoSugerido": "Suspeita de Complexo Respiratorio Felino / Asma Felina",
  "nivelUrgencia": "Emergencia",
  "recomendacoesCuidados": [
    "Encaminhamento imediato para unidade de pronto atendimento veterinario.",
    "Manter o paciente em repouso e evitar movimentacao brusca."
  ],
  "dataAvaliacao": "2026-10-26T13:25:00Z",
  "modeloIaUtilizado": "ClyvoVet-ClinicalDecision-v2"
}
```

### Marcador de Deploy para DevOps (`GET /api/status`)
```json
{
  "application": "Clyvo Vet API",
  "version": "1.0.0-sprint4",
  "environment": "Production",
  "status": "Online",
  "timestamp": "2026-10-26T13:25:00Z"
}
```

---

## 7. Variaveis de Ambiente e Segredos

A aplicacao adota a politica estrita de **Zero Credenciais Expostas** em arquivos versionados. Todas as credenciais no arquivo `appsettings.json` possuem valor vazio `""` e sao injetadas em tempo de execucao.

### Variaveis de Ambiente Suportadas:
| Variavel | Descricao | Exemplo / Formato |
| :--- | :--- | :--- |
| `ASPNETCORE_ENVIRONMENT` | Ambiente de execucao da aplicacao | `Development` ou `Production` |
| `PORT` | Porta HTTP da aplicacao | `8080` ou `5177` |
| `ConnectionStrings__DefaultConnection` | String de conexao com o PostgreSQL | Montada em runtime via variaveis atomicas |
| `MongoDbSettings__ConnectionString` | URI de conexao com o cluster MongoDB | `mongodb+srv://user:pass@cluster.mongodb.net/` |
| `MongoDbSettings__DatabaseName` | Nome da base de dados NoSQL | `db_clyvovet_nosql` |
| `JWT_SECRET` ou `Jwt__Key` | Chave criptografica simetrica (minimo 32 caracteres) | `chave_secreta_jwt_minimo_32_caracteres_long` |
| `AUTH_USERNAME` | Usuario administrador para autenticacao inicial | `admin` |
| `AUTH_PASSWORD` | Senha administrativa protegida | `<senha_gerada_em_segredo>` |
| `KEY_VAULT_NAME` | Nome do cofre Azure Key Vault para resolucao | `kv-bytecare-564361` |

### Configuracao Local via Dotnet User-Secrets:
```bash
dotnet user-secrets init --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj
dotnet user-secrets set "Jwt:Key" "chave_jwt_para_desenvolvimento_local_32_chars!" --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj
dotnet user-secrets set "Auth:Password" "SenhaDev123!" --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj
```

---

## 8. Instrucoes de Instalacao, Execucao e Testes

### 1. Restaurar Pacotes e Dependencias
```bash
dotnet restore
```

### 2. Compilar a Solucao
```bash
dotnet build --configuration Release
```

### 3. Aplicar Migrations no Banco de Dados Relacional
Para PostgreSQL em ambiente local ou na nuvem:
```bash
dotnet ef database update --project ClyvoVetChallenge.Infrastructure/ClyvoVetChallenge.Infrastructure.csproj --startup-project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj
```
*(Nota: O script completo DDL com carga inicial e Stored Procedures encontra-se no arquivo `script_bd.sql` na raiz).*

### 4. Executar a Aplicacao Localmente
```bash
export JWT_SECRET="chave_jwt_para_desenvolvimento_local_32_chars!"
dotnet run --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj --launch-profile http
```

### 5. Executar Toda a Suite de Testes Automatizados
```bash
dotnet test --configuration Release
```

---

## 9. Guia de Deploy e Monitoramento em Nuvem

### Deploy Automatizado via Azure CLI
O script `deploy.sh` provisiona os recursos na regiao `brazilsouth` sob a assinatura academica oficial:
```bash
az account set --subscription "2026-2TDSPJ-JOAO-H-BATISTA-LEAL-RM564361"
read -s -p "Digite a senha do PostgreSQL: " DB_PASSWORD; echo
export DB_PASSWORD
read -s -p "Digite o JWT Secret: " JWT_SECRET; echo
export JWT_SECRET
chmod +x deploy.sh
./deploy.sh
```

### Monitoramento de Saude (Health Checks)
A API fornece tres niveis de verificacao automatizada:
- `/health`: Analisa a saude completa, executando `SELECT 1` no banco de dados e verificando integridade de servicos externos.
- `/health/ready`: Probe de prontidao para orquestradores de container.
- `/health/live`: Probe leve de vivacidade do processo HTTP.

### Roteiro para a Gravacao do Video de DevOps (Demonstracao Pratica):
1. **Pipeline de CI/CD no Azure DevOps:** Exiba a organizacao do Azure DevOps, o projeto com visibilidade privada e o professor como Project Administrator.
2. **Gatilho Automatico:** Altere a string de versao no arquivo `StatusController.cs` (ex: de `"1.0.0-sprint4"` para `"1.0.1-sprint4"`), faca o commit e push na branch `main`/`master`.
3. **Execucao da Pipeline:** Demonstre a pipeline disparando automaticamente, executando os 138 testes com aprovacao total e publicando o artefato `drop`.
4. **Validacao em Producao:** Acesse a URL pública no Azure (`http://<fqdn>:8080/api/status` e `/swagger`) e mostre a nova versao no ar.
5. **Operacao de CRUD com SELECT em Nuvem:** Realize a insercao de um tutor via Swagger (`POST /api/Dono`) e comprove a persistencia direta na tabela `TB_DONO` atraves de um comando `SELECT` executado no banco em nuvem.
