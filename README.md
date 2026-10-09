# Clyvo Vet API (ByteCare)

API RESTful desenvolvida em .NET 9.0 para projeto em colaboração com a divisão de medicina veterinária da empresa Clyvo, concebida para atender aos requisitos de arquitetura de software, qualidade e infraestrutura em nuvem Microsoft Azure.

---

## 👥 Integrantes da Equipe (ByteCare)
- **Erik Naoki Miyasato** - RM: 565771
- **Gustavo Arthur Carvalho Sartori** - RM: 561650
- **Gutemberg Rocha Silva** - RM: 562267
- **João Henrique Batista Leal** - RM: 564361
- **Juliana da Silva Stigliani** - RM: 561171

> [!NOTE]
> **Aviso ao Professor / Avaliador:** O arquivo `script_bd.sql` localizado na raiz deste repositório é destinado exclusivamente aos requisitos e avaliação da disciplina de **DevOps Tools & Cloud Computing**, contendo o script DDL completo em sintaxe PostgreSQL com comentários estruturais nas tabelas/colunas core e carga inicial de dados.

---

## 🏛️ Arquitetura da Solução em Nuvem (Opção 1: ACR + ACI)

![Arquitetura Azure](docs/architecture/azure_architecture.png)

### Topologia e Componentes Homologados:
1. **Computação da Aplicação (ACI):** Container `aci-api-bytecare` executando a API .NET 9.0 sobre Azure Container Instances (ACI), em ambiente Linux sob usuário restrito não-root (`USER app`, `uid=1654`, `gid=1654`) na porta `8080`.
2. **Banco de Dados Relacional (ACI):** Container `aci-db-bytecare` executando PostgreSQL 16 Alpine em Azure Container Instances (ACI), com porta `5432` exposta na rede virtual da nuvem. O banco SQLite é estritamente restrito para desenvolvimento e testes automatizados locais isolados.
3. **Registro de Contêineres (ACR):** Azure Container Registry (`acrbytecare.azurecr.io`) provisionado para hospedar as imagens imutáveis da solução: `api-bytecare:v1` e `postgres-bytecare:v1`.
4. **Gerenciamento de Segredos e Conformidade (Azure Key Vault):** Integração nativa da aplicação com o **Azure Key Vault** (`kv-bytecare-564361`) para armazenamento e resolução segura de chaves criptográficas (`Jwt--Key`) e string de conexão (`ConnectionStrings--DefaultConnection`).
5. **Injeção Segura de Parâmetros:** No provisionamento via Azure CLI, senhas e chaves criptográficas são repassadas exclusivamente via parâmetro `--secure-environment-variables` nos containers ACI, garantindo zero exposição de credenciais em logs ou inspeções de recursos.
6. **Assinatura Azure Obrigatória:** Contexto configurado na subscription `2026-2TDSPJ-JOAO-H-BATISTA-LEAL-RM564361`, sob o Resource Group `rg-bytecare` na região `brazilsouth`.

---

## 🏗️ Arquitetura da Solução de Software

A solução foi desenvolvida adotando princípios de Domain-Driven Design (DDD), Clean Architecture e padrão em camadas:

- **ClyvoVetChallenge.Domain:** Entidades de domínio com encapsulamento, enums de status e regras fundamentais do negócio veterinário.
- **ClyvoVetChallenge.Application:** DTOs de requisição e resposta, contratos de interfaces de serviços e repositórios, validações de entrada e orquestração das regras de aplicação.
- **ClyvoVetChallenge.Infrastructure:** Persistência via Entity Framework Core com mapeamento Fluent API, suporte homologado a PostgreSQL 16 (produção e nuvem ACI) e SQLite para testes locais isolados, repositórios concretos e migrações versionadas.
- **ClyvoVetChallenge.Api:** Camada de apresentação RESTful com documentação Swagger/OpenAPI, autenticação JWT Bearer, tratamento global de erros no padrão RFC 9110 (ProblemDetails), logging estruturado Serilog, middleware de Correlation ID e instrumentação OpenTelemetry.
- **ClyvoVetChallenge.Tests.Unit:** 92 testes unitários com xUnit e Moq para isolamento rigoroso de regras de negócio e serviços.
- **ClyvoVetChallenge.Tests.Integration:** 31 testes de integração com `WebApplicationFactory` validando o ciclo HTTP completo, autorização, validação estrita de inicialização e códigos de status.

---

## 📌 Catálogo Completo de Endpoints

### 1. Autenticação e Segurança
- `POST /api/Auth/login`: Autentica o usuário e retorna o token de acesso JWT.
- `GET /api/Auth/perfil`: Retorna os dados do perfil autenticado (Requer `Authorization: Bearer <token>`).

### 2. Animais (`/api/Animal`)
- `GET /api/Animal`: Lista todos os animais cadastrados.
- `GET /api/Animal/{id}`: Busca um animal específico pelo ID.
- `POST /api/Animal`: Cadastra um novo animal (Valida que a data de nascimento não pode ser futura; retorna HTTP 422 em caso de violação).
- `PUT /api/Animal/{id}`: Atualiza os dados de um animal existente.
- `DELETE /api/Animal/{id}`: Remove um animal do sistema.

### 3. Clínicas Veterinárias (`/api/Clinica`)
- `GET /api/Clinica`: Lista todas as clínicas cadastradas.
- `GET /api/Clinica/{id}`: Busca uma clínica específica pelo ID.
- `POST /api/Clinica`: Cadastra uma nova clínica (validação de CNPJ).
- `PUT /api/Clinica/{id}`: Atualiza os dados de uma clínica existente.
- `DELETE /api/Clinica/{id}`: Remove uma clínica do sistema.

### 4. Consultas Veterinárias (`/api/Consulta`)
- `GET /api/Consulta`: Lista todas as consultas cadastradas.
- `GET /api/Consulta/{id}`: Busca uma consulta específica pelo ID.
- `GET /api/Consulta/animal/{animalId}`: Lista todas as consultas vinculadas a um animal.
- `POST /api/Consulta`: Agenda uma nova consulta vinculando animal, veterinário e clínica.
- `PUT /api/Consulta/{id}/resultado`: Registra diagnóstico, observações e altera o status da consulta.
- `DELETE /api/Consulta/{id}`: Cancela ou remove uma consulta do sistema.

### 5. Diários de Sintomas (`/api/Diario`)
- `GET /api/Diario`: Lista todos os registros de diário cadastrados.
- `GET /api/Diario/{id}`: Busca um registro de diário pelo ID.
- `GET /api/Diario/animal/{animalId}`: Lista todos os diários de um animal específico.
- `POST /api/Diario`: Cria um novo registro de diário de saúde para o animal.
- `PUT /api/Diario/{id}`: Atualiza anotações do diário.
- `DELETE /api/Diario/{id}`: Remove um registro de diário.
- `POST /api/Diario/{diarioId}/sintomas`: Adiciona um sintoma observado com nível de intensidade ao diário.
- `DELETE /api/Diario/{diarioId}/sintomas/{sintomaId}`: Remove um sintoma associado ao diário.

### 6. Tutores / Donos (`/api/Dono`)
- `GET /api/Dono`: Lista todos os donos cadastrados.
- `GET /api/Dono/{id}`: Busca um dono específico pelo ID.
- `POST /api/Dono`: Cadastra um novo dono (validação de CPF único).
- `PUT /api/Dono/{id}`: Atualiza informações de contato e endereço do dono.
- `DELETE /api/Dono/{id}`: Remove um dono do sistema.

### 7. Históricos Médicos (`/api/Historico`)
- `GET /api/Historico`: Lista todos os eventos de histórico médico.
- `GET /api/Historico/{id}`: Busca um registro de histórico pelo ID.
- `GET /api/Historico/animal/{animalId}`: Lista os registros médicos de um animal específico.
- `POST /api/Historico`: Registra um novo evento histórico para o animal.
- `PUT /api/Historico/{id}`: Atualiza descrição e resultados do histórico.
- `DELETE /api/Historico/{id}`: Remove um evento histórico.

### 8. Prescrições (`/api/Prescricao`)
- `GET /api/Prescricao`: Lista todas as prescrições cadastradas.
- `GET /api/Prescricao/{id}`: Busca uma prescrição específica pelo ID.
- `GET /api/Prescricao/consulta/{consultaId}`: Lista prescrições vinculadas a uma consulta.
- `POST /api/Prescricao`: Emite uma nova prescrição com medicamento, dosagem e duração.
- `PUT /api/Prescricao/{id}`: Atualiza posologia e observações da prescrição.
- `DELETE /api/Prescricao/{id}`: Remove uma prescrição.

### 9. Catálogo de Sintomas (`/api/Sintoma`)
- `GET /api/Sintoma`: Lista os tipos de sintomas pré-cadastrados.
- `GET /api/Sintoma/{id}`: Busca um tipo de sintoma pelo ID.
- `POST /api/Sintoma`: Cadastra um novo sintoma no catálogo.
- `PUT /api/Sintoma/{id}`: Atualiza nome e descrição do sintoma.
- `DELETE /api/Sintoma/{id}`: Remove um sintoma do catálogo.

### 10. Médicos Veterinários (`/api/Veterinario`)
- `GET /api/Veterinario`: Lista todos os veterinários cadastrados.
- `GET /api/Veterinario/{id}`: Busca um veterinário específico pelo ID.
- `POST /api/Veterinario`: Cadastra um novo veterinário (validação de CRMV).
- `PUT /api/Veterinario/{id}/contato`: Atualiza dados de contato (telefone e email) do veterinário.
- `DELETE /api/Veterinario/{id}`: Remove um veterinário do sistema.

### 11. Monitoramento e Observabilidade
- `GET /health`: Diagnóstico abrangente da aplicação com validação do banco de dados e serviços externos.
- `GET /health/ready`: Probe de prontidão (Readiness) filtrando dependências essenciais.
- `GET /health/live`: Probe de vivacidade (Liveness) leve para orquestradores.
- `GET /metrics`: Métricas no formato Prometheus exportadas via OpenTelemetry.

---

## 📋 Exemplos de Payload

### Autenticação (`POST /api/Auth/login`)
```json
{
  "username": "admin",
  "password": "SuaSenhaSeguraAqui!"
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
  "motivo": "Vacinação anual e revisão geral",
  "tipo": "Presencial"
}
```

### Atualização de Resultado da Consulta (`PUT /api/Consulta/{id}/resultado`)
```json
{
  "diagnostico": "Animal saudável, vacinas polivalente e antirrábica aplicadas",
  "observacoes": "Retorno agendado para daqui a 12 meses",
  "status": 2
}
```

---

## 📊 Monitoramento e Observabilidade

A API conta com recursos nativos e estruturados de observabilidade:

### Endpoints de Health Check
- `GET /health`: Executa os testes de conectividade ativos contra o banco de dados (`AddDbContextCheck<AppDbContext>`) e serviços externos. Retorna HTTP 200 (Healthy / Degraded) ou HTTP 503 (Unhealthy).
- `GET /health/ready`: Probe de prontidão avaliando dependências essenciais marcadas com tag `ready`.
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

### Métricas Prometheus (`/metrics`)
O endpoint `/metrics` expõe métricas no padrão Prometheus alimentadas pelo OpenTelemetry:
- Métricas de runtime do .NET: contadores de Garbage Collection, thread pool, consumo de memória e CPU.
- Métricas HTTP do ASP.NET Core: duração de requisições, requisições ativas e distribuição de status HTTP.
- Métricas de banco de dados via Entity Framework Core: comandos executados e latência de conexão.
- Métricas de requisições de clientes externos (`HttpClient`).

### Logging Estruturado (Serilog)
- **Console:** Formatado para inspeção ágil em ambiente de desenvolvimento.
- **Rolling File:** Gravação persistente no caminho `logs/clyvovet-.log` com criação de novos arquivos a cada dia e retenção automática por 31 dias.
- **Correlation ID:** O middleware customizado `CorrelationIdMiddleware` captura ou gera um `X-Correlation-Id`, propagando-o no cabeçalho de resposta e no contexto estruturado de log (`LogContext`).
- **Tratamento Global de Exceções:** Centralizado via `GlobalExceptionHandler` retornando ProblemDetails padronizado (RFC 9110) com correlação do `TraceId` ativo.

---

## 🚀 Como Executar o Projeto

### Opção 1: Execução Local com SQLite (Desenvolvimento Isolado)
No ambiente `Development`, a API pode utilizar SQLite local com schema gerenciado em arquivo, sem necessidade de banco externo.

1. Restaure as dependências e compile o projeto:
```bash
dotnet build
```

2. Defina uma chave JWT segura para a sessão e inicie a API:
```bash
export JWT_SECRET="chave_jwt_para_desenvolvimento_local_min_32_chars!"
dotnet run --project ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj --launch-profile http
```

3. Acesse os serviços:
- **Swagger UI:** [http://localhost:5177/swagger](http://localhost:5177/swagger)
- **Health Check:** [http://localhost:5177/health](http://localhost:5177/health)
- **Métricas:** [http://localhost:5177/metrics](http://localhost:5177/metrics)

---

### Opção 2: Execução com PostgreSQL via Docker Compose
Para testar a aplicação localmente integrada ao PostgreSQL 16 containerizado:

1. Configure as variáveis de ambiente necessárias no arquivo `.env` (a partir do modelo `.env.example`):
```bash
cp .env.example .env
# Preencha POSTGRES_PASSWORD e JWT_SECRET no arquivo .env
```

2. Inicie a stack completa:
```bash
docker compose up --build
```
A API ficará acessível na porta `8080`: [http://localhost:8080/swagger](http://localhost:8080/swagger).

---

### Opção 3: Deploy Automatizado na Nuvem Azure (ACR + ACI)
O projeto conta com o script `deploy.sh` totalmente automatizado e em conformidade com as diretrizes de segurança da Sprint 3.

Consulte o [Guia de Deploy na Nuvem](#4-guia-de-deploy-na-nuvem) para instruções passo a passo.

---

## 🧪 Guia de Execução de Testes

O projeto conta com suíte automatizada de testes seguindo o padrão AAA (Arrange, Act, Assert) e convenções estritas de engenharia de software:

### Estrutura dos Projetos de Teste
- `ClyvoVetChallenge.Tests.Unit`: 92 testes unitários cobrindo todas as entidades do domínio e serviços de aplicação. O isolamento de dependências é garantido via **Moq**, sem acesso a rede ou banco de dados real.
- `ClyvoVetChallenge.Tests.Integration`: 31 testes de integração cobrindo o ciclo HTTP com `WebApplicationFactory<Program>`, validação estrita de segredos no startup, autorização JWT e persistência isolada.

### Comandos de Teste

Executar toda a suíte de testes (123 testes automatizados):
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

Executar com saída detalhada:
```bash
dotnet test --logger "console;verbosity=detailed"
```

---

# Avaliação DevOps Tools & Cloud Computing: Sprint 3

> [!IMPORTANT]
> Seção destinada à avaliação da disciplina DevOps Tools & Cloud Computing, cobrindo arquitetura em nuvem Microsoft Azure com Container Registry (ACR), Container Instances (ACI), Azure Key Vault, segurança de credenciais, execução não-root e testes de integração.

---

## 1. Descrição da Solução

A ByteCare API é uma aplicação RESTful desenvolvida em .NET 9.0 com arquitetura em camadas e banco relacional PostgreSQL 16 containerizado. O sistema gerencia o ecossistema de clínicas veterinárias, centralizando:
- Cadastro de donos de animais com informações de contato e endereço.
- Cadastro de animais com espécie, raça, data de nascimento e peso.
- Registro de clínicas veterinárias e corpo médico com validação de CRMV.
- Agendamento de consultas com registro de diagnósticos.
- Emissão de prescrições médicas com posologia e período de tratamento.
- Diário clínico com catálogo de sintomas e histórico médico.
- Endpoints de integridade em `/health`, `/health/ready`, `/health/live` e métricas Prometheus em `/metrics`.

---

## 2. Descrição dos Benefícios para o Negócio

A solução em nuvem proporciona:
1. **Padronização e Segurança da Informação:** Centralização de prontuários com integridade referencial no PostgreSQL 16, substituindo controles descentralizados.
2. **Prevenção de Falhas Clínicas:** Histórico médico e sintomas acessíveis em tempo real durante o atendimento, auxiliando diagnósticos e condutas.
3. **Escalabilidade e Eficiência:** O uso de Azure Container Instances e Container Registry dispensa infraestrutura física, permitindo provisionamento rápido e tarifação sob demanda.
4. **Resiliência Operacional:** Sondas contínuas de integridade validam o status do banco e da aplicação com exportação de métricas.
5. **Segurança Avançada e Conformidade:** Execução do contêiner de aplicação com usuário não-root, gerenciamento de segredos via Azure Key Vault e injeção protegida com `--secure-environment-variables`.

---

## 3. Conformidade com os Requisitos de Avaliação

| Item | Descrição | Implementação no Projeto |
| :--- | :--- | :--- |
| **8.1** | Recursos criados via Azure CLI | Provisionamento automatizado no script `deploy.sh` via comandos `az group create`, `az acr create`, `az keyvault create` e `az container create`. |
| **8.2** | Container do App sem privilégios de root | Configuração `USER app` presente no `Dockerfile`, operando sob `uid=1654` e `gid=1654`. |
| **8.3** | Uso de Dockerfile ou Docker Compose | Disponibilizados `Dockerfile` e `docker-compose.yml` na raiz do projeto. |
| **8.4** | Scripts e comandos documentados | Comandos de build, push e execução documentados passo a passo sem credenciais expostas. |
| **9.3** | Demonstração de operações do CRUD no banco | Validação das operações de inserção, leitura, atualização e remoção com consultas SQL diretas no PostgreSQL. |

---

## 4. Guia de Deploy na Nuvem

### 4.1. Pré-requisitos
- Azure CLI instalado e autenticado via `az login`.
- Docker Desktop em execução.
- Terminal compatível com Bash.

### 4.2. Autenticação no Azure e Definição da Assinatura
Conecte-se à conta e garanta o contexto da assinatura obrigatória:
```bash
az login
az account set --subscription "2026-2TDSPJ-JOAO-H-BATISTA-LEAL-RM564361"
az account show --output table
```

### 4.3. Execução do Deploy Automatizado
Exporte as credenciais obrigatórias e execute o script:
```bash
export DB_USER="bytecare_admin"
export DB_PASSWORD="SuaSenhaFortePostgres123!"
export JWT_SECRET="SuaChaveJwtSecretaComMaisDe32CaracteresDeComprimento!"
chmod +x deploy.sh
./deploy.sh
```

### 4.4. Execução Manual dos Comandos

1. **Criação do Grupo de Recursos:**
   ```bash
   az group create --name rg-bytecare --location brazilsouth
   ```

2. **Criação e Configuração do Azure Key Vault:**
   ```bash
   az keyvault create --resource-group rg-bytecare --name kv-bytecare-564361 --location brazilsouth --sku standard
   ```

3. **Criação do Azure Container Registry:**
   ```bash
   az acr create --resource-group rg-bytecare --name acrbytecare --sku Basic --admin-enabled true
   az acr login --name acrbytecare
   ```

4. **Credenciais do Registro:**
   ```bash
   ACR_LOGIN_SERVER=$(az acr show --name acrbytecare --query "loginServer" -o tsv)
   ACR_USERNAME=$(az acr credential show --name acrbytecare --query "username" -o tsv)
   ACR_PASSWORD=$(az acr credential show --name acrbytecare --query "passwords[0].value" -o tsv)
   docker login $ACR_LOGIN_SERVER -u $ACR_USERNAME -p $ACR_PASSWORD
   ```

5. **Container do Banco PostgreSQL (ACI com Variável Segura):**
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
     --secure-environment-variables \
       POSTGRES_PASSWORD="$DB_PASSWORD"
   ```

6. **Build e Publicação da Imagem da API:**
   ```bash
   docker build -t ${ACR_LOGIN_SERVER}/api-bytecare:v1 .
   docker push ${ACR_LOGIN_SERVER}/api-bytecare:v1
   ```

7. **Registro de Secrets no Azure Key Vault:**
   ```bash
   DB_HOST="db-bytecare.brazilsouth.azurecontainer.io"
   CONNECTION_STRING="Host=${DB_HOST};Port=5432;Database=db_bytecare;Username=${DB_USER};Password=${DB_PASSWORD}"

   az keyvault secret set --vault-name kv-bytecare-564361 --name "Jwt--Key" --value "$JWT_SECRET"
   az keyvault secret set --vault-name kv-bytecare-564361 --name "ConnectionStrings--DefaultConnection" --value "$CONNECTION_STRING"
   ```

8. **Container da API .NET no Azure Container Instances (Injeção Segura):**
   ```bash
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
       DB_HOST="$DB_HOST" \
       DB_PORT="5432" \
       DB_NAME="db_bytecare" \
       DB_USER="$DB_USER" \
       KEY_VAULT_NAME="kv-bytecare-564361" \
       KEY_VAULT_URI="https://kv-bytecare-564361.vault.azure.net/" \
     --secure-environment-variables \
       JWT_SECRET="$JWT_SECRET" \
       ConnectionStrings__DefaultConnection="$CONNECTION_STRING" \
       DB_PASSWORD="$DB_PASSWORD"
   ```

9. **Verificação dos Endereços:**
   ```bash
   az container list --resource-group rg-bytecare --output table
   ```

---

## 5. Evidência de Usuário Não-Root

O `Dockerfile` estabelece a execução sem privilégios de administrador:
```dockerfile
USER app
```

Ao verificar a identidade do processo no contêiner ativo:
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

Para desalocar todos os recursos após a avaliação ou término dos testes:
```bash
az group delete --name rg-bytecare --yes --no-wait
```
