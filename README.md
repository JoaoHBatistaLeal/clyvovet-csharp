# Clyvo Vet API

API RESTful desenvolvida em .NET 9.0 para projeto em colaboracao com a divisao de medicina veterinaria da empresa Clyvo.

## Integrantes da Equipe
- Erik Naoki Miyasato - RM: 565771
- Gustavo Arthur Carvalho Sartori - RM: 561650
- Gutemberg Rocha Silva - RM: 562267
- Joao Henrique Batista Leal - RM: 564361
- Juliana da Silva Stigliani - RM: 561171

> [!NOTE]
> **Aviso ao Professor / Avaliador:** O arquivo `script_bd.sql` localizado na raiz deste repositório é destinado exclusivamente aos requisitos e avaliação da disciplina de **DevOps Tools & Cloud Computing**, contendo o script DDL completo em sintaxe PostgreSQL com comentários estruturais nas tabelas/colunas core e carga inicial de dados.

---

## Arquitetura da Solucao

A solucao foi desenhada adotando principios de Domain-Driven Design (DDD), Clean Architecture e padrao de camadas:

- **ClyvoVetChallenge.Domain:** Entidades de dominio com encapsulamento, enums de status e regras fundamentais do negocio veterinario.
- **ClyvoVetChallenge.Application:** DTOs de requisicao e resposta, contratos de interfaces de servicos e repositorios, validacoes de entrada e orquestracao das regras de aplicacao.
- **ClyvoVetChallenge.Infrastructure:** Persistencia via Entity Framework Core com mapeamento Fluent API, suporte hibrido a SQLite e Oracle 19c, repositorios concretos e migrações versionadas.
- **ClyvoVetChallenge.Api:** Camada de apresentacao RESTful com documentacao Swagger/OpenAPI, autenticacao JWT Bearer, tratamento global de erros no padrao RFC 9110 (ProblemDetails), logging estruturado Serilog, middleware de Correlation ID e instrumentacao OpenTelemetry.
- **ClyvoVetChallenge.Tests.Unit:** 92 testes unitarios com xUnit e Moq para isolamento rigoroso de regras de negocio e servicos.
- **ClyvoVetChallenge.Tests.Integration:** 28 testes de integracao com WebApplicationFactory validando o ciclo HTTP completo, autorizacao e codigos de status.

---

## Catalogo Completo de Endpoints

### 1. Autenticacao e Seguranca
- `POST /api/Auth/login`: Autentica o usuario e retorna o token de acesso JWT.
- `GET /api/Auth/perfil`: Retorna os dados do perfil autenticado (Requer `Authorization: Bearer <token>`).

### 2. Animais (`/api/Animal`)
- `GET /api/Animal`: Lista todos os animais cadastrados.
- `GET /api/Animal/{id}`: Busca um animal especifico pelo ID.
- `POST /api/Animal`: Cadastra um novo animal (Valida que a data de nascimento nao pode ser futura; retorna HTTP 422 em caso de violacao).
- `PUT /api/Animal/{id}`: Atualiza os dados de um animal existente.
- `DELETE /api/Animal/{id}`: Remove um animal do sistema.

### 3. Clinicas Veterinarias (`/api/Clinica`)
- `GET /api/Clinica`: Lista todas as clinicas cadastradas.
- `GET /api/Clinica/{id}`: Busca uma clinica especifica pelo ID.
- `POST /api/Clinica`: Cadastra uma nova clinica (validacao de CNPJ).
- `PUT /api/Clinica/{id}`: Atualiza os dados de uma clinica existente.
- `DELETE /api/Clinica/{id}`: Remove uma clinica do sistema.

### 4. Consultas Veterinarias (`/api/Consulta`)
- `GET /api/Consulta`: Lista todas as consultas cadastradas.
- `GET /api/Consulta/{id}`: Busca uma consulta especifica pelo ID.
- `GET /api/Consulta/animal/{animalId}`: Lista todas as consultas vinculadas a um animal.
- `POST /api/Consulta`: Agenda uma nova consulta vinculando animal, veterinario e clinica.
- `PUT /api/Consulta/{id}/resultado`: Registra diagnostico, observacoes e altera o status da consulta.
- `DELETE /api/Consulta/{id}`: Cancela ou remove uma consulta do sistema.

### 5. Diarios de Sintomas (`/api/Diario`)
- `GET /api/Diario`: Lista todos os registros de diario cadastrados.
- `GET /api/Diario/{id}`: Busca um registro de diario pelo ID.
- `GET /api/Diario/animal/{animalId}`: Lista todos os diarios de um animal especifico.
- `POST /api/Diario`: Cria um novo registro de diario de saude para o animal.
- `PUT /api/Diario/{id}`: Atualiza anotacoes do diario.
- `DELETE /api/Diario/{id}`: Remove um registro de diario.
- `POST /api/Diario/{diarioId}/sintomas`: Adiciona um sintoma observado com nivel de intensidade ao diario.
- `DELETE /api/Diario/{diarioId}/sintomas/{sintomaId}`: Remove um sintoma associado ao diario.

### 6. Tutores / Donos (`/api/Dono`)
- `GET /api/Dono`: Lista todos os donos cadastrados.
- `GET /api/Dono/{id}`: Busca um dono especifico pelo ID.
- `POST /api/Dono`: Cadastra um novo dono (validacao de CPF unico).
- `PUT /api/Dono/{id}`: Atualiza informacoes de contato e endereco do dono.
- `DELETE /api/Dono/{id}`: Remove um dono do sistema.

### 7. Historicos Medicos (`/api/Historico`)
- `GET /api/Historico`: Lista todos os eventos de historico medico.
- `GET /api/Historico/{id}`: Busca um registro de historico pelo ID.
- `GET /api/Historico/animal/{animalId}`: Lista os registros medicos de um animal especifico.
- `POST /api/Historico`: Registra um novo evento historico para o animal.
- `PUT /api/Historico/{id}`: Atualiza descricao e resultados do historico.
- `DELETE /api/Historico/{id}`: Remove um evento historico.

### 8. Prescricoes (`/api/Prescricao`)
- `GET /api/Prescricao`: Lista todas as prescricoes cadastradas.
- `GET /api/Prescricao/{id}`: Busca uma prescricao especifica pelo ID.
- `GET /api/Prescricao/consulta/{consultaId}`: Lista prescricoes vinculadas a uma consulta.
- `POST /api/Prescricao`: Emite uma nova prescricao com medicamento, dosagem e duracao.
- `PUT /api/Prescricao/{id}`: Atualiza posologia e observacoes da prescricao.
- `DELETE /api/Prescricao/{id}`: Remove uma prescricao.

### 9. Catalogo de Sintomas (`/api/Sintoma`)
- `GET /api/Sintoma`: Lista os tipos de sintomas pre-cadastrados.
- `GET /api/Sintoma/{id}`: Busca um tipo de sintoma pelo ID.
- `POST /api/Sintoma`: Cadastra um novo sintoma no catalogo.
- `PUT /api/Sintoma/{id}`: Atualiza nome e descricao do sintoma.
- `DELETE /api/Sintoma/{id}`: Remove um sintoma do catalogo.

### 10. Medicos Veterinarios (`/api/Veterinario`)
- `GET /api/Veterinario`: Lista todos os veterinarios cadastrados.
- `GET /api/Veterinario/{id}`: Busca um veterinario especifico pelo ID.
- `POST /api/Veterinario`: Cadastra um novo veterinario (validacao de CRMV).
- `PUT /api/Veterinario/{id}/contato`: Atualiza dados de contato (telefone e email) do veterinario.
- `DELETE /api/Veterinario/{id}`: Remove um veterinario do sistema.

### 11. Monitoramento e Observabilidade
- `GET /health`: Diagnostico abrangente da aplicacao com validacao do banco de dados e servicos externos.
- `GET /health/ready`: Probe de prontidao (Readiness) filtrando dependencias essenciais.
- `GET /health/live`: Probe de vivacidade (Liveness) leve para orquestradores.
- `GET /metrics`: Metricas no formato Prometheus exportadas via OpenTelemetry.

---

## Exemplos de Payload

### Autenticacao (`POST /api/Auth/login`)
```json
{
  "usuario": "admin",
  "senha": "password123"
}
```

### Cadastro de Animal (`POST /api/Animal`)
```json
{
  "donoId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "nome": "Rex",
  "especie": "Canina",
  "raca": "Labrador",
  "sexo": "Macho",
  "dataNascimento": "2022-05-10",
  "peso": 25.5
}
```

### Agendamento de Consulta (`POST /api/Consulta`)
```json
{
  "animalId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "veterinarioId": "4ba95f64-5717-4562-b3fc-2c963f66afa7",
  "clinicaId": "5ca95f64-5717-4562-b3fc-2c963f66afa8",
  "dataConsulta": "2026-08-10",
  "horaConsulta": "14:00:00",
  "motivo": "Vacinacao anual e revisao geral",
  "tipo": "Presencial"
}
```

### Atualizacao de Resultado da Consulta (`PUT /api/Consulta/{id}/resultado`)
```json
{
  "diagnostico": "Animal saudavel, vacinas polivalente e antirrabica aplicadas",
  "observacoes": "Retorno agendado para daqui a 12 meses",
  "status": 2
}
```

---

## Monitoramento e Observabilidade

A API conta com recursos nativos e estruturados de observabilidade:

### Endpoints de Health Check
- `GET /health`: Executa os testes de conectividade ativos contra o banco de dados (`AddDbContextCheck<ClyvoVetContext>`) e servicos externos. Retorna HTTP 200 (Healthy / Degraded) ou HTTP 503 (Unhealthy).
- `GET /health/ready`: Probe de prontidao avaliando dependencias essenciais marcadas com tag `ready`.
- `GET /health/live`: Probe de vivacidade leve para orquestradores (Kubernetes e Docker).

#### Exemplo de Resposta do Health Check (`/health`)
```json
{
  "status": "Healthy",
  "duration": "00:00:00.7297179",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.0377955",
      "error": null
    },
    {
      "name": "fiap-portal",
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.7201480",
      "error": null
    }
  ]
}
```

### Metricas Prometheus (`/metrics`)
O endpoint `/metrics` expoe metricas no padrao Prometheus alimentadas pelo OpenTelemetry:
- Metricas de runtime do .NET: contadores de Garbage Collection, thread pool, consumo de memoria e CPU.
- Metricas HTTP do ASP.NET Core: duracao de requisicoes, requisicoes ativas e distribuicao de status HTTP.
- Metricas de banco de dados via Entity Framework Core: comandos executados e latencia de conexao.
- Metricas de requisicoes de clientes externos (`HttpClient`).

### Logging Estruturado (Serilog)
- **Console:** Formatado com cores para inspecao agil em ambiente de desenvolvimento.
- **Rolling File:** Gravacao persistente no caminho `logs/clyvovet-.log` com criacao de novos arquivos a cada dia e retencao automatica por 31 dias.
- **Correlation ID:** O middleware customizado `CorrelationIdMiddleware` captura ou gera um `X-Correlation-Id`, propagando-o no cabecalho de resposta e no contexto estruturado de log (`LogContext`).
- **Tratamento Global de Excecoes:** Centralizado via `GlobalExceptionHandler` retornando ProblemDetails padronizado (RFC 9110) com correlacao do `TraceId` ativo.

---

## Como Executar o Projeto

### Opcao 1: Execucao Local com SQLite (Recomendado para Desenvolvimento)
Por padrao, no ambiente `Development`, a API esta configurada para utilizar SQLite local com auto-geracao de schema via `Database.EnsureCreated()`, nao necessitando de containers ou banco externo.

1. Restaure as dependencias e compile o projeto:
```bash
dotnet build
```

2. Inicie a API com o perfil HTTP:
```bash
dotnet run --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj --launch-profile http
```

3. Acesse os servicos:
- **Swagger UI:** [http://localhost:5177/swagger](http://localhost:5177/swagger)
- **Health Check:** [http://localhost:5177/health](http://localhost:5177/health)
- **Metricas:** [http://localhost:5177/metrics](http://localhost:5177/metrics)

---

### Opcao 2: Execucao com Banco de Dados Oracle
Para conectar em uma instancia Oracle Database:

1. Altere em `ClyvoVetChallenge.Api/appsettings.Development.json` a chave `"Database": { "UseSqlite": false }`.
2. Defina a connection string via variavel de ambiente ou arquivo `.env` na raiz:
```bash
DEV_ENV_DB="Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=XE)));User Id=system;Password=suasenha"
```
3. Execute o projeto normalmente:
```bash
dotnet run --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj
```

---

### Opcao 3: Execucao via Docker Compose
O projeto conta com definicao pronta no `docker-compose.yml` contendo a API e o banco Oracle XE:

```bash
docker compose up --build
```
A API ficara acessivel na porta `8080`: [http://localhost:8080/swagger](http://localhost:8080/swagger).

---

## Guia de Execucao de Testes

O projeto conta com suíte automatizada de testes seguindo o padrao AAA (Arrange, Act, Assert) e a convencao estrita de nomenclatura `NomeDoMetodo_CenarioOuEstado_ResultadoEsperado`.

### Estrutura dos Projetos de Teste
- `ClyvoVetChallenge.Tests.Unit`: 92 testes unitarios cobrindo todas as entidades do dominio e servicos de aplicacao. O isolamento de dependencias e garantido via **Moq**, sem acesso a rede ou banco de dados real.
- `ClyvoVetChallenge.Tests.Integration`: 28 testes de integracao cobrindo o ciclo HTTP completo com `WebApplicationFactory<Program>`, colecoes compartilhadas (`IntegrationTestCollection`) e banco SQLite isolado descartavel por bateria.

### Comandos de Teste

Executar toda a suite de testes (120 testes):
```bash
dotnet test
```

Executar apenas os testes unitarios:
```bash
dotnet test ClyvoVetChallenge.Tests.Unit/ClyvoVetChallenge.Tests.Unit.csproj
```

Executar apenas os testes de integracao:
```bash
dotnet test ClyvoVetChallenge.Tests.Integration/ClyvoVetChallenge.Tests.Integration.csproj
```

Executar com saida detalhada:
```bash
dotnet test --logger "console;verbosity=detailed"
```

Executar filtrando por classe especifica:
```bash
dotnet test --filter "FullyQualifiedName~ConsultaApiTests"
```

---

## Changelog da Sprint 3

1. **Observabilidade e Monitoramento:**
   - Adicao de endpoints `/health`, `/health/ready` e `/health/live` com escritor customizado em JSON (`HealthCheckResponseWriter`).
   - Verificacao ativa de conexao com banco de dados via `AddDbContextCheck<ClyvoVetContext>` e probe de URL externa.
   - Instrumentacao OpenTelemetry com exposicao de metricas Prometheus em `/metrics`.
   - Logging estruturado com Serilog (Console e Rolling File diário em `logs/clyvovet-.log`).
   - Propagacao do cabecalho `X-Correlation-Id` via `CorrelationIdMiddleware`.
   - Captura centralizada de erros com `GlobalExceptionHandler` seguindo a RFC 9110 com `TraceId`.

2. **Seguranca e Autenticacao:**
   - Implementacao de autenticacao JWT Bearer com `/api/auth/login` e `/api/auth/perfil`.
   - Suporte a Bearer Authorization configurado no Swagger UI.

3. **Garantia de Qualidade e Testes:**
   - 92 testes unitarios cobrindo entidades de dominio e servicos de aplicacao com Moq.
   - 28 testes de integracao cobrindo o ciclo HTTP com `WebApplicationFactory<Program>`, validando codigos 200, 201, 204, 400, 401, 404 e 422.
   - Total de 120 testes automatizados integrados e aprovados.

---

# Avaliação DevOps Tools & Cloud Computing: Sprint 3

> [!IMPORTANT]
> Seção destinada à avaliação da disciplina DevOps Tools & Cloud Computing, cobrindo arquitetura em nuvem Microsoft Azure com Container Registry e Container Instances, segurança de containers, instruções de deploy e testes de integração.

---

## 1. Descrição da Solução

A ByteCare API é uma aplicação RESTful desenvolvida em .NET 9.0 com arquitetura em camadas e banco relacional PostgreSQL 16. O sistema gerencia o ecossistema de clínicas veterinárias, centralizando:
- Cadastro de donos de animais com informações de contato e endereço.
- Cadastro de animais com espécie, raça, data de nascimento e peso.
- Registro de clínicas veterinárias e corpo médico com validação de CRMV.
- Agendamento de consultas presenciais e remotas com registro de diagnósticos.
- Emissão de prescrições médicas com posologia e período de tratamento.
- Diário clínico com catálogo de sintomas e histórico médico.
- Endpoints de integridade em `/health`, `/health/ready`, `/health/live` e métricas Prometheus em `/metrics`.

---

## 2. Descrição dos Benefícios para o Negócio

A solução em nuvem proporciona:
1. **Padronização e Segurança da Informação:** Centralização de prontuários com integridade referencial no PostgreSQL, substituindo controles descentralizados.
2. **Prevenção de Falhas Clínicas:** Histórico médico e sintomas acessíveis em tempo real durante o atendimento, auxiliando diagnósticos e condutas.
3. **Escalabilidade e Eficiência:** O uso de Azure Container Instances e Container Registry dispensa infraestrutura física, permitindo provisionamento rápido e tarifação sob demanda.
4. **Resiliência Operacional:** Sondas contínuas de integridade validam o status do banco e da aplicação com exportação de métricas.
5. **Segurança:** Execução do container de aplicação com usuário não-root e isolamento nativo da nuvem pública.

---

## 3. Conformidade com os Requisitos de Avaliação

| Item | Descrição | Implementação no Projeto |
| :--- | :--- | :--- |
| **8.1** | Recursos criados via Azure CLI | Provisionamento automatizado no script `deploy.sh` via comandos `az group create`, `az acr create`, `az storage account create` e `az container create`. |
| **8.2** | Container do App sem privilégios de root | Configuração `USER app` presente no `Dockerfile`, operando sob `uid=1654` e `gid=1654`. |
| **8.3** | Uso de Dockerfile ou Docker Compose | Disponibilizados `Dockerfile` e `docker-compose.yml` na raiz do projeto. |
| **8.4** | Scripts e comandos documentados | Comandos de build, push e execução documentados passo a passo. |
| **9.3** | Demonstração de operações do CRUD no banco | Validação das operações de inserção, leitura, atualização e remoção com consultas SQL diretas. |

---

## 4. Guia de Deploy na Nuvem

### 4.1. Pré-requisitos
- Azure CLI instalado e autenticado via `az login`.
- Docker Desktop em execução.
- Terminal compatível com Bash.

### 4.2. Autenticação no Azure
Conecte-se à conta e verifique a assinatura ativa:
```bash
az login
az account show --output table
```

### 4.3. Execução do Deploy Automatizado
Defina as credenciais do banco e execute o script:
```bash
export DB_USER="bytecare_admin"
export DB_PASSWORD="ByteK4re!"
chmod +x deploy.sh
./deploy.sh
```

### 4.4. Execução Manual dos Comandos

1. **Criação do Grupo de Recursos:**
   ```bash
   az group create --name rg-bytecare --location brazilsouth
   ```

2. **Criação do Azure Container Registry:**
   ```bash
   az acr create --resource-group rg-bytecare --name acrbytecare --sku Basic --admin-enabled true
   az acr login --name acrbytecare
   ```

3. **Credenciais do Registro:**
   ```bash
   ACR_LOGIN_SERVER=$(az acr show --name acrbytecare --query "loginServer" -o tsv)
   ACR_USERNAME=$(az acr credential show --name acrbytecare --query "username" -o tsv)
   ACR_PASSWORD=$(az acr credential show --name acrbytecare --query "passwords[0].value" -o tsv)
   docker login $ACR_LOGIN_SERVER -u $ACR_USERNAME -p $ACR_PASSWORD
   ```

4. **Container do Banco PostgreSQL:**
   ```bash
   docker pull postgres:16-alpine
   docker tag postgres:16-alpine ${ACR_LOGIN_SERVER}/postgres-bytecare:v1
   docker push ${ACR_LOGIN_SERVER}/postgres-bytecare:v1

   az container create \
     --resource-group rg-bytecare \
     --name aci-db-bytecare \
     --image ${ACR_LOGIN_SERVER}/postgres-bytecare:v1 \
     --os-type Linux \
     --cpu 1 \
     --memory 1.5 \
     --registry-login-server ${ACR_LOGIN_SERVER} \
     --registry-username $ACR_USERNAME \
     --registry-password $ACR_PASSWORD \
     --dns-name-label db-bytecare \
     --ports 5432 \
     --environment-variables \
       POSTGRES_DB="db_bytecare" \
       POSTGRES_USER="$DB_USER" \
       POSTGRES_PASSWORD="$DB_PASSWORD"
   ```

5. **Build e Publicação da Imagem da API:**
   ```bash
   docker build -t ${ACR_LOGIN_SERVER}/api-bytecare:v1 .
   docker push ${ACR_LOGIN_SERVER}/api-bytecare:v1
   ```

6. **Container da API .NET no Azure Container Instances:**
   ```bash
   DB_HOST="db-bytecare.brazilsouth.azurecontainer.io"

   az container create \
     --resource-group rg-bytecare \
     --name aci-api-bytecare \
     --image ${ACR_LOGIN_SERVER}/api-bytecare:v1 \
     --os-type Linux \
     --cpu 1 \
     --memory 1.5 \
     --registry-login-server ${ACR_LOGIN_SERVER} \
     --registry-username $ACR_USERNAME \
     --registry-password $ACR_PASSWORD \
     --dns-name-label api-bytecare \
     --ports 8080 \
     --environment-variables \
       ASPNETCORE_ENVIRONMENT="Production" \
       ASPNETCORE_HTTP_PORTS="8080" \
       PORT="8080" \
       JWT_SECRET="clyvovet_dev_jwt_secret_key_sprint_3_fiap_2026!" \
       ConnectionStrings__DefaultConnection="Host=${DB_HOST};Port=5432;Database=db_bytecare;Username=${DB_USER};Password=${DB_PASSWORD}" \
       DB_HOST="$DB_HOST" \
       DB_PORT="5432" \
       DB_NAME="db_bytecare" \
       DB_USER="$DB_USER" \
       DB_PASSWORD="$DB_PASSWORD"
   ```

7. **Verificação dos Endereços:**
   ```bash
   az container list --resource-group rg-bytecare --output table
   ```

---

## 5. Evidência de Usuário Não-Root

O `Dockerfile` estabelece a execução sem privilégios de administrador:
```dockerfile
USER app
```

Ao verificar a identidade do processo no container ativo:
```bash
az container exec --resource-group rg-bytecare --name aci-api-bytecare --exec-command "id"
```
A resposta confirma a execução restrita:
```
uid=1654(app) gid=1654(app) groups=1654(app)
```

---

## 6. Demonstração das Operações do CRUD no Banco de Dados

### 6.1. Inserção de Registro

Requisição:
```bash
curl -X POST "http://<API_IP>:8080/api/Dono" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Gabriel Siqueira",
    "cpf": "987.654.321-99",
    "email": "gabriel.siqueira@bytecare.com",
    "telefone": "(11) 91234-5678",
    "dataNascimento": "1995-08-20",
    "endereco": "Rua Vergueiro, 1000 - SP"
  }'
```

Resposta HTTP 201:
```json
{
  "id": "cb652095-db91-498f-bc59-741a7dd3c3de",
  "nome": "Gabriel Siqueira",
  "cpf": "987.654.321-99",
  "email": "gabriel.siqueira@bytecare.com",
  "telefone": "(11) 91234-5678",
  "dataNascimento": "1995-08-20",
  "endereco": "Rua Vergueiro, 1000 - SP"
}
```

Consulta SQL no PostgreSQL:
```sql
SELECT "Id", "Nome", "Cpf", "Email", "Telefone", "Endereco", "CreatedAt", "Active"
FROM "TB_DONO"
WHERE "Id" = 'cb652095-db91-498f-bc59-741a7dd3c3de';
```

Resultado retornado pelo banco:
```
                  Id                  |       Nome       |      Cpf       |             Email             |    Telefone     |         Endereco         |           CreatedAt           | Active 
--------------------------------------+------------------+----------------+-------------------------------+-----------------+--------------------------+-------------------------------+--------
 cb652095-db91-498f-bc59-741a7dd3c3de | Gabriel Siqueira | 987.654.321-99 | gabriel.siqueira@bytecare.com | (11) 91234-5678 | Rua Vergueiro, 1000 - SP | 2026-09-10 22:00:03.494627+00 | t
(1 row)
```

---

### 6.2. Consulta de Registro

Requisição:
```bash
curl -X GET "http://<API_IP>:8080/api/Dono/cb652095-db91-498f-bc59-741a7dd3c3de"
```

Resposta HTTP 200:
```json
{
  "id": "cb652095-db91-498f-bc59-741a7dd3c3de",
  "nome": "Gabriel Siqueira",
  "cpf": "987.654.321-99",
  "email": "gabriel.siqueira@bytecare.com",
  "telefone": "(11) 91234-5678",
  "dataNascimento": "1995-08-20",
  "endereco": "Rua Vergueiro, 1000 - SP"
}
```

---

### 6.3. Atualização de Registro

Requisição:
```bash
curl -X PUT "http://<API_IP>:8080/api/Dono/cb652095-db91-498f-bc59-741a7dd3c3de" \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Gabriel Siqueira Silva",
    "cpf": "987.654.321-99",
    "email": "gabriel.silva@bytecare.com",
    "telefone": "(11) 99999-0000",
    "dataNascimento": "1995-08-20",
    "endereco": "Av. Paulista, 2000 - Apto 101, SP"
  }'
```

Resposta HTTP 200:
```json
{
  "id": "cb652095-db91-498f-bc59-741a7dd3c3de",
  "nome": "Gabriel Siqueira Silva",
  "cpf": "987.654.321-99",
  "email": "gabriel.silva@bytecare.com",
  "telefone": "(11) 99999-0000",
  "dataNascimento": "1995-08-20",
  "endereco": "Av. Paulista, 2000 - Apto 101, SP"
}
```

Consulta SQL no PostgreSQL:
```sql
SELECT "Id", "Nome", "Cpf", "Email", "Telefone", "Endereco", "Active"
FROM "TB_DONO"
WHERE "Id" = 'cb652095-db91-498f-bc59-741a7dd3c3de';
```

Resultado retornado pelo banco:
```
                  Id                  |          Nome          |      Cpf       |           Email            |    Telefone     |             Endereco              | Active 
--------------------------------------+------------------------+----------------+----------------------------+-----------------+-----------------------------------+--------
 cb652095-db91-498f-bc59-741a7dd3c3de | Gabriel Siqueira Silva | 987.654.321-99 | gabriel.silva@bytecare.com | (11) 99999-0000 | Av. Paulista, 2000 - Apto 101, SP | t
(1 row)
```

---

### 6.4. Exclusão de Registro

Requisição:
```bash
curl -X DELETE "http://<API_IP>:8080/api/Dono/cb652095-db91-498f-bc59-741a7dd3c3de"
```

Resposta HTTP 204 No Content.

Consulta SQL no PostgreSQL:
```sql
SELECT "Id", "Nome", "Active"
FROM "TB_DONO"
WHERE "Id" = 'cb652095-db91-498f-bc59-741a7dd3c3de';
```

Resultado retornado pelo banco:
```
 Id | Nome | Active 
----+------+--------
(0 rows)
```

Registros com dependências vinculadas têm sua exclusão bloqueada por chaves estrangeiras, preservando a integridade referencial do esquema relacional.

---

### 6.5. Teste de Conectividade em `/health`

Requisição:
```bash
curl -s http://<API_IP>:8080/health
```

Resposta HTTP 200:
```json
{
  "status": "Healthy",
  "duration": "00:00:01.2769822",
  "checks": [
    {
      "name": "database",
      "status": "Healthy",
      "description": null,
      "duration": "00:00:00.0666526",
      "error": null
    },
    {
      "name": "fiap-portal",
      "status": "Healthy",
      "description": null,
      "duration": "00:00:01.2251252",
      "error": null
    }
  ]
}
```

---

## 7. Descomissionamento dos Recursos

Para desalocar todos os recursos após os testes:
```bash
az group delete --name rg-bytecare --yes --no-wait
```
