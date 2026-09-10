using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClyvoVetChallenge.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_CLINICA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Cnpj = table.Column<string>(type: "character varying(18)", maxLength: 18, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Endereco = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HorarioFuncionamento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CLINICA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_DONO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Endereco = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_DONO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_SINTOMA_CATALOGO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_SINTOMA_CATALOGO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_VETERINARIO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Crmv = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Especialidade = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VETERINARIO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_ANIMAL",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DonoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Especie = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Raca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Sexo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DataNascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    Peso = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_ANIMAL", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_ANIMAL_TB_DONO_DonoId",
                        column: x => x.DonoId,
                        principalTable: "TB_DONO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_CONSULTA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    VeterinarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClinicaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataConsulta = table.Column<DateOnly>(type: "date", nullable: false),
                    HoraConsulta = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    Motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Diagnostico = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Observacoes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CONSULTA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_CONSULTA_TB_ANIMAL_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "TB_ANIMAL",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_CONSULTA_TB_CLINICA_ClinicaId",
                        column: x => x.ClinicaId,
                        principalTable: "TB_CLINICA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_CONSULTA_TB_VETERINARIO_VeterinarioId",
                        column: x => x.VeterinarioId,
                        principalTable: "TB_VETERINARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_DIARIO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataRegistro = table.Column<DateOnly>(type: "date", nullable: false),
                    HoraRegistro = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    ObservacaoLivre = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_DIARIO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_DIARIO_TB_ANIMAL_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "TB_ANIMAL",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_HISTORICO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TipoEvento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Resultado = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_HISTORICO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_HISTORICO_TB_ANIMAL_AnimalId",
                        column: x => x.AnimalId,
                        principalTable: "TB_ANIMAL",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_HISTORICO_TB_CONSULTA_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "TB_CONSULTA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_PRESCRICAO",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConsultaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Medicamento = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Dosagem = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Frequencia = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    DuracaoDias = table.Column<int>(type: "integer", nullable: false),
                    Observacao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DataPrescricao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PRESCRICAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_PRESCRICAO_TB_CONSULTA_ConsultaId",
                        column: x => x.ConsultaId,
                        principalTable: "TB_CONSULTA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_DIARIO_SINTOMA",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DiarioId = table.Column<Guid>(type: "uuid", nullable: false),
                    SintomaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Intensidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_DIARIO_SINTOMA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_DIARIO_SINTOMA_TB_DIARIO_DiarioId",
                        column: x => x.DiarioId,
                        principalTable: "TB_DIARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TB_DIARIO_SINTOMA_TB_SINTOMA_CATALOGO_SintomaId",
                        column: x => x.SintomaId,
                        principalTable: "TB_SINTOMA_CATALOGO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_ANIMAL_DonoId",
                table: "TB_ANIMAL",
                column: "DonoId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLINICA_Cnpj",
                table: "TB_CLINICA",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONSULTA_AnimalId",
                table: "TB_CONSULTA",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONSULTA_ClinicaId",
                table: "TB_CONSULTA",
                column: "ClinicaId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONSULTA_VeterinarioId",
                table: "TB_CONSULTA",
                column: "VeterinarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_DIARIO_AnimalId",
                table: "TB_DIARIO",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_DIARIO_SINTOMA_DiarioId",
                table: "TB_DIARIO_SINTOMA",
                column: "DiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_DIARIO_SINTOMA_SintomaId",
                table: "TB_DIARIO_SINTOMA",
                column: "SintomaId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_DONO_Cpf",
                table: "TB_DONO",
                column: "Cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_DONO_Email",
                table: "TB_DONO",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_HISTORICO_AnimalId",
                table: "TB_HISTORICO",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_HISTORICO_ConsultaId",
                table: "TB_HISTORICO",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PRESCRICAO_ConsultaId",
                table: "TB_PRESCRICAO",
                column: "ConsultaId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_SINTOMA_CATALOGO_Nome",
                table: "TB_SINTOMA_CATALOGO",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_VETERINARIO_Crmv",
                table: "TB_VETERINARIO",
                column: "Crmv",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_VETERINARIO_Email",
                table: "TB_VETERINARIO",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_DIARIO_SINTOMA");

            migrationBuilder.DropTable(
                name: "TB_HISTORICO");

            migrationBuilder.DropTable(
                name: "TB_PRESCRICAO");

            migrationBuilder.DropTable(
                name: "TB_DIARIO");

            migrationBuilder.DropTable(
                name: "TB_SINTOMA_CATALOGO");

            migrationBuilder.DropTable(
                name: "TB_CONSULTA");

            migrationBuilder.DropTable(
                name: "TB_ANIMAL");

            migrationBuilder.DropTable(
                name: "TB_CLINICA");

            migrationBuilder.DropTable(
                name: "TB_VETERINARIO");

            migrationBuilder.DropTable(
                name: "TB_DONO");
        }
    }
}
