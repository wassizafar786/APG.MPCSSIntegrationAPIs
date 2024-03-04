using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APGTransaction.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class messageLanguage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "MerchantMPCSSTransactionRequest",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "MerchantMPCSSTransactionRequest");
        }
    }
}
