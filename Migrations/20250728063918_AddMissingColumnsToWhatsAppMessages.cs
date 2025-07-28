using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WhatsappWebHook.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingColumnsToWhatsAppMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WhatsappMessages",
                table: "WhatsappMessages");

            migrationBuilder.RenameTable(
                name: "WhatsappMessages",
                newName: "WhatsAppMessages");

            migrationBuilder.AddColumn<string>(
                name: "AccountSid",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApiVersion",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ChannelMetadata",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MessageSid",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MessageType",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "NumMedia",
                table: "WhatsAppMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumSegments",
                table: "WhatsAppMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfileName",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SmsMessageSid",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SmsStatus",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "To",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WaId",
                table: "WhatsAppMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WhatsAppMessages",
                table: "WhatsAppMessages",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_WhatsAppMessages",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "AccountSid",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "ApiVersion",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "ChannelMetadata",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "MessageSid",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "MessageType",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "NumMedia",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "NumSegments",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "ProfileName",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "SmsMessageSid",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "SmsStatus",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "To",
                table: "WhatsAppMessages");

            migrationBuilder.DropColumn(
                name: "WaId",
                table: "WhatsAppMessages");

            migrationBuilder.RenameTable(
                name: "WhatsAppMessages",
                newName: "WhatsappMessages");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WhatsappMessages",
                table: "WhatsappMessages",
                column: "Id");
        }
    }
}
