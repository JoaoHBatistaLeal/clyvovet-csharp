CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_CLINICA" (
        "Id" uuid NOT NULL,
        "Cnpj" character varying(18) NOT NULL,
        "Email" character varying(150) NOT NULL,
        "Telefone" character varying(20) NOT NULL,
        "Endereco" character varying(255) NOT NULL,
        "HorarioFuncionamento" character varying(100) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_CLINICA" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_DONO" (
        "Id" uuid NOT NULL,
        "Nome" character varying(150) NOT NULL,
        "Cpf" character varying(14) NOT NULL,
        "Email" character varying(150) NOT NULL,
        "Telefone" character varying(20) NOT NULL,
        "DataNascimento" date NOT NULL,
        "Endereco" character varying(255) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_DONO" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_SINTOMA_CATALOGO" (
        "Id" uuid NOT NULL,
        "Nome" character varying(100) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_SINTOMA_CATALOGO" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_VETERINARIO" (
        "Id" uuid NOT NULL,
        "Nome" character varying(150) NOT NULL,
        "Crmv" character varying(20) NOT NULL,
        "Especialidade" character varying(100) NOT NULL,
        "Telefone" character varying(20) NOT NULL,
        "Email" character varying(150) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_VETERINARIO" PRIMARY KEY ("Id")
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_ANIMAL" (
        "Id" uuid NOT NULL,
        "DonoId" uuid NOT NULL,
        "Nome" character varying(100) NOT NULL,
        "Especie" character varying(50) NOT NULL,
        "Raca" character varying(50) NOT NULL,
        "Sexo" character varying(20) NOT NULL,
        "DataNascimento" date NOT NULL,
        "Peso" numeric(5,2) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_ANIMAL" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TB_ANIMAL_TB_DONO_DonoId" FOREIGN KEY ("DonoId") REFERENCES "TB_DONO" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_CONSULTA" (
        "Id" uuid NOT NULL,
        "AnimalId" uuid NOT NULL,
        "VeterinarioId" uuid NOT NULL,
        "ClinicaId" uuid NOT NULL,
        "DataConsulta" date NOT NULL,
        "HoraConsulta" time without time zone NOT NULL,
        "Motivo" character varying(500) NOT NULL,
        "Diagnostico" character varying(1000),
        "Observacoes" character varying(1000),
        "Status" character varying(20) NOT NULL,
        "Tipo" character varying(50) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_CONSULTA" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TB_CONSULTA_TB_ANIMAL_AnimalId" FOREIGN KEY ("AnimalId") REFERENCES "TB_ANIMAL" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_TB_CONSULTA_TB_CLINICA_ClinicaId" FOREIGN KEY ("ClinicaId") REFERENCES "TB_CLINICA" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_TB_CONSULTA_TB_VETERINARIO_VeterinarioId" FOREIGN KEY ("VeterinarioId") REFERENCES "TB_VETERINARIO" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_DIARIO" (
        "Id" uuid NOT NULL,
        "AnimalId" uuid NOT NULL,
        "DataRegistro" date NOT NULL,
        "HoraRegistro" time without time zone NOT NULL,
        "ObservacaoLivre" character varying(1000) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_DIARIO" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TB_DIARIO_TB_ANIMAL_AnimalId" FOREIGN KEY ("AnimalId") REFERENCES "TB_ANIMAL" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_HISTORICO" (
        "Id" uuid NOT NULL,
        "AnimalId" uuid NOT NULL,
        "ConsultaId" uuid NOT NULL,
        "DataRegistro" timestamp with time zone NOT NULL,
        "Descricao" character varying(500) NOT NULL,
        "TipoEvento" character varying(50) NOT NULL,
        "Resultado" character varying(500) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_HISTORICO" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TB_HISTORICO_TB_ANIMAL_AnimalId" FOREIGN KEY ("AnimalId") REFERENCES "TB_ANIMAL" ("Id") ON DELETE RESTRICT,
        CONSTRAINT "FK_TB_HISTORICO_TB_CONSULTA_ConsultaId" FOREIGN KEY ("ConsultaId") REFERENCES "TB_CONSULTA" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_PRESCRICAO" (
        "Id" uuid NOT NULL,
        "ConsultaId" uuid NOT NULL,
        "Medicamento" character varying(150) NOT NULL,
        "Dosagem" character varying(50) NOT NULL,
        "Frequencia" character varying(50) NOT NULL,
        "DuracaoDias" integer NOT NULL,
        "Observacao" character varying(500) NOT NULL,
        "DataPrescricao" timestamp with time zone NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_PRESCRICAO" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TB_PRESCRICAO_TB_CONSULTA_ConsultaId" FOREIGN KEY ("ConsultaId") REFERENCES "TB_CONSULTA" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE TABLE "TB_DIARIO_SINTOMA" (
        "Id" uuid NOT NULL,
        "DiarioId" uuid NOT NULL,
        "SintomaId" uuid NOT NULL,
        "Intensidade" character varying(50) NOT NULL,
        "CreatedAt" timestamp with time zone NOT NULL,
        "Active" boolean NOT NULL,
        CONSTRAINT "PK_TB_DIARIO_SINTOMA" PRIMARY KEY ("Id"),
        CONSTRAINT "FK_TB_DIARIO_SINTOMA_TB_DIARIO_DiarioId" FOREIGN KEY ("DiarioId") REFERENCES "TB_DIARIO" ("Id") ON DELETE CASCADE,
        CONSTRAINT "FK_TB_DIARIO_SINTOMA_TB_SINTOMA_CATALOGO_SintomaId" FOREIGN KEY ("SintomaId") REFERENCES "TB_SINTOMA_CATALOGO" ("Id") ON DELETE RESTRICT
    );
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_ANIMAL_DonoId" ON "TB_ANIMAL" ("DonoId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE UNIQUE INDEX "IX_TB_CLINICA_Cnpj" ON "TB_CLINICA" ("Cnpj");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_CONSULTA_AnimalId" ON "TB_CONSULTA" ("AnimalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_CONSULTA_ClinicaId" ON "TB_CONSULTA" ("ClinicaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_CONSULTA_VeterinarioId" ON "TB_CONSULTA" ("VeterinarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_DIARIO_AnimalId" ON "TB_DIARIO" ("AnimalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_DIARIO_SINTOMA_DiarioId" ON "TB_DIARIO_SINTOMA" ("DiarioId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_DIARIO_SINTOMA_SintomaId" ON "TB_DIARIO_SINTOMA" ("SintomaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE UNIQUE INDEX "IX_TB_DONO_Cpf" ON "TB_DONO" ("Cpf");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE UNIQUE INDEX "IX_TB_DONO_Email" ON "TB_DONO" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_HISTORICO_AnimalId" ON "TB_HISTORICO" ("AnimalId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_HISTORICO_ConsultaId" ON "TB_HISTORICO" ("ConsultaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE INDEX "IX_TB_PRESCRICAO_ConsultaId" ON "TB_PRESCRICAO" ("ConsultaId");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE UNIQUE INDEX "IX_TB_SINTOMA_CATALOGO_Nome" ON "TB_SINTOMA_CATALOGO" ("Nome");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE UNIQUE INDEX "IX_TB_VETERINARIO_Crmv" ON "TB_VETERINARIO" ("Crmv");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    CREATE UNIQUE INDEX "IX_TB_VETERINARIO_Email" ON "TB_VETERINARIO" ("Email");
    END IF;
END $EF$;

DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20260910182414_InitialPostgreMigration') THEN
    INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
    VALUES ('20260910182414_InitialPostgreMigration', '9.0.0');
    END IF;
END $EF$;
COMMIT;



-- ============================================================================
-- COMENTARIOS ESTRUTURAIS NAS TABELAS E COLUNAS CORE (ByteCare API)
-- ============================================================================

COMMENT ON TABLE "TB_DONO" IS 'Tabela responsavel pelo armazenamento dos dados cadastrais dos tutores (donos) dos animais.';
COMMENT ON COLUMN "TB_DONO"."Id" IS 'Identificador unico do tutor (UUID gerado automaticamente).';
COMMENT ON COLUMN "TB_DONO"."Nome" IS 'Nome completo do tutor.';
COMMENT ON COLUMN "TB_DONO"."Cpf" IS 'Numero de CPF do tutor (unico, max 14 caracteres).';
COMMENT ON COLUMN "TB_DONO"."Email" IS 'Endereco de e-mail do tutor (unico, max 150 caracteres).';
COMMENT ON COLUMN "TB_DONO"."Telefone" IS 'Numero de telefone/contato do tutor.';
COMMENT ON COLUMN "TB_DONO"."DataNascimento" IS 'Data de nascimento do tutor (DateOnly / date).';
COMMENT ON COLUMN "TB_DONO"."Endereco" IS 'Endereco residencial completo do tutor.';
COMMENT ON COLUMN "TB_DONO"."CreatedAt" IS 'Data e hora da criacao do registro (timestamp with time zone).';
COMMENT ON COLUMN "TB_DONO"."Active" IS 'Indicador booleano de registro ativo no sistema.';

COMMENT ON TABLE "TB_ANIMAL" IS 'Tabela responsavel pelo cadastro dos pacientes/animais vinculados a um tutor.';
COMMENT ON COLUMN "TB_ANIMAL"."Id" IS 'Identificador unico do animal (UUID gerado automaticamente).';
COMMENT ON COLUMN "TB_ANIMAL"."DonoId" IS 'Chave estrangeira vinculando o animal ao tutor correspondente em TB_DONO.';
COMMENT ON COLUMN "TB_ANIMAL"."Nome" IS 'Nome do animal (max 100 caracteres).';
COMMENT ON COLUMN "TB_ANIMAL"."Especie" IS 'Especie do animal (ex: Canina, Felina, Ave).';
COMMENT ON COLUMN "TB_ANIMAL"."Raca" IS 'Raca do animal (max 50 caracteres).';
COMMENT ON COLUMN "TB_ANIMAL"."Sexo" IS 'Sexo biologico do animal (ex: Macho, Femea).';
COMMENT ON COLUMN "TB_ANIMAL"."DataNascimento" IS 'Data de nascimento do animal (DateOnly / date).';
COMMENT ON COLUMN "TB_ANIMAL"."Peso" IS 'Peso do animal em quilogramas (numeric(5,2)).';
COMMENT ON COLUMN "TB_ANIMAL"."CreatedAt" IS 'Data e hora da criacao do registro (timestamp with time zone).';
COMMENT ON COLUMN "TB_ANIMAL"."Active" IS 'Indicador booleano de registro ativo no sistema.';

COMMENT ON TABLE "TB_CLINICA" IS 'Tabela responsavel pelo cadastro das clinicas e unidades veterinarias.';
COMMENT ON TABLE "TB_VETERINARIO" IS 'Tabela de medicos veterinarios com CRMV e especialidade.';
COMMENT ON TABLE "TB_CONSULTA" IS 'Tabela de consultas e agendamentos veterinarios vinculando animal, veterinario e clinica.';
COMMENT ON TABLE "TB_HISTORICO" IS 'Tabela de historico medico e prontuario de evolucao dos pacientes.';
COMMENT ON TABLE "TB_PRESCRICAO" IS 'Tabela de prescricoes medicamentosas vinculadas a consultas.';
COMMENT ON TABLE "TB_DIARIO" IS 'Tabela de registros do diario de saude dos animais.';
COMMENT ON TABLE "TB_DIARIO_SINTOMA" IS 'Tabela associativa entre diarios de saude e catalogo de sintomas observados.';
COMMENT ON TABLE "TB_SINTOMA_CATALOGO" IS 'Catalogo padronizado de sintomas clinicos veterinarios.';

-- ============================================================================
-- CARGA INICIAL OBRIGATORIA (Tabelas Core: TB_DONO e TB_ANIMAL)
-- Utiliza gen_random_uuid() para chaves primarias compativeis com PostgreSQL 16
-- ============================================================================

DO $$
DECLARE
    v_dono1_id uuid := gen_random_uuid();
    v_dono2_id uuid := gen_random_uuid();
    v_animal1_id uuid := gen_random_uuid();
    v_animal2_id uuid := gen_random_uuid();
BEGIN
    -- Insercao de Donos / Tutores Core
    IF NOT EXISTS (SELECT 1 FROM "TB_DONO" WHERE "Cpf" = '12345678901') THEN
        INSERT INTO "TB_DONO" ("Id", "Nome", "Cpf", "Email", "Telefone", "DataNascimento", "Endereco", "CreatedAt", "Active")
        VALUES (v_dono1_id, 'Carlos Eduardo Silva', '12345678901', 'carlos.silva@bytecare.com', '11987654321', '1985-04-12', 'Av. Paulista, 1000 - Bela Vista, Sao Paulo - SP', NOW(), true);
    ELSE
        SELECT "Id" INTO v_dono1_id FROM "TB_DONO" WHERE "Cpf" = '12345678901';
    END IF;

    IF NOT EXISTS (SELECT 1 FROM "TB_DONO" WHERE "Cpf" = '98765432100') THEN
        INSERT INTO "TB_DONO" ("Id", "Nome", "Cpf", "Email", "Telefone", "DataNascimento", "Endereco", "CreatedAt", "Active")
        VALUES (v_dono2_id, 'Mariana Ferreira Costa', '98765432100', 'mariana.costa@bytecare.com', '11912345678', '1990-09-25', 'Rua Augusta, 500 - Consolacao, Sao Paulo - SP', NOW(), true);
    ELSE
        SELECT "Id" INTO v_dono2_id FROM "TB_DONO" WHERE "Cpf" = '98765432100';
    END IF;

    -- Insercao de Animais Core vinculados aos tutores correspondentes
    IF NOT EXISTS (SELECT 1 FROM "TB_ANIMAL" WHERE "DonoId" = v_dono1_id AND "Nome" = 'Rex') THEN
        INSERT INTO "TB_ANIMAL" ("Id", "DonoId", "Nome", "Especie", "Raca", "Sexo", "DataNascimento", "Peso", "CreatedAt", "Active")
        VALUES (v_animal1_id, v_dono1_id, 'Rex', 'Canina', 'Golden Retriever', 'Macho', '2021-06-15', 32.50, NOW(), true);
    END IF;

    IF NOT EXISTS (SELECT 1 FROM "TB_ANIMAL" WHERE "DonoId" = v_dono2_id AND "Nome" = 'Mia') THEN
        INSERT INTO "TB_ANIMAL" ("Id", "DonoId", "Nome", "Especie", "Raca", "Sexo", "DataNascimento", "Peso", "CreatedAt", "Active")
        VALUES (v_animal2_id, v_dono2_id, 'Mia', 'Felina', 'Siames', 'Femea', '2022-03-20', 4.10, NOW(), true);
    END IF;
END $$;
