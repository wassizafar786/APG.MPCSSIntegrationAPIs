using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace APGTransaction.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class intialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateSequence(
                name: "MPCSSMessageIdSequence");

            migrationBuilder.CreateTable(
                name: "DigitalTransaction",
                columns: table => new
                {
                    IdN = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    From = table.Column<string>(type: "varchar(25)", nullable: true),
                    To = table.Column<string>(type: "varchar(25)", nullable: true),
                    MerchantRefId = table.Column<long>(type: "bigint", nullable: false),
                    TerminalNodeId = table.Column<long>(type: "bigint", nullable: false),
                    TerminalId = table.Column<long>(type: "bigint", nullable: false),
                    MerchantId = table.Column<long>(type: "bigint", nullable: false),
                    BankId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,0)", nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CreatedDatetime = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    MaxResponseDatetime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResponseDatetime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    AggregatorId = table.Column<long>(type: "bigint", nullable: true),
                    MerchantAccountTypeId = table.Column<int>(type: "integer", nullable: false),
                    ResponseCode = table.Column<string>(type: "text", nullable: true),
                    TransactionTypeId = table.Column<int>(type: "int", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "varchar(50)", nullable: false),
                    OriginalExternalTransactionId = table.Column<string>(type: "text", nullable: true),
                    SenderMobileNo = table.Column<string>(type: "varchar(250)", nullable: true),
                    ReceiverMobileNo = table.Column<string>(type: "varchar(25)", nullable: true),
                    OriginalTransactionIdN = table.Column<long>(type: "bigint", nullable: true),
                    OriginalDigitalTransactionIdN = table.Column<long>(type: "bigint", nullable: true),
                    SenderName = table.Column<string>(type: "varchar(25)", nullable: true),
                    SenderAddress = table.Column<string>(type: "varchar(250)", nullable: true),
                    ReceiverName = table.Column<string>(type: "varchar(25)", nullable: true),
                    ReceiverAddress = table.Column<string>(type: "varchar(250)", nullable: true),
                    SenderReferenceNo = table.Column<string>(type: "varchar(100)", nullable: true),
                    IsRefunded = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    RefundReason = table.Column<string>(type: "varchar(100)", nullable: true),
                    RefundSource = table.Column<string>(type: "varchar(100)", nullable: true),
                    RefundCreatorId = table.Column<string>(type: "varchar(100)", nullable: true),
                    TransactionMethodId = table.Column<int>(type: "integer", nullable: false),
                    RequestSourceId = table.Column<int>(type: "integer", nullable: false),
                    OrderId = table.Column<string>(type: "VARCHAR(50)", nullable: false),
                    MerchantBranchId = table.Column<long>(type: "bigint", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalTransaction", x => x.IdN);
                    table.ForeignKey(
                        name: "FK_DigitalTransaction_DigitalTransaction_OriginalDigitalTransa~",
                        column: x => x.OriginalDigitalTransactionIdN,
                        principalTable: "DigitalTransaction",
                        principalColumn: "IdN");
                });

            migrationBuilder.CreateTable(
                name: "MerchantMPCSSTransactionRequest",
                columns: table => new
                {
                    IdN = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QROrderId = table.Column<long>(type: "bigint", nullable: true),
                    ParticipantPrefix = table.Column<string>(type: "VARCHAR(4)", nullable: true),
                    SequenceId = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "NEXTVAL('\"MPCSSMessageIdSequence\"')"),
                    UniqueNotificationId = table.Column<string>(type: "varchar(100)", nullable: true),
                    MessageId = table.Column<string>(type: "VARCHAR(18)", nullable: false, computedColumnSql: "\"ParticipantPrefix\" || REPEAT('0', 14 - LENGTH(TRIM(TRAILING ' ' FROM \"SequenceId\"::TEXT))) || TRIM(TRAILING ' ' FROM \"SequenceId\"::TEXT)", stored: true),
                    TransactionType = table.Column<string>(type: "varchar(50)", nullable: false),
                    CreationDate = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    ErrorCode = table.Column<int>(type: "integer", nullable: true),
                    ErrorMessage = table.Column<string>(type: "varchar(1000)", nullable: true),
                    Status = table.Column<string>(type: "varchar(30)", nullable: true),
                    RequestSourceId = table.Column<int>(type: "integer", nullable: false),
                    PaymentViewType = table.Column<int>(type: "integer", nullable: true),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantMPCSSTransactionRequest", x => x.IdN);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTimeoutEnquiry",
                columns: table => new
                {
                    IdN = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    JobState = table.Column<string>(type: "varchar(15)", nullable: false),
                    TransactionStatus = table.Column<string>(type: "text", nullable: true),
                    ActionTaken = table.Column<string>(type: "text", nullable: true),
                    OriginalMessageId = table.Column<string>(type: "text", nullable: true),
                    TransactionTypeId = table.Column<int>(type: "integer", nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    NextExecutionTime = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    UpdatedDateTime = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    NumberOfRuns = table.Column<int>(type: "integer", nullable: false),
                    DigitalTransactionId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTimeoutEnquiry", x => x.IdN);
                    table.ForeignKey(
                        name: "FK_TransactionTimeoutEnquiry_DigitalTransaction_DigitalTransac~",
                        column: x => x.DigitalTransactionId,
                        principalTable: "DigitalTransaction",
                        principalColumn: "IdN",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DigitalTransaction_OriginalDigitalTransactionIdN",
                table: "DigitalTransaction",
                column: "OriginalDigitalTransactionIdN");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTimeoutEnquiry_DigitalTransactionId",
                table: "TransactionTimeoutEnquiry",
                column: "DigitalTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantMPCSSTransactionRequest");

            migrationBuilder.DropTable(
                name: "TransactionTimeoutEnquiry");

            migrationBuilder.DropTable(
                name: "DigitalTransaction");

            migrationBuilder.DropSequence(
                name: "MPCSSMessageIdSequence");
        }
    }
}
