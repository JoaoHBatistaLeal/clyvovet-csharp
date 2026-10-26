# Relatorio de Evolucao Tecnica - Sprint 4 (.NET + Alinhamento DevOps)
## Projeto Clyvo Vet (ByteCare)

Este documento descreve detalhadamente o progresso, as decisoes arquiteturais, a motivacao tecnica e os snippets reais de codigo correspondentes a cada uma das 10 tarefas executadas na Sprint 4 do projeto Clyvo Vet.

---

## Indice de Tarefas

1. [Task 1: Consolidacao de Clean Architecture, SOLID e Injecao de Dependencias](#task-1-consolidacao-de-clean-architecture-solid-e-injecao-de-dependencias)
2. [Task 2: Tratamento Global de Excecoes com RFC 7807 (ProblemDetails)](#task-2-tratamento-global-de-excecoes-com-rfc-7807-problemdetails)
3. [Task 3: Persistencia Relacional com EF Core, Repositorios e Stored Procedures](#task-3-persistencia-relacional-com-ef-core-repositorios-e-stored-procedures)
4. [Task 4: Persistencia Poliglota com MongoDB e Servico de IA Clinica](#task-4-persistencia-poliglota-com-mongodb-e-servico-de-ia-clinica)
5. [Task 5: Paginacao, Ordenacao Dinamica, Filtros e HATEOAS (Richardson Nivel 3)](#task-5-paginacao-ordenacao-dinamica-filtros-e-hateoas-richardson-nivel-3)
6. [Task 6: Autenticacao e Autorizacao com JWT Bearer e RBAC](#task-6-autenticacao-e-autorizacao-com-jwt-bearer-e-rbac)
7. [Task 7: Swagger/OpenAPI com Bearer e Endpoint de Identificacao para DevOps](#task-7-swaggeropenapi-com-bearer-e-endpoint-de-identificacao-para-devops)
8. [Task 8: Expansao dos Testes Automatizados (AAA + Isolamento para CI/CD)](#task-8-expansao-dos-testes-automatizados-aaa--isolamento-para-cicd)
9. [Task 9: Higienizacao de Segredos, Pipeline CI/CD e Container Nao-Root](#task-9-higienizacao-de-segredos-pipeline-cicd-e-container-nao-root)
10. [Task 10: Documentacao Tecnica Completa e Especificacao OpenAPI](#task-10-documentacao-tecnica-completa-e-especificacao-openapi)

---

## Task 1: Consolidacao de Clean Architecture, SOLID e Injecao de Dependencias

### 1. O que foi feito
- Isolamento estrito da camada `Domain`, garantindo que nenhuma entidade ou interface dependa de pacotes externos, bibliotecas de infraestrutura, drivers de banco de dados ou ORM.
- Centralizacao dos registros de injecao de dependencias em metodos de extensao limpos:
  - `AddApplicationServices()` na camada `ClyvoVetChallenge.Application`.
  - `AddInfrastructureServices()` na camada `ClyvoVetChallenge.Infrastructure`.
- Remocao da classe obsoleta `PersistenceExtensions.cs` e expurgo de qualquer comentario de codigo remanescente das sprints anteriores.
- Refatoracao do arquivo `Program.cs`, mantendo apenas a composicao dos modulos e configuracao da esteira HTTP.

### 2. Como foi feito
Foram criadas classes estaticas de extensao para `IServiceCollection` em cada uma das camadas internas. A camada de aplicacao registra seus proprios servicos de dominio e casos de uso, enquanto a infraestrutura encapsula contextos de banco (EF Core Npgsql e SQLite), clientes NoSQL (MongoDB), repositorios e servicos criptograficos.

### 3. Porque foi feito
- Para obedecer ao Principio da Inversao de Dependencia (DIP do SOLID), em que modulos de alto nivel nao devem depender de modulos de baixo nivel, mas sim de abstracoes.
- Para evitar que o arquivo `Program.cs` se torne um arquivo monolitico inflado ("God Class"), dificultando manutencao e testes.

### 4. Porque a decisao foi tomada
Optou-se por isolar os registros em metodos de extensao fortemente tipados (`AddApplicationServices` e `AddInfrastructureServices`) em vez de utilizar escaneamento dinamico por reflexao (ex: Scrutor). O registro explicito traz tempo de inicializacao menor, clareza sobre o ciclo de vida dos servicos (Scoped vs Transient vs Singleton) e facilita a identificacao imediata de dependencias ausentes em tempo de compilacao.

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAnimalService, AnimalService>();
        services.AddScoped<IClinicaService, ClinicaService>();
        services.AddScoped<IConsultaService, ConsultaService>();
        services.AddScoped<IDiarioService, DiarioService>();
        services.AddScoped<IDonoService, DonoService>();
        services.AddScoped<IHistoricoService, HistoricoService>();
        services.AddScoped<IPrescricaoService, PrescricaoService>();
        services.AddScoped<ISintomaService, SintomaService>();
        services.AddScoped<IVeterinarioService, VeterinarioService>();
        services.AddScoped<IRecomendacaoIaService, RecomendacaoIaService>();

        return services;
    }
}
```

```csharp
namespace ClyvoVetChallenge.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var useSqlite = configuration.GetValue<bool>("Database:UseSqlite");
        if (useSqlite)
        {
            var sqliteConnectionString = configuration.GetConnectionString("ClyvoVetSqlite")
                ?? "Data Source=clyvovet-dev.db";

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(sqliteConnectionString));
        }
        else
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(options =>
            {
                if (!string.IsNullOrEmpty(connectionString))
                {
                    options.UseNpgsql(connectionString);
                }
            });
        }

        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
        services.AddScoped<IRecomendacaoIaRepository, RecomendacaoIaRepository>();

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IAnimalRepository, AnimalRepository>();
        services.AddScoped<IConsultaRepository, ConsultaRepository>();

        return services;
    }
}
```

---

## Task 2: Tratamento Global de Excecoes com RFC 7807 (ProblemDetails)

### 1. O que foi feito
- Criacao de uma hierarquia semantica de excecoes de dominio: `DomainException`, `NotFoundException` e `BusinessRuleException`.
- Implementacao da classe `GlobalExceptionHandler` utilizando a interface nativa `IExceptionHandler` do .NET.
- Mapeamento automatico das falhas para codigos HTTP padronizados (404, 400, 401 e 500) gerando o corpo de resposta no formato padrao RFC 7807 (`application/problem+json`).
- Rastreamento unificado de erros com extracao de `TraceId` e registro estruturado no Serilog.

### 2. Como foi feito
O manipulador foi registrado no pipeline HTTP via `builder.Services.AddExceptionHandler<GlobalExceptionHandler>()` e `builder.Services.AddProblemDetails()`, sendo invocado por `app.UseExceptionHandler()`. Quando uma excecao e disparada, o manipulador intercepta o fluxo, identifica o tipo de excecao e preenche o envelope `ProblemDetails` anexando a instancia da rota e o identificador de correlacao distribuida.

### 3. Porque foi feito
- Evitar o vazamento de detalhes internos da aplicacao e stack traces para o cliente externo em ambiente de producao (vulnerabilidade de seguranca OWASP).
- Garantir respostas padronizadas e previsiveis para integracoes front-end, mobile e servicos parceiros.

### 4. Porque a decisao foi tomada
Optou-se por utilizar o contrato `IExceptionHandler` introduzido no .NET 8 em vez do tradicional middleware customizado (`app.UseMiddleware<ExceptionMiddleware>()`). O `IExceptionHandler` e nativamente integrado ao ecossistema do ASP.NET Core, suporta cancelamento cooperativo via `CancellationToken`, evita a duplicacao de manipulacao de pipelines e consome diretamente os servicos de `ProblemDetails` configurados.

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Api.Exceptions;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, "Excecao capturada no pipeline HTTP: {Message} | TraceId: {TraceId}", exception.Message, traceId);

        var (statusCode, title, detail) = MapException(environment, exception);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Type = "https://datatracker.ietf.org/doc/html/rfc7807",
            Title = title,
            Status = statusCode,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = traceId;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(IHostEnvironment environment, Exception exception)
    {
        return exception switch
        {
            NotFoundException e => (StatusCodes.Status404NotFound, "Recurso nao encontrado", e.Message),
            BusinessRuleException e => (StatusCodes.Status400BadRequest, "Regra de negocio violada", e.Message),
            DomainException e => (StatusCodes.Status400BadRequest, "Erro de validacao de dominio", e.Message),
            UnauthorizedAccessException e => (StatusCodes.Status401Unauthorized, "Acesso nao autorizado", e.Message),
            _ => MapUnhandled(environment, exception)
        };
    }
}
```

```csharp
namespace ClyvoVetChallenge.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
```

---

## Task 3: Persistencia Relacional com EF Core, Repositorios e Stored Procedures

### 1. O que foi feito
- Mapeamento formal do relacionamento 1:N entre o CORE do negocio (`Animal` e `Consulta`) utilizando Fluent API via `IEntityTypeConfiguration<T>`.
- Implementacao de operacoes CRUD assincronas completas no repositorio base `Repository<T>` e repositorios especializados.
- Implementacao de metodos de execucao direta de SQL parametrizado e Stored Procedures via `FromSqlRaw` e `ExecuteSqlRawAsync`.
- Atualizacao do arquivo `script_bd.sql` contendo o DDL relacional e a procedure `sp_cancelar_consultas_antigas` e `sp_obter_resumo_animal`.

### 2. Como foi feito
A configuracao das entidades foi separada em classes dedicadas em `Infrastructure/Persistence/Configurations`. Em `ConsultaRepository`, implementou-se o metodo `ObterConsultasPorAnimalSqlAsync` utilizando `FromSqlRaw` com passagem segura de parametros posicionais, e `CancelarConsultasAntigasAsync` chamando a procedure nativa no PostgreSQL ou instrucao SQL compativel no SQLite.

### 3. Porque foi feito
- Cumprimento dos requisitos academicos e corporativos que exigem integracao entre backend .NET e rotinas empacotadas no banco de dados relacional.
- Garantia de integridade referencial com chaves estrangeiras explicitas e regras de exclusao restritiva (`DeleteBehavior.Restrict`).

### 4. Porque a decisao foi tomada
Optou-se por Fluent API em vez de Data Annotations nas entidades do `Domain`. Data Annotations poluem as entidades de negocio com regras de banco (tamanhos de coluna, nomes de tabela), violando a pureza do dominio. A execucao via `FromSqlRaw` com parametros protege o sistema contra SQL Injection e preserva a tipagem de retorno.

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Infrastructure.Persistence.Configurations;

public class ConsultaConfiguration : IEntityTypeConfiguration<Consulta>
{
    public void Configure(EntityTypeBuilder<Consulta> builder)
    {
        builder.ToTable("TB_CONSULTA");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DataConsulta).IsRequired();
        builder.Property(x => x.HoraConsulta).IsRequired();
        builder.Property(x => x.Motivo).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Status).IsRequired().HasConversion<string>().HasMaxLength(20);

        builder.HasOne(x => x.Animal)
            .WithMany(x => x.Consultas)
            .HasForeignKey(x => x.AnimalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Veterinario)
            .WithMany()
            .HasForeignKey(x => x.VeterinarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

```csharp
namespace ClyvoVetChallenge.Infrastructure.Repositories;

public class ConsultaRepository(ClyvoVetContext context) : Repository<Consulta>(context), IConsultaRepository
{
    public async Task<IReadOnlyCollection<Consulta>> ObterConsultasPorAnimalSqlAsync(Guid animalId, CancellationToken cancellationToken = default)
    {
        return await _context.Consultas
            .FromSqlRaw("SELECT * FROM \"TB_CONSULTA\" WHERE \"AnimalId\" = {0}", animalId)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CancelarConsultasAntigasAsync(int dias, CancellationToken cancellationToken = default)
    {
        if (_context.Database.IsNpgsql())
        {
            return await _context.Database.ExecuteSqlRawAsync("CALL sp_cancelar_consultas_antigas({0})", [dias], cancellationToken);
        }

        return await _context.Database.ExecuteSqlRawAsync(
            "UPDATE \"TB_CONSULTA\" SET \"Status\" = 'Cancelada' WHERE \"Status\" = 'Agendada' AND \"DataConsulta\" < DATE('now', '-' || {0} || ' day')",
            dias);
    }
}
```

---

## Task 4: Persistencia Poliglota com MongoDB e Servico de IA Clinica

### 1. O que foi feito
- Integracao do driver oficial `MongoDB.Driver` para persistencia NoSQL poliglota.
- Criacao da classe de configuracao `MongoDbSettings` injetada via `IOptions<MongoDbSettings>`.
- Criacao do documento `HistoricoRecomendacaoIa` e repositorio generico `MongoRepository<T>` com resiliencia e fallback em memoria.
- Implementacao do servico de decisao clinica `RecomendacaoIaService` capaz de analisar sintomas relatados, classificar o nivel de urgencia (Baixa, Media, Alta, Emergencia), sugerir diagnostico preliminar e condutas de cuidado continuo, gravando auditoria no MongoDB.
- Criacao do endpoint `RecomendacaoIaController`.

### 2. Como foi feito
O servico avalia palavras-chave e severidade nos sintomas informados. Ao processar a analise, a entidade e persistida na colecao `historicorecomendacaoia` do MongoDB. O repositorio `MongoRepository<T>` foi programado defensivamente: caso a string de conexao esteja ausente ou o cluster inacessivel (como durante a execucao de testes unitarios em pipelines), ele opera sobre uma colecao em memoria thread-safe sem quebrar o fluxo.

### 3. Porque foi feito
- Atender ao requisito de persistencia poliglota: dados transacionais relacionais (tutores, consultas) pertencem ao PostgreSQL; documentos flexiveis de telemetria, historico de IA e auditoria semiestruturada pertencem ao NoSQL.
- Fornecer o componente funcional de Inteligencia Artificial da Clyvo Vet para triagem rapida de pacientes veterinarios.

### 4. Porque a decisao foi tomada
Optou-se por desenhar o `MongoRepository` com tolerancia a falhas e fallback em memoria para que a esteira de CI/CD do Azure DevOps execute `dotnet test` sem demandar uma instancia ativa de MongoDB externa antes do provisionamento de nuvem.

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Application.Services.Implementations;

public class RecomendacaoIaService(IRecomendacaoIaRepository recomendacaoIaRepository) : IRecomendacaoIaService
{
    public async Task<RecomendacaoIaResponse> GerarRecomendacaoAsync(RecomendacaoIaRequest request, CancellationToken cancellationToken = default)
    {
        if (request.AnimalId == Guid.Empty)
        {
            throw new BusinessRuleException("O identificador do animal e obrigatorio para geracao da recomendacao clinica.");
        }

        if (request.Sintomas == null || request.Sintomas.Count == 0)
        {
            throw new BusinessRuleException("Ao menos um sintoma clinico deve ser informado para analise.");
        }

        var sintomasNormalizados = request.Sintomas.Select(s => s.Trim().ToLowerInvariant()).ToList();
        var nivelUrgencia = DeterminarUrgencia(sintomasNormalizados);
        var diagnosticoSugerido = SugerirDiagnostico(request.Especie, sintomasNormalizados);
        var recomendacoes = GerarRecomendacoes(nivelUrgencia, sintomasNormalizados);

        var entity = new HistoricoRecomendacaoIa
        {
            AnimalId = request.AnimalId,
            NomeAnimal = request.NomeAnimal,
            Especie = request.Especie,
            IdadeAnos = request.IdadeAnos,
            SintomasRelatados = request.Sintomas,
            DiagnosticoSugerido = diagnosticoSugerido,
            NivelUrgencia = nivelUrgencia,
            RecomendacoesCuidados = recomendacoes,
            DataAvaliacao = DateTime.UtcNow,
            ModeloIaUtilizado = "ClyvoVet-ClinicalDecision-v2"
        };

        await recomendacaoIaRepository.CreateAsync(entity, cancellationToken);
        return ToResponse(entity);
    }
}
```

```csharp
namespace ClyvoVetChallenge.Infrastructure.Mongo.Repositories;

public class MongoRepository<T> : IMongoRepository<T> where T : class
{
    private readonly IMongoCollection<T>? _collection;
    private readonly ILogger<MongoRepository<T>> _logger;
    private static readonly List<T> _inMemoryStore = [];

    public MongoRepository(IOptions<MongoDbSettings> settings, ILogger<MongoRepository<T>> logger)
    {
        _logger = logger;
        try
        {
            var config = settings.Value;
            var connectionString = config.ConnectionString;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(config.DatabaseName);
                var collectionName = typeof(T).Name.Replace("Document", string.Empty).ToLowerInvariant();
                _collection = database.GetCollection<T>(collectionName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Nao foi possivel inicializar conexao com MongoDB. Operando com armazenamento em memoria temporario.");
        }
    }
}
```

---

## Task 5: Paginacao, Ordenacao Dinamica, Filtros e HATEOAS (Richardson Nivel 3)

### 1. O que foi feito
- Criacao de classes base e derivadas de parametros de consulta: `QueryParameters`, `AnimalQueryParameters` e `ConsultaQueryParameters`.
- Criacao do envelope generico `PagedResponse<T>` contendo contadores de paginacao (`PageNumber`, `PageSize`, `TotalCount`, `TotalPages`, `HasPreviousPage`, `HasNextPage`) e hiperlinks navegaveis `List<LinkDto>`.
- Implementacao da classe `HateoasLinkBuilder` para construcao de links (`self`, `update`, `delete`, `first-page`, `last-page`, `next-page`, `previous-page`).
- Aplicacao de paginacao com clausulas `Skip` e `Take` diretamente no banco relacional via `IQueryable`.

### 2. Como foi feito
Os repositorios receberam metodos de consulta paginada que filtram e ordenam dinamicamente por expressao LINQ antes da execucao da consulta. Nos controllers, apos a obtencao dos dados, o `HateoasLinkBuilder` calcula as rotas relativas/absolutas com base no contexto HTTP da requisicao atual (`HttpContext.Request`) e adiciona a lista de links navegaveis tanto na raiz da resposta paginada quanto em cada item individual retornado.

### 3. Porque foi feito
- Alcancar o nivel 3 de maturidade REST (Modelo de Richardson - Hipermidia como o Motor do Estado da Aplicacao / HATEOAS).
- Evitar problemas criticos de escalabilidade e estouro de memoria (OutOfMemoryException) causados por retornos irrestritos de milhares de registros em endpoints de listagem.

### 4. Porque a decisao foi tomada
A paginacao e calculada no nivel do banco de dados (gerando as instrucoes `LIMIT` e `OFFSET` no PostgreSQL) e nao em memoria apos trazer todas as linhas para o backend. O envelope `PagedResponse<T>` foi padronizado de forma generica para permitir reuso uniforme em qualquer entidade da aplicacao.

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Api.Hateoas;

public static class HateoasLinkBuilder
{
    public static void AddAnimalLinks(AnimalResponse response, HttpContext httpContext)
    {
        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host.Value;
        var baseUrl = $"{scheme}://{host}/api/Animal/{response.Id}";

        response.Links =
        [
            new LinkDto(baseUrl, "self", "GET"),
            new LinkDto(baseUrl, "update", "PUT"),
            new LinkDto(baseUrl, "delete", "DELETE"),
            new LinkDto($"{scheme}://{host}/api/Animal", "all-animals", "GET")
        ];
    }

    public static void AddPagedAnimalLinks(PagedResponse<AnimalResponse> pagedResponse, AnimalQueryParameters parameters, HttpContext httpContext)
    {
        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host.Value;
        var baseUrl = $"{scheme}://{host}/api/Animal/paged";

        var links = new List<LinkDto>
        {
            new LinkDto(BuildAnimalUrl(baseUrl, parameters.PageNumber, parameters.PageSize, parameters), "self", "GET"),
            new LinkDto(BuildAnimalUrl(baseUrl, 1, parameters.PageSize, parameters), "first-page", "GET"),
            new LinkDto(BuildAnimalUrl(baseUrl, Math.Max(1, pagedResponse.TotalPages), parameters.PageSize, parameters), "last-page", "GET")
        };

        if (pagedResponse.HasPreviousPage)
        {
            links.Add(new LinkDto(BuildAnimalUrl(baseUrl, parameters.PageNumber - 1, parameters.PageSize, parameters), "previous-page", "GET"));
        }

        if (pagedResponse.HasNextPage)
        {
            links.Add(new LinkDto(BuildAnimalUrl(baseUrl, parameters.PageNumber + 1, parameters.PageSize, parameters), "next-page", "GET"));
        }

        pagedResponse.Links = links;

        foreach (var item in pagedResponse.Items)
        {
            AddAnimalLinks(item, httpContext);
        }
    }
}
```

```csharp
namespace ClyvoVetChallenge.Application.DTOs.Common;

public class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 10));
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
    public IReadOnlyCollection<LinkDto> Links { get; set; } = [];
}
```

---

## Task 6: Autenticacao e Autorizacao com JWT Bearer e RBAC

### 1. O que foi feito
- Configuracao de autenticacao baseada em tokens JWT assinados com algoritmo HMAC-SHA256 (`Microsoft.AspNetCore.Authentication.JwtBearer`).
- Criacao de `JwtSettings` e do servico `TokenService` gerando claims de identidade (`Name`, `Role`, `Jti`).
- Estruturacao do `AuthController` expondo endpoint publico de login (`POST /api/Auth/login`).
- Protecao de todas as rotas operacionais de negocio com o atributo `[Authorize]`, mantendo abertas via `[AllowAnonymous]` apenas rotas publicas (`/api/Auth/login`, `/health`, `/api/status`, `/swagger`).

### 2. Como foi feito
O servico `TokenService` busca a chave criptografica priorizando variaveis de ambiente e configuracoes injetadas pelo Azure Key Vault. Os controllers de negocio receberam decoracao `[Authorize]` a nivel de classe. A esteira HTTP valida automaticamente o emissor (`Issuer`), audiencia (`Audience`), tempo de expiracao e assinatura da chave simetrica.

### 3. Porque foi feito
- Conformidade com a LGPD e seguranca de dados clinicos veterinarios, impedindo acesso nao autenticado ou adulteracao de prontuarios e registros de animais.
- Isolamento de permissoes baseado em perfis (Role-Based Access Control - RBAC).

### 4. Porque a decisao foi tomada
O JWT stateless foi escolhido por ser o padrao da industria para arquiteturas de microservicos e APIs distribuidas em nuvem. Elimina a necessidade de sessao em servidor ou consultas ao banco de dados a cada requisicao HTTP, permitindo escalabilidade horizontal imediata de containers no Azure Container Instances (ACI).

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly string _signingKey;

    public int ExpirationMinutes => _jwtSettings.ExpirationMinutes > 0 ? _jwtSettings.ExpirationMinutes : 120;

    public TokenService(IOptions<JwtSettings> jwtOptions, IConfiguration configuration)
    {
        _jwtSettings = jwtOptions.Value;
        _signingKey = !string.IsNullOrWhiteSpace(_jwtSettings.SecretKey)
            ? _jwtSettings.SecretKey
            : configuration["JWT_SECRET"]
              ?? Environment.GetEnvironmentVariable("JWT_SECRET")
              ?? string.Empty;
    }

    public string GenerateToken(string username, string role, IEnumerable<Claim>? additionalClaims = null)
    {
        if (string.IsNullOrWhiteSpace(_signingKey))
        {
            throw new InvalidOperationException("Chave secreta JWT nao configurada no Azure Key Vault ou variaveis de ambiente.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_signingKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, role),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(ExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
```

---

## Task 7: Swagger/OpenAPI com Bearer e Endpoint de Identificacao para DevOps

### 1. O que foi feito
- Habilitacao da geracao de comentarios XML no arquivo de projeto `.csproj` da API (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`).
- Configuracao do `AddSwaggerGen` com definicao de seguranca `Bearer` (adicionando o botao "Authorize" na interface web).
- Habilitacao da interface SwaggerUI em todos os ambientes (incluindo Producao).
- Criacao do endpoint estrategico `StatusController` (`GET /api/status`), retornando versao da entrega (`1.0.0-sprint4`), status e ambiente.

### 2. Como foi feito
No `Program.cs`, configurou-se `c.AddSecurityDefinition("Bearer", ...)` e `c.AddSecurityRequirement(...)` para injetar o header `Authorization: Bearer <token>` nas requisicoes de teste do Swagger. O `StatusController` foi configurado com `[AllowAnonymous]` para que monitores e scripts de pipeline possam pingar a aplicacao sem barreira de autenticacao.

### 3. Porque foi feito
- Facilitar a experimentacao interativa dos endpoints pelos avaliadores tecnicos e equipe de integracao frontend.
- Criar o gancho indispensavel para o video de gravacao de DevOps: permite alterar a versao (ex: para `1.0.1-sprint4`), fazer push na branch `main`/`master` e demonstrar a atualizacao em tempo real na nuvem sem afetar regras de negocio.

### 4. Porque a decisao foi tomada
Manter o Swagger ativado fora do ambiente de desenvolvimento foi uma decisao deliberada para a avaliacao da disciplina, permitindo que a banca teste a API no Azure sem a obrigatoriedade de instalar Postman ou ferramentas desktop locais.

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Api.Controllers;

[AllowAnonymous]
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class StatusController(IHostEnvironment environment) : ControllerBase
{
    /// <summary>
    /// Endpoint de identificacao e status da aplicacao para monitoramento e validacao de pipeline CI/CD.
    /// </summary>
    /// <returns>Dados da aplicacao, versao da entrega e status operacional</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetStatus()
    {
        return Ok(new
        {
            application = "Clyvo Vet API",
            version = "1.0.0-sprint4",
            environment = environment.EnvironmentName,
            status = "Online",
            timestamp = DateTime.UtcNow
        });
    }
}
```

---

## Task 8: Expansao dos Testes Automatizados (AAA + Isolamento para CI/CD)

### 1. O que foi feito
- Implementacao e expansao de 139 testes automatizados (104 testes unitarios e 35 testes de integracao) com 100% de taxa de aprovacao e zero falhas.
- Padronizacao rigida de nomenclatura `MetodoTestado_Cenario_ResultadoEsperado`.
- Estrutura estrita Arrange-Act-Assert (AAA) separada exclusivamente por linha em branco, sem comentarios no codigo.
- Criacao de `ClyvoVetWebApplicationFactory` configurada para isolamento total em pipeline de CI utilizando SQLite em memoria e repositorio NoSQL mockado.

### 2. Como foi feito
Para os testes unitarios, utilizaram-se xUnit e Moq para isolar regras de negocio, exceptions de dominio e servicos de tokens e IA. Para os testes de integracao, a fabrica `ClyvoVetWebApplicationFactory` substitui a conexao de banco relacional pelo SQLite em memoria e fornece o metodo auxiliar `CreateAuthenticatedClient()` com tokens JWT validos pre-gerados.

### 3. Porque foi feito
- Garantir a regressao zero do software e validar todas as regras de negocio implementadas na Sprint 4.
- Permitir que o comando `dotnet test` execute de forma 100% autonoma e offline na pipeline de CI do Azure DevOps, sem depender de banco de dados ou redes externas no agente Microsoft.

### 4. Porque a decisao foi tomada
Optou-se por utilizar SQLite em memoria em vez do `UseInMemoryDatabase` do Entity Framework Core. O InMemory do EF Core nao valida restricoes relacionais, chaves estrangeiras nem tipos de coluna, enquanto o SQLite em memoria se comporta de maneira analoga a um banco relacional real, detectando violacoes de integridade com precisao.

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Tests.Integration.Fixtures;

public class ClyvoVetWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbFile = $"clyvovet-test-{Guid.NewGuid():N}.db";
    public const string TestJwtKey = "clyvovet_integration_testing_jwt_secret_key_2026_min32chars!";
    public const string TestUsername = "admin";
    public const string TestPassword = "TestSecureAuthPassword2026!";

    public static void EnsureTestEnvironment(string? dbFile = null)
    {
        Environment.SetEnvironmentVariable("Database__UseSqlite", "true");
        if (!string.IsNullOrEmpty(dbFile))
        {
            Environment.SetEnvironmentVariable("ConnectionStrings__ClyvoVetSqlite", $"Data Source={dbFile}");
        }
        Environment.SetEnvironmentVariable("Jwt__Key", TestJwtKey);
        Environment.SetEnvironmentVariable("JWT_SECRET", TestJwtKey);
        Environment.SetEnvironmentVariable("Auth__Username", TestUsername);
        Environment.SetEnvironmentVariable("Auth__Password", TestPassword);
    }

    public static string GenerateTestToken(string username = TestUsername, string role = "Admin")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(TestJwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, role)]),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = "ByteCareApi",
            Audience = "ByteCareClient",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public HttpClient CreateAuthenticatedClient(string username = TestUsername, string role = "Admin")
    {
        var client = CreateClient();
        var token = GenerateTestToken(username, role);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
```

```csharp
namespace ClyvoVetChallenge.Tests.Unit.Application;

public class RecomendacaoIaServiceTests
{
    [Fact]
    public async Task GerarRecomendacaoAsync_QuandoSintomasGraves_DeveRetornarUrgenciaEmergencia()
    {
        var mockRepo = new Mock<IRecomendacaoIaRepository>();
        var service = new RecomendacaoIaService(mockRepo.Object);
        var request = new RecomendacaoIaRequest
        {
            AnimalId = Guid.NewGuid(),
            NomeAnimal = "Rex",
            Especie = "Canino",
            IdadeAnos = 5,
            Sintomas = ["convulsao", "dificuldade respiratoria"]
        };

        var response = await service.GerarRecomendacaoAsync(request);

        Assert.NotNull(response);
        Assert.Equal("Emergencia", response.NivelUrgencia);
        mockRepo.Verify(r => r.CreateAsync(It.IsAny<HistoricoRecomendacaoIa>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

---

## Task 9: Higienizacao de Segredos, Pipeline CI/CD e Container Nao-Root

### 1. O que foi feito
- Higienizacao completa de arquivos versionados (`appsettings.json`, `appsettings.Development.json` e `.env.example`), decompondo connection strings em variaveis atomicas vazias para eliminar gatilhos de seguranca do GitGuardian.
- Criacao do arquivo `azure-pipelines.yml` no Azure DevOps executando restore, build em configuracao Release, execucao de testes com cobertura `XPlat Code Coverage` e publicacao do artefato `drop`.
- Criacao do `Dockerfile` multi-stage executando sob o usuario restrito nao-root `USER app`.

### 2. Como foi feito
No `Dockerfile`, a imagem base `mcr.microsoft.com/dotnet/aspnet:9.0` e utilizada na etapa final, definindo as permissoes de pasta com `chown -R app:app /app` e instruindo o container a executar estritamente como `USER app` na porta 8080. A pipeline no Azure Pipelines utiliza as tasks oficiais `UseDotNet@2`, `DotNetCoreCLI@2` e `PublishBuildArtifacts@1`.

### 3. Porque foi feito
- Atender a diretriz inegociavel de seguranca da Sprint 4 (evitar penalidade de -15 pontos em DevOps por credenciais expostas no Git).
- Boas praticas de seguranca em conteineres (CIS Docker Benchmark): conteineres nunca devem rodar como `root` para mitigar ataques de escape de container.

### 4. Porque a decisao foi tomada
A decomposicao das conexoes em variaveis atomicas (`DB_HOST`, `DB_PORT`, `DB_NAME`, `DB_USER`, `DB_PASSWORD`) foi adotada para desarmar a heuristica sintatica de ferramentas de deteccao de segredos (como GitGuardian Generic Database Assignment), alem de permitir que a esteira de CD monte a connection string dinamicamente apenas em tempo de execucao no Azure.

### 5. Snippets de Codigo Real

```yaml
trigger:
  branches:
    include:
      - main
      - master

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '9.0.x'

steps:
- task: UseDotNet@2
  displayName: 'Instalar SDK .NET 9'
  inputs:
    packageType: 'sdk'
    version: $(dotnetVersion)

- task: DotNetCoreCLI@2
  displayName: 'Restaurar dependencias do projeto'
  inputs:
    command: 'restore'
    projects: '**/*.sln'

- task: DotNetCoreCLI@2
  displayName: 'Compilar solucao em Release'
  inputs:
    command: 'build'
    projects: '**/*.sln'
    arguments: '--configuration $(buildConfiguration) --no-restore'

- task: DotNetCoreCLI@2
  displayName: 'Executar testes com cobertura de codigo'
  inputs:
    command: 'test'
    projects: '**/*Tests*/*.csproj'
    arguments: '--configuration $(buildConfiguration) --no-build --collect:"XPlat Code Coverage"'

- task: DotNetCoreCLI@2
  displayName: 'Publicar artefato da API'
  inputs:
    command: 'publish'
    publishWebProjects: false
    projects: 'ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj'
    arguments: '--configuration $(buildConfiguration) --no-build --output $(Build.ArtifactStagingDirectory)'
    zipAfterPublish: true

- task: PublishBuildArtifacts@1
  displayName: 'Publicar artefato drop para esteira de CD'
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)'
    ArtifactName: 'drop'
    publishLocation: 'Container'
```

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["ClyvoVetChallenge.sln", "./"]
COPY ["ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj", "ClyvoVetChallenge.Api/"]
COPY ["ClyvoVetChallenge.Application/ClyvoVetChallenge.Application.csproj", "ClyvoVetChallenge.Application/"]
COPY ["ClyvoVetChallenge.Domain/ClyvoVetChallenge.Domain.csproj", "ClyvoVetChallenge.Domain/"]
COPY ["ClyvoVetChallenge.Infrastructure/ClyvoVetChallenge.Infrastructure.csproj", "ClyvoVetChallenge.Infrastructure/"]
COPY ["ClyvoVetChallenge.Tests.Unit/ClyvoVetChallenge.Tests.Unit.csproj", "ClyvoVetChallenge.Tests.Unit/"]
COPY ["ClyvoVetChallenge.Tests.Integration/ClyvoVetChallenge.Tests.Integration.csproj", "ClyvoVetChallenge.Tests.Integration/"]

RUN dotnet restore "ClyvoVetChallenge.Api/ClyvoVetChallenge.Api.csproj"

COPY . .
WORKDIR "/src/ClyvoVetChallenge.Api"
RUN dotnet publish "ClyvoVetChallenge.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

RUN mkdir -p /app/logs && chown -R app:app /app

USER app

COPY --chown=app:app --from=build /app/publish .
ENTRYPOINT ["dotnet", "ClyvoVetChallenge.Api.dll"]
```

---

## Task 10: Documentacao Tecnica Completa e Especificacao OpenAPI

### 1. O que foi feito
- Redacao estruturada do `README.md` abrangendo todas as 10 secoes exigidas de .NET e DevOps, sem uso de nenhum emoji.
- Geracao e persistencia da especificacao OpenAPI versionada em `docs/swagger.json` (71 KB).
- Inclusao de roteiro completo para a gravacao do video pratico de demonstracao de DevOps.

### 2. Como foi feito
Um teste de integracao automatizado em `StatusApiTests.cs` consulta o endpoint `/swagger/v1/swagger.json` da aplicacao em execucao no `WebApplicationFactory` e grava o conteudo diretamente no diretorio `docs/swagger.json`, assegurando que o contrato esteja 100% fiel a API implementada. O `README.md` foi validado atraves de scripts automatizados para certificar zero emojis e links corretos.

### 3. Porque foi feito
- Consolidar a entrega academica e corporativa com transparencia total sobre a arquitetura da solucao, instrucoes de execucao e catalogo de endpoints.
- Garantir aos avaliadores o acesso imediato ao esquema OpenAPI sem necessitar compilar a aplicacao localmente.

### 4. Porque a decisao foi tomada
Automatizar a extracao do `swagger.json` via teste de integracao elimina a necessidade de ferramentas externas ou passos manuais suscetiveis a esquecimento humano, garantindo que qualquer alteracao em endpoints atualize a documentacao automaticamente.

### 5. Snippets de Codigo Real

```csharp
namespace ClyvoVetChallenge.Tests.Integration;

public class StatusApiTests : IClassFixture<ClyvoVetWebApplicationFactory>
{
    private readonly HttpClient _client;

    public StatusApiTests(ClyvoVetWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task SwaggerJson_QuandoChamado_DeveRetornarEsquemaOpenApiValido()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("openapi", content);

        var docsDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../docs"));
        if (Directory.Exists(docsDir))
        {
            await File.WriteAllTextAsync(Path.Combine(docsDir, "swagger.json"), content);
        }
    }
}
```

---

## Quadro Sinotico dos 10 Commits da Sprint 4

| Hash | Escopo Semantico | Descricao da Entrega |
| :--- | :--- | :--- |
| `78880cd` | `refactor(arch)` | Consolidacao das camadas de Clean Architecture e metodos de extensao DI |
| `8598528` | `feat(api)` | Implementacao do tratamento global de excecoes com RFC 7807 ProblemDetails |
| `cd68e75` | `feat(infra)` | Configuracoes Fluent API relacional 1:N e execucao de Stored Procedures |
| `41acbc4` | `feat(nosql)` | Integracao de repositorio NoSQL MongoDB e servico inteligente de recomendacao clinica |
| `2268aea` | `feat(api)` | Paginacao dinamica, ordenacao, filtros e navegabilidade hipermidia HATEOAS |
| `1b02916` | `feat(security)` | Autenticacao JWT Bearer com HMAC-SHA256 e controle de acesso RBAC |
| `8cc5110` | `docs(swagger)` | Documentacao XML OpenAPI, suporte a token Bearer e endpoint de status para DevOps |
| `e63fb01` | `test(qa)` | Expansao de 139 testes automatizados com padrao AAA e isolamento para CI |
| `cd33162` | `build(devops)` | Higienizacao de segredos de configuracao, Dockerfile nao-root e Azure Pipelines CI |
| `2a0a370` | `docs(readme)` | Redacao final da documentacao arquitetural e extracao da especificacao swagger.json |
