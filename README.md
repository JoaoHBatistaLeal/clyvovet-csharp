# Clyvo Vet API

API RESTful desenvolvida em .net 9.0 para projeto em colaboração com a divisão de medicina veterinária da empresa Clyvo

Erik Naoki Miyasato RM: 565771
Gustavo Arthur Carvalho Sartori RM: 561650
Gutemberg Rocha Silva RM: 562267
João Henrique Batista Leal RM: 564361
Juliana da Silva Stigliani RM: 561171

## Funcionalidades
Sistema completo com CRUD para:
- Animais, Donos, Clínicas, Consultas, Prescrições, Históricos, Sintomas e Veterinários.

## Endpoints

para o funcionamento correto é preciso que mude `{entidade}` pelo nome de alguma entidade Ex.: `Veterinarios`, `Donos`, `Animais`

- `GET /api/{entidade}`: Lista todos os recursos.
- `GET /api/{entidade}/{id}`: Busca recurso específico.
- `POST /api/{entidade}`: Cria novo recurso.
- `PUT /api/{entidade}/{id}`: Atualiza recurso existente.
- `DELETE /api/{entidade}/{id}`: Remove recurso.

*(Nota: Algumas entidades possuem rotas específicas, como `GET /api/Consulta/animal/{animalId}` e `PUT /api/Consulta/{id}/resultado`).*

### Exemplo de Payload (POST /api/Animal)
```json
{
  "nome": "Rex",
  "especie": "Canina",
  "raca": "Labrador",
  "sexo": "M",
  "dataNascimento": "2020-05-10",
  "peso": 25.5,
  "donoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

## Monitoramento e Observabilidade

A API conta com recursos integrados de observabilidade, incluindo health checks customizados, métricas no formato Prometheus, rastreamento distribuído e logging estruturado.

### Endpoints de Health Check

- `GET /health`: Diagnóstico completo da saúde da aplicação, validando a conectividade com o banco de dados e a acessibilidade aos serviços externos dependentes. Retorna status HTTP 200 (Healthy ou Degraded) ou HTTP 503 (Unhealthy).
- `GET /health/ready`: Probe de prontidão (readiness) para orquestradores e balanceadores, avaliando componentes essenciais marcados com a tag `ready` (banco de dados e serviços externos).
- `GET /health/live`: Probe de vivacidade (liveness) leve, verificando se o processo da aplicação está ativo e respondendo a requisições sem validar dependências externas.

#### Exemplo de Payload de Resposta (`/health`)
```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0451234",
  "entries": [
    {
      "name": "database",
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.0123456",
      "tags": [
        "db",
        "ready"
      ]
    },
    {
      "name": "fiap-portal",
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.0327890",
      "tags": [
        "external",
        "ready"
      ]
    }
  ]
}
```

### Métricas Prometheus

- `GET /metrics`: Expõe métricas coletadas pelo OpenTelemetry no formato padrão do Prometheus para raspagem (scraping). Inclui:
  - Métricas do runtime do .NET (uso de memória, contadores de GC, thread pool, contadores de CPU).
  - Métricas de requisições HTTP do ASP.NET Core (latência de requisição, requisições em andamento, códigos de status retornados).
  - Métricas de clientes HTTP externos (`HttpClient`).
  - Métricas de comandos executados via Entity Framework Core.

### Logs Estruturados e Rastreabilidade

- **Serilog:** Pipeline estruturado com enriquecimento contextual.
  - Saída no Console para desenvolvimento e visualização rápida em containers.
  - Saída em arquivo com rotação diária (`Rolling File`) no caminho `logs/clyvovet-.log` com retenção configurada de 31 dias.
- **Correlation ID:** O middleware `CorrelationIdMiddleware` captura ou gera automaticamente o cabeçalho `X-Correlation-Id` em cada requisição HTTP, propagando-o no cabeçalho de resposta e no contexto de log (`LogContext`).
- **OpenTelemetry Tracing:** Rastreamento distribuído integrado com ASP.NET Core, chamadas HTTP externas e Entity Framework Core.
- **Tratamento Global de Exceções:** Implementado via `GlobalExceptionHandler` seguindo a RFC 9110 (ProblemDetails), vinculando o `TraceId` ativo (`Activity.Current`) a cada erro não tratado retornado ao cliente.

## Autenticação e Autorização

A API utiliza autenticação baseada em tokens JWT (JSON Web Token):

- `POST /api/auth/login`: Autentica o usuário e gera o token de acesso.
- `GET /api/auth/perfil`: Endpoint protegido que requer o envio do header `Authorization: Bearer <token>`, retornando os dados do perfil autenticado.
- A interface do Swagger UI conta com o botão `Authorize` configurado para aceitar o esquema `Bearer <token>`.

## Guia de Execução de Testes

O projeto conta com suíte automatizada de testes seguindo o padrão AAA (Arrange, Act, Assert) e a convenção estrita de nomenclatura `NomeDoMetodo_CenarioOuEstado_ResultadoEsperado`.

### Estrutura dos Projetos de Teste

- `ClyvoVetChallenge.Tests.Unit`: Testes unitários cobrindo entidades do domínio (`ClyvoVetChallenge.Domain`) e serviços de aplicação (`ClyvoVetChallenge.Application`), com total isolamento de dependências externas através do framework Moq.
- `ClyvoVetChallenge.Tests.Integration`: Testes de integração cobrindo o ciclo de vida HTTP completo dos endpoints, autenticação JWT, validações e códigos de status (200, 201, 204, 400, 401, 404, 422) utilizando `WebApplicationFactory<Program>` e fixture compartilhada `[CollectionDefinition("IntegrationTests")]`.

### Pré-requisitos para Execução dos Testes

- .NET 9.0 SDK instalado na máquina.
- Não é necessária nenhuma conexão com banco de dados externo ou container Oracle em execução: os testes de integração utilizam SQLite isolado gerenciado automaticamente pela fixture de teste (`ClyvoVetWebApplicationFactory`), garantindo independência e repetibilidade.

### Comandos de Execução

Executar toda a suíte de testes da solução:
```bash
dotnet test
```

Executar apenas os testes unitários:
```bash
dotnet test ClyvoVetChallenge.Tests.Unit/ClyvoVetChallenge.Tests.Unit.csproj
```

Executar apenas os testes de integração:
```bash
dotnet test ClyvoVetChallenge.Tests.Integration/ClyvoVetChallenge.Tests.Integration.csproj
```

Executar testes com relatório detalhado:
```bash
dotnet test --logger "console;verbosity=detailed"
```

Executar testes com filtro por classe ou cenário:
```bash
dotnet test --filter "FullyQualifiedName~AnimalApiTests"
```

## Changelog da Sprint 3

A Sprint 3 introduziu melhorias substanciais focadas em observabilidade, confiabilidade, arquitetura e garantia de qualidade:

1. **Camada de Observabilidade e Monitoramento:**
   - Adição de endpoints de Health Check (`/health`, `/health/ready`, `/health/live`) com escritor customizado em JSON (`HealthCheckResponseWriter`).
   - Verificação ativa da conectividade com o banco de dados via `AddDbContextCheck<ClyvoVetContext>` e verificação de URL externa.
   - Integração com OpenTelemetry para métricas e rastreamento distribuído, expondo métricas via `/metrics` para coleta do Prometheus.
   - Configuração do Serilog com sinks para Console e Rolling File diário em `logs/clyvovet-.log`.
   - Criação do `CorrelationIdMiddleware` para propagação de `X-Correlation-Id` em requisições, respostas e logs estruturados.
   - Criação do `GlobalExceptionHandler` para captura centralizada de exceções e resposta no padrão ProblemDetails com `TraceId`.

2. **Segurança e Autenticação:**
   - Implementação de autenticação JWT Bearer com o endpoint `/api/auth/login` e `/api/auth/perfil`.
   - Configuração do Swagger com suporte a Bearer Authentication (`SecurityDefinition` e `SecurityRequirement`).

3. **Suíte de Testes Automatizados:**
   - Separação clara entre projetos de testes unitários (`ClyvoVetChallenge.Tests.Unit`) e testes de integração (`ClyvoVetChallenge.Tests.Integration`).
   - 92 testes unitários cobrindo regras de negócio das entidades de domínio e fluxos dos serviços com Moq.
   - 28 testes de integração validando o ciclo HTTP dos endpoints com `WebApplicationFactory<Program>`, autenticação, validação de regras de negócio (status 422 para datas futuras) e códigos de status HTTP 200, 201, 204, 400, 401, 404.
   - Total de 120 testes automatizados integrados e aprovados.

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
4. no terminal da solução, rode os seguintes comandos:
```dotnet 
dotnet build
dotnet run --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj
```
5. abra o navegador e acesse a documentação via swagger ui
```
http://localhost:PORTA/swagger
```

